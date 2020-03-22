using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Popups;

namespace Capstone.SpeechRecognition
{
    public static class AudioCapturePermissions
    {
        // If no microphone is present, an exception is thrown with the following HResult value.
        private static readonly int NoCaptureDevicesHResult = -1072845856;

        public static async Task RequestMicrophonePermission()
        {
            try
            {
                var settings = new MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                settings.MediaCategory = MediaCategory.Speech;
                var capture = new MediaCapture();

                await capture.InitializeAsync(settings);
            }
            catch (TypeLoadException)
            {
                var messageDialog = new MessageDialog("Media player components are unavailable.");
                await messageDialog.ShowAsync();
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (Exception exception)
            {
                if (exception.HResult == NoCaptureDevicesHResult)
                {
                    var messageDialog =
                        new MessageDialog("No Audio Capture devices are present on this system.");
                    await messageDialog.ShowAsync();
                    return;
                }

                throw;
            }
        }
    }
}