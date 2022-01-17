using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Maw.Domain.Photos;

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

        if (fi.Exists)
        {
            using var image = Image.Load(fi.PhysicalPath);
            var leftPos = (image.Width / 2) - (maxDimension / 2);
            var topPos = (image.Height / 2) - (maxDimension / 2);

            var ms = new MemoryStream();

            var rect = new Rectangle(leftPos, topPos, maxDimension, maxDimension);
            image.Mutate(x => x.Crop(rect));
            image.SaveAsJpeg(ms);

            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        return null;
    }
}
