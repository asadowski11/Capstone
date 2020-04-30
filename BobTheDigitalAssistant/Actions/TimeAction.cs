using Capstone.Common;
using System;

namespace Capstone.Actions
{
    public class TimeAction : Action
    {

        public TimeAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        public override void PerformAction()
        {
            this.ClearArea();
            this.CommandString = this.CommandString.ToLower();
            if (this.CommandString.Contains("time"))
            {
                string time = DateTime.Now.ToString("h:mm tt");
                string text = $"Currently, it is {time}";
                string ssml = new SSMLBuilder().Prosody(text, contour: "(20%, +8%) (60%,-8%) (80%, +2%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssml);
                this.ShowMessage(text);

            }
            else if (this.CommandString.Contains("date"))
            {
                string date = DateTime.Now.ToString("MMM dd yyyy");
                string text = $"Today's date is {date}";
                string ssml = new SSMLBuilder().Prosody(text, contour: "(20%, +8%) (60%,-8%) (80%, +2%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssml);
                this.ShowMessage(text);
            }
        }
    }
}
