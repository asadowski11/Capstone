using Capstone.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Capstone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private bool IsMenuExpanded = false;
        public SettingsPage()
        {
            this.InitializeComponent();
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

        private void BGRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb != null )
            {
                
            }
        }

        private void BorderRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb != null )
            {
                
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var taskList = new List<Task>();
            //taskList.Add(SaveMapSelection());
            taskList.Add(SaveSearchEngineSelection());

            await Task.WhenAll(taskList);
        }

        private async Task SaveSearchEngineSelection()
        {

            if (DuckDuckGo.IsChecked == true)
            {
                StoredProcedures.UpdateSettings(2, true);
            }
            if (Google.IsChecked == true)
            {
                StoredProcedures.UpdateSettings(1, true);
            }
            if (Bing.IsChecked == true)
            {
                StoredProcedures.UpdateSettings(3, true);
            }
        }

        //private async Task SaveMapSelection()
        //{
        //
        //    if (StreetMap.IsChecked == true)
        //    {
        //        StoredProcedures.UpdateSettings(1, false);
        //    }
        //    if (BingMap.IsChecked == true)
        //    {
        //        StoredProcedures.UpdateSettings(1, false);
        //    }
        //    if (GoogleMap.IsChecked == true)
        //    {
        //       StoredProcedures.UpdateSettings(1, false);
        //    }
        //}


    }
}
