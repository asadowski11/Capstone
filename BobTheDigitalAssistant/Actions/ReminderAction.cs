using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.Models;
using BobTheDigitalAssistant.SpeechRecognition;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BobTheDigitalAssistant.Actions
{
	public class ReminderAction : Action
	{
		public ReminderAction(ReminderActionTypes ActionType, string CommandString)
		{
			this.ActionType = ActionType;
			this.CommandString = CommandString;
		}

		public enum ReminderActionTypes
		{
			CREATE = 0,
			EDIT = 1,
			DELETE = 2
		}

		public ReminderActionTypes ActionType { get; set; }

		public override async void PerformAction()
		{
			ReminderActionTypes desiredAction = this.GetActionFromCommand();
			switch (desiredAction)
			{
				case ReminderActionTypes.CREATE:
					var reminder = await this.NewReminderAsync();
					// if the reminder is null, don't do anything
					if (reminder != null)
					{
						this.ClearArea();
						if (this.DynamicArea != null)
						{
							RelativePanel panel = CreateReminderCard(reminder);
							this.DynamicArea.Children.Add(panel);
							RelativePanel.SetAlignHorizontalCenterWithPanel(panel, true);
							RelativePanel.SetAlignVerticalCenterWithPanel(panel, true);
						}
					}
					break;

				case ReminderActionTypes.EDIT:
					this.EditReminder();
					break;

				case ReminderActionTypes.DELETE:
					this.DeleteReminder();
					break;
			}
		}

		private Button CreateDeleteButton()
		{
			Button deleteButton = new Button();
			deleteButton.Click += (sender, eventArgs) => this.DeleteLatestReminder();
			deleteButton.Content = "Delete";
			deleteButton.Width = 150;
			deleteButton.Margin = new Thickness(10);
			return deleteButton;
		}

		private TextBlock CreateDescriptionBlock(Reminder ReminderToAdd)
		{
			var DescriptionBlock = new TextBlock();
			DescriptionBlock.TextWrapping = TextWrapping.Wrap;
			DescriptionBlock.MaxLines = 4;
			DescriptionBlock.Margin = new Thickness(10);
			DescriptionBlock.Text = ReminderToAdd.Description;
			// if the reminder is expired, gray out the text and strike through it
			if (ReminderToAdd.IsExpired)
			{
				DescriptionBlock.TextDecorations = TextDecorations.Strikethrough;
				Brush grayBrush = new SolidColorBrush(Colors.Gray);
				DescriptionBlock.Foreground = grayBrush;
			}
			return DescriptionBlock;
		}

		private Button CreateEditButton()
		{
			Button editButton = new Button();
			editButton.Click += (sender, eventArgs) => this.EditLatestReminder();
			editButton.Content = "Edit";
			editButton.Width = 150;
			editButton.Margin = new Thickness(10);
			return editButton;
		}

		private async Task<Reminder> CreateReminderAsync()
		{
			Reminder createdReminder = new Reminder();
			DateTime dateTime = GetReminderDateAndTime();
			string title = await GetReminderTitleAsync();
			if (title == null)
			{
				string message = "Sorry, but reminders require a title. If you don't need a title, try setting an alarm instead.";
				TextToSpeechEngine.SpeakText(this.MediaElement, message);
				this.ShowMessage(message);
				createdReminder = null;
			}
			else
			{
				// description can't be easily input with text, so don't set it
				createdReminder.Title = title;
				createdReminder.ActivateDateAndTime = dateTime;
			}

			return createdReminder;
		}

		private RelativePanel CreateReminderCard(Reminder ReminderToAdd)
		{
			// each reminder is wrapped in a relative panel
			RelativePanel reminderPanel = new RelativePanel();
			reminderPanel.Margin = new Thickness(5, 0, 5, 5);
			var borderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
			reminderPanel.BorderBrush = borderBrush;
			reminderPanel.BorderThickness = new Thickness(1);
			// create the text blocks for the title and date
			var ReminderTitleBlock = this.CreateReminderTitleBlock(ReminderToAdd);
			var ReminderDateBlock = this.CreateReminderDateBlock(ReminderToAdd);
			var ReminderDescriptionBlock = this.CreateDescriptionBlock(ReminderToAdd);
			var deleteButton = this.CreateDeleteButton();
			var editButton = this.CreateEditButton();
			// add the blocks and buttons
			reminderPanel.Children.Add(ReminderTitleBlock);
			reminderPanel.Children.Add(ReminderDateBlock);
			reminderPanel.Children.Add(ReminderDescriptionBlock);
			reminderPanel.Children.Add(deleteButton);
			reminderPanel.Children.Add(editButton);
			// relatively place the edit text block
			RelativePanel.SetRightOf(ReminderDateBlock, ReminderTitleBlock);
			RelativePanel.SetAlignRightWithPanel(ReminderDateBlock, true);
			// relatively place the description block
			RelativePanel.SetBelow(ReminderDescriptionBlock, ReminderTitleBlock);
			// relatively place the delete button
			RelativePanel.SetAlignBottomWithPanel(deleteButton, true);
			RelativePanel.SetAlignLeftWithPanel(deleteButton, true);
			RelativePanel.SetLeftOf(deleteButton, editButton);
			// relatively place the edit button
			RelativePanel.SetAlignBottomWithPanel(editButton, true);
			RelativePanel.SetAlignRightWithPanel(editButton, true);
			return reminderPanel;
		}

		private TextBlock CreateReminderDateBlock(Reminder ReminderToAdd)
		{
			var ReminderDateBlock = new TextBlock();
			ReminderDateBlock.Text = ReminderToAdd.ActivateDateAndTime.ToString("g");
			ReminderDateBlock.Margin = new Thickness(10);
			ReminderDateBlock.TextAlignment = TextAlignment.Right;
			// if the reminder is expired, gray out the text and strike through it
			if (ReminderToAdd.IsExpired)
			{
				ReminderDateBlock.TextDecorations = TextDecorations.Strikethrough;
				Brush grayBrush = new SolidColorBrush(Colors.Gray);
				ReminderDateBlock.Foreground = grayBrush;
			}
			return ReminderDateBlock;
		}

		private TextBlock CreateReminderTitleBlock(Reminder ReminderToAdd)
		{
			var ReminderTitleBlock = new TextBlock();
			ReminderTitleBlock.Text = ReminderToAdd.Title;
			ReminderTitleBlock.Margin = new Thickness(10);
			ReminderTitleBlock.TextWrapping = TextWrapping.Wrap;
			ReminderTitleBlock.MaxLines = 2;
			ReminderTitleBlock.MaxWidth = 175;
			// if the reminder is expired, gray out the text and strike through it
			if (ReminderToAdd.IsExpired)
			{
				ReminderTitleBlock.TextDecorations = TextDecorations.Strikethrough;
				Brush grayBrush = new SolidColorBrush(Colors.Gray);
				ReminderTitleBlock.Foreground = grayBrush;
			}
			return ReminderTitleBlock;
		}

		private void DeleteLatestReminder()
		{
			StoredProcedures.DeleteLatestReminder();
			// if we have a dynamic area, remove the children from it
			if (this.DynamicArea != null)
			{
				ShowMessage("Reminder deleted.");
			}
		}

		private void DeleteReminder()
		{
			Reminder reminderToDelete = GetReminderForClosestMatchToPassedDate();
			if (reminderToDelete != null)
			{
				StoredProcedures.DeleteReminder(reminderToDelete.ReminderID);
				string message = new SSMLBuilder().Prosody("Successfully deleted reminder.", contour: "(1%,+2%) (50%,-1%) (80%,-1%)").Build();
				TextToSpeechEngine.SpeakInflectedText(this.MediaElement, message);
				ShowMessage($"Successfully deleted reminder {reminderToDelete.Title}");
			}
			else
			{
				this.ClearArea();
				// no reminder found, tell the user
				string message = new SSMLBuilder().Prosody("Sorry, but I wasn't able to find a reminder for that time.", contour: "(0%,+5%) (1%,-5%) (2%,+1%) (3%,-1%) (10%,+1%) (20%,-1%) (30%,+1%) (40%,-1%) (50%,+1%) (80%,-1%)").Build();
				TextToSpeechEngine.SpeakInflectedText(this.MediaElement, message);
				this.ShowMessage("Sorry, but I wasn't able to find a reminder for that time.");
			}
		}

		private void EditLatestReminder()
		{
			(Window.Current.Content as Frame).Navigate(typeof(RemindersFormPage), StoredProcedures.QueryLatestReminder());
		}

		private void EditReminder()
		{
			// it's pretty hard to figure out which reminder to edit and which fields need to be edited, so direct the users to the reminders page
			this.ClearArea();
			string text = "For now, editing reminders through voice is not supported. You can edit a reminder by going to the reminders page, finding the reminder you want to edit, and clicking the \"edit\" button.";
			string ssmlText = new SSMLBuilder().Prosody(text, pitch: "+2%", contour: "(10%,-2%) (40%, -3%) (80%, +3%)").Build();
			TextToSpeechEngine.SpeakInflectedText(this.MediaElement, ssmlText);
			this.ShowMessage(text);
		}

		private async Task<string> GetReminderTitleAsync()
		{
			string title = "";
			var titleIdentifierRegex = new Regex("(?<=(named |called |titled )).+");
			var match = titleIdentifierRegex.Match(this.CommandString);
			if (match.Success)
			{
				title = match.Value;
			}
			else
			{
				// ask the user to give a title if listening is enabled, else tell the user to retry with a title
				if (Utils.IsListeningSettingEnabled())
				{
					TextToSpeechEngine.SpeakText(this.MediaElement, "Sure, what's the title of the reminder?");
					await SpeechRecognitionManager.RequestListen(this.GetType(), (text) => title = text);
				}
				else
				{
					// title should be null, which will be used in the calling method to tell the user that title is required
					title = null;
				}


			}
			return title;
		}

		private ReminderActionTypes GetActionFromCommand()
		{
			var addRegex = new Regex("(?i)(create|set|new|add)(?-i)");
			var editRegex = new Regex("(?i)(edit|update|change|alter)(?-i)");
			var deleteRegex = new Regex("(?i)(delete|remove|clear|void|cancel)(?-i)");
			if (addRegex.IsMatch(this.CommandString))
			{
				return ReminderActionTypes.CREATE;
			}
			else if (editRegex.IsMatch(this.CommandString))
			{
				return ReminderActionTypes.EDIT;
			}
			else if (deleteRegex.IsMatch(this.CommandString))
			{
				return ReminderActionTypes.DELETE;
			}
			else
			{
				// default to create if we can't tell which the user wanted
				return ReminderActionTypes.CREATE;
			}
		}

		private DateTime GetReminderDateAndTime()
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
		/// Attempts to find the reminder whose activation date matches the passed date in this object's commandString
		/// </summary>
		/// <returns>the found reminder, which may be null if an reminder wasn't found</returns>
		private Reminder GetReminderForClosestMatchToPassedDate()
		{
			List<Reminder> reminders = StoredProcedures.QueryAllUnexpiredReminders();
			// get the date from the command text
			var now = DateTime.Now;
			// putting in the datetime to start at midnight of this day so that times don't get shifted around. I know it's marked as only used for tests, but it's what I had to do
			DateTime commandDateTime = DateTimeParser.ParseDateTimeFromText(this.CommandString, new DateTime(now.Year, now.Month, now.Day, 0, 0, 0));
			Reminder foundReminder = null;
			// if there's only one reminder, that's the one we're getting
			if (reminders.Count == 1)
			{
				foundReminder = reminders[0];
			}
			else
			{
				// find the first reminder whose hour and minute matches the command date time
				foundReminder = reminders.Find(reminder => reminder.ActivateDateAndTime.Hour == commandDateTime.Hour && reminder.ActivateDateAndTime.Minute == commandDateTime.Minute);
			}
			return foundReminder;
		}

		private async Task<Reminder> NewReminderAsync()
		{
			Reminder createdReminder = await this.CreateReminderAsync();
			// if the reminder is null, then don't do anything
			if (createdReminder != null)
			{
				// insert the reminder into the database
				StoredProcedures.CreateReminder(createdReminder.Title, createdReminder.ActivateDateAndTime, createdReminder.Description);
				string mainPart = $"Alright, reminder set for ";
				string datePart = createdReminder.ActivateDateAndTime.ToString("MMM d");
				string timePart = createdReminder.ActivateDateAndTime.ToString("h:mm tt");
				string rawSSML = new SSMLBuilder().Add(mainPart).SayAs(datePart, SSMLBuilder.SayAsTypes.DATE).Add(" at ").SayAs(timePart, SSMLBuilder.SayAsTypes.TIME).BuildWithoutWrapperElement();
				string prosodySSML = new SSMLBuilder().Prosody(rawSSML, pitch: "+5%", contour: "(10%,+5%) (50%,-5%) (80%,-5%)").Build();
				TextToSpeechEngine.SpeakInflectedText(this.MediaElement, prosodySSML);
			}

			return createdReminder;
		}
	}
}
