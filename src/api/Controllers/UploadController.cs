using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Maw.Security;
using MawApi.Models.Upload;


namespace MawMvcApp.Controllers
{
    [ApiController]
    [Authorize]
    [Authorize(Policy.CanUpload)]
    [Route("upload")]
    public class UploadController
        : Controller
    {
        readonly ILogger<UploadController> _log;
        readonly FileUploadConfig _uploadConfig;


		public UploadController(ILogger<UploadController> log,
							    IHostingEnvironment env,
                                IOptions<FileUploadConfig> uploadOpts)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _uploadConfig = uploadOpts?.Value ?? throw new ArgumentNullException(nameof(uploadOpts));
        }


        [HttpGet("files")]
        public IActionResult Files()
        {
            if(!Directory.Exists(_uploadConfig.RootDirectory))
            {
                _log.LogError($"Upload root directory [{_uploadConfig.RootDirectory}] does not exist!");

                return NotFound();
            }

            if(Role.IsAdmin(User))
            {
                return Ok(GetAllFiles());
            }
            else
            {
                return Ok(GetUserFiles());
            }
        }


        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok(await SaveFileAsync(file));
        }


        [HttpPost("delete")]
        public IActionResult Delete(string[] deleteFiles)
        {
            return Ok(DeleteFiles(deleteFiles));
        }


        [HttpGet("download")]
        public IActionResult Download(string[] downloadFiles)
        {
            return BadRequest();
        }


        IEnumerable<FileOperationResult> DeleteFiles(string[] files)
        {
            var results = new List<FileOperationResult>();

            if(files.Length == 0)
            {
                return results;
            }

            foreach(var file in files)
            {
                var result = new FileOperationResult();

                result.Operation = FileOperation.Delete;
                result.FileName = Path.GetFileName(file);

                results.Add(result);

                if(Path.IsPathFullyQualified(file))
                {
                    result.Error = "Invalid file path";
                    continue;
                }

                var user = file.Split('/')[0];

                if(!Role.IsAdmin(User) && !string.Equals(user, User.Identity.Name, StringComparison.OrdinalIgnoreCase)) {
                    result.Error = "Invalid file path";
                    continue;
                }

                var path = Path.Combine(_uploadConfig.RootDirectory, file);

                if(!System.IO.File.Exists(path))
                {
                    result.Error = "File does not exist";
                    continue;
                }

                try
                {
                    System.IO.File.Delete(path);

                    result.WasSuccessful = true;
                }
                catch(Exception ex)
                {
                    result.Error = "Unable to delete file";

                    _log.LogError(ex, $"Unable to delete file for path {path}");
                }
            }

            return results;
        }


        async Task<FileOperationResult> SaveFileAsync(IFormFile file)
        {
            var result = new FileOperationResult();
            var filename = Path.GetFileName(file.FileName);
            var userDir = GetUserDirectory();
            var destPath = Path.Combine(userDir, filename);

            result.Operation = FileOperation.Upload;
            result.FileName = filename;

            if(file.Length == 0)
            {
                result.Error = $"File with name {filename} is empty.";

                return result;;
            }

            if(System.IO.File.Exists(destPath))
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

            using (var stream = new FileStream(destPath, FileMode.Create))
            {
                try
                {
                    await file.CopyToAsync(stream);

                    result.WasSuccessful = true;
                }
                catch(Exception ex)
                {
                    _log.LogError(ex, $"Unable to save file upload for {User.Identity.Name} with filename {filename}.");

                    result.Error = $"There was an error trying to save {filename}.";
                }
            }

            return result;
        }


        IEnumerable<UploadedFile> GetAllFiles()
        {
            var list = new List<UploadedFile>();

            var userDirs = Directory.GetDirectories(_uploadConfig.RootDirectory);

            foreach(var userDir in userDirs)
            {
                list.AddRange(GetFilesInDirectory(userDir));
            }

            return list;
        }


        IEnumerable<UploadedFile> GetUserFiles()
        {
            var userDir = GetUserDirectory();

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
                .GetFiles()
                .Select(fi => new UploadedFile
                    {
                        Username = fi.Directory.Name,
                        Filename = fi.Name,
                        CreationTime = fi.CreationTime,
                        SizeInBytes = fi.Length
                    })
                .OrderByDescending(x => x.CreationTime);
        }


        string GetUserDirectory()
        {
            return Path.Combine(_uploadConfig.RootDirectory, User.Identity.Name);
        }


        void EnsureUserDirectoryExists(string userDir)
        {
            if(!Directory.Exists(userDir))
            {
                Directory.CreateDirectory(userDir);
            }
        }
    }
}
