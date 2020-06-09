using System;
using System.Collections.Generic;
using BobTheDigitalAssistant.Common;

namespace BobTheDigitalAssistant.Actions
{
	public class WhatCanYouDoAction : Action
	{
		private List<string> AvailableActions = new List<string>()
		{
			"manage alarms and reminders",
			"get the weather",
			"tell jokes",
			"search the web for you",
			"display driving directions to a place for you in your browser",
			"search specific sites like Amazon and Wikipedia",
			"tell the time and date",
			"create voice notes",
			"flip a coin",
			"roll a die",
			"pick a random number"
		};

		public WhatCanYouDoAction(string CommandString)
		{
			this.CommandString = CommandString;
		}

		public override void PerformAction()
		{
			// pick 2 actions that bob can do and recommend them.
			Random random = new Random();
			string firstSuggestion = this.AvailableActions[random.Next(0, this.AvailableActions.Count)];
			string secondSuggestion;
			// a body-less while loop that keeps picking a suggestion until it's not the first suggestion
			while ((secondSuggestion = this.AvailableActions[random.Next(0, this.AvailableActions.Count)]) == firstSuggestion) ;
			this.ClearArea();
			string text = $"My list of skills is growing, but right now some things I can do are {firstSuggestion}, and {secondSuggestion}";
			string ssmlText = new SSMLBuilder().Prosody(text, contour: "(5%, +10%) (20%, -5%) (60%, -5%)").Build();
			// our media element will be set in the call of PerformAction(mediaElement, dynamicArea, ssmlText)
			TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssmlText);
			this.ShowMessage(text);
		}
	}
}
