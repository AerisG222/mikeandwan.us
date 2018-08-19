using System;


namespace MawApi.ViewModels.Upload
{
    public class UploadedFile
    {
        public string Username { get; set; }
        public string Filename { get; set; }
        public DateTime CreationTime { get; set; }
        public long SizeInBytes { get; set; }


        public string RelativePath
        {
            get
            {
                return $"{Username}/{Filename}";
            }
        }
    }
}
