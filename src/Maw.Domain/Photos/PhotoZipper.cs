using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Maw.Domain.Photos
{
    public class PhotoZipper
        : IPhotoZipper
    {
        const int TWENTY_MB = 20 * 1024 * 1024;

        readonly ILogger _log;
        readonly IFileProvider _fileProvider;


        public PhotoZipper(ILogger log,
                           IFileProvider fileProvider)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }


        public Stream Zip(IEnumerable<Photo> photos)
        {
            var ms = new MemoryStream(TWENTY_MB);

            using(var za = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach(var photo in photos)
                {
					var path = _fileProvider.GetFileInfo(photo.LgInfo.Path).PhysicalPath;

					_log.LogInformation(string.Format("Adding file {0} to archive", path));

					za.CreateEntryFromFile(path, Path.GetFileName(path));
                }
            }

			ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}
