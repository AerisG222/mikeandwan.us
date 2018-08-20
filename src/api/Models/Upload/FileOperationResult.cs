namespace MawApi.Models.Upload
{
    public class FileOperationResult
    {
        public FileOperation Operation { get; set; }
        public string FileName { get; set; }
        public bool WasSuccessful { get; set; }
        public string Error { get; set; }
    }
}
