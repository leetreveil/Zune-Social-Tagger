using System;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel
    {
        private static ZuneWizardModel instance = new ZuneWizardModel();

        public event Action<ZuneNetAlbumMetaData> AlbumMetaDataChanged;
        
        public void InvokeAlbumMetaDataChanged(ZuneNetAlbumMetaData obj)
        {
            Action<ZuneNetAlbumMetaData> changed = AlbumMetaDataChanged;
            if (changed != null) changed(obj);
        }

        private ZuneWizardModel()
        {
        }

        public static ZuneWizardModel GetInstance()
        {
            return instance;
        }
    }
}