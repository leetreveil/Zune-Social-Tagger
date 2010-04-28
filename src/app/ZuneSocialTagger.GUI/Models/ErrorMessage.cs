using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{
    public class ErrorMessage
    {
        public ErrorMessage(ErrorMode errorMode, string message)
        {
            this.ErrorMode = errorMode;
            this.Message = message;
        }
        public ErrorMode ErrorMode { get; set; }
        public string Message { get; set; }
    }
}
