using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using FilePathAndContainer=ZuneSocialTagger.GUIV2.Models.FilePathAndContainer;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : ZuneWizardPageViewModelBase
    {
        private RelayCommand _fromFolderCommand;

        internal override bool IsValid()
        {
            return true;
        }

        //private DelegateCommand _fromFolderCommand;
        //private DelegateCommand _fromFilesCommand;

        public ICommand FromFolderCommand
        {
            get
            {
                if (_fromFolderCommand == null)
                    _fromFolderCommand = new RelayCommand(SelectFolder);

                return _fromFolderCommand;
            }
        }

        //public ICommand FromFilesCommand
        //{
        //    get
        //    {
        //        if (_fromFilesCommand == null)
        //            _fromFilesCommand = new DelegateCommand(SelectFiles);

        //        return _fromFilesCommand;
        //    }
        //}

        private void SelectFolder()
        {
            var fbd = new FolderBrowserDialog { ShowNewFolderButton = false };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath, "*.mp3");

                IEnumerable<FilePathAndContainer> containers = CreateContainerFromFiles(files);

                ZuneTagContainer container = containers.Select(x=> x.Container).First();

                MetaData data = container.ReadMetaData();
            
                var metaData = new ZuneNetAlbumMetaData
                                   {
                                       Artist = data.AlbumArtist,
                                       Title = data.AlbumTitle,
                                       Year = data.Year,
                                       SongCount = container.Count().ToString()
                                   };


                ZuneWizardModel.GetInstance().InvokeAlbumMetaDataChanged(metaData);
            }
        }

        //private static void SelectFiles()
        //{
        //    var ofd = new OpenFileDialog {Multiselect = true, Filter = "Audio files (*.mp3)|*.mp3"};

        //    if (ofd.ShowDialog() == DialogResult.OK)
        //    {
        //        IEnumerable<FilePathAndContainer> containers = CreateContainerFromFiles(ofd.FileNames);
        //    }
        //}

        private static IEnumerable<FilePathAndContainer> CreateContainerFromFiles(IEnumerable<string> filePaths)
        {
            foreach (var file in filePaths)
            {
                yield return new FilePathAndContainer { FilePath = file, Container = ZuneTagContainerFactory.GetContainer(file) };
            }
        }
    }
}