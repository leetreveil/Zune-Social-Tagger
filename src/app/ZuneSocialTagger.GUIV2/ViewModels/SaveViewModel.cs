using System;
using System.Windows.Input;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SaveViewModel
    {
        private readonly ZuneWizardModel _model;
        private RelayCommand _saveCommand;

        public SaveViewModel(ZuneWizardModel model)
        {
            _model = model;
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new RelayCommand(() => this.Save());

                return _saveCommand;
            }
        }

        /// <summary>
        /// Takes the changes made by the user and updates each song and saves each song to file
        /// </summary>
        public void Save()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            foreach (var row in _model.Rows)
            {
                try
                {
                   var container = row.Container;

                    if (Properties.Settings.Default.UpdateAlbumInfo)
                            if (row.SelectedSong.HasAllMediaIDs)
                            {
                                container.AddZuneMediaId(new MediaIdGuid(MediaIds.ZuneAlbumMediaID, row.SelectedSong.AlbumMediaID));
                                container.AddZuneMediaId(new MediaIdGuid(MediaIds.ZuneAlbumArtistMediaID, row.SelectedSong.ArtistMediaID));
                                container.AddZuneMediaId(new MediaIdGuid(MediaIds.ZuneMediaID, row.SelectedSong.MediaID));
                                container.AddMetaData(row.SelectedSong.MetaData);

                                container.WriteToFile(row.FilePath);
                            }

                    //TODO: run a verifier over whats been written to ensure that the tags have actually been written to file
                }
                catch
                {
                    //TODO: better error handling
                    Console.WriteLine("error saving {0}", row.FilePath);
                }
            }

            Mouse.OverrideCursor = null;

            new SuccessView(new SuccessViewModel(_model)).Show();
        }
    }
}