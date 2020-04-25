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
            CommandString = CommandString.ToUpper();
            string strDestination = "";
            if (CommandString.Contains(" TO "))
            {
                strDestination = CommandString.Substring(CommandString.IndexOf(" TO ") + 4);
            }
            else
            {
                strDestination = await InputTextDialogAsync("Destination");
            }
            if (StringUtils.IsNotBlank(strDestination))
            {
                Dictionary<string, double> coordinates = await LocationProvider.GetLatitudeAndLongitude();
                double latitude = coordinates["latitude"];
                double longitude = coordinates["longitude"];
                Setting setting = StoredProcedures.QuerySettingByName("Map Provider");
                MapProvider mapProvider = StoredProcedures.QueryMapProvider(setting.GetSelectedOption().DisplayName);
                string strQuery = mapProvider.BaseURL.ToString();
                strQuery = strQuery.Replace("{Latitude}", latitude.ToString()).Replace("{Longitude}", longitude.ToString()).Replace("{Destination}", HttpUtility.UrlEncode(strDestination));
                var uriMap = new Uri(strQuery);
                var success = await Windows.System.Launcher.LaunchUriAsync(uriMap);
            }  
        }
    }
}