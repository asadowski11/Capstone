using System;
using System.Collections.Generic;
using System.Threading;
using BobTheDigitalAssistant.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace BobTheDigitalAssistant.Common
{
	public static class AlarmAndReminderTracker
	{
		public static Boolean hasStarted { get; private set; } = false;

		public static void Start()
		{
			var trackerThread = new Thread(new ThreadStart(TrackAlarmsAndReminders));
			// make sure it's a background thread so that it doesn't interrupt the main process
			trackerThread.IsBackground = true;
			trackerThread.Start();
			hasStarted = true;
		}

		/// <summary>
		/// Periodically checks for expired alarms/reminders in the database that haven't been marked as expired, and updates their expired status in the database to expired. This should be called in a thread separate from the UI thread.
		/// </summary>
		private static void TrackAlarmsAndReminders()
		{
			// this lets us repeatedly check
			while (true)
			{
				// get all the alarms and reminders from the database
				try
				{
					List<Alarm> alarms = StoredProcedures.QueryAllUnexpiredAlarms();
					List<Reminder> reminders = StoredProcedures.QueryAllUnexpiredReminders();
					var time = DateTime.Now;
					// for each expired alarm, mark it as so in the database
					alarms.ForEach(alarm =>
					{
						if (alarm.ActivateDateAndTime <= time)
						{
							// expire the alarm in the database
							StoredProcedures.ExpireAlarm(alarm.AlarmID);
						}
					});
					// for each expired reminder, mark it as so in the database
					reminders.ForEach(reminder =>
					{
						if (reminder.ActivateDateAndTime <= time)
						{
							// expire the reminder from the database
							StoredProcedures.ExpireReminder(reminder.ReminderID);
						}
					});
				}
				catch (SqliteException)
				{
					Console.WriteLine("Something went wrong with the database!");
				}
				catch (Exception)
				{
					Console.WriteLine("Something bad happened and I don't know what!");
				}
				// sleep for 2 seconds
				Thread.Sleep(2_000);
			}
		}

		private static void ShowAlarmToast(Alarm alarm)
		{
			var toastContent = new ToastContent()
			{
				Visual = new ToastVisual()
				{
					BindingGeneric = new ToastBindingGeneric()
					{
						Children =
						{
							new AdaptiveText()
							{
								Text = alarm.Title
							}
						}
					}
				},
				Actions = new ToastActionsCustom()
				{
					Inputs =
					{
						new ToastSelectionBox("snoozeTimes")
						{
							Title = "Snooze for",
							Items =
							{
								new ToastSelectionBoxItem("5", "5 Minutes"),
								new ToastSelectionBoxItem("10", "10 Minutes"),
								new ToastSelectionBoxItem("15", "15 Minutes"),
								new ToastSelectionBoxItem("30", "30 Minutes")
							},
							DefaultSelectionBoxItemId = "5"
						}
					},
					Buttons =
					{
						new ToastButtonSnooze() {
							SelectionBoxId = "snoozeTimes"
						},
						new ToastButtonDismiss("Dismiss")
					}
				},

				Audio = new ToastAudio()
				{
					Src = new Uri("ms-winsoundevent:Notification.Looping.Alarm"),
					Loop = true
				},
				Scenario = ToastScenario.Alarm
			};

			// Create the toast notification
			var toastNotif = new ToastNotification(toastContent.GetXml());

			// And send the notification
			ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
		}

		private static void ShowReminderToast(Reminder reminder)
		{
			var toastContent = new ToastContent()
			{
				Visual = new ToastVisual()
				{
					BindingGeneric = new ToastBindingGeneric()
					{
						Children =
						{
							new AdaptiveText()
							{
								Text = reminder.Title
							},
							new AdaptiveText()
							{
								Text = reminder.Description
							}
						}
					}
				},
				Actions = new ToastActionsCustom()
				{
					Inputs =
					{
						new ToastSelectionBox("snoozeTimes")
						{
							Title = "Snooze for",
							Items =
							{
								new ToastSelectionBoxItem("5", "5 Minutes"),
								new ToastSelectionBoxItem("10", "10 Minutes"),
								new ToastSelectionBoxItem("15", "15 Minutes"),
								new ToastSelectionBoxItem("30", "30 Minutes")
							},
							DefaultSelectionBoxItemId = "5"
						}
					},
					Buttons =
					{
						new ToastButtonSnooze() {
							SelectionBoxId = "snoozeTimes"
						},
						new ToastButtonDismiss("Dismiss")
					}
				},

				//Audio = new ToastAudio()
				//{
				//    Src = new Uri("ms-winsoundevent:Notification.Looping.Alarm"),
				//    Loop = true
				//},
				Scenario = ToastScenario.Reminder
			};

			// Create the toast notification
			var toastNotif = new ToastNotification(toastContent.GetXml());

			// And send the notification
			ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
		}
	}
}
