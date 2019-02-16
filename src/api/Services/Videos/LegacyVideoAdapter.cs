using System;
using System.Collections.Generic;
using System.Linq;
using Maw.Domain.Videos;
using MawApi.Services.Videos;


namespace MawApi.Services.Videos
{
    public class LegacyVideoAdapter
    {
        readonly LegacyMultimediaInfoAdapter _adapter;


        public LegacyVideoAdapter(
            LegacyVideoCategoryAdapter categoryAdapter,
            LegacyMultimediaInfoAdapter adapter)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }


        public MawApi.ViewModels.LegacyVideos.Video Adapt(Video v)
        {
            return new MawApi.ViewModels.LegacyVideos.Video {
                Id = (short)v.Id,
                Duration = (short)v.Duration,
                ScaledVideo = _adapter.Adapt(v.VideoScaled),
                FullsizeVideo = _adapter.Adapt(v.VideoFull),
                ThumbnailVideo = _adapter.Adapt(v.Thumbnail)
            };
        }


        public IEnumerable<MawApi.ViewModels.LegacyVideos.Video> Adapt(IEnumerable<Video> videos)
        {
            return videos.Select(v => Adapt(v));
        }
    }
}
