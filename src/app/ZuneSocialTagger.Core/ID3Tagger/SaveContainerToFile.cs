using ID3Tag;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class SaveContainerToFile
    {
        private readonly FilePathAndContainer _filePathAndContainer;

        public SaveContainerToFile(FilePathAndContainer filePathAndContainer)
        {
            _filePathAndContainer = filePathAndContainer;
        }

        public void Save()
        {
            Id3TagManager.WriteV2Tag(_filePathAndContainer.FilePath,_filePathAndContainer.Container.GetContainer());
        }
    }
}