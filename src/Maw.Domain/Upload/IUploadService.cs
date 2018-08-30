using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Maw.Domain.Upload
{
    public interface IUploadService
    {
        FileOperationResult DeleteFile(ClaimsPrincipal user, string relativePath);
        IEnumerable<FileOperationResult> DeleteFiles(ClaimsPrincipal user, IEnumerable<string> relativePaths);
        Task<Stream> GetFileAsync(ClaimsPrincipal user, string relativePath);
        Task<Stream> GetFilesAsync(ClaimsPrincipal user, IEnumerable<string> relativePaths);
        IEnumerable<UploadedFile> GetFileList(ClaimsPrincipal user);
        Task<FileOperationResult> SaveFileAsync(ClaimsPrincipal user, string filename, Stream stream);
    }
}
