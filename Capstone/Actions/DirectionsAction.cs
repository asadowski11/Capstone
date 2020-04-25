using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Capstone.Common;
using Capstone.Models;
using Capstone.Providers;
using Capstone.SpeechRecognition;
using Windows.UI.Xaml.Controls;

namespace Capstone.Actions
{
    public class DirectionsAction : Action
    {

        private string DestinationName { get; set; }
        // TODO change to a different type once we know more about the location stuff for objects
        private string StartingPoint { get; set; }

        public DirectionsAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        private void getDestinationName()
        {
            // TODO ask for destination name and set it to our destinationName


        }
        private async Task<string> InputTextDialogAsync(string title)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Ok";
            dialog.SecondaryButtonText = "Cancel";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "";
        }

        private void getStartingPointName()
        {
            // TODO ask for starting location name and set it to our startingPoint
        }

        public async override void PerformAction()
        {
            CommandString = CommandString.ToUpper();
            string strDestination = "";
            string successMessage = "alright, getting directions to {destination}";
            if (CommandString.Contains(" TO "))
            {
                strDestination = CommandString.Substring(CommandString.IndexOf(" TO ") + 4);
            }
            else
            {
                // have bob ask the user where they want to go
                TextToSpeechEngine.SpeakText(this.MediaElement, "Sure, where do you want to go?");
                // sleep the thread to give bob enough time to speak
                if (!await SpeechRecognitionManager.RequestListen(this.GetType(), (text) =>
                {
                    strDestination = text;
                    GetDirections(text);
                }))
                {
                    string message = "Sorry, but something went wrong. To get directions, say \"Hey Bob, how do I get to thePlace\"";
                    TextToSpeechEngine.SpeakText(this.MediaElement, message);
                    this.ShowMessage(message);
                }
                else
                {
                    ProvideDirectionsSuccessMessage(strDestination);
                }
            }
            if (StringUtils.IsNotBlank(strDestination))
            {
                GetDirections(strDestination);
                ProvideDirectionsSuccessMessage(strDestination);
            }
        }
        private async void ProvideDirectionsSuccessMessage(string destination)
        {
            // show a link to the search 
            this.ClearArea();
            var linkElement = new HyperlinkButton();
            linkElement.Content = $"Directions to {destination.ToLower()}";
            linkElement.NavigateUri = new Uri(await GetDirectionsLink(destination));
            linkElement.FontSize = 24;
            RelativePanel.SetAlignHorizontalCenterWithPanel(linkElement, true);
            RelativePanel.SetAlignVerticalCenterWithPanel(linkElement, true);
            this.DynamicArea.Children.Add(linkElement);
            TextToSpeechEngine.SpeakText(this.MediaElement, $"Alright, getting {linkElement.Content.ToString().ToLower()}");
        }

        private async void GetDirections(string destination)
        {
            string query = await GetDirectionsLink(destination);
            var uriMap = new Uri(query);
            var success = await Windows.System.Launcher.LaunchUriAsync(uriMap);
        }

        private async Task<string> GetDirectionsLink(string destination)
        {
            Dictionary<string, double> coordinates = await LocationProvider.GetLatitudeAndLongitude();
            double latitude = coordinates["latitude"];
            double longitude = coordinates["longitude"];
            Setting setting = StoredProcedures.QuerySettingByName("Map Provider");
            MapProvider mapProvider = StoredProcedures.QueryMapProvider(setting.GetSelectedOption().DisplayName);
            string query = mapProvider.BaseURL.ToString();
            query = query.Replace("{Latitude}", latitude.ToString()).Replace("{Longitude}", longitude.ToString()).Replace("{Destination}", HttpUtility.UrlEncode(destination));
            return query;

        }
    }
}