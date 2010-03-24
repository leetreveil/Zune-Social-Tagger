using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
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
