using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capstone.Common;
using Capstone.Helpers;
using Capstone.Models;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace Capstone
{
    /// <summary>
    /// A page to view and create voice Memos from as well as listen to them and delete them
    /// </summary>
    public sealed partial class VoiceMemosPage : Page
    {

        AudioRecorder _audioRecorder;

        public List<VoiceMemo> VoiceMemos { get; set; }

        private VoiceMemo CreateVoiceMemo = new VoiceMemo();
        public VoiceMemosPage()
        {
            this.InitializeComponent();
            this.HideInitialControls();
            //used to record, stop, and play voice note
            this._audioRecorder = new AudioRecorder();
            this.PopulateListOfVoiceMemos();

        }

        private void HideInitialControls()
        {
            this.saveRecording.Visibility = Visibility.Collapsed;
            this.stopRecording.Visibility = Visibility.Collapsed;
            this.deleteRecording.Visibility = Visibility.Collapsed;
            this.displayName.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            //make sure user wants to leave page in case of work in progress
            bool goToMainPage = await DisplayGoBackToMainPageDialog();

            if (goToMainPage)
            {
                _audioRecorder.StopPlaybackMedia();
                _audioRecorder.DisposeStream();
                _audioRecorder.DisposeMedia();
                _audioRecorder.DisposeMemoryBuffer();
                UIUtils.GoToMainPage(this);
            }

        }

        private List<VoiceMemo> ReadVoiceMemosFromDatabase()
        {
            List<VoiceMemo> voiceMemos = StoredProcedures.QueryAllVoiceMemos();
            return voiceMemos;
        }

        private void PopulateListOfVoiceMemos()
        {
            this.VoiceNoteList.Children.Clear();
            this.VoiceMemos = ReadVoiceMemosFromDatabase();
            this.VoiceMemos.ForEach(BuildMemoPanel);
        }

        private void BuildMemoPanel(VoiceMemo VoiceMemoToAdd)
        {
            this.VoiceNoteList.Children.Add(VoiceMemoUIHelper.BuildVoiceMemoPanel(VoiceMemoToAdd, this._audioRecorder, PopulateListOfVoiceMemos));
        }


        private async void Button_ClickDelete(object sender, RoutedEventArgs e)
        {

            //we need to make sure user understands nothing will be saved
            bool discardFile = false;
            discardFile = await DisplayDeleteFileDialog();

            if (discardFile)
            {
                //stop the recording and go back to main voice notes screen
                _audioRecorder.DisposeMedia();
                _audioRecorder.DisposeMemoryBuffer();
                _audioRecorder.DisposeStream();
                this.ResetUIComponents();
            }
        }

        private void Button_ClickStart(object sender, RoutedEventArgs e)
        {
            //toggle pause/play buttons
            OnStartRecordingToggle();
            this._audioRecorder.Record();
        }
        private void Button_ClickStop(object sender, RoutedEventArgs e)
        {
            //toggle buttons
            OnStopRecordingToggle();
            this._audioRecorder.StopRecording();
        }

        private async void Button_ClickSave(object sender, RoutedEventArgs e)
        {
            //make sure user enters a file name. does not need to be unique, as saveaudiotofile will create a unique file if the file name already exists
            bool validateName = ValidateFileName();
            if (validateName)
            {
                //set values
                CreateVoiceMemo.DisplayName = displayName.Text;
                //unfortunately, we don't know the file name for sure until this is ran
                CreateVoiceMemo.FileName = await this._audioRecorder.SaveAudioToFile();
                CreateVoiceMemo.FullFilePath = $"{Windows.ApplicationModel.Package.Current.InstalledLocation.Path}\\VoiceNotes";
                CreateVoiceMemo.RecordingDuration = await _audioRecorder.GetAudioDuration(CreateVoiceMemo.FileName);
                CreateVoiceMemo.DateRecorded = _audioRecorder.GetDateRecorded();
                DateTime timeRecorded = _audioRecorder.GetTimeRecorded();
                // insert the voice memo's details into the database
                StoredProcedures.CreateVoiceNote(CreateVoiceMemo.FileName, CreateVoiceMemo.DisplayName, CreateVoiceMemo.RecordingDuration, CreateVoiceMemo.FullFilePath, CreateVoiceMemo.DateRecorded, timeRecorded);

                // refresh the voice memo list and hide our controls while showing the record button
                this.PopulateListOfVoiceMemos();
                this.displayName.Text = "";
                this.HideInitialControls();
                this.startRecording.Visibility = Visibility.Visible;
            }
            else
            {
                DisplayEnterNameDialog();
            }
        }

        private void OnStartRecordingToggle()
        {
            this.startRecording.Visibility = Visibility.Collapsed;
            this.stopRecording.Visibility = Visibility.Visible;
        }

        private void OnStopRecordingToggle()
        {
            this.stopRecording.Visibility = Visibility.Collapsed;
            this.saveRecording.Visibility = Visibility.Visible;
            this.deleteRecording.Visibility = Visibility.Visible;
            this.displayName.Visibility = Visibility.Visible;
        }

        private async Task<bool> DisplayDeleteFileDialog()
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

        private async Task<bool> DisplayGoBackToMainPageDialog()
        {
            ContentDialog goToMainPageDialog = new ContentDialog
            {
                Title = "Exit Voice Notes",
                Content = "Are you sure you want to go back to Main Page. All works in progress will be lost.",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await goToMainPageDialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        private async void DisplayEnterNameDialog()
        {
            ContentDialog noNameDialog = new ContentDialog
            {
                Title = "No Name Entered",
                Content = "Please Enter a Display Name to save the File.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noNameDialog.ShowAsync();
        }

        private bool ValidateFileName()
        {
            return StringUtils.IsNotBlank(displayName.Text);
        }

        private void ResetUIComponents()
        {
            // hide all the ui components, reset our text box, and show the record button
            this.HideInitialControls();
            this.displayName.Text = "";
            this.startRecording.Visibility = Visibility.Visible;
        }

    }
}
