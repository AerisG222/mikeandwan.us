using System.IO;

namespace Maw.Domain.Photos;

public interface IImageCropper
{
    Stream? CropImage(string path, int maxDimension);
}
