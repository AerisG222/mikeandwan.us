using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Maw.Security;
using NMagickWand;
using NMagickWand.Enums;

namespace Maw.Domain.Upload
{
    public class UploadService
        : IUploadService
    {
        const int TWENTY_MB = 20 * 1024 * 1024;

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
            FileLocation location = null;

            var result = new FileOperationResult() {
                Operation = FileOperation.Delete,
                RelativePathSpecified = relativePath
            };

            try
            {
                location = GetValidatedLocation(user, relativePath, true);
            }
            catch(Exception)
            {
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

            result.UploadedFile = GetFileDetails(location);

            try
            {
                File.Delete(absolutePath);

                result.WasSuccessful = true;

                _log.LogInformation($"User [{location.Username}] successfully deleted [{location.RelativePath}].");
            }
            catch(Exception ex)
            {
                result.Error = "Unable to delete file";

                _log.LogError(ex, $"Unable to delete file for path [{absolutePath}]");
            }

            return result;
        }


        public IEnumerable<FileOperationResult> DeleteFiles(ClaimsPrincipal user, IEnumerable<string> relativePaths)
        {
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


        public string GetAbsoluteFilePath(ClaimsPrincipal user, string relativePath)
        {
            var location =  GetValidatedLocation(user, relativePath, true);

            return GetAbsoluteFilePath(location);
        }


        public Stream GetFile(ClaimsPrincipal user, string relativePath)
        {
            var location = GetValidatedLocation(user, relativePath, true);

            if(!UserCanAccessFile(user, location))
            {
                var msg = $"User [{location.Username}] does not have access to the requested file [{location.RelativePath}].";

                _log.LogError(msg);

                throw new UnauthorizedAccessException(msg);
            }

            _log.LogInformation($"Delivering file [{location.RelativePath}] for user [{user.Identity.Name}].");

            return File.OpenRead(GetAbsoluteFilePath(location));
        }


        public Stream GetFiles(ClaimsPrincipal user, IEnumerable<string> relativePaths)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(relativePaths == null || relativePaths.Count() == 0)
            {
                throw new ArgumentNullException(nameof(relativePaths));
            }

            var validLocations = new List<FileLocation>();

            foreach(var relativePath in relativePaths)
            {
                FileLocation location = null;

                try
                {
                    location = FileLocation.FromRelativePath(relativePath);
                }
                catch(Exception ex)
                {
                    _log.LogError(ex, $"Unable to parse relative path [{relativePath}].  This will not be added to the zip archive.");

                    continue;
                }

                if(UserCanAccessFile(user, location))
                {
                    validLocations.Add(location);
                }
                else
                {
                    _log.LogError($"User [{location.Username}] does not have access to the requested file [{location.RelativePath}].");
                }
            }

            if(validLocations.Count == 0)
            {
                throw new UnauthorizedAccessException();
            }

            var ms = new MemoryStream(TWENTY_MB);

            using(var za = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach(var location in validLocations)
                {
                    _log.LogDebug($"Adding {location.RelativePath} to archive.");

                    za.CreateEntryFromFile(GetAbsoluteFilePath(location), location.RelativePath);
                }
            }

            ms.Seek(0, SeekOrigin.Begin);

            _log.LogInformation($"Zip file created for user {user.Identity.Name} with size {ms.Length / 1024 / 1024} MB");

            return ms;
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

            var result = new FileOperationResult {
                Operation = FileOperation.Upload,
                RelativePathSpecified = filename
            };

            var location = new FileLocation {
                Username = user.Identity.Name,
                Filename = Path.GetFileName(filename)
            };
            var userDir = GetAbsoluteUserDirectory(location.Username);
            var destPath = GetAbsoluteFilePath(location);

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

                    result.UploadedFile = GetFileDetails(location);
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


        public Stream GetThumbnail(ClaimsPrincipal user, string relativePath, int maxDimension)
        {
            FileLocation location = null;

            location = GetValidatedLocation(user, relativePath, true);

            var absolutePath = Path.Combine(_cfg.RootDirectory, location.RelativePath);

            if(!File.Exists(absolutePath))
            {
                return null;
            }

            using(var mw = new MagickWand(absolutePath))
            {
                if(mw.ImageWidth > 256 || mw.ImageHeight > 256)
                {
                    // crop 20%
                    var xoffset = Convert.ToInt32(mw.ImageWidth * 0.1);
                    var yoffset = Convert.ToInt32(mw.ImageHeight * 0.1);
                    var newWidth = mw.ImageWidth - (2 * xoffset);
                    var newHeight = mw.ImageHeight - (2 * yoffset);

                    mw.CropImage((uint)newWidth, (uint)newHeight, xoffset, yoffset);
                }

                var leftPos = (mw.ImageWidth / 2) - (maxDimension / 2);
                var topPos = (mw.ImageHeight / 2) - (maxDimension / 2);

                var ms = new MemoryStream();

                mw.ResizeImage((uint)maxDimension, (uint)maxDimension, FilterTypes.Lanczos2SharpFilter, 1);
                mw.Compression = CompressionType.JPEGCompression;
                mw.CompressionQuality = 72;
                mw.WriteImage(ms);

                ms.Seek(0, SeekOrigin.Begin);

                return ms;
            }
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


        UploadedFile GetFileDetails(FileLocation location)
        {
            var fi = new FileInfo(GetAbsoluteFilePath(location));

            return new UploadedFile {
                Location = location,
                CreationTime = fi.CreationTime,
                SizeInBytes = fi.Length
            };
        }


        FileLocation GetValidatedLocation(ClaimsPrincipal user, string relativePath, bool verifyPermissions)
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

            try
            {
                location = FileLocation.FromRelativePath(relativePath);
            }
            catch(Exception ex)
            {
                _log.LogError(ex, $"Unable to parse relative path [{relativePath}].");

                throw;
            }

            if(verifyPermissions && !UserCanAccessFile(user, location)) {
                _log.LogWarning($"User [{location.Username}] does not have access to [{location.RelativePath}].");

                throw new ApplicationException();
            }

            return location;
        }
    }
}
