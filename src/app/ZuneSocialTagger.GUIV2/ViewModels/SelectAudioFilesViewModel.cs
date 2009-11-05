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
        private RelayCommand _fromFolderCommand;
        private RelayCommand _fromFilesCommand;

        internal override bool IsValid()
        {
            return true;
        }

        internal override bool CanMoveNext()
        {
            return false;
        }

        public ICommand FromFolderCommand
        {
            get
            {
                if (_fromFolderCommand == null)
                    _fromFolderCommand = new RelayCommand(SelectFolder);

                return _fromFolderCommand;
            }
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

        private void SelectFolder()
        {
            var fbd = new FolderBrowserDialog { ShowNewFolderButton = false };

            if (fbd.ShowDialog() == DialogResult.OK)
            {

                string[] files = Directory.GetFiles(fbd.SelectedPath, "*.mp3");
                ReadFiles(files);
            }
        }

        private void ReadFiles(string[] files)
        {
            WebsiteAlbumMetaDataViewModel albumDetailsFromFile = ZuneWizardModel.GetInstance().AlbumDetailsFromFile;
            ObservableCollection<DetailRow> detailViewRows = ZuneWizardModel.GetInstance().Rows;

            detailViewRows.Clear();

            try
            {
                IEnumerable<FilePathAndContainer> containers = CreateContainerFromFiles(files);

                int counter = 0;
                foreach (var cont in containers)
                {
                    counter++;
                    MetaData metaData = cont.Container.ReadMetaData();

                    detailViewRows.Add(new DetailRow(new SongWithNumberAndGuid { Title = metaData.SongTitle, Number = counter.ToString() }) { SongPathAndContainer = cont });
                }


                ZuneTagContainer container = containers.Select(x => x.Container).First();

                MetaData data = container.ReadMetaData();

                albumDetailsFromFile.Artist = data.AlbumArtist;
                albumDetailsFromFile.Title = data.AlbumTitle;
                albumDetailsFromFile.Year = data.Year;
                albumDetailsFromFile.SongCount = counter.ToString();

                base.OnMoveNextOverride();
            }
            catch (ID3TagException id3TagException)
            {
                //TODO: display the error from reading the tags
               Console.WriteLine("could not read one or more audio files");
            }


        }

        private void SelectFiles()
        {
            var ofd = new OpenFileDialog { Multiselect = true, Filter = "Audio files (*.mp3)|*.mp3" };

            if (ofd.ShowDialog() == DialogResult.OK)
                ReadFiles(ofd.FileNames);
        }

        private static IEnumerable<FilePathAndContainer> CreateContainerFromFiles(IEnumerable<string> filePaths)
        {
            foreach (var file in filePaths)
            {
                yield return new FilePathAndContainer { FilePath = file, Container = ZuneTagContainerFactory.GetContainer(file) };
            }
        }
    }
}