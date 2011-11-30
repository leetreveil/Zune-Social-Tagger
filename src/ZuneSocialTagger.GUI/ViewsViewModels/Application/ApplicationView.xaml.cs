using System.Windows;
using System.Windows.Input;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Shared;
using GalaSoft.MvvmLight.Messaging;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : DraggableWindow
    {
        public ApplicationView()
        {
            InitializeComponent();
        }

        void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string url = "";

            string business = "leetreveil@gmail.com";
            string lc = "GB";
            string item_name = "Zune Social Tagger";
            string no_note = "0";
            string currency_code = "USD";

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + lc +
                "&item_name=" + item_name +
                "&no_note=" + no_note +
                "&currency_code=" + currency_code +
                "&bn=" + "PP-DonationsBF:btn_donate_SM.gif:NonHostedGuest";

            System.Diagnostics.Process.Start(url);
        }
    }
}
