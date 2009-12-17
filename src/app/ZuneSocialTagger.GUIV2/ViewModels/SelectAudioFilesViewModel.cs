using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using ID3Tag;

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
            var ofd = new OpenFileDialog { Multiselect = true, Filter = "Audio files (*.mp3)|*.mp3" };

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

                    _model.Rows.Add(new DetailRow(filePath, container));
                    _model.Rows.Add(new DetailRow(filePath,container));
                }

                //takes the first track read from the model and updates the metadata view
                SetAlbumDetailsFromFile(_model.Rows.Count, _model.Rows.First().MetaData);

                base.OnMoveNextOverride();
            }
            catch (ID3TagException id3TagException)
            {
                Console.WriteLine(id3TagException);
                ErrorMessageBox.Show("Error reading album " + Environment.NewLine + id3TagException.Message);
            }
        }

        private void SetAlbumDetailsFromFile(int songCount, MetaData songMetaData)
        {
            _model.AlbumDetailsFromFile = new WebsiteAlbumMetaDataViewModel
                                              {
                                                  Artist = songMetaData.AlbumArtist,
                                                  Title = songMetaData.AlbumTitle,
                                                  Year = songMetaData.Year,
                                                  SongCount = songCount.ToString(),
                                              };

            //fall back to contributing artists if album artist is not available
            if (String.IsNullOrEmpty(songMetaData.AlbumArtist))
                _model.AlbumDetailsFromFile.Artist = songMetaData.ContributingArtist;


            //add info so search bar displays the album artist and album title from 
            //the album that has been selected
            _model.SearchBarViewModel.SearchText = songMetaData.AlbumTitle + " " +
                                                   songMetaData.AlbumArtist;
        }
    }
}