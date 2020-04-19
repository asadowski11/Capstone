using Capstone.Common;
using Capstone.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.Web;
using Windows.Services.Maps;
using Windows.Devices.Geolocation;
using Capstone.Models;

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
            string strDestination = await InputTextDialogAsync("Destination");
            Dictionary<string, double> coordinates = await LocationProvider.GetLatitudeAndLongitude();
            double latitude = coordinates["latitude"];
            double longitude = coordinates["longitude"];
            Setting setting = StoredProcedures.QuerySettingByName("Search Engine");
            if (setting.GetSelectedOption().DisplayName == "Bing")
            {
                var uriMap = new Uri(@"bingmaps:?rtp=pos." + latitude + "_" + longitude + "~adr." + HttpUtility.UrlEncode(strDestination));
                var success = await Windows.System.Launcher.LaunchUriAsync(uriMap);
            }else
            {
                var uriMap = new Uri(@"https://www.google.com/maps/dir/?api=1&origin=" + latitude + "," + longitude + "&destination=" + HttpUtility.UrlEncode(strDestination));
                var success = await Windows.System.Launcher.LaunchUriAsync(uriMap);
            }
            
        }
    }
}
