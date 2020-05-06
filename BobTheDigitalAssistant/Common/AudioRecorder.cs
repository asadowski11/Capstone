using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace Capstone.Common
{
    public class AudioRecorder
    {
        public MediaElement playbackMediaElement = new MediaElement();
        private MediaCapture _mediaCapture;
        private InMemoryRandomAccessStream _memoryBuffer;
        private static IRandomAccessStream stream;

        private string _fileName;
        public bool IsRecording { get; set; }

        public async void Record()
        {
            try
            {
                if (IsRecording)
                {
                    throw new InvalidOperationException("Recording already in progress!");
                }

                _memoryBuffer = new InMemoryRandomAccessStream();

                DisposeMedia();
                MediaCaptureInitializationSettings settings =
                new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio
                };

                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(settings);
                await _mediaCapture.StartRecordToStreamAsync(
                MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto), _memoryBuffer);
                IsRecording = true;
            }
            catch (Exception)
            {
                IsRecording = false;
                // show a message to the user
                if (playbackMediaElement != null)
                {
                    TextToSpeechEngine.SpeakText(playbackMediaElement, "Sorry, but there was an issue with recording your voice. Try to make sure your machine has a microphone, and that microphone access is enabled for this app.");
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
                await _mediaCapture.StopRecordAsync();
                DisposeMedia();
                IsRecording = false;
            }
            catch (Exception)
            {
                // still make sure IsRecording is set to false
                IsRecording = false;
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
            IRandomAccessStream audioStream = _memoryBuffer.CloneStream();
            StorageFile storageFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            this._fileName = storageFile.Name;
            using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await RandomAccessStream.CopyAndCloseAsync(audioStream.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                await audioStream.FlushAsync();
                audioStream.Dispose();
            }
            DisposeMemoryBuffer();

            return this._fileName;
        }

        public async Task PlayFromDisk(string fileName)
        {
            DisposeStream();

            Utils.RunOnMainThread(async () =>
            {
                StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("VoiceNotes");
                StorageFile storageFile = await storageFolder.GetFileAsync(fileName);
                stream = await storageFile.OpenAsync(FileAccessMode.Read);
                playbackMediaElement.SetSource(stream, storageFile.FileType);
                playbackMediaElement.Play();
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
            if (_memoryBuffer != null)
            {
                _memoryBuffer.Dispose();
            }
        }

        public void DisposeMedia()
        {
            if (_mediaCapture != null)
            {
                _mediaCapture.Dispose();
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
            if (playbackMediaElement != null)
            {
                playbackMediaElement.Stop();
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