using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace BobTheDigitalAssistant.Common
{
	public class AudioRecorder
	{
		public MediaElement playbackMediaElement = new MediaElement();
		private MediaCapture MediaCapture;
		private InMemoryRandomAccessStream MemoryBuffer;
		private static IRandomAccessStream stream;

		private string _fileName;
		public bool IsRecording { get; set; }

		public async void Record()
		{
			try
			{
				if (this.IsRecording)
				{
					throw new InvalidOperationException("Recording already in progress!");
				}

				this.MemoryBuffer = new InMemoryRandomAccessStream();

				this.DisposeMedia();
				MediaCaptureInitializationSettings settings =
				new MediaCaptureInitializationSettings
				{
					StreamingCaptureMode = StreamingCaptureMode.Audio
				};

				this.MediaCapture = new MediaCapture();
				await this.MediaCapture.InitializeAsync(settings);
				await this.MediaCapture.StartRecordToStreamAsync(
				MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto), this.MemoryBuffer);
				this.IsRecording = true;
			}
			catch (Exception)
			{
				this.IsRecording = false;
				// show a message to the user
				if (this.playbackMediaElement != null)
				{
					TextToSpeechEngine.SpeakText(this.playbackMediaElement, "Sorry, but there was an issue with recording your voice. Try to make sure your machine has a microphone, and that microphone access is enabled for this app.");
				}
				else
				{
					// create a new media element
					TextToSpeechEngine.SpeakText(new MediaElement(), "Sorry, but there was an issue with recording your voice. Try to make sure your machine has a microphone, and that microphone access is enabled for this app.");
				}
			}
		}

		public async void StopRecording()
		{
			try
			{
				await this.MediaCapture.StopRecordAsync();
				this.DisposeMedia();
				this.IsRecording = false;
			}
			catch (Exception)
			{
				// still make sure IsRecording is set to false
				this.IsRecording = false;
			}
		}

		/// <summary>
		/// Saves the audio to a file in our local state folder
		/// </summary>
		/// <returns>the saved file's name</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<string> SaveAudioToFile()
		{
			string dateToday = DateTime.Now.ToString("yyyy-MM-dd");
			string ticks = DateTime.Now.Ticks.ToString();
			string mp3 = ".mp3";
			string fileName = String.Format("record_{0}_{1}{2}", dateToday, ticks, mp3);
			StorageFolder localStateFolder = ApplicationData.Current.LocalFolder;
			StorageFolder storageFolder;
			if (!Directory.Exists(Path.Combine(localStateFolder.Path, "VoiceNotes")))
			{
				storageFolder = await localStateFolder.CreateFolderAsync("VoiceNotes");
			}
			else
			{
				storageFolder = await localStateFolder.GetFolderAsync("VoiceNotes");
			}
			IRandomAccessStream audioStream = this.MemoryBuffer.CloneStream();
			StorageFile storageFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
			this._fileName = storageFile.Name;
			using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
			{
				await RandomAccessStream.CopyAndCloseAsync(audioStream.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
				await audioStream.FlushAsync();
				audioStream.Dispose();
			}
			this.DisposeMemoryBuffer();

			return this._fileName;
		}

		public void PlayFromDisk(string fileName)
		{
			this.DisposeStream();

			Utils.RunOnMainThread(async () =>
			{
				StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("VoiceNotes");
				StorageFile storageFile = await storageFolder.GetFileAsync(fileName);
				stream = await storageFile.OpenAsync(FileAccessMode.Read);
				this.playbackMediaElement.SetSource(stream, storageFile.FileType);
				this.playbackMediaElement.Play();
			});
		}

		public async void DeleteFile(string file)
		{
			StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("VoiceNotes");
			StorageFile fileToDelete = await storageFolder.GetFileAsync(file);
			if (fileToDelete != null)
			{
				await fileToDelete.DeleteAsync();
			}
		}

		public void DisposeMemoryBuffer()
		{
			if (this.MemoryBuffer != null)
			{
				this.MemoryBuffer.Dispose();
			}
		}

		public void DisposeMedia()
		{
			if (this.MediaCapture != null)
			{
				this.MediaCapture.Dispose();
			}
		}

		public void DisposeStream()
		{
			if (stream != null)
			{
				stream.Dispose();
			}
		}

		public void StopPlaybackMedia()
		{
			if (this.playbackMediaElement != null)
			{
				this.playbackMediaElement.Stop();
			}
		}

		public async Task<int> GetAudioDuration(string fileName)
		{
			int duration = 0;
			StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("VoiceNotes");
			StorageFile audioFile = await storageFolder.GetFileAsync(fileName);
			MusicProperties properties = await audioFile.Properties.GetMusicPropertiesAsync();
			TimeSpan myTrackDuration = properties.Duration;
			duration = myTrackDuration.Seconds;
			return duration;
		}

		public DateTime GetDateRecorded()
		{
			DateTime today = DateTime.Now;
			return Convert.ToDateTime(today.ToShortDateString());
		}

		public DateTime GetTimeRecorded()
		{
			DateTime today = DateTime.Now;
			return Convert.ToDateTime(today.ToShortTimeString());
		}
	}
}
