using Capstone.Common;
using Capstone.Models;

namespace Capstone.Actions
{
    internal class JokeAction : Action
    {
        public override void PerformAction()
        {
            Joke joke = StoredProcedures.QueryRandomJoke();
            this.ClearArea();
            if (joke != null)
            {
                TextToSpeechEngine.SpeakText(this.MediaElement, joke.Text);
                this.ShowMessage(joke.Text);
            }
        }
    }
}