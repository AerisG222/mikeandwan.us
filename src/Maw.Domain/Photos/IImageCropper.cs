using System.IO;
using Microsoft.Extensions.FileProviders;


namespace Maw.Domain.Photos
{
    public interface IImageCropper
    {
        Stream CropImage(string path, int maxDimension);
    }
}
