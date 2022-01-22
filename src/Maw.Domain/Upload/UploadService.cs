using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Maw.Security;

namespace Maw.Domain.Upload;

public class UploadService
    : IUploadService
{
    const int TWENTY_MB = 20 * 1024 * 1024;

    readonly UploadConfig _cfg;
    readonly ILogger _log;

    public UploadService(
        IOptions<UploadConfig> uploadConfig,
        ILogger<UploadService> log)
    {
        _cfg = uploadConfig?.Value ?? throw new ArgumentNullException(nameof(uploadConfig));
        _log = log ?? throw new ArgumentNullException(nameof(log));

        if (!Directory.Exists(_cfg.RootDirectory))
        {
            _log.LogError("Upload root directory [{Directory}] does not exist!", _cfg.RootDirectory);

            throw new DirectoryNotFoundException($"Could not find File Upload root directory [{_cfg.RootDirectory}]");
        }
    }

    public FileOperationResult DeleteFile(ClaimsPrincipal user, string relativePath)
    {
        FileLocation location;

        var result = new FileOperationResult()
        {
            Operation = FileOperation.Delete,
            RelativePathSpecified = relativePath
        };

        try
        {
            location = GetValidatedLocation(user, relativePath, true);
        }
        catch (Exception)
        {
            result.Error = "Invalid file path";

            return result;
        }

        var absolutePath = Path.Combine(_cfg.RootDirectory, location.RelativePath);

        if (!File.Exists(absolutePath))
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

            _log.LogInformation("User [{Username}] successfully deleted [{File}].", location.Username, location.RelativePath);
        }
        catch (Exception ex)
        {
            result.Error = "Unable to delete file";

            _log.LogError(ex, "Unable to delete file [{File}]", absolutePath);
        }

        return result;
    }

    public IEnumerable<FileOperationResult> DeleteFiles(ClaimsPrincipal user, IEnumerable<string> relativePaths)
    {
        if (relativePaths == null)
        {
            throw new ArgumentNullException(nameof(relativePaths));
        }

        var results = new List<FileOperationResult>();

        foreach (var relativePath in relativePaths)
        {
            results.Add(DeleteFile(user, relativePath));
        }

        return results;
    }

    public string GetAbsoluteFilePath(ClaimsPrincipal user, string relativePath)
    {
        var location = GetValidatedLocation(user, relativePath, true);

        return GetAbsoluteFilePath(location);
    }

    public Stream GetFile(ClaimsPrincipal user, string relativePath)
    {
        var username = user.GetUsername();
        var location = GetValidatedLocation(user, relativePath, true);

        if (!UserCanAccessFile(user, location))
        {
            _log.LogError(
                "User [{Username}] does not have access to the requested file [{RelativePath}].",
                username,
                location.RelativePath
            );

            var msg = $"User [{location.Username}] does not have access to the requested file [{location.RelativePath}].";

            throw new UnauthorizedAccessException(msg);
        }

        _log.LogDebug("Delivering file [{File}] for user [{Username}].", location.RelativePath, username);

        return File.OpenRead(GetAbsoluteFilePath(location));
    }

    public Stream GetFiles(ClaimsPrincipal user, IEnumerable<string> relativePaths)
    {
        if (relativePaths == null || !relativePaths.Any())
        {
            throw new ArgumentNullException(nameof(relativePaths));
        }

        var validLocations = new List<FileLocation>();

        foreach (var relativePath in relativePaths)
        {
            FileLocation location;

            try
            {
                location = FileLocation.FromRelativePath(relativePath);
            }
            catch (IOException ex)
            {
                _log.LogError(ex, "Unable to parse relative path [{File}].  This will not be added to the zip archive.", relativePath);

                continue;
            }

            if (UserCanAccessFile(user, location))
            {
                validLocations.Add(location);
            }
            else
            {
                _log.LogError("User [{Username}] does not have access to the requested file [{File}].", location.Username, location.RelativePath);
            }
        }

        if (validLocations.Count == 0)
        {
            throw new UnauthorizedAccessException();
        }

        var ms = new MemoryStream(TWENTY_MB);

        using (var za = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            foreach (var location in validLocations)
            {
                _log.LogDebug("Adding {File} to archive.", location.RelativePath);

                za.CreateEntryFromFile(GetAbsoluteFilePath(location), location.RelativePath);
            }
        }

        ms.Seek(0, SeekOrigin.Begin);

        _log.LogInformation("Zip file created for user {Username} with size {FileSize} MB", user.GetUsername(), ms.Length / 1024 / 1024);

        return ms;
    }

    public IEnumerable<UploadedFile> GetFileList(ClaimsPrincipal user)
    {
        var username = user.GetUsername();

        if (user.IsAdmin())
        {
            return GetAllFiles();
        }
        else
        {
            return GetUserFiles(username);
        }
    }

    public async Task<FileOperationResult> SaveFileAsync(ClaimsPrincipal user, string filename, Stream stream)
    {
        var username = user.GetUsername();

        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        var result = new FileOperationResult
        {
            Operation = FileOperation.Upload,
            RelativePathSpecified = filename
        };

        var location = new FileLocation(username, Path.GetFileName(filename));
        var userDir = GetAbsoluteUserDirectory(location.Username);
        var destPath = GetAbsoluteFilePath(location);

        if (stream == null || stream.Length == 0)
        {
            result.Error = $"File with name {filename} is empty.";

            return result;
        }

        if (File.Exists(destPath))
        {
            result.Error = $"File with name {filename} already exists.";

            return result;
        }

        try
        {
            EnsureUserDirectoryExists(userDir);
        }
        catch (IOException ex)
        {
            _log.LogError(ex, "Could not create user directory {UserDirectory}", userDir);

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
            catch (IOException ex)
            {
                _log.LogError(
                    ex,
                    "Unable to save file upload for {Username} with filename {Filename}, and absolute path {DestinationPath}.",
                    location.Username,
                    location.Filename,
                    destPath);

                result.Error = $"There was an error trying to save {filename}.";
            }
        }

        return result;
    }

    public Stream? GetThumbnail(ClaimsPrincipal user, string relativePath, int maxDimension)
    {
        FileLocation location;

        location = GetValidatedLocation(user, relativePath, true);

        var absolutePath = Path.Combine(_cfg.RootDirectory, location.RelativePath);

        if (!File.Exists(absolutePath))
        {
            return null;
        }

        using var image = Image.Load(absolutePath);

        if (image.Width > 256 || image.Height > 256)
        {
            // crop 20%
            var xOffset = Convert.ToInt32(image.Width * 0.1);
            var yOffset = Convert.ToInt32(image.Height * 0.1);
            var newWidth = image.Width - (2 * xOffset);
            var newHeight = image.Height - (2 * yOffset);

            var rect = new Rectangle(xOffset, yOffset, newWidth, newHeight);

            image.Mutate(x => x.Crop(rect));
        }

        var leftPos = (image.Width / 2) - (maxDimension / 2);
        var topPos = (image.Height / 2) - (maxDimension / 2);

        var ms = new MemoryStream();

        image.Mutate(x => x.Resize(maxDimension, maxDimension, KnownResamplers.Lanczos3));

        image.SaveAsJpeg(ms);

        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    IEnumerable<UploadedFile> GetAllFiles()
    {
        var list = new List<UploadedFile>();

        var userDirs = Directory.GetDirectories(_cfg.RootDirectory);

        foreach (var userDir in userDirs)
        {
            list.AddRange(GetFilesInDirectory(userDir));
        }

        return list;
    }

    IEnumerable<UploadedFile> GetUserFiles(string username)
    {
        var userDir = GetAbsoluteUserDirectory(username);

        if (!Directory.Exists(userDir))
        {
            return new List<UploadedFile>();
        }

        return GetFilesInDirectory(userDir);
    }

    static IEnumerable<UploadedFile> GetFilesInDirectory(string path)
    {
        var dir = new DirectoryInfo(path);

        return dir
            .EnumerateFiles()
            .Select(fi => new UploadedFile
            {
                Location = new FileLocation(dir.Name, fi.Name),
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

    static void EnsureUserDirectoryExists(string userDir)
    {
        if (!Directory.Exists(userDir))
        {
            Directory.CreateDirectory(userDir);
        }
    }

    static bool UserCanAccessFile(ClaimsPrincipal user, FileLocation location)
    {
        return user.IsAdmin() ||
            string.Equals(location.Username, user.Identity?.Name, StringComparison.OrdinalIgnoreCase);
    }

    UploadedFile GetFileDetails(FileLocation location)
    {
        var fi = new FileInfo(GetAbsoluteFilePath(location));

        return new UploadedFile
        {
            Location = location,
            CreationTime = fi.CreationTime,
            SizeInBytes = fi.Length
        };
    }

    FileLocation GetValidatedLocation(ClaimsPrincipal user, string relativePath, bool verifyPermissions)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentNullException(nameof(relativePath));
        }

        FileLocation location;

        try
        {
            location = FileLocation.FromRelativePath(relativePath);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unable to parse relative path [{RelativePath}].", relativePath);

            throw;
        }

        if (verifyPermissions && !UserCanAccessFile(user, location))
        {
            _log.LogWarning("User [{Username}] does not have access to [{File}].", location.Username, location.RelativePath);

            throw new ApplicationException();
        }

        return location;
    }
}
