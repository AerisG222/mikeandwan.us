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
#pragma warning disable CA1303
                throw new ArgumentException("Invalid file path");
#pragma warning restore CA1303
            }

            var parts = relativePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            if(parts.Length != 2)
            {
#pragma warning disable CA1303
                throw new ArgumentException("Invalid file path");
#pragma warning restore CA1303
            }

            return new FileLocation {
                Username = parts[0],
                Filename = parts[1]
            };
        }
    }
}
