using System;
using Capstone.Common;
using Capstone.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Capstone.Actions
{
    public class VoiceMemoAction : Action
    {
        private string VoiceMemoTitle { get; set; }
        private AudioRecorder AudioRecorder { get; set; }
        private Button StartRecordingButton;
        private Button StopRecordingButton;
        private Button SaveRecordingButton;
        private Button DeleteRecordingButton;
        private TextBox RecordingNameBox;

        public VoiceMemoAction(string CommandString)
        {
            this.CommandString = CommandString;
            this.AudioRecorder = new AudioRecorder();
        }

        public override void PerformAction()
        {
            // TODO start recording voice and show controls on the dynamic area
            if (this.DynamicArea != null)
            {
                // have bob tell the user to click the recording button when they're ready
                string ssmlText = new SSMLBuilder().Prosody("Sure, just click the button on your screen when you're ready.", contour: "(0%, +10%) (50%, -5%) (80%, -15%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssmlText);
                this.ClearArea();
                this.SetUpUI();
            }
        }

        private void SetUpUI()
        {
            this.StartRecordingButton = VoiceMemoUIHelper.BuildStartRecordingButton(this.StartRecording);
            this.StopRecordingButton = VoiceMemoUIHelper.BuildStopRecordingButton(this.StopRecording);
            this.SaveRecordingButton = VoiceMemoUIHelper.BuildSaveRecordingButton(this.SaveRecording);
            this.DeleteRecordingButton = VoiceMemoUIHelper.BuildDeleteRecordingButton(this.DeleteRecording);
            this.RecordingNameBox = VoiceMemoUIHelper.BuildDisplayNameTextBox();
            // relatively place all the components
            RelativePanel.SetAlignVerticalCenterWithPanel(StartRecordingButton, true);
            RelativePanel.SetAlignHorizontalCenterWithPanel(StartRecordingButton, true);
            RelativePanel.SetAlignVerticalCenterWithPanel(StopRecordingButton, true);
            RelativePanel.SetAlignHorizontalCenterWithPanel(StopRecordingButton, true);
            RelativePanel.SetAlignVerticalCenterWithPanel(SaveRecordingButton, true);
            RelativePanel.SetAlignHorizontalCenterWithPanel(SaveRecordingButton, true);
            RelativePanel.SetAlignVerticalCenterWithPanel(DeleteRecordingButton, true);
            RelativePanel.SetAlignHorizontalCenterWithPanel(DeleteRecordingButton, true);
            // place the save and delete buttons next to each other
            RelativePanel.SetLeftOf(DeleteRecordingButton, SaveRecordingButton);
            // place the text box below the save button
            RelativePanel.SetBelow(RecordingNameBox, SaveRecordingButton);
            RelativePanel.SetAlignLeftWith(RecordingNameBox, SaveRecordingButton);
            RelativePanel.SetAlignRightWith(RecordingNameBox, SaveRecordingButton);
            // hide all our buttons except the start recording one
            this.ResetUIComponents();
            // now add everything to the dynamic area
            this.DynamicArea.Children.Add(StartRecordingButton);
            this.DynamicArea.Children.Add(StopRecordingButton);
            this.DynamicArea.Children.Add(SaveRecordingButton);
            this.DynamicArea.Children.Add(DeleteRecordingButton);
            this.DynamicArea.Children.Add(RecordingNameBox);
        }

        /// <summary>
        /// Starts the recording process using our <see cref="AudioRecorder"/> class
        /// </summary>
        private void StartRecording()
        {
            // this ensures that we can make UI changes since it's not guaranteed that we have access to the UI at this point
            Utils.RunOnMainThread(() =>
            {
                try
                {
                    this.AudioRecorder.Record();
                    this.StartRecordingButton.Visibility = Visibility.Collapsed;
                    this.StopRecordingButton.Visibility = Visibility.Visible;
                }
                catch (InvalidOperationException)
                {
                    // no op
                }
            });
        }

        /// <summary>
        /// Stops the recording process of our <see cref="AudioRecorder"/>
        /// </summary>
        private void StopRecording()
        {
            // this ensures that we can make UI changes since it's not guaranteed that we have access to the UI at this point
            Utils.RunOnMainThread(() =>
            {
                this.AudioRecorder.StopRecording();
                // hide the stop recording button and show the save and cancel buttons as well as the input for the recording title
                this.StopRecordingButton.Visibility = Visibility.Collapsed;
                this.SaveRecordingButton.Visibility = Visibility.Visible;
                this.DeleteRecordingButton.Visibility = Visibility.Visible;
                this.RecordingNameBox.Visibility = Visibility.Visible;
            });
        }

        private void SaveRecording()
        {
            // make sure to run this in the main thread so that we can access our recording name box
            Utils.RunOnMainThread(async () =>
            {
                if (StringUtils.IsNotBlank(this.RecordingNameBox.Text))
                {
                    try
                    {
                        string DisplayName = RecordingNameBox.Text;
                        string FileName = await this.AudioRecorder.SaveAudioToFile();
                        string FullFilePath = $"{Windows.ApplicationModel.Package.Current.InstalledLocation.Path}\\VoiceNotes";
                        int RecordingDuration = await this.AudioRecorder.GetAudioDuration(FileName);
                        DateTime DateRecorded = this.AudioRecorder.GetDateRecorded();
                        DateTime timeRecorded = this.AudioRecorder.GetTimeRecorded();
                        // insert the voice memo into the database
                        StoredProcedures.CreateVoiceNote(FileName, DisplayName, RecordingDuration, FullFilePath, DateRecorded, DateRecorded);
                        // clear our dynamic area and show the ui for the newly-recorded voice memo
                        this.ClearArea();
                        RelativePanel voiceMemoPanel = VoiceMemoUIHelper.BuildVoiceMemoPanel(StoredProcedures.QueryLatestVoiceMemo(), this.AudioRecorder, () => Utils.RunOnMainThread(() => this.ClearArea()));
                        // position the panel in the center of the panel
                        RelativePanel.SetAlignHorizontalCenterWithPanel(voiceMemoPanel, true);
                        RelativePanel.SetAlignVerticalCenterWithPanel(voiceMemoPanel, true);
                        this.DynamicArea.Children.Add(voiceMemoPanel);
                    }
                    catch (Exception)
                    {
                        // TODO display an error message
                    }
                }
                else
                {
                    UIUtils.HighlightUIElement(this.RecordingNameBox);
                }
            });
        }

        /// <summary>
        /// Deletes the recording the user just made
        /// </summary>
        private void DeleteRecording()
        {
            // make sure to run this on the main thread
            Utils.RunOnMainThread(async () =>
            {
                // show a dialog asking the user if they actually want to delete it
                ContentDialog confirmDeleteDialog = new ContentDialog
                {
                    Title = "Delete file permanently?",
                    Content = "If you delete this file, you won't be able to recover it. Do you want to delete it?",
                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel"
                };
                bool deleteConfirmed = await confirmDeleteDialog.ShowAsync() == ContentDialogResult.Primary;
                if (deleteConfirmed)
                {
                    // dispose the media, memory buffer, and stream
                    this.AudioRecorder.DisposeMedia();
                    this.AudioRecorder.DisposeMemoryBuffer();
                    this.AudioRecorder.DisposeStream();
                    // clear our dynamic area
                    this.ClearArea();
                }
            });
        }

        private void ResetUIComponents()
        {
            this.StopRecordingButton.Visibility = Visibility.Collapsed;
            this.SaveRecordingButton.Visibility = Visibility.Collapsed;
            this.DeleteRecordingButton.Visibility = Visibility.Collapsed;
            this.RecordingNameBox.Text = "";
            this.RecordingNameBox.Visibility = Visibility.Collapsed;
            this.StartRecordingButton.Visibility = Visibility.Visible;
        }
    }
}