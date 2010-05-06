using System;

namespace ZuneSocialTagger.Core.IO
{
    public class AudioFileReadException : Exception
    {
        public AudioFileReadException(string message, Exception innerException):base(message,innerException)
        {
            
        }

        public AudioFileReadException(string message):base(message)
        {
            
        }
    }
}