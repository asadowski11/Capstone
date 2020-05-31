using System;
using System.Collections.Generic;
using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.Models;
using BobTheDigitalAssistant.Providers;

namespace BobTheDigitalAssistant.Actions
{
	public class WeatherAction : Action
	{
		public WeatherAction(string CommandString)
		{
			this.CommandString = CommandString;
		}

		public async override void PerformAction()
		{
			List<WeatherInfo> weatherInfos = await WeatherService.GetWeather();
			// if there's a date in the command string, find it and compare it with the dates provided
			DateTime commandDate = DateTimeParser.ParseDateTimeFromText(this.CommandString);
			// get the first applicable weather info
			WeatherInfo firstApplicableWeatherInfo = weatherInfos.Find(info => info.DateApplicable >= commandDate);
			if (firstApplicableWeatherInfo != null && this.MediaElement != null)
			{
				this.ClearArea();
				// TODO get better at determining where there should be inflection. Right now this works but sounds a bit too robotic
				string inflectionData = new SSMLBuilder().Prosody(SplitWeatherDescUpIntoSSMLSentences(firstApplicableWeatherInfo.Description), contour: "(30%,+10%) (60%,-10%) (90%,+5%)").Build();
				TextToSpeechEngine.SpeakInflectedText(this.MediaElement, inflectionData);
				this.ShowMessage(firstApplicableWeatherInfo.Description);
			}
			else if (firstApplicableWeatherInfo == null)
			{
				this.ClearArea();
				string message = "I could not find any weather info for the date specified. Try making sure that you have location enabled, and that this app can access your location through system settings, privacy, location";
				TextToSpeechEngine.SpeakText(this.MediaElement, message);
				this.ShowMessage(message.Replace("settings, privacy, location", "settings > privacy > location"));
			}
		}

		/// <summary>
		/// enforces spoken sentence structure in weather description by wrapping all the sentences in an ssml &lt;sentence&gt; tag. This prevents the text to speech engine from misreading a sentence (e.g. 45mph. will not be counted as a sentence because it thinks the period is used in an abbreviation)
		/// </summary>
		/// <param name="weatherDescription">the string to split into forced sentences</param>
		/// <returns></returns>
		private string SplitWeatherDescUpIntoSSMLSentences(string weatherDescription)
		{
			string ssmlDescription = "";
			string[] descriptionSentences = weatherDescription.Split(".");
			foreach (string sentence in descriptionSentences)
			{
				ssmlDescription += new SSMLBuilder().Sentence(sentence + ".").BuildWithoutWrapperElement();
			}

			return ssmlDescription;
		}
	}
}
