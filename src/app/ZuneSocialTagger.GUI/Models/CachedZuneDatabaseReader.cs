using System;
using System.Collections.Generic;
using System.IO;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUI.ViewModels;
using ZuneSocialTagger.GUI.Properties;

namespace ZuneSocialTagger.GUI.Models
{
    public class CachedZuneDatabaseReader
    {
        private List<AlbumDetailsViewModel> _deserializedAlbums;

        public event Action FinishedReadingAlbums = delegate { };
        public event Action StartedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public bool CanInitialize
        {
            get { return true; }
        }

        public bool Initialize()
        {
            try
            {
                using (var fs = new FileStream(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache.xml"), FileMode.Open))
                    _deserializedAlbums = fs.XmlDeserializeFromStream<List<AlbumDetailsViewModel>>();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public IEnumerable<AlbumDetailsViewModel> ReadAlbums()
        {
            StartedReadingAlbums.Invoke();

            int counter = 0;

            foreach (var album in _deserializedAlbums)
            {
                counter++;
                yield return album;

                ProgressChanged.Invoke(counter, _deserializedAlbums.Count);
            }

            FinishedReadingAlbums.Invoke();
        }
    }
}