using System;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Success
{
    public class SuccessViewModel : ViewModelBase
    {
        private readonly IApplicationViewModel _avm;

        public SuccessViewModel(IApplicationViewModel avm)
        {
            _avm = avm;
            this.AlbumDetailsFromFile = avm.AlbumDetailsFromFile;
            this.AlbumDetailsFromWebsite = avm.AlbumDetailsFromWeb;

            this.OKCommand = new RelayCommand(OkClicked);
        }

        private void OkClicked()
        {
            _avm.SwitchToFirstView();
            _avm.NotifyAppThatAnAlbumHasBeenLinked();
        }

        public RelayCommand OKCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; private set; }
    }
}