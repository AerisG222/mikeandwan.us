using System.Collections.Generic;
using System.IO;
using Maw.Domain.Models.Photos;

namespace Maw.Domain.Photos;

public interface IPhotoZipper
{
    Stream Zip(IEnumerable<Photo> photos);
}
