using System;
using System.IO;


namespace Maw.Domain.Upload
{
    public class FileLocation
    {
        public string Username { get; set; }
        public string Filename { get; set; }

        public string RelativePath
        {
            get
            {
                return $"{Username}/{Filename}";
            }
        }


        public static FileLocation FromRelativePath(string relativePath) {
            if(string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            if(Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException("Invalid file path");
            }

            var parts = relativePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            if(parts.Length != 2)
            {
                throw new ArgumentException("Invalid file path");
            }

            return new FileLocation {
                Username = parts[0],
                Filename = parts[1]
            };
        }
    }
}
