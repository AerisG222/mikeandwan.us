using System;
using Maw.Domain;
using Maw.Domain.Photos;
using MawApi.ViewModels;
using MawApi.ViewModels.LegacyVideos;
using MawApi.ViewModels.Photos;


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
