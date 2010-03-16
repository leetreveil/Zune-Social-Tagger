using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;
using Screen = Caliburn.PresentationFramework.Screens.Screen;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;

        public SelectAudioFilesViewModel(IUnityContainer container,
                                         IZuneWizardModel model)
        {
            _container = container;
            _model = model;
        }

        public void SwitchToNewMode()
        {
            _model.CurrentPage = _container.Resolve<WebAlbumListViewModel>();
        }

        public void SelectFiles()
        {
            var ofd = new OpenFileDialog { Multiselect = true, Filter = "Audio files |*.mp3;*.wma" };

            if (ofd.ShowDialog() == DialogResult.OK)
                ReadFiles(ofd.FileNames);
        }

        private void ReadFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                try
                {
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(file);

                    _model.Rows.Add(new DetailRow(file,container));

                    _model.Rows =  _model.Rows.OrderBy(SharedMethods.SortByTrackNumber()).ToBindableCollection();
                }
                catch (NotSupportedException ex)
                {
                    ZuneMessageBox.Show(ex.Message, ErrorMode.Error);
                    return;
                }
            }

            MetaData ftMetaData = _model.Rows.First().MetaData;

            var searchViewModel = _container.Resolve<SearchViewModel>();

            searchViewModel.SearchHeader.SearchBar.SearchText = ftMetaData.AlbumArtist + " " +
                                                                ftMetaData.AlbumName;

           
            searchViewModel.SearchHeader.AlbumDetails = new ExpandedAlbumDetailsViewModel
            {
                Artist = ftMetaData.AlbumArtist,
                Title = ftMetaData.AlbumName,
                SongCount = _model.Rows.Count.ToString(),
                Year = ftMetaData.Year
            };

            _model.CurrentPage = searchViewModel;
        }



    }
}