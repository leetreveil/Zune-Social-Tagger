using System;
using System.IO;
using ASFTag;
using Id3Tag;
using TagLib;
using ZuneSocialTagger.Core.IO.ID3Tagger;
using ZuneSocialTagger.Core.IO.WMATagger;
using File = System.IO.File;

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
                    return new ZuneMP3TagContainer(TagLib.File.Create(path));
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