using System.Collections.Generic;
using System.Windows.Input;
using ZuneSocialTagger.GUI.Commands;
using System.Windows.Forms;
using System.IO;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SelectAudioFilesViewModel : ViewModelBase
    {
        private DelegateCommand _fromFolderCommand;
        private DelegateCommand _fromFilesCommand;

        public ICommand FromFolderCommand
        {
            get
            {
                if (_fromFolderCommand == null)
                    _fromFolderCommand = new DelegateCommand(SelectFolder);

                return _fromFolderCommand;
            }
        }

        public ICommand FromFilesCommand
        {
            get
            {
                if (_fromFilesCommand == null)
                    _fromFilesCommand = new DelegateCommand(SelectFiles);

                return _fromFilesCommand;
            }
        }

        private static void SelectFolder()
        {
            var fbd = new FolderBrowserDialog {ShowNewFolderButton = false};

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath,"*.mp3");

                IEnumerable<FilePathAndContainer> containers = CreateContainerFromFiles(files);


            }
        }

        private static void SelectFiles()
        {
            var ofd = new OpenFileDialog {Multiselect = true, Filter = "Audio files (*.mp3)|*.mp3"};

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                IEnumerable<FilePathAndContainer> containers = CreateContainerFromFiles(ofd.FileNames);
            }
        }

        private static IEnumerable<FilePathAndContainer> CreateContainerFromFiles(IEnumerable<string> filePaths)
        {
            foreach (var file in filePaths)
            {
                yield return  new FilePathAndContainer() {FilePath = file, Container = ZuneTagContainerFactory.GetContainer(file)}; 
            }
        }
    }
}