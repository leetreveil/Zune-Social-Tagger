using System;
using System.IO;
using ASFTag;
using Id3Tag;
using ZuneSocialTagger.Core.IO.ID3Tagger;
using ZuneSocialTagger.Core.IO.WMATagger;

namespace ZuneSocialTagger.Core.IO
{
    public static class ZuneTagContainerFactory
    {
        public static IZuneTagContainer GetContainer(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(String.Format("File does not exist: {0}", path), path);

            string extension = Path.GetExtension(path);

            if (extension.ToLower() == ".mp3")
            {
                try
                {
                    var tagManager = new Id3TagManager();

                    //TODO: app crashes here when a file is loaded from a remote directory, i.e. on network
                    FileState status = tagManager.GetTagsStatus(path);

                    //if we just have id3v1.1 tags
                    if (status.Id3V1TagFound && !status.Id3V2TagFound)
                        throw new Id3TagException("Couldn't read: " + path + " Error: " + " cannot read id3v1.1");

                    return new ZuneMP3TagContainer(tagManager.ReadV2Tag(path), path);
                }
                catch (Id3TagException ex)
                {
                    Exception excep = ex;

                    if (ex.InnerException != null)
                        excep = ex.InnerException;
                    
                    throw new AudioFileReadException("Couldn't read: " + path + " Error: " + ex.Message, excep);
                }
            }

            if (extension.ToLower() == ".wma")
            {
                try
                {
                    return new ZuneWMATagContainer(ASFTagManager.ReadTag(path), path);
                }
                catch (Exception ex)
                {
                    throw new AudioFileReadException("Couldn't read: " + path + " Error: " + ex.Message);
                }
            }

            throw new AudioFileReadException("Couldn't read: " + path + " Error: " +
                "The " + Path.GetExtension(path) +
                " file extension is not supported with zune social tagger");
        }
    }
}