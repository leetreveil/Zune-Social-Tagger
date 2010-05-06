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
                        throw new Id3TagException("cannot read id3v1.1");

                    return new ZuneMP3TagContainer(tagManager.ReadV2Tag(path));
                }
                catch (Id3TagException ex)
                {
                    if (ex.InnerException != null)
                        throw new AudioFileReadException(ex.InnerException.Message, ex.InnerException);

                    throw new AudioFileReadException(ex.Message);
                }
            }

            if (extension.ToLower() == ".wma")
            {
                try
                {
                    return new ZuneWMATagContainer(ASFTagManager.ReadTag(path));
                }
                catch (Exception ex)
                {
                    throw new AudioFileReadException(ex.Message,ex);
                }
            }

            throw new AudioFileReadException("The " + Path.GetExtension(path) + " file extension is not supported with zune social tagger.");
        }
    }
}