using Capstone.Common;
using Capstone.SpeechRecognition;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Capstone
{
    /// <summary>
    /// The main page that is shown to the user when they open up the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool IsMenuExpanded = false;
        public MainPage()
        {
            this.InitializeComponent();
            // hide the main menu
            this.MenuColumn.Width = new GridLength(0);
        }

        private void MenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!this.IsMenuExpanded)
            {
                // show the menu column
                this.MenuColumn.Width = new GridLength(320);
            }
            else
            {
                this.MenuColumn.Width = new GridLength(0);
            }
            this.IsMenuExpanded = !this.IsMenuExpanded;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO navigate to settings screen (this.Frame.Navigate(typeof(screenName)))
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private void RemindersButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RemindersPage));
        }

        private void AlarmsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AlarmsPage));
        }

        private void VoiceNotesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(VoiceMemosPage));
        }

        private void DataPrivacyButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DataPrivacyTips));
        }

        private void LibrariesButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO navigate to settings screen (this.Frame.Navigate(typeof(screenName)))
        }
    }
}
