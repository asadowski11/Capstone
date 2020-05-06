using System;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Capstone.Common
{
    public class AudioPlayer
    {
        private static MediaPlayer mediaPlayer;
        private static bool IsStarted = false;

        public static void Start()
        {
            // must use a gate so that we don't cause memory leaks by not disposing the media player
            if (!IsStarted)
            {
                IsStarted = true;
                mediaPlayer = new MediaPlayer();
            }
        }

        public static void Stop()
        {
            if (IsStarted && mediaPlayer != null)
            {
                mediaPlayer.Dispose();
                mediaPlayer = null;
                IsStarted = false;
            }
        }

        /// <summary>
        /// Plays a .wav file with the passed <paramref name="SoundName"/> in our application's /Assets/Sounds folder.
        /// </summary>
        /// <param name="SoundName">the name of the sound file, without the .wav extension, that you want to be played</param>
        public static void PlaySound(string SoundName)
        {
            if (IsStarted)
            {
                mediaPlayer.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/Sounds/{SoundName}.wav"));
                mediaPlayer.Play();
            }
        }
    }
}