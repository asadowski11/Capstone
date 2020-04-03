using Capstone.Common;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Capstone.Models;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Capstone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlarmsPage : Page
    {
        private readonly List<Alarm> Alarms;
        public AlarmsPage()
        {
            this.InitializeComponent();
            this.Alarms = new List<Alarm>();
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            UIUtils.GoToMainPage(this);
        }

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            var NewAlarm = new Alarm();
            this.Frame.Navigate(typeof(AlarmsFormPage), NewAlarm);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Alarms.Clear();
            this.Alarms.AddRange(this.GetAlarmsFromDatabase());
            this.PopulateScreenWithAlarms();
        }

        private List<Alarm> GetAlarmsFromDatabase()
        {
            List<Alarm> alarms = StoredProcedures.QueryAllAlarms();
            return alarms;
        }

        private void PopulateScreenWithAlarms()
        {
            // clear all alarms and re-populate them
            var children = this.VariableGrid.Children;
            if (children.Count > 1)
            {
                // more than 1 means that there's alarms, as the button is a child.
                while (children.Count > 1)
                {
                    children.RemoveAt(children.Count - 1);
                }

            }
            this.Alarms.ForEach(this.AddAlarmToScreen);
        }

        private void AddAlarmToScreen(Alarm AlarmToAdd)
        {
            // each alarm is wrapped in a relative panel
            RelativePanel alarmPanel = new RelativePanel();
            alarmPanel.Margin = new Thickness(5, 0, 5, 5);
            var borderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            alarmPanel.BorderBrush = borderBrush;
            alarmPanel.BorderThickness = new Thickness(1);
            // create the text blocks for the title and date
            var alarmTitleBlock = this.CreateAlarmTitleBlock(AlarmToAdd);
            var alarmDateBlock = this.CreateAlarmDateBlock(AlarmToAdd);
            var deleteButton = this.CreateDeleteButton(AlarmToAdd);
            var editButton = this.CreateEditButton(AlarmToAdd);
            // add the blocks and buttons
            alarmPanel.Children.Add(alarmTitleBlock);
            alarmPanel.Children.Add(alarmDateBlock);
            alarmPanel.Children.Add(deleteButton);
            alarmPanel.Children.Add(editButton);
            // relatively place the edit text block
            RelativePanel.SetBelow(alarmDateBlock, alarmTitleBlock);
            // relatively place the delete button
            RelativePanel.SetAlignBottomWithPanel(deleteButton, true);
            RelativePanel.SetAlignLeftWithPanel(deleteButton, true);
            RelativePanel.SetBelow(deleteButton, alarmDateBlock);
            RelativePanel.SetLeftOf(deleteButton, editButton);
            // relatively place the edit button
            RelativePanel.SetAlignBottomWithPanel(editButton, true);
            RelativePanel.SetAlignRightWithPanel(editButton, true);
            RelativePanel.SetBelow(editButton, alarmDateBlock);
            this.VariableGrid.Children.Add(alarmPanel);
        }

        private TextBlock CreateAlarmTitleBlock(Alarm AlarmToAdd)
        {
            var alarmTitleBlock = new TextBlock();
            alarmTitleBlock.Text = AlarmToAdd.Title;
            alarmTitleBlock.Margin = new Thickness(10);
            alarmTitleBlock.FontSize = 32;
            alarmTitleBlock.TextWrapping = TextWrapping.Wrap;
            alarmTitleBlock.MaxLines = 2;
            return alarmTitleBlock;
        }

        private TextBlock CreateAlarmDateBlock(Alarm AlarmToAdd)
        {
            var alarmDateBlock = new TextBlock();
            alarmDateBlock.Text = AlarmToAdd.ActivateDateAndTime.ToString("g");
            alarmDateBlock.FontSize = 24;
            alarmDateBlock.Margin = new Thickness(10);
            return alarmDateBlock;
        }

        private Button CreateDeleteButton(Alarm AlarmToAdd)
        {
            Button deleteButton = new Button();
            deleteButton.Click += (sender, eventArgs) => this.DeleteAlarm(AlarmToAdd);
            deleteButton.Content = "Delete";
            deleteButton.Width = 150;
            deleteButton.Margin = new Thickness(10, 10, 0, 0);
            return deleteButton;
        }

        private Button CreateEditButton(Alarm AlarmToAdd)
        {
            Button editButton = new Button();
            editButton.Click += (sender, eventArgs) => this.EditAlarm(AlarmToAdd);
            editButton.Content = "Edit";
            editButton.Width = 150;
            editButton.Margin = new Thickness(0, 10, 10, 0);
            return editButton;
        }

        private void DeleteAlarm(Alarm AlarmToDelete)
        {
            StoredProcedures.DeleteAlarm(AlarmToDelete.AlarmID);
            // clear the list of reminders and re-populate them
            this.Alarms.Clear();
            this.Alarms.AddRange(GetAlarmsFromDatabase());
            this.PopulateScreenWithAlarms();
        }

        private void EditAlarm(Alarm AlarmToEdit)
        {
            this.Frame.Navigate(typeof(AlarmsFormPage), AlarmToEdit);
        }
    }
}
