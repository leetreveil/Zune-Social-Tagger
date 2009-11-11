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
            try
            {
                foreach (var row in _model.Rows)
                {
                    row.UpdateContainer();

                    var saveContainerToFile = new SaveContainerToFile(row.SongPathAndContainer);

                    saveContainerToFile.Save();
                }

                var successView = new SuccessView(new SuccessViewModel(_model)) {ShowInTaskbar = false, Topmost = true};
                successView.Show();
            }
            catch (Exception ex)
            {
                //TODO: better error handling
                Console.WriteLine("saving error");
            }

        }

    }
}