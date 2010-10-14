using System;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.MoreInfo
{
    public class MoreInfoRow
    {
        public TrackWithTrackNum TrackFromFile { get; set; }
        public Uri LinkStatusImage { get; set; }
        public string LinkStatusText { get; set; }
        public TrackWithTrackNum TrackFromWeb { get; set; }
    }
}