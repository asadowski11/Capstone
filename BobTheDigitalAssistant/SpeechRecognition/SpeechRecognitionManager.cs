using System;
using System.Threading.Tasks;
using BobTheDigitalAssistant.Common;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml.Controls;

namespace BobTheDigitalAssistant.SpeechRecognition
{
    public class SpeechRecognitionManager
    {
        // the current class that has the listener's attention
        private static Type CurrentListener { get; set; }

        // used by the current listener to tell it when the current listener is done listening
        public static bool IsCurrentListenerDone { get; set; }

        // the reference to what the objects the main page uses when listening
        private static Action<string> MainPageFunction { get; set; }

        private static TextBox MainPageTextBox { get; set; }

        /// <summary>
        /// Attempts to request access to the speech recognizer to recognize text. Access to the speech recognizer is not guaranteed, and a boolean <code>false</code> will be returned if access is rejected
        /// </summary>
        /// <param name="callerType">The type of the class calling this method</param>
        /// <param name="callbackFunction">the function to be invoked with the recognized text.</param>
        /// <returns>true if access was granted and no errors were thrown, false otherwise</returns>
        public static async Task<bool> RequestListen(Type callerType, Action<string> callbackFunction)
        {
            // if the current listener is the main screen, then it's fine to interrupt. Otherwise we need to check if the current listener is done
            if (typeof(MainPage) != CurrentListener && !IsCurrentListenerDone || !Utils.IsListeningSettingEnabled())
            {
                // we can't listen, so return false
                return false;
            }
            else
            {
                // stop the current speech recognition session
                SpeechRecognitionUtils.Stop();
                try
                {
                    // set the class that's listening
                    IsCurrentListenerDone = false;
                    CurrentListener = callerType;
                    SpeechRecognitionResult result = await SpeechRecognitionUtils.ListenOnceAsync();
                    SpeechRecognitionUtils.Stop();
                    IsCurrentListenerDone = true;
                    // make the main page listen again
                    StartListeningForMainPage();
                    // now call the callback function
                    callbackFunction.Invoke(result.Text);
                    return true; // successfully listened and did the command
                }
                catch (Exception)
                {
                    return false; // something went wrong with listening
                }
            }
        }

        /// <summary>
        /// Starts the speech recognition for the main page, using the passed <paramref name="callbackFunction"/> and the passed <paramref name="textBox"/>
        /// </summary>
        /// <param name="callbackFunction"></param>
        /// <param name="textBox"></param>
        public static void StartListeningForMainPage(Action<string> callbackFunction, TextBox textBox)
        {
            // the main page takes priority over everything when it comes to listening, so force stop
            SpeechRecognitionUtils.Stop();
            CurrentListener = typeof(MainPage);
            IsCurrentListenerDone = false;
            MainPageFunction = callbackFunction;
            MainPageTextBox = textBox;
            SpeechRecognitionUtils.StartLooping(MainPageFunction, MainPageTextBox);
        }

        /// <summary>
        /// should only be called after <see cref="StartListeningForMainPage(Action{string}, TextBox)"/> has been called since the main page was navigated to
        /// </summary>
        private static void StartListeningForMainPage()
        {
            if (Utils.IsListeningSettingEnabled())
            {
                if (MainPageFunction != null && MainPageTextBox != null)
                {
                    // the main page takes priority over everything when it comes to listening, so force stop
                    SpeechRecognitionUtils.Stop();
                    CurrentListener = typeof(MainPage);
                    IsCurrentListenerDone = false;
                    SpeechRecognitionUtils.StartLooping(MainPageFunction, MainPageTextBox);
                }
            }
            else
            {
                TextToSpeechEngine.SpeakText(new MediaElement(), "Sorry, but something went wrong with setting up your microphone. You cannot use me through speech, but you can still use the command bar at the bottom of the screen.");
            }
        }
    }
}