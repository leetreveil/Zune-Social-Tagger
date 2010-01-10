using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : ZuneWizardPageViewModelBase
    {
        private readonly ZuneWizardModel _model;
        private RelayCommand _fromFilesCommand;

        public SelectAudioFilesViewModel(ZuneWizardModel model)
        {
            _model = model;
        }

        internal override bool IsNextEnabled()
        {
            return false;
        }

        public ICommand FromFilesCommand
        {
            get
            {
                if (_fromFilesCommand == null)
                    _fromFilesCommand = new RelayCommand(SelectFiles);

                return _fromFilesCommand;
            }
        }

        private void SelectFiles()
        {
            var ofd = new OpenFileDialog { Multiselect = true, Filter = "Audio files |*.mp3;*.wma" };

            if (ofd.ShowDialog() == DialogResult.OK)
                ReadFiles(ofd.FileNames);
        }

        private void ReadFiles(IEnumerable<string> files)
        {
            _model.Rows = new ObservableCollection<DetailRow>();


            //TODO: need to sort by track no after loading
            try
            {
                foreach (var filePath in files)
                {
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(filePath);

                    //for each song in the album add the container to the models row list
                    _model.Rows.Add(new DetailRow(filePath, container));
                }

                //sort the rows by track number
                var sortedTracks = _model.Rows.OrderBy(SortByTrackNumber()).ToList();

                _model.Rows.Clear();

                foreach (var row in sortedTracks)
                    _model.Rows.Add(row);

                //takes the first track read from the model and updates the metadata view
                SetAlbumDetailsFromFile(_model.Rows.Count, _model.Rows.First().MetaData);

                base.OnMoveNextOverride();
            }
            catch (Exception id3TagException)
            {
                ErrorMessageBox.Show("Error reading album " + Environment.NewLine + id3TagException.Message);
            }
        }

        private static Func<DetailRow, int> SortByTrackNumber()
        {
            return key =>
                       {
                           int result;
                           Int32.TryParse(key.MetaData.TrackNumber, out result);

                           return result;
                       };
        }

        private void SetAlbumDetailsFromFile(int songCount, MetaData songMetaData)
        {
            _model.AlbumDetailsFromFile = new WebsiteAlbumMetaDataViewModel
                                              {
                                                  Artist = songMetaData.AlbumArtist,
                                                  Title = songMetaData.AlbumName,
                                                  Year = songMetaData.Year,
                                                  SongCount = songCount.ToString(),
                                              };

            //fall back to contributing artists if album artist is not available
            if (String.IsNullOrEmpty(songMetaData.AlbumArtist))
                _model.AlbumDetailsFromFile.Artist = songMetaData.ContributingArtists.FirstOrDefault();


            //add info so search bar displays the album artist and album title from 
            //the album that has been selected
            _model.SearchBarViewModel.SearchText = songMetaData.AlbumName + " " +
                                                   _model.AlbumDetailsFromFile.Artist;
        }
    }
}