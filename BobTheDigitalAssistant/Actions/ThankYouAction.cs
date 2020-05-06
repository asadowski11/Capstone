using BobTheDigitalAssistant.Common;

namespace BobTheDigitalAssistant.Actions
{
    internal class ThankYouAction : Action
    {
        public ThankYouAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        public override void PerformAction()
        {
            this.ClearArea();
            this.CommandString = this.CommandString.ToLower();
            string thankYouPhrase = "thank you";
            string thanks = "thanks";

            if (this.CommandString.Contains(thankYouPhrase) || this.CommandString.Contains(thanks))
            {
                string text = "You are welcome.";
                string ssml = new SSMLBuilder().Prosody(text, contour: "(20%, +8%) (60%,-8%) (80%, +2%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssml);
                this.ShowMessage(text);
            }
            else
            {
                string text = @"I'm sorry I do not understand.";
                string ssml = new SSMLBuilder().Prosody(text, contour: "(20%, +8%) (60%,-8%) (80%, +2%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssml);
                this.ShowMessage(text);
            }
        }
    }
}