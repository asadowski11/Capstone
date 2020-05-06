using Capstone.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Capstone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataPrivacyTips : Page
    {
        public DataPrivacyTips()
        {
            this.InitializeComponent();
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            UIUtils.GoToMainPage(this);
        }

        private void LibrariesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LibrariesWeUse));
        }
    }
}