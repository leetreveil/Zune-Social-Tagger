using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    public static class ExtensionMethods
    {
      /// <summary>
      /// A simple WPF threading extension method, to invoke a delegate
      /// on the correct thread if it is not currently on the correct thread
      /// Which can be used with DispatcherObject types
      /// </summary>
      /// <param name="disp">The Dispatcher object on which to do the Invoke</param>
      /// <param name="dotIt">The delegate to run</param>
      /// <param name="priority">The DispatcherPriority</param>
      public static void InvokeIfRequired(this Dispatcher disp, Action dotIt, DispatcherPriority priority)
      {
          if (disp.Thread != Thread.CurrentThread)
          {
              disp.Invoke(priority, dotIt);
          }
          else
              dotIt();
      }
    }
}
