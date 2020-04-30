using Capstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Actions
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
            "search specific sites like YouTube and Amazon",
            "tell the time and date",
            "create voice notes"
        };

        public WhatCanYouDoAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        public override void PerformAction()
        {
            // pick 2 actions that bob can do and recommend them.
            Random random = new Random();
            string firstSuggestion = AvailableActions[random.Next(0, AvailableActions.Count)];
            string secondSuggestion;
            // a body-less while loop that keeps picking a suggestion until it's not the first suggestion
            while ((secondSuggestion = AvailableActions[random.Next(0, AvailableActions.Count)]) == firstSuggestion) ;
            this.ClearArea();
            string text = $"My list of skills is growing, but right now some things I can do are {firstSuggestion}, and {secondSuggestion}";
            string ssmlText = new SSMLBuilder().Prosody(text, contour: "(5%, +10%) (20%, -5%) (60%, -5%)").Build();
            // our media element will be set in the call of PerformAction(mediaElement, dynamicArea, ssmlText)
            TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssmlText);
            this.ShowMessage(text);
        }
    }
}
