using System.Collections.Generic;
using System.IO;


namespace Maw.Domain.Photos
{
    public interface IPhotoZipper
    {
        Stream Zip(IEnumerable<Photo> photos);
    }
}
