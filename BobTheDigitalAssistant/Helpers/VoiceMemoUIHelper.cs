using System;
using System.Threading.Tasks;
using Capstone.Common;
using Capstone.Models;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Capstone.Helpers
{
    public static class VoiceMemoUIHelper
    {
        /// <summary>
        /// Builds and returns a button to delete the recorded voice note. (Actually what it does is call the passed <paramref name="ClickAction"/>, but the button is designed to intiuitively mean that clicking it should delete the voice note)
        /// </summary>
        /// <param name="ClickAction"></param>
        /// <returns></returns>
        public static Button BuildDeleteRecordingButton(Action ClickAction)
        {
            Button button = new Button();
            button.FontSize = 32;
            button.Width = 150;
            button.Height = 150;
            button.FontFamily = new FontFamily("Segoe MDL2 Assets");
            button.Content = "\uE107"; // trash can symbol
            button.Background = new SolidColorBrush(GetSystemAccentColor());
            button.Foreground = new SolidColorBrush(GetSystemForeGroundAccentColor());
            button.Click += (sender, args) => ClickAction();
            // give it a bit of spacing around itself
            button.Margin = new Thickness(10);
            return button;
        }

        /// <summary>
        /// Builds and returns the text box that is used to input a voice memo's name
        /// </summary>
        /// <returns></returns>
        public static TextBox BuildDisplayNameTextBox()
        {
            TextBox box = new TextBox();
            box.Header = "Display Name";
            return box;
        }

        /// <summary>
        /// Builds and returns a button to save the recorded voice note. (Actually what it does is call the passed <paramref name="ClickAction"/>, but the button is designed to intiuitively mean that clicking it should save the recorded voice note)
        /// </summary>
        /// <param name="ClickAction"></param>
        /// <returns></returns>
        public static Button BuildSaveRecordingButton(Action ClickAction)
        {
            Button button = new Button();
            button.FontSize = 32;
            button.Width = 150;
            button.Height = 150;
            button.FontFamily = new FontFamily("Segoe MDL2 Assets");
            button.Content = "\uE105"; // floppy disk symbol
            button.Background = new SolidColorBrush(GetSystemAccentColor());
            button.Foreground = new SolidColorBrush(GetSystemForeGroundAccentColor());
            button.Click += (sender, args) => ClickAction();
            // give it a bit of spacing around itself
            button.Margin = new Thickness(10);
            return button;
        }

        /// <summary>
        /// Builds and returns a button to start the recording process. (Actually what it does is call the passed <paramref name="ClickAction"/>, but the button is designed to intiuitively mean that clicking it should start the recording process)
        /// </summary>
        /// <param name="ClickAction"></param>
        /// <returns></returns>
        public static Button BuildStartRecordingButton(Action ClickAction)
        {
            Button button = new Button();
            button.FontSize = 32;
            button.Width = 150;
            button.Height = 150;
            button.FontFamily = new FontFamily("Segoe MDL2 Assets");
            button.Content = "\uE1D6"; // the microphone symbol
            // set the color of the button to be equal to the system theme's accent color
            Brush backgroundBrush = new SolidColorBrush(GetSystemAccentColor());
            Brush foregroundBrush = new SolidColorBrush(GetSystemForeGroundAccentColor());
            button.Background = backgroundBrush;
            button.Foreground = foregroundBrush;
            button.Click += (sender, args) => ClickAction();
            return button;
        }

        /// <summary>
        /// Builds and returns a button to stop the recording process. (Actually what it does is call the passed <paramref name="ClickAction"/>, but the button is designed to intiuitively mean that clicking it should stop the recording process)
        /// </summary>
        /// <param name="ClickAction"></param>
        /// <returns></returns>
        public static Button BuildStopRecordingButton(Action ClickAction)
        {
            Button button = new Button();
            button.FontSize = 32;
            button.Width = 150;
            button.Height = 150;
            button.FontFamily = new FontFamily("Segoe MDL2 Assets");
            button.Content = "\uE15B"; // stop symbol
            button.Background = new SolidColorBrush(GetSystemAccentColor());
            button.Foreground = new SolidColorBrush(GetSystemForeGroundAccentColor());
            button.Click += (sender, args) => ClickAction();
            return button;
        }

        /// <summary>
        /// Builds a relative panel containing the details of a voice memo
        /// </summary>
        /// <param name="voiceMemo">The voice memo to build the panel for</param>
        /// <param name="audioRecorder">The object that will play the voice memo's audio</param>
        /// <param name="DeleteCallBack">the callback function for when the voice memo's delete button is clicked</param>
        /// <returns></returns>
        public static RelativePanel BuildVoiceMemoPanel(VoiceMemo voiceMemo, AudioRecorder audioRecorder, Action DeleteCallBack = null)
        {
            var panel = new RelativePanel();
            panel.Margin = new Thickness(0, 10, 0, 10);
            var ellipse = BuildMemoEllipse();
            var titleBlock = BuildTitleBlock(voiceMemo);
            var durationBlock = BuildDurationBlock(voiceMemo);
            var dateRecordedBlock = BuildDateRecordedBlock(voiceMemo);
            var deleteButton = BuildDeleteButton(voiceMemo, audioRecorder, DeleteCallBack);
            var playbackButton = BuildPlayBackButton(voiceMemo, audioRecorder);
            panel.Children.Add(ellipse);
            panel.Children.Add(titleBlock);
            panel.Children.Add(durationBlock);
            panel.Children.Add(dateRecordedBlock);
            panel.Children.Add(deleteButton);
            panel.Children.Add(playbackButton);
            // position the elements within the panel
            RelativePanel.SetRightOf(titleBlock, ellipse);
            RelativePanel.SetAlignVerticalCenterWith(titleBlock, ellipse);
            RelativePanel.SetBelow(durationBlock, titleBlock);
            RelativePanel.SetAlignLeftWith(durationBlock, titleBlock);
            RelativePanel.SetBelow(dateRecordedBlock, durationBlock);
            RelativePanel.SetAlignLeftWith(dateRecordedBlock, durationBlock);
            RelativePanel.SetBelow(deleteButton, dateRecordedBlock);
            RelativePanel.SetAlignBottomWithPanel(deleteButton, true);
            RelativePanel.SetAlignLeftWithPanel(deleteButton, true);
            RelativePanel.SetBelow(playbackButton, dateRecordedBlock);
            RelativePanel.SetAlignBottomWithPanel(playbackButton, true);
            RelativePanel.SetAlignRightWithPanel(playbackButton, true);
            return panel;
        }

        /// <summary>
        /// Builds and returns a text block whose text is the <see cref="VoiceMemo.DateRecorded"/> of the passed <paramref name="VoiceMemoToAdd"/>.
        /// </summary>
        /// <param name="VoiceMemoToAdd"></param>
        /// <returns></returns>
        private static TextBlock BuildDateRecordedBlock(VoiceMemo VoiceMemoToAdd)
        {
            var dateRecordedBlock = new TextBlock();
            dateRecordedBlock.Margin = new Thickness(0, 0, 0, 20);
            dateRecordedBlock.Text = VoiceMemoToAdd.DateRecorded.ToShortDateString();
            return dateRecordedBlock;
        }

        /// <summary>
        /// Builds and returns a button that deletes the passed <paramref name="VoiceMemoToAdd"/> from the database and the file system
        /// </summary>
        /// <param name="VoiceMemoToAdd"></param>
        /// <param name="audioRecorder"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        private static Button BuildDeleteButton(VoiceMemo VoiceMemoToAdd, AudioRecorder audioRecorder, Action Callback = null)
        {
            var deleteButton = new Button();
            deleteButton.Content = "Delete";
            deleteButton.Click += (sender, arguments) => DeleteVoiceMemoAsync(VoiceMemoToAdd, audioRecorder, Callback);
            return deleteButton;
        }

        /// <summary>
        /// Builds and returns a text block whose contents are the <see cref="VoiceMemo.RecordingDuration"/> of the passed <paramref name="VoiceMemoToAdd"/>.
        /// </summary>
        /// <param name="VoiceMemoToAdd"></param>
        /// <returns></returns>
        private static TextBlock BuildDurationBlock(VoiceMemo VoiceMemoToAdd)
        {
            var durationBlock = new TextBlock();
            durationBlock.Text = $"Duration: {TimeSpan.FromSeconds(VoiceMemoToAdd.RecordingDuration):mm\\:ss}";
            return durationBlock;
        }

        /// <summary>
        /// Builds an Ellipse whose color is the same as the system's UI Accent color and returns it
        /// </summary>
        /// <returns></returns>
        private static Ellipse BuildMemoEllipse()
        {
            var ellipse = new Ellipse();
            ellipse.Margin = new Thickness(10);
            ellipse.Width = 25;
            ellipse.Height = 25;
            var brush = new SolidColorBrush();
            ellipse.Fill = new SolidColorBrush(GetSystemAccentColor());
            // change the color of this ellipse when the system colors change
            return ellipse;
        }

        /// <summary>
        /// Builds and returns a button that plays back the passed <paramref name="VoiceMemoToAdd"/> using the passed <paramref name="audioRecorder"/>
        /// </summary>
        /// <param name="VoiceMemoToAdd"></param>
        /// <param name="audioRecorder"></param>
        /// <returns></returns>
        private static Button BuildPlayBackButton(VoiceMemo VoiceMemoToAdd, AudioRecorder audioRecorder)
        {
            var playbackButton = new Button();
            playbackButton.Content = "Playback";
            playbackButton.Click += (sender, arguments) => PlayVoiceMemo(VoiceMemoToAdd, audioRecorder);
            return playbackButton;
        }

        /// <summary>
        /// Builds and returns text block for the <paramref name="VoiceMemoToAdd"/>. The text of this block is the <see cref="VoiceMemo.DisplayName"/> of the VoiceMemoToAdd.
        /// </summary>
        /// <param name="VoiceMemoToAdd"></param>
        /// <returns></returns>
        private static TextBlock BuildTitleBlock(VoiceMemo VoiceMemoToAdd)
        {
            var titleBlock = new TextBlock();
            titleBlock.FontWeight = FontWeights.Bold;
            titleBlock.Text = VoiceMemoToAdd.DisplayName;
            return titleBlock;
        }

        /// <summary>
        /// Stops playback of the passed <paramref name="audioRecorder"/>, deletes the passed <paramref name="VoiceMemoToDelete"/> from the database and the file system, and calls the passed <paramref name="Callback"/> if it's not null
        /// </summary>
        /// <param name="VoiceMemoToDelete"></param>
        /// <param name="audioRecorder"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        private static async Task DeleteVoiceMemoAsync(VoiceMemo VoiceMemoToDelete, AudioRecorder audioRecorder, Action Callback = null)
        {
            if (await DisplayDeleteFileDialog())
            {
                //stop playing and dispose stream
                audioRecorder.StopPlaybackMedia();
                audioRecorder.DisposeStream();

                //delete file and from database and file system
                audioRecorder.DeleteFile(VoiceMemoToDelete.FileName);
                StoredProcedures.DeleteVoiceNote(VoiceMemoToDelete.VoiceMemoID);
                // call the callback function if it's not null
                Callback?.Invoke();
            }
        }

        /// <summary>
        /// Displays a dialog asking if the user actually wants to confirm the deletion of a voice memo, and then returns true/false depending on if the user actually wants to delete it
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> DisplayDeleteFileDialog()
        {
            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Delete file permanently?",
                Content = "If you delete this file, you won't be able to recover it. Do you want to delete it?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteFileDialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        private static Color GetSystemAccentColor()
        {
            return (Color)Application.Current.Resources["SystemAccentColor"];
        }

        private static Color GetSystemForeGroundAccentColor()
        {
            return (Color)Application.Current.Resources["SystemAccentColorLight3"];
        }

        /// <summary>
        /// Plays back the passed <paramref name="VoiceMemoToPlay"/> using the passed <paramref name="audioRecorder"/>
        /// </summary>
        /// <param name="VoiceMemoToPlay"></param>
        /// <param name="audioRecorder"></param>
        private static async void PlayVoiceMemo(VoiceMemo VoiceMemoToPlay, AudioRecorder audioRecorder)
        {
            //don't let playback if in recording session
            if (!audioRecorder.IsRecording)
            {
                await audioRecorder.PlayFromDisk(VoiceMemoToPlay.FileName);
            }
        }
    }
}