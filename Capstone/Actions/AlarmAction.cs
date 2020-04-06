using Capstone.Common;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Capstone.Actions
{
    public class AlarmAction : Action
    {
        public AlarmAction(AlarmActionTypes ActionType, string CommandString)
        {
            this.ActionType = ActionType;
            this.CommandString = CommandString;
        }

        public enum AlarmActionTypes
        {
            CREATE = 0,
            EDIT = 1,
            DELETE = 2
        }

        public AlarmActionTypes ActionType { get; set; }
        public override async void PerformAction()
        {
            AlarmActionTypes desiredAction = this.GetActionFromCommand();
            switch (desiredAction)
            {
                case AlarmActionTypes.CREATE:
                    var alarm = await this.NewAlarm();
                    this.ClearArea();
                    if (this.DynamicArea != null)
                    {
                        RelativePanel panel = CreateAlarmCard(alarm);
                        this.DynamicArea.Children.Add(panel);
                        RelativePanel.SetAlignHorizontalCenterWithPanel(panel, true);
                        RelativePanel.SetAlignVerticalCenterWithPanel(panel, true);
                    }
                    break;
                case AlarmActionTypes.EDIT:
                    this.EditAlarm();
                    break;
                case AlarmActionTypes.DELETE:
                    this.DeleteAlarm();
                    break;
            }
        }

        private async Task<Alarm> CreateAlarm()
        {
            Alarm createdAlarm = new Alarm();
            DateTime dateTime = await GetAlarmDateAndTime();
            string title = FindAlarmTitle();
            createdAlarm.Title = title;
            createdAlarm.ActivateDateAndTime = dateTime;
            return createdAlarm;
        }

        private RelativePanel CreateAlarmCard(Alarm AlarmToAdd)
        {
            // each alarm is wrapped in a relative panel
            RelativePanel alarmPanel = new RelativePanel();
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
            return alarmPanel;
        }

        private TextBlock CreateAlarmDateBlock(Alarm AlarmToAdd)
        {
            var alarmDateBlock = new TextBlock();
            alarmDateBlock.Text = AlarmToAdd.ActivateDateAndTime.ToString("g");
            alarmDateBlock.FontSize = 24;
            alarmDateBlock.Margin = new Thickness(10);
            // if the reminder is expired, gray out the text and strike through it
            if (AlarmToAdd.IsExpired)
            {
                alarmDateBlock.TextDecorations = TextDecorations.Strikethrough;
                Brush grayBrush = new SolidColorBrush(Colors.Gray);
                alarmDateBlock.Foreground = grayBrush;
            }
            return alarmDateBlock;
        }

        private TextBlock CreateAlarmTitleBlock(Alarm AlarmToAdd)
        {
            var alarmTitleBlock = new TextBlock();
            alarmTitleBlock.Text = AlarmToAdd.Title;
            alarmTitleBlock.Margin = new Thickness(10);
            alarmTitleBlock.FontSize = 32;
            alarmTitleBlock.TextWrapping = TextWrapping.Wrap;
            alarmTitleBlock.MaxLines = 2;
            // if the alarm is expired, gray out the text and strike through it
            if (AlarmToAdd.IsExpired)
            {
                alarmTitleBlock.TextDecorations = TextDecorations.Strikethrough;
                Brush grayBrush = new SolidColorBrush(Colors.Gray);
                alarmTitleBlock.Foreground = grayBrush;
            }
            return alarmTitleBlock;
        }

        private Button CreateDeleteButton(Alarm AlarmToAdd)
        {
            Button deleteButton = new Button();
            deleteButton.Click += (sender, eventArgs) => this.DeleteLatestAlarm();
            deleteButton.Content = "Delete";
            deleteButton.Width = 150;
            deleteButton.Margin = new Thickness(10);
            return deleteButton;
        }

        private Button CreateEditButton(Alarm AlarmToAdd)
        {
            Button editButton = new Button();
            editButton.Click += (sender, eventArgs) => this.EditLatestAlarm();
            editButton.Content = "Edit";
            editButton.Width = 150;
            editButton.Margin = new Thickness(10);
            return editButton;
        }

        private void DeleteAlarm()
        {
            Alarm alarmToDelete = GetAlarmForClosestMatchToPassedDate();
            if (alarmToDelete != null)
            {
                StoredProcedures.DeleteAlarm(alarmToDelete.AlarmID);
                string message = new SSMLBuilder().Prosody("Alright, cancelled your alarm.", contour: "(0%, +5%) (10%,-5%) (50%,+1%) (80%,+5%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, message);
                ShowMessage($"Successfully deleted alarm {alarmToDelete.Title}");
            }
            else
            {
                this.ClearArea();
                // no alarm found, tell the user
                string message = new SSMLBuilder().Prosody("Sorry, but I wasn't able to find an alarm for that time.", contour: "(0%,+5%) (1%,-5%) (2%,+1%) (3%,-1%) (10%,+1%) (20%,-1%) (30%,+1%) (40%,-1%) (50%,+1%) (80%,-1%)").Build();
                TextToSpeechEngine.SpeakInflectedText(this.MediaElement, message);
                this.ShowMessage("Sorry, but I wasn't able to find an alarm for that time.");
            }
        }

        private void DeleteLatestAlarm()
        {
            StoredProcedures.DeleteLatestAlarm();
            // if we have a dynamic area, remove the children from it
            if (this.DynamicArea != null)
            {
                ShowMessage("Alarm deleted.");
            }
        }

        private void EditAlarm()
        {
            // it's pretty hard to figure out which alarm to edit and which fields need to be edited, so direct the users to the alarms page
            this.ClearArea();
            string text = "For now, editing alarms through voice is not supported. You can edit an alarm by going to the alarms page, finding the alarm you want to edit, and clicking the \"edit\" button.";
            string ssmlText = new SSMLBuilder().Prosody(text, pitch: "+2%", contour: "(10%,-2%) (40%, -3%) (80%, +3%)").Build();
            TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssmlText);
            this.ShowMessage(text);
        }

        private void EditLatestAlarm()
        {
            (Window.Current.Content as Frame).Navigate(typeof(AlarmsFormPage), StoredProcedures.QueryLatestAlarm());
        }

        private string FindAlarmTitle()
        {
            string title = "";
            var titleIdentifierRegex = new Regex("(?<=(named |called |titled )).+");
            var match = titleIdentifierRegex.Match(this.CommandString);
            if (match.Success)
            {
                title = match.Value;
            }
            return title;
        }

        private AlarmActionTypes GetActionFromCommand()
        {
            var addRegex = new Regex("(?i)(create|set|new|add)(?-i)");
            var editRegex = new Regex("(?i)(edit|update|change|alter)(?-i)");
            var deleteRegex = new Regex("(?i)(delete|remove|clear|void|cancel)(?-i)");
            if (addRegex.IsMatch(this.CommandString))
            {
                return AlarmActionTypes.CREATE;
            }
            else if (editRegex.IsMatch(this.CommandString))
            {
                return AlarmActionTypes.EDIT;
            }
            else if (deleteRegex.IsMatch(this.CommandString))
            {
                return AlarmActionTypes.DELETE;
            }
            else
            {
                // default to create if we can't tell which the user wanted
                return AlarmActionTypes.CREATE;
            }
        }

        private async Task<DateTime> GetAlarmDateAndTime()
        {
            DateTime activatedDateTime;

            try
            {
                var titleRegex = new Regex("(?i)(called|titled|named)(?-i)");
                var commandWithoutTitle = titleRegex.Split(this.CommandString)[0].Trim();
                activatedDateTime = DateTimeParser.ParseDateTimeFromText(commandWithoutTitle);
            }
            catch (DateParseException)
            {
                // TODO ask for the date and time since it could not be parsed once the speech recognition is set up
                activatedDateTime = DateTime.Now;
            }

            return activatedDateTime;
        }
        /// <summary>
        /// Attempts to find the alarm whose activation date matches the passed date in this object's commandString
        /// </summary>
        /// <returns>the found alarm, which may be null if an alarm wasn't found</returns>
        private Alarm GetAlarmForClosestMatchToPassedDate()
        {
            List<Alarm> alarms = StoredProcedures.QueryAllUnexpiredAlarms();
            // get the date from the command text
            var now = DateTime.Now;
            // putting in the datetime to start at midnight of this day so that times don't get shifted around. I know it's marked as only used for tests, but it's what I had to do
            DateTime commandDateTime = DateTimeParser.ParseDateTimeFromText(this.CommandString, new DateTime(now.Year, now.Month, now.Day, 0, 0, 0));
            Alarm foundAlarm = null;
            // if there's only one alarm, that's the one we're getting
            if (alarms.Count == 1)
            {
                foundAlarm = alarms[0];
            }
            else
            {
                // find the first alarm whose hour and minute matches the command date time
                foundAlarm = alarms.Find(alarm => alarm.ActivateDateAndTime.Hour == commandDateTime.Hour && alarm.ActivateDateAndTime.Minute == commandDateTime.Minute);
            }
            return foundAlarm;
        }

        private async Task<Alarm> NewAlarm()
        {
            Alarm createdAlarm = await this.CreateAlarm();
            // insert the alarm into the database
            StoredProcedures.CreateAlarm(createdAlarm.Title, createdAlarm.ActivateDateAndTime);
            string mainPart = $"Alright, alarm set for ";
            string datePart = createdAlarm.ActivateDateAndTime.ToString("MMM d");
            string timePart = createdAlarm.ActivateDateAndTime.ToString("h:mm tt");
            string rawSSML = new SSMLBuilder().Add(mainPart).SayAs(datePart, SSMLBuilder.SayAsTypes.DATE).Add(" at ").SayAs(timePart, SSMLBuilder.SayAsTypes.TIME).BuildWithoutWrapperElement();
            string prosodySSML = new SSMLBuilder().Prosody(rawSSML, pitch: "+5%", contour: "(10%,+5%) (50%,-5%) (80%,-5%)").Build();
            TextToSpeechEngine.SpeakInflectedText(this.MediaElement, prosodySSML);
            return createdAlarm;
        }
    }
}
