namespace Maw.Domain.Upload;

public class FileOperationResult
{
    public FileOperation Operation { get; set; }
    public string RelativePathSpecified { get; set; }
    public UploadedFile UploadedFile { get; set; }
    public bool WasSuccessful { get; set; }
    public string Error { get; set; }
}
