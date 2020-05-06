using System;
using Capstone.Common;

namespace Capstone.Actions
{
    internal class AgeAction : Action
    {
        private int ageInyears { get; set; }
        private int ageInMonths { get; set; }
        public int ageInDays { get; set; }

        public AgeAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        public override void PerformAction()
        {
            this.ClearArea();
            this.CommandString = this.CommandString.ToLower();
            string oldPhrase = "how old are you";
            string agePhrase = "what is your age";
            string birthDate = "what is your birthdate";
            string born = "when were you born";

            if (this.CommandString.Contains(oldPhrase) || this.CommandString.Contains(agePhrase) || this.CommandString.Contains(birthDate) || this.CommandString.Contains(born))
            {
                DateTime dobBob = new DateTime(2020, 5, 4, 18, 30, 0);
                CalculateAge(dobBob);
                string text = String.Format("I was released to the world on {0}. Therefore, I am {1} years, {2} months, and {3} days old.", dobBob.ToString(), ageInyears, ageInMonths, ageInDays);
                string ssml = new SSMLBuilder().Prosody(text, contour: "(20%, +8%) (60%,-8%) (80%, +2%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssml);
                this.ShowMessage(text);
            }
            else
            {
                string text = @"I'm sorry I do not understand. If you are interested in how old I am, please say,'hey bob, how old are you'";
                string ssml = new SSMLBuilder().Prosody(text, contour: "(20%, +8%) (60%,-8%) (80%, +2%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssml);
                this.ShowMessage(text);
            }
        }

        private void CalculateAge(DateTime dob)
        {
            // get current date.
            DateTime now = DateTime.Now;

            // find the literal difference
            ageInDays = now.Day - dob.Day;
            ageInMonths = now.Month - dob.Month;
            ageInyears = now.Year - dob.Year;

            //comparing to see if dob is before todays date. we only need to do this until the presentation
            if (DateTime.Compare(dob, now) < 0)
            {
                if (ageInDays < 0)
                {
                    ageInDays += DateTime.DaysInMonth(now.Year, now.Month);
                    ageInMonths--;
                }
                if (ageInMonths < 0)
                {
                    ageInMonths += 12;
                    ageInyears--;
                }
            }
            else
            {
                ageInDays = 0;
                ageInMonths = 0;
                ageInyears = 0;
            }
        }
    }
}