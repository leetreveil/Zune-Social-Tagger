using System;
using System.Windows.Input;
using ID3Tag;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;
using ZuneSocialTagger.Core.ZuneWebsite;

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
                    row.Container.Add(new MediaIdGuid(MediaIds.ZuneAlbumMediaID, row.AlbumDetails.AlbumMediaID));
                    row.Container.Add(new MediaIdGuid(MediaIds.ZuneAlbumArtistMediaID, row.SelectedSong.ArtistMediaID));
                    row.Container.Add(new MediaIdGuid(MediaIds.ZuneMediaID, row.SelectedSong.MediaID));


                    if (Properties.Settings.Default.UpdateAlbumInfo)
                        if (row.AlbumDetails.HasAllMetaData)
                            if (row.SelectedSong.HasAllMetaData)
                            {
                                var converter = new TrackAndAlbumToMetaDataConverter(row.AlbumDetails, row.SelectedSong);

                                if (converter.CanConvert)
                                {
                                    MetaData metaData = converter.Convert();

                                    if (metaData.IsValid)
                                        row.Container.WriteMetaData(metaData);
                                }
                            }

                    Id3TagManager.WriteV2Tag(row.FilePath, row.Container.GetContainer());
                }
                catch (Exception ex)
                {
                    //TODO: better error handling
                    Console.WriteLine("error saving {0}", row.FilePath);
                }
            }

            Mouse.OverrideCursor = null;
            var successView = new SuccessView(new SuccessViewModel(_model)) {ShowInTaskbar = false, Topmost = true};
            successView.Show();
        }
    }
}