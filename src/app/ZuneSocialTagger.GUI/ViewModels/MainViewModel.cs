using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using ZuneSocialTagger.GUI.Commands;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private DelegateCommand<string> _getAlbumInfoCommand;

        public ICommand GetAlbumInfoCommand
        {
            get
            {
                if (_getAlbumInfoCommand == null)
                {
                    _getAlbumInfoCommand = new DelegateCommand<string>(DownloadUrl);
                }
                return _getAlbumInfoCommand;
            }
        }

        private void DownloadUrl(string url)
        {

    
        }
    }
}
