using Capstone.Common;
using Capstone.SpeechRecognition;
using System;
using Windows.UI.Popups;
using Capstone.Actions;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Capstone.Actions;
using Capstone.Common;

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
            ActionRouter.SetUp();

            // DEBUG -- uncomment this code if you want to test out the weather service.
            //string command = "what's the weather?";
            //Func<string, Actions.Action> foundAction = ActionRouter.GetFunctionFromCommandString(command);
            //foundAction(command).PerformAction(this.media);
			// END DEBUG
			
            // prevent the application from closing when the user hits the x button. This will alarms and notifications to still trigger
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += CloseHandle;
            Window.Current.SizeChanged += SizeChangedHandler;
            if (!ActionRouter.IsSetup)
            {
                ActionRouter.SetUp();
            }
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
        private async void CloseHandle(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            // stop the event from continuing
            e.Handled = true;
            IList<AppDiagnosticInfo> infos = await AppDiagnosticInfo.RequestInfoForAppAsync();
            IList<AppResourceGroupInfo> resourceInfos = infos[0].GetResourceGroups();
            await resourceInfos[0].StartSuspendAsync();
        }

        private void SizeChangedHandler(object sender, WindowSizeChangedEventArgs e)
        {
            if (e.Size.Height < 600)
            {
                // reduce the height of the text box row to give the dynamic area more space to display content
                this.TextBoxRow.Height = new GridLength(112);
            }
            else
            {
                this.TextBoxRow.Height = new GridLength(224);
            }
        }

        private void CommandBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && this.CommandBox.Text.Trim().Length > 0)
            {
                string text = this.CommandBox.Text;
                this.CommandBox.Text = "";
                this.performActionFromCommandBoxText(text);
            }
        }

        private void performActionFromCommandBoxText(string text)
        {
            // get the action for the text in the text box
            Func<string, Actions.Action> actionPrimer = ActionRouter.GetFunctionFromCommandString(text);
            if (actionPrimer != null)
            {
                Actions.Action action = actionPrimer.Invoke(text);
                action.PerformAction(this.media, this.DynamicArea);
            }
            else
            {
                // TODO pull this response from the database once the story to create bob responses is done
                string message = "Sorry, I don't understand.";
                string ssml = new SSMLBuilder().Prosody(message, contour: "(5%, +10%) (30%, -10%) (80%, +0.5%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.media, ssml);
            }
        }
    }
}
