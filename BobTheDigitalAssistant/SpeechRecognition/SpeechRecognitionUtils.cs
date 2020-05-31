using System;
using System.Threading;
using System.Threading.Tasks;
using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.Models;
using Windows.Media.SpeechRecognition;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace BobTheDigitalAssistant.SpeechRecognition
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
		private static string SpokenText = "";

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
							if (result != null && StringUtils.Contains(result.Text.Trim(), activatorString))
							{
								SpokenText = result.Text.Trim();
								// if the result is only "hey bob", then listen again
								if (StringUtils.AreEqual(SpokenText, activatorString))
								{
									result = await recognizer.RecognizeAsync();
									SpokenText += " " + result.Text;
								}
								EnsureSpokenTextDoesNotContainStuffBeforeActivatorString(ref SpokenText);
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
								Utils.RunOnMainThread(async () =>
								{
									// turn off speech recognition in bob
									Setting speechRecognitionSetting = StoredProcedures.QuerySettingByName("Voice Activation");
									speechRecognitionSetting.SelectOption("Disabled");
									StoredProcedures.SelectOption(speechRecognitionSetting.SettingID, speechRecognitionSetting.GetSelectedOption().OptionID);
									var message = new MessageDialog("Microsoft's privacy statement was declined." +
																	"Go to Settings -> Privacy -> Speech, and turn on online speech recognition.\nBob's speech recognition capabilities will be turned off. To turn it back on, you must manually re-enable it in app settings.");
									await message.ShowAsync();
								});
								break;
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
				try
				{
					recognizer.HypothesisGenerated -= Recognizer_HypothesisGenerated;
					recognizer.Dispose();
				}
				catch (ObjectDisposedException)
				{
					// nothing to do here, it was already disposed. The real issue here is was the user doing weird stuff, or did the recognizer not startup properly? TODO
				}
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
						string tempText = SpokenText + " " + args.Hypothesis.Text;
						EnsureSpokenTextDoesNotContainStuffBeforeActivatorString(ref tempText);
						commandBox.Text = tempText;
					}
				});
			}
		}

		/// <summary>
		/// So we have a dilemma; bob MUST always listen for the activator string. That means that SpokenText may contain what the user said before they said "hey bob".
		/// Everyone who can understand the code will know that bob doesn't do this maliciously, and it's out of necessity.Anything that is said before "hey bob" may show up in the command box, or Bob might use that text and interpret it as something else.
		/// This this method removes everything before the "hey bob" so that the user doesn't incorrectly believe we're spying on them.
		/// </summary>
		/// <param name="text"></param>
		private static void EnsureSpokenTextDoesNotContainStuffBeforeActivatorString(ref string text)
		{
			var splitSpokenText = text.ToLower().Split(activatorString + " ");
			if (splitSpokenText.Length > 1)
			{
				// only keep the stuff after the "hey bob" and get rid of the rest
				text = activatorString + " " + splitSpokenText[1];
			}
			else
			{
				// we shouldn't do anything with what the user says before "hey bob", so get rid of what the user said
				text = "";
			}
		}
	}
}
