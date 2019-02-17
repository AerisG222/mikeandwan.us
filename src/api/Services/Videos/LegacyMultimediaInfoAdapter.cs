using Maw.Domain;
using MawApi.ViewModels.LegacyVideos;


namespace MawApi.Services.Videos
{
    public class LegacyMultimediaInfoAdapter
    {
        public VideoInfo Adapt(MultimediaInfo info)
        {
            if(info == null)
            {
                return new VideoInfo();
            }

            return new VideoInfo {
                Height = info.Height,
                Width = info.Width,
                Path = info.Path
            };
        }
    }
}
