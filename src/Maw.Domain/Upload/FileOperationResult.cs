namespace Maw.Domain.Upload
{
    public class FileOperationResult
    {
        public FileOperation Operation { get; set; }
        public FileLocation FileLocation { get; set; }
        public bool WasSuccessful { get; set; }
        public string Error { get; set; }
    }
}
