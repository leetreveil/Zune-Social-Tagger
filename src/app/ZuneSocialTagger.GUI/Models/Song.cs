using ZuneSocialTagger.Core.IO;

namespace ZuneSocialTagger.GUI.Models
{
    /// <summary>
    /// Every row in the DetailsView has this information.
    /// </summary>
    public class Song
    {
        public MetaData MetaData { get; set; }
        public string FilePath { get; private set; }
        public IZuneTagContainer Container { get; set; }

        public Song(string filePath, IZuneTagContainer container)
        {
            FilePath = filePath;
            Container = container;
            this.MetaData = container.ReadMetaData();
        }

        public Song()
        {
  
        }
    }
}