using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel
    {
        #region SingletonStuff

        private static ZuneWizardModel instance = new ZuneWizardModel();

        private ZuneWizardModel()
        {
        }

        public static ZuneWizardModel GetInstance()
        {
            return instance;
        }

        #endregion

        public event Action<ZuneNetAlbumMetaData> AlbumMetaDataChanged = delegate { };
        public event Action<IEnumerable<AlbumArtistAndTitleWithUrl>> NewAlbumsAvail = delegate { };

        public void InvokeNewAlbumsAvailable(IEnumerable<AlbumArtistAndTitleWithUrl> albums)
        {
            Action<IEnumerable<AlbumArtistAndTitleWithUrl>> action = NewAlbumsAvail;
            if (action != null) action(albums);
        }

        public void InvokeAlbumMetaDataChanged(ZuneNetAlbumMetaData obj)
        {
            Action<ZuneNetAlbumMetaData> changed = AlbumMetaDataChanged;
            if (changed != null) changed(obj);
        }

    }
}