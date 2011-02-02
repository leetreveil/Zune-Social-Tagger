using System;
using System.Windows;

namespace ZuneSocialTagger.GUI.Controls
{
    /// <summary>
    /// Interaction logic for ErrorReportDialog.xaml
    /// </summary>
    public partial class ErrorReportDialog : DraggableWindow
    {
        private readonly Action _sendErrorCallback;

        public static void Show(string errorReport, Action sendErrorCallback)
        {
            new ErrorReportDialog(errorReport,sendErrorCallback).Show();
        }

        public ErrorReportDialog(string errorReport, Action sendErrorCallback)
        {
            _sendErrorCallback = sendErrorCallback;
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            this.tbErrorReport.Text = errorReport;
        }

        private void expMoreInfo_Expanded(object sender, RoutedEventArgs e)
        {
            //hack to get the buttons at the bottom to be in the correct position after the 
            //expander control has expanded
            this.btnSendErrorRep.Margin = new Thickness(0,20,0,0);
        }

        private void expMoreInfo_Collapsed(object sender, RoutedEventArgs e)
        {
            this.btnSendErrorRep.Margin = new Thickness(0, -20, 0, 0);
        }

        private void btnSendErrorRep_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;

            _sendErrorCallback();

            this.Cursor = null;
        }
    }
}
