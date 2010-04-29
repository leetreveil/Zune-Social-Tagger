using System.Windows;
using System;

namespace ZuneSocialTagger.GUI.Controls
{
    /// <summary>
    /// Interaction logic for ErrorReportDialog.xaml
    /// </summary>
    public partial class ErrorReportDialog : DraggableWindow
    {
        private readonly Action _sendErrorCallback;
        private readonly Action _cancelCallback;

        public static void Show(string errorReport, Action sendErrorCallback, Action cancelCallback)
        {
            new ErrorReportDialog(errorReport,sendErrorCallback,cancelCallback).Show();
        }

        public ErrorReportDialog(string errorReport, Action sendErrorCallback, Action cancelCallback)
        {
            _sendErrorCallback = sendErrorCallback;
            _cancelCallback = cancelCallback;
            InitializeComponent();

            this.tbErrorReport.Text = errorReport;
        }

        private void expMoreInfo_Expanded(object sender, RoutedEventArgs e)
        {
            //hack to get the buttons at the bottom to be in the correct position after the 
            //expander control has expanded
            this.btnSendErrorRep.Margin = new Thickness(0,20,0,0);
            this.btnCancel.Margin = new Thickness(10,20,0,0);
        }

        private void expMoreInfo_Collapsed(object sender, RoutedEventArgs e)
        {
            this.btnSendErrorRep.Margin = new Thickness(0, -20, 0, 0);
            this.btnCancel.Margin = new Thickness(10, -20, 0, 0);
        }

        private void btnSendErrorRep_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;

            _sendErrorCallback();

            this.Cursor = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cancelCallback();
        }
    }
}
