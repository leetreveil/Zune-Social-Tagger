using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using Screen = Caliburn.PresentationFramework.Screens.Screen;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;

        public SelectAudioFilesViewModel(IUnityContainer container, IZuneWizardModel model)
        {
            _container = container;
            _model = model;
        }

        public void SelectFiles()
        {
            var ofd = new OpenFileDialog { Multiselect = true, Filter = "Audio files |*.mp3;*.wma" + "|All Files|*.*" };

            if (ofd.ShowDialog() == DialogResult.OK)
                ReadFiles(ofd.FileNames);
        }

        private void ReadFiles(IEnumerable<string> files)
        {
            _model.Rows = new ObservableCollection<DetailRow>();

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

                _model.CurrentPage = _container.Resolve<SearchViewModel>();
            }
            catch (Exception id3TagException)
            {
                ErrorMessageBox.Show("Error reading album " + Environment.NewLine + id3TagException.Message);
            }
        }

        /// <summary>
        /// converts TrackNumber string into an int
        /// </summary>
        /// <returns></returns>
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