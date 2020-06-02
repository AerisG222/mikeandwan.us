using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using NMagickWand;


namespace Maw.Domain.Photos
{
    public class ImageCropper
        : IImageCropper
    {
        readonly IFileProvider _fileProvider;


        public ImageCropper(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }


        public Stream CropImage(string path, int maxDimension)
        {
            var fi = _fileProvider.GetFileInfo(path);

            if(fi.Exists)
            {
                using var mw = new MagickWand(fi.PhysicalPath);
                var leftPos = (mw.ImageWidth / 2) - (maxDimension / 2);
                var topPos = (mw.ImageHeight / 2) - (maxDimension / 2);

                var ms = new MemoryStream();

                mw.CropImage((uint)maxDimension, (uint)maxDimension, (int)leftPos, (int)topPos);
                mw.WriteImage(ms);

                ms.Seek(0, SeekOrigin.Begin);

                return ms;
            }

            return null;
        }
    }
}
