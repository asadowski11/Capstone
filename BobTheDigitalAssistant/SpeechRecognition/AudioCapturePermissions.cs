using System;
using System.Threading.Tasks;
using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.Models;
using Windows.Media.Capture;
using Windows.UI.Popups;

namespace Captsone.SpeechRecognition
{
	/// <summary>
	/// This class is used to get permissions to access the user's microphone. It was taken from
	/// https://docs.microsoft.com/en-us/windows/uwp/design/input/speech-recognition
	/// </summary>
	public static class AudioCapturePermissions
	{
		// If no microphone is present, an exception is thrown with the following HResult value.
		private static int NoCaptureDevicesHResult = -1072845856;

		/// <summary>
		/// Note that this method only checks the Settings->Privacy->Microphone setting, it does not handle
		/// the Cortana/Dictation privacy check.
		///
		/// You should perform this check every time the app gets focus, in case the user has changed
		/// the setting while the app was suspended or not in focus.
		/// </summary>
		/// <returns>True, if the microphone is available.</returns>
		public async static Task<bool> RequestMicrophonePermission()
		{
			try
			{
				// Request access to the audio capture device.
				MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
				settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
				settings.MediaCategory = MediaCategory.Speech;
				MediaCapture capture = new MediaCapture();

				await capture.InitializeAsync(settings);
			}
			catch (TypeLoadException)
			{
				// Thrown when a media player is not available.
				var messageDialog = new Windows.UI.Popups.MessageDialog("Media player components are unavailable.");
				await messageDialog.ShowAsync();
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				// Thrown when permission to use the audio capture device is denied.
				// If this occurs, show an error or disable recognition functionality.
				var dialog = new MessageDialog("Microphone permissions are allowed in app settings, but are disabled in system settings. Do you want to enable microphone access for this app?");
				dialog.Commands.Add(new UICommand("No", async (command) =>
				{
					// get the setting for voice detection in the database, mark it as disabled, and update it in the database
					Setting voiceDetectionSetting = StoredProcedures.QuerySettingByName("Voice Activation");
					voiceDetectionSetting.SelectOption("Disabled");
					StoredProcedures.SelectOption(voiceDetectionSetting.SettingID, voiceDetectionSetting.GetSelectedOption().OptionID);
					var confirmationDialog = new MessageDialog("Ok, voice detection will be disabled. To Re-activate it, go to the settings page.");
					await confirmationDialog.ShowAsync();
				}));
				dialog.Commands.Add(new UICommand("Yes, take me to the system settings", async (command) => await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone"))));
				dialog.DefaultCommandIndex = 1;
				await dialog.ShowAsync();
				return false;
			}
			catch (Exception exception)
			{
				// Thrown when an audio capture device is not present.
				if (exception.HResult == NoCaptureDevicesHResult)
				{
					var messageDialog = new Windows.UI.Popups.MessageDialog("No Audio Capture devices are present on this system.");
					await messageDialog.ShowAsync();
					return false;
				}
				else
				{
					throw;
				}
			}
			return true;
		}
	}
}
