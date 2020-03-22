using System;
using Windows.Media.SpeechRecognition;
using Windows.System;
using Windows.UI.Popups;

namespace Capstone.SpeechRecognition
{
    internal class SpeechRecogService
    {
        private static readonly uint HResultPrivacyStatementDeclined = 0x80045509;

        public async void StartRecognizing()
        {
            var speechRecognizer = new SpeechRecognizer();

            await speechRecognizer.CompileConstraintsAsync();

            SpeechRecognitionResult speechRecognitionResult = null;

            try
            {
                speechRecognitionResult = await speechRecognizer.RecognizeWithUIAsync();
            }

            catch (Exception exception)

            {
                if ((uint) exception.HResult == HResultPrivacyStatementDeclined)
                {
                    var message = new MessageDialog("The privacy statement was declined." +
                                                    "Go to Settings -> Privacy -> Speech, inking and typing, and ensure you" +
                                                    "have viewed the privacy policy, and 'Get To Know You' is enabled.");
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-accounts"));
                    await message.ShowAsync();

                    return;
                }
            }


            var messageDialog = new MessageDialog(speechRecognitionResult.Text, "Text spoken");
            await messageDialog.ShowAsync();
        }
    }
}