using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.Models;
using BobTheDigitalAssistant.Providers;
using BobTheDigitalAssistant.SpeechRecognition;
using Windows.UI.Xaml.Controls;

namespace BobTheDigitalAssistant.Actions
{
	public class DirectionsAction : Action
	{
		public DirectionsAction(string CommandString)
		{
			this.CommandString = CommandString;
		}

		public async override void PerformAction()
		{
			this.CommandString = this.CommandString.ToUpper();
			string strDestination = "";
			if (this.CommandString.Contains(" TO "))
			{
				strDestination = this.CommandString.Substring(this.CommandString.IndexOf(" TO ") + 4);
			}
			else
			{
				// have bob ask the user where they want to go
				TextToSpeechEngine.SpeakText(this.MediaElement, "Sure, where do you want to go?");
				// ask to listen, and if so set strDestination and get directions
				if (!await SpeechRecognitionManager.RequestListen(this.GetType(), (text) =>
				{
					strDestination = text;
					this.GetDirections(text);
				}))
				{
					string message = "Sorry, but something went wrong. To get directions, say \"Hey Bob, how do I get to destination\"";
					TextToSpeechEngine.SpeakText(this.MediaElement, message);
					this.ShowMessage(message);
				}
				else
				{
					this.ProvideDirectionsSuccessMessage(strDestination);
				}
			}
			if (StringUtils.IsNotBlank(strDestination))
			{
				this.GetDirections(strDestination);
				this.ProvideDirectionsSuccessMessage(strDestination);
			}
		}

		private async void ProvideDirectionsSuccessMessage(string destination)
		{
			// show a link to the search
			this.ClearArea();
			var linkElement = new HyperlinkButton();
			linkElement.Content = $"Directions to {destination.ToLower()}";
			string directionsLink = await this.GetDirectionsLink(destination);
			if (directionsLink != null)
			{
				linkElement.NavigateUri = new Uri(directionsLink);
				linkElement.FontSize = 24;
				RelativePanel.SetAlignHorizontalCenterWithPanel(linkElement, true);
				RelativePanel.SetAlignVerticalCenterWithPanel(linkElement, true);
				this.DynamicArea.Children.Add(linkElement);
				TextToSpeechEngine.SpeakText(this.MediaElement, $"Alright, getting {linkElement.Content.ToString().ToLower()}");
			}
		}

		private async void GetDirections(string destination)
		{
			string query = await this.GetDirectionsLink(destination);
			if (query != null)
			{
				var uriMap = new Uri(query);
				var success = await Windows.System.Launcher.LaunchUriAsync(uriMap);
			}
		}

		private async Task<string> GetDirectionsLink(string destination)
		{
			string query = null;
			Dictionary<string, double> coordinates = await LocationProvider.GetLatitudeAndLongitude();
			if (coordinates != null)
			{
				double latitude = coordinates["latitude"];
				double longitude = coordinates["longitude"];
				Setting setting = StoredProcedures.QuerySettingByName("Map Provider");
				MapProvider mapProvider = StoredProcedures.QueryMapProvider(setting.GetSelectedOption().DisplayName);
				query = mapProvider.BaseURL.ToString();
				query = query.Replace("{Latitude}", latitude.ToString()).Replace("{Longitude}", longitude.ToString()).Replace("{Destination}", HttpUtility.UrlEncode(destination));
			}
			else
			{
				// have bob tell the user that location information couldn't be retrieved
				string message = "Sorry, but I couldn't get your location. In order to get you directions to a place, I need your location to use for the starting point.";
				TextToSpeechEngine.SpeakText(this.MediaElement, message);
				this.ShowMessage(message);
			}
			return query;
		}
	}
}
