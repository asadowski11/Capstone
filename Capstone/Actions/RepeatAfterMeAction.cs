using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Common;
using Capstone.SpeechRecognition;

namespace Capstone.Actions
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
        }
    }
}
