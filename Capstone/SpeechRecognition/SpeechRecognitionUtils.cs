using Capstone.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Capstone.SpeechRecognition
{
    public static class SpeechRecognitionUtils
    {
        private static SpeechRecognizer recognizer;
        public static bool IsStarted { get; private set; } = false;
        // if the user has disabled the "get to know you" setting, this is the error message
        private static readonly uint HResultPrivacyStatementDeclined = 0x80045509;
        private static readonly string activatorString = "hey bob";
        // the text box to populate the spoken words with
        public static TextBox commandBox;
        private static Thread thread;
        // holds what the recognizer thinks the user is saying
        private static string SpokenText;

        /// <summary>
        /// Starts up the speech recognizer in a background thread to listen for input speech. if speech is heard and contains "hey bob", then when the user stops speaking the resulting text is used to find and perform a corresponding action.
        /// This method does not run if it has already started
        /// </summary>
        /// <param name="speechInputFunction"></param>
        /// <param name="box"></param>
        public static async void StartLooping(Action<string> speechInputFunction, TextBox box)
        {
            if (!IsStarted)
            {
                IsStarted = true;
                commandBox = box;
                recognizer = new SpeechRecognizer();
                // compile the grammar and speech contstraings. TODO we may want to create our own grammar file. I don't know how much effort that will take up though
                await recognizer.CompileConstraintsAsync();
                recognizer.HypothesisGenerated += Recognizer_HypothesisGenerated;
                // add a second delay for the user to have pauses in their speech
                recognizer.ContinuousRecognitionSession.AutoStopSilenceTimeout += new TimeSpan(1_000_000); // 1,000,000 ticks = 1 second
                SpeechRecognitionResult result = null;

                thread = new Thread(new ThreadStart(async () =>
                {
                    while (IsStarted)
                    {
                        try
                        {
                            result = await recognizer.RecognizeAsync();
                            if (result != null && StringUtils.Contains(result.Text, activatorString))
                            {
                                SpokenText = result.Text;
                                // if the result is only "hey bob", then listen again
                                if (StringUtils.AreEqual(SpokenText, activatorString))
                                {
                                    result = await recognizer.RecognizeAsync();
                                    SpokenText += " " + result.Text;
                                }
                                // clear the command box and run the command
                                Utils.RunOnMainThread(() =>
                                {
                                    AudioPlayer.PlaySound("bob_activate");
                                    // give the sound enough time to play
                                    Thread.Sleep(750);
                                    speechInputFunction.Invoke(SpokenText);
                                    // clear the spoken text variable to prevent the text box from holding old and new commands at once
                                    SpokenText = "";
                                });
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            // the page was changed, or we were stopped. Either way, be sure to mark us as not started just in case
                            IsStarted = false;
                        }
                        catch (Exception exception)
                        {

                            if ((uint)exception.HResult == HResultPrivacyStatementDeclined)
                            {
                                var message = new MessageDialog("The privacy statement was declined." +
                                                                "Go to Settings -> Privacy -> Speech, inking and typing, and ensure you" +
                                                                "have viewed the privacy policy, and 'Get To Know You' is enabled.");
                                await message.ShowAsync();

                            }
                        }
                    }
                }));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public static async Task<SpeechRecognitionResult> ListenOnceAsync()
        {
            if (!IsStarted)
            {
                try
                {
                    IsStarted = true;
                    recognizer = new SpeechRecognizer();
                    // compile the speech constraints and start listening
                    await recognizer.CompileConstraintsAsync();
                    // keep listening until the result isn't an empty string since sometimes it rings up false positives
                    SpeechRecognitionResult result = null;
                    while (result == null || StringUtils.IsBlank(result.Text))
                    {
                        result = await recognizer.RecognizeAsync();
                    }
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("Can't Listen when already started!");
            }
        }

        /// <summary>
        /// Stops the speech recognition process and disposes of it, and ends the background thread by killing the while loop inside of it
        /// </summary>
        public static void Stop()
        {
            if (IsStarted)
            {
                recognizer.HypothesisGenerated -= Recognizer_HypothesisGenerated;
                recognizer.Dispose();
                IsStarted = false;
            }
        }

        /// <summary>
        /// Event handler used to display what's being heard in the main screen's text box
        /// </summary>
        /// <param name="recognizer"></param>
        /// <param name="args"></param>
        private static void Recognizer_HypothesisGenerated(SpeechRecognizer recognizer, SpeechRecognitionHypothesisGeneratedEventArgs args)
        {
            if (StringUtils.Contains(args.Hypothesis.Text, activatorString) || StringUtils.AreEqual(SpokenText, activatorString))
            {
                Utils.RunOnMainThread(() =>
                {
                    if (commandBox != null)
                    {
                        commandBox.Text = SpokenText + " " + args.Hypothesis.Text;
                    }
                });

            }
        }

    }
}
