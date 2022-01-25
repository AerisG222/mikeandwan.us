namespace Maw.Domain.Models.Upload;

public class FileOperationResult
{
    public FileOperation Operation { get; set; }
    public string RelativePathSpecified { get; set; } = null!;
    public UploadedFile UploadedFile { get; set; } = null!;
    public bool WasSuccessful { get; set; }
    public string? Error { get; set; }
}
