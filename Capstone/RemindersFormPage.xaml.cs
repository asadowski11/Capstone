using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Capstone.Models;
using Capstone.Common;
using Windows.UI;

namespace Capstone
{

    /// <summary>
    /// The form for editing and creating a Reminder. Technically this page is only used to edit a Reminder, as "creating" a Reminder involves passing in a blank Reminder to this page
    /// </summary>
    public sealed partial class RemindersFormPage : Page
    {
        public Reminder ReminderToEdit { get; set; }
        public RemindersFormPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // get the Reminder being passed to this screen
            this.ReminderToEdit = (Reminder)e.Parameter;
            this.PopulateFormFromReminder();
        }

        public void PopulateFormFromReminder()
        {
            // set the title field
            this.ReminderTitleInput.Text = this.ReminderToEdit.Title;
            // set the description field
            this.ReminderDescriptionInput.Text = this.ReminderToEdit.Description;
            // set the date and time fields
            this.ReminderDatePicker.Date = this.ReminderToEdit.ActivateDateAndTime.Date;
            this.ReminderTimePicker.Time = this.ReminderToEdit.ActivateDateAndTime.TimeOfDay;

            // set the minimum date on our date picker field (minimum time can't be set on a timepicker field)
            this.ReminderDatePicker.MinDate = System.DateTime.Now.Date;
        }

        private void PopulateReminderFromForm()
        {
            var date = this.ReminderDatePicker.Date;
            var time = this.ReminderTimePicker.Time;
            this.ReminderToEdit.ActivateDateAndTime = date.Value.DateTime + time;
            this.ReminderToEdit.Title = this.ReminderTitleInput.Text.Trim();
            this.ReminderToEdit.Description = this.ReminderDescriptionInput.Text.Trim();
        }

        private void SubmitReminderButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.ValidateForm())
            {
                this.PopulateReminderFromForm();
                if (this.ReminderToEdit.ReminderID == -1)
                {
                    StoredProcedures.CreateReminder(this.ReminderToEdit.Title, this.ReminderToEdit.ActivateDateAndTime, this.ReminderToEdit.Description);
                }
                else
                {
                    StoredProcedures.UpdateReminder(this.ReminderToEdit.ReminderID, this.ReminderToEdit.Title, this.ReminderToEdit.ActivateDateAndTime, this.ReminderToEdit.Description, false);
                }
                this.Frame.Navigate(typeof(RemindersPage));
            }
        }

        public bool ValidateForm()
        {
            // remove highlighting from our title and time fields
            UIUtils.HighlightUIElement(this.ReminderTitleInput, Colors.Transparent);
            UIUtils.HighlightUIElement(this.ReminderTimePicker, Colors.Transparent);
            // make sure that the title is not empty or blank
            bool isValid = true;
            if (Common.StringUtils.IsBlank(this.ReminderTitleInput.Text))
            {
                isValid = false;
                UIUtils.HighlightUIElement(this.ReminderTitleInput);
            }
            if (!this.ValidateTime())
            {
                UIUtils.HighlightUIElement(this.ReminderTimePicker);
                isValid = false;
            }
            // don't need to validate date since the earliest it can go is today, and description is optional
            return isValid;
        }

        public bool ValidateTime()
        {
            // get the hours and minutes of our time and compare them against date.now
            var now = System.DateTime.Now;
            var timeHours = this.ReminderTimePicker.Time.Hours;
            var timeMinutes = this.ReminderTimePicker.Time.Minutes;
            var timeDay = this.ReminderDatePicker.Date.Value.DayOfYear;
            return timeDay > now.DayOfYear || timeHours > now.Hour || (timeHours >= now.Hour && timeMinutes >= now.Minute);
        }

        private void CancelReminderButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // TODO maybe we should add a confirmation box? or maybe we shouldn't because of how simple it is to create a Reminder
            this.Frame.Navigate(typeof(RemindersPage));
        }
    }
}
