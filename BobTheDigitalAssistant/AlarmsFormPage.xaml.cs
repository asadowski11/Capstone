using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.Helpers;
using BobTheDigitalAssistant.Models;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BobTheDigitalAssistant
{
    /// <summary>
    /// The form for editing and creating an alarm. Technically this page is only used to edit an alarm, as "creating" an alarm involves passing in a blank alarm to this page
    /// </summary>
    public sealed partial class AlarmsFormPage : Page
    {
        public Alarm AlarmToEdit { get; set; }

        public AlarmsFormPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // get the alarm being passed to this screen
            this.AlarmToEdit = (Alarm)e.Parameter;
            this.PopulateFormFromAlarm();
        }

        public void PopulateFormFromAlarm()
        {
            // set the title field
            this.AlarmTitleInput.Text = this.AlarmToEdit.Title;
            // set the date and time fields
            this.AlarmDatePicker.Date = this.AlarmToEdit.ActivateDateAndTime.Date;
            this.AlarmTimePicker.Time = this.AlarmToEdit.ActivateDateAndTime.TimeOfDay;

            // set the minimum date on our date picker field (minimum time can't be set on a timepicker field)
            this.AlarmDatePicker.MinDate = System.DateTime.Now.Date;
        }

        private void PopulateAlarmFromForm()
        {
            var date = this.AlarmDatePicker.Date;
            var time = this.AlarmTimePicker.Time;
            this.AlarmToEdit.ActivateDateAndTime = date.Value.DateTime + time;
            this.AlarmToEdit.Title = this.AlarmTitleInput.Text.Trim();
        }

        private void SubmitAlarmButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.ValidateForm())
            {
                this.PopulateAlarmFromForm();
                if (this.AlarmToEdit.AlarmID == -1)
                {
                    StoredProcedures.CreateAlarm(this.AlarmToEdit.Title, this.AlarmToEdit.ActivateDateAndTime);
                    AlarmAndReminderHelper.ScheduleAlarm(StoredProcedures.QueryLatestAlarm());
                }
                else
                {
                    AlarmAndReminderHelper.RescheduleAlarm(this.AlarmToEdit);
                    StoredProcedures.UpdateAlarm(this.AlarmToEdit.AlarmID, this.AlarmToEdit.Title, this.AlarmToEdit.ActivateDateAndTime, false);
                }
                UIUtils.GoBack(this, typeof(AlarmsPage));
            }
        }

        public bool ValidateForm()
        {
            // remove highlighting from the time field
            UIUtils.HighlightUIElement(this.AlarmTimePicker, Colors.Transparent);
            // make sure that the title is not empty or blank
            bool isValid = true;
            if (!this.ValidateTime())
            {
                isValid = false;
                UIUtils.HighlightUIElement(this.AlarmTimePicker);
            }
            // don't need to validate date since the earliest it can go is today
            return isValid;
        }

        public bool ValidateTime()
        {
            // get the hours and minutes of our time and compare them against date.now
            var now = System.DateTime.Now;
            var timeHours = this.AlarmTimePicker.Time.Hours;
            var timeMinutes = this.AlarmTimePicker.Time.Minutes;
            var timeDay = this.AlarmDatePicker.Date.Value.DayOfYear;
            return timeDay > now.DayOfYear || timeHours > now.Hour || (timeHours >= now.Hour && timeMinutes >= now.Minute);
        }

        private void CancelAlarmButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UIUtils.GoBack(this, typeof(AlarmsPage));
        }
    }
}