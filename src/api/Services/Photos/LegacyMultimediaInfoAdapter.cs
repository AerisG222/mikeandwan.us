using Maw.Domain;
using MawApi.ViewModels.LegacyPhotos;


namespace MawApi.Services.Photos
{
    public class LegacyMultimediaInfoAdapter
    {
        public PhotoInfo Adapt(MultimediaInfo info)
        {
            if(info == null)
            {
                return new PhotoInfo();
            }

            return new PhotoInfo {
                Height = info.Height,
                Width = info.Width,
                Path = info.Path
            };
        }
    }
}
