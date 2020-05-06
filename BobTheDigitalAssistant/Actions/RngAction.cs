

using System;
using System.Text.RegularExpressions;
using BobTheDigitalAssistant.Common;

namespace BobTheDigitalAssistant.Actions
{
    class RngAction : Action
    {

        public RngAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        public override void PerformAction()
        {
            this.ClearArea();
            types type = GetTypeFromCommand();
            if (type == types.DICE)
            {
                this.RollDie();
            }
            else if (type == types.COIN)
            {
                this.FlipCoin();
            }
            else if (type == types.NUMBER)
            {
                this.PickRandomNumber();
            }
            else
            {
                TextToSpeechEngine.SpeakText(this.MediaElement, "Sorry, but something went wrong and I couldn't pick a random number for you");
            }
        }

        private types GetTypeFromCommand()
        {
            var type = types.NONE;
            // different regexes to pull out the different types
            var dieRegex = new Regex("(?i)(die|dice|D[0-9]+)(?-i)");
            var coinRegex = new Regex("(?i)(coin)(?-i)");
            var numberRegex = new Regex("(?i)(number)(?-i)");

            if (dieRegex.IsMatch(this.CommandString))
            {
                type = types.DICE;
            }
            else if (coinRegex.IsMatch(this.CommandString))
            {
                type = types.COIN;
            }
            else if (numberRegex.IsMatch(this.CommandString))
            {
                type = types.NUMBER;
            }

            return type;
        }

        private void RollDie()
        {
            // the first number we come across is the number of sides the die has, if we don't come across a number we default it to 6 sides
            string numberOfSides = new Regex("[0-9]+").Match(this.CommandString).Value;
            int parsedSideCount = 6;
            if (numberOfSides != "")
            {
                parsedSideCount = int.Parse(numberOfSides);
            }
            // pick the random number with the range [1, numberOfSides]
            int pickedSide = new Random().Next(1, parsedSideCount + 1);
            // TODO play a sound of a die rolling
            TextToSpeechEngine.SpeakText(this.MediaElement, $"You rolled a {pickedSide}");
        }

        private void FlipCoin()
        {
            // coins are always 2-sided, so pick a random number and associate it with heads or tails
            int side = new Random().Next(2);
            string sideName = side == 0 ? "Heads" : "Tails";
            string text = $"It landed on {sideName}";
            // TODO play a sound of a coin flipping or something
            TextToSpeechEngine.SpeakText(this.MediaElement, text);
        }

        private void PickRandomNumber()
        {
            // there will be 2 numbers that the user picks (e.g. "pick a random number between 1 and 5")
            var numberRegex = new Regex("[0-9]+|one");
            var matches = numberRegex.Matches(this.CommandString);
            // if there's less than 2 matches, tell the user they need 2 numbers
            if (matches.Count < 2)
            {
                TextToSpeechEngine.SpeakText(this.MediaElement, "Sorry, but in order to pick a random number I need 2 numbers to pick between");
            }
            else
            {
                int lowerBound = matches[0].Value.ToLower() == "one" ? 1 : int.Parse(matches[0].Value);
                int upperBound = matches[1].Value.ToLower() == "one" ? 1 : int.Parse(matches[1].Value);
                // get the actual upper and lower bounds (the user could have put the higher number first, but they could also have put it last so we need to figure out which it is)
                if (lowerBound > upperBound)
                {
                    var temp = lowerBound;
                    lowerBound = upperBound;
                    upperBound = temp;
                }
                // if the user said "exclusive", move the lower bound up and the upper bound down
                if (this.CommandString.ToLower().Contains("exclusive"))
                {
                    lowerBound++;
                    upperBound--;
                }
                // make sure the upper bound is greater than the lower bound
                if (upperBound == lowerBound)
                {
                    TextToSpeechEngine.SpeakText(this.MediaElement, $"Sorry, but I can't pick a whole number between {lowerBound} and {upperBound}");
                }
                else
                {
                    int pickedNumber = new Random().Next(lowerBound, upperBound);
                    // TODO play a sound maybe? idk
                    TextToSpeechEngine.SpeakText(this.MediaElement, $"It's {pickedNumber}");
                }
            }
        }

        private enum types
        {
            NONE = -1, // represents an error
            DICE = 0,
            COIN = 1,
            NUMBER = 2
        }
    }
}
