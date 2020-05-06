using System;
using Capstone.Common;

namespace Capstone.Actions
{
    internal class GreetingAction : Action
    {
        public GreetingAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        public override void PerformAction()
        {
            this.ClearArea();
            this.CommandString = this.CommandString.ToLower();
            string text = GetGreeting();
            string ssml = new SSMLBuilder().Prosody(text, contour: "(20%, +8%) (60%,-8%) (80%, +2%)").Build();
            TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssml);
            this.ShowMessage(text);
        }

        private string GetGreeting()
        {
            DateTime now = DateTime.Now;
            string greet;

            if (now.Hour >= 6 && now.Hour < 12)
            {
                greet = "Good Morning.";
            }
            else if (now.Hour >= 12 && now.Hour < 17)
            {
                greet = "Good Afternoon.";
            }
            else if (now.Hour >= 17 && now.Hour < 20)
            {
                greet = "Good Evening.";
            }
            else
            {
                greet = "Good Night.";
            }

            return greet;
        }
    }
}