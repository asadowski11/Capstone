using System;
using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.SpeechRecognition;

namespace BobTheDigitalAssistant.Actions
{
    public class RepeatAfterMeAction : Action
    {
        public RepeatAfterMeAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        public override async void PerformAction()
        {
            Action<string> repeatAction = (text) =>
            {
                this.ClearArea();
                TextToSpeechEngine.SpeakText(this.MediaElement, $"{text}");
                this.ShowMessage($"You said {text}");
            };
            var executedSuccessfully = await SpeechRecognitionManager.RequestListen(this.GetType(), repeatAction);
            if (!executedSuccessfully)
            {
                this.ClearArea();
                string message = "Something went wrong with listening to you, so I cannot repeat after you. Do you have voice activation set to off in the app settings or system settings?";
                TextToSpeechEngine.SpeakText(this.MediaElement, message);
                this.ShowMessage(message);
            }
        }
    }
}