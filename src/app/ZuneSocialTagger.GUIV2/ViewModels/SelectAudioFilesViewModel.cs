using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
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
                    _model.Rows.Add(new DetailRow(ZuneTagContainerFactory.GetContainer(filePath), filePath));


                SetModelDetailsFromFirstAudioFile(files.Count(), _model.Rows.First().TagContainer.ReadMetaData());

                base.OnMoveNextOverride();
            }
            catch (ID3TagException id3TagException)
            {
                Console.WriteLine(id3TagException);
                ErrorMessageBox.Show("Error reading album " + Environment.NewLine + id3TagException.Message);
            }
        }

        private void SetModelDetailsFromFirstAudioFile(int songCount, MetaData songMetaData)
        {
            _model.AlbumDetailsFromFile = new WebsiteAlbumMetaDataViewModel
                                              {
                                                  Artist = songMetaData.AlbumArtist,
                                                  Title = songMetaData.AlbumTitle,
                                                  Year = songMetaData.Year,
                                                  SongCount = songCount.ToString(),
                                              };


            //add info so search bar displays what the album artist and album title from 
            //the album that has been selected

            _model.SearchBarViewModel.SearchText = songMetaData.AlbumTitle + " " +
                                                   songMetaData.AlbumArtist;
        }
    }
}