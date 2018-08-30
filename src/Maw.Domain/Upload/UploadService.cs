using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Maw.Security;


namespace Maw.Domain.Upload
{
    public class UploadService
        : IUploadService
    {
        readonly UploadConfig _cfg;
        readonly ILogger<UploadService> _log;


        public UploadService(IOptions<UploadConfig> uploadConfig,
                             ILogger<UploadService> log) {
            this._cfg = uploadConfig?.Value ?? throw new ArgumentNullException(nameof(uploadConfig));
            this._log = log ?? throw new ArgumentNullException(nameof(log));

            if(!Directory.Exists(_cfg.RootDirectory))
            {
                _log.LogError($"Upload root directory [{_cfg.RootDirectory}] does not exist!");

                throw new DirectoryNotFoundException($"Could not find File Upload root directory [{_cfg.RootDirectory}]");
            }
        }


        public FileOperationResult DeleteFile(ClaimsPrincipal user, string relativePath)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            FileLocation location = null;

            var result = new FileOperationResult() {
                Operation = FileOperation.Delete
            };

            try
            {
                location = FileLocation.FromRelativePath(relativePath);
            }
            catch(Exception)
            {
                result.Error = "Invalid file path";

                return result;
            }

            result.FileLocation = location;

            if(!UserCanAccessFile(user, location)) {
                result.Error = "Invalid file path";

                return result;
            }

            var absolutePath = Path.Combine(_cfg.RootDirectory, location.RelativePath);

            if(!File.Exists(absolutePath))
            {
                // perhaps something else deleted this, but the final state is equivalent to the file being deleted, so return success
                result.WasSuccessful = true;

                return result;
            }

            try
            {
                File.Delete(absolutePath);

                result.WasSuccessful = true;
            }
            catch(Exception ex)
            {
                result.Error = "Unable to delete file";

                _log.LogError(ex, $"Unable to delete file for path {absolutePath}");
            }

            return result;
        }


        public IEnumerable<FileOperationResult> DeleteFiles(ClaimsPrincipal user, IEnumerable<string> relativePaths)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(relativePaths == null)
            {
                throw new ArgumentNullException(nameof(relativePaths));
            }

            var results = new List<FileOperationResult>();

            foreach(var relativePath in relativePaths) {
                results.Add(DeleteFile(user, relativePath));
            }

            return results;
        }


        public Task<Stream> GetFileAsync(ClaimsPrincipal user, string relativePath)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            throw new NotImplementedException();
        }


        public Task<Stream> GetFilesAsync(ClaimsPrincipal user, IEnumerable<string> relativePaths)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(relativePaths == null)
            {
                throw new ArgumentNullException(nameof(relativePaths));
            }

            throw new NotImplementedException();
        }


        public IEnumerable<UploadedFile> GetFileList(ClaimsPrincipal user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(Role.IsAdmin(user))
            {
                return GetAllFiles();
            }
            else
            {
                return GetUserFiles(user.Identity.Name);
            }
        }


        public async Task<FileOperationResult> SaveFileAsync(ClaimsPrincipal user, string filename, Stream stream)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            var result = new FileOperationResult();
            var location = new FileLocation {
                Username = user.Identity.Name,
                Filename = Path.GetFileName(filename)
            };
            var userDir = GetAbsoluteUserDirectory(location.Username);
            var destPath = GetAbsoluteFilePath(location);

            result.Operation = FileOperation.Upload;
            result.FileLocation = location;

            if(stream == null || stream.Length == 0)
            {
                result.Error = $"File with name {filename} is empty.";

                return result;
            }

            if(File.Exists(destPath))
            {
                result.Error = $"File with name {filename} already exists.";

                return result;
            }

            try
            {
                EnsureUserDirectoryExists(userDir);
            }
            catch(Exception ex)
            {
                _log.LogError(ex, $"Could not create user directory {userDir}");

                result.WasSuccessful = false;
                result.Error = $"There was an error trying to save {filename}";

                return result;
            }

            using (var outputStream = new FileStream(destPath, FileMode.Create))
            {
                try
                {
                    await stream.CopyToAsync(outputStream);

                    result.WasSuccessful = true;
                }
                catch(Exception ex)
                {
                    _log.LogError(ex, $"Unable to save file upload for {location.Username} with filename {location.Filename}, and absolute path {destPath}.");

                    result.Error = $"There was an error trying to save {filename}.";
                }
            }

            return result;
        }


        IEnumerable<UploadedFile> GetAllFiles()
        {
            var list = new List<UploadedFile>();

            var userDirs = Directory.GetDirectories(_cfg.RootDirectory);

            foreach(var userDir in userDirs)
            {
                list.AddRange(GetFilesInDirectory(userDir));
            }

            return list;
        }


        IEnumerable<UploadedFile> GetUserFiles(string username)
        {
            var userDir = GetAbsoluteUserDirectory(username);

            if(!Directory.Exists(userDir))
            {
                return new List<UploadedFile>();
            }

            return GetFilesInDirectory(userDir);
        }


        IEnumerable<UploadedFile> GetFilesInDirectory(string path)
        {
            var dir = new DirectoryInfo(path);

            return dir
                .EnumerateFiles()
                .Select(fi => new UploadedFile
                    {
                        Location = new FileLocation {
                            Username = fi.Directory.Name,
                            Filename = fi.Name
                        },
                        CreationTime = fi.CreationTime,
                        SizeInBytes = fi.Length
                    })
                .OrderByDescending(x => x.CreationTime);
        }


        string GetAbsoluteUserDirectory(string username)
        {
            return Path.Combine(_cfg.RootDirectory, username);
        }


        string GetAbsoluteFilePath(FileLocation location)
        {
            return Path.Combine(GetAbsoluteUserDirectory(location.Username), location.Filename);
        }


        void EnsureUserDirectoryExists(string userDir)
        {
            if(!Directory.Exists(userDir))
            {
                Directory.CreateDirectory(userDir);
            }
        }


        bool UserCanAccessFile(ClaimsPrincipal user, FileLocation location) {
            return Role.IsAdmin(user) ||
                string.Equals(location.Username, user.Identity.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
