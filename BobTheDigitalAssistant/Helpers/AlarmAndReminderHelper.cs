using System;
using System.Collections.Generic;
using System.Linq;
using BobTheDigitalAssistant.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace BobTheDigitalAssistant.Helpers
{
	public class AlarmAndReminderHelper
	{
		private static readonly string TOAST_GROUP = "BobTheDigitalAssistant";
		// created statically to keep a single open resource instead of spending cpu time creating a new one every time we need it.
		private static readonly ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();

		/// <summary>
		/// Creates a scheduled toast notification with the passed <paramref name="alarmToSchedule"/>'s information to be displayed by the user at the alarm's activate date and time
		/// </summary>
		/// <param name="alarmToSchedule"></param>
		public static void ScheduleAlarm(Alarm alarmToSchedule)
		{
			var toast = new ScheduledToastNotification(CreateAlarmToast(alarmToSchedule).GetXml(), alarmToSchedule.ActivateDateAndTime);
			toast.Id = GetAlarmNotificationID(alarmToSchedule);
			toast.Group = TOAST_GROUP;
			toast.Tag = toast.Id;
			toastNotifier.AddToSchedule(toast);
		}

		public static void ScheduleReminder(Reminder reminderToSchedule)
		{
			var toast = new ScheduledToastNotification(CreateReminderToast(reminderToSchedule).GetXml(), reminderToSchedule.ActivateDateAndTime);
			toast.Id = GetReminderNotificationID(reminderToSchedule);
			toast.Group = TOAST_GROUP;
			toast.Tag = toast.Id;
			toastNotifier.AddToSchedule(toast);
		}

		public static void RescheduleAlarm(Alarm alarmToReschedule)
		{
			UnscheduleAlarm(alarmToReschedule);
			ScheduleAlarm(alarmToReschedule);
		}

		public static void RescheduleReminder(Reminder reminderToReschedule)
		{
			UnscheduleReminder(reminderToReschedule);
			ScheduleReminder(reminderToReschedule);
		}

		public static void UnscheduleAlarm(Alarm alarmToUnschedule)
		{
			ScheduledToastNotification foundNotification = FindToastNotificationForID(GetAlarmNotificationID(alarmToUnschedule));
			if (foundNotification != null)
			{
				toastNotifier.RemoveFromSchedule(foundNotification);
			}
		}

		/// <summary>
		/// returns the <see cref="ScheduledToastNotification"/> whose id matches the passed id and whose <see cref="ScheduledToastNotification.Group"/> matches <see cref="TOAST_GROUP"/>
		/// </summary>
		/// <param name="id"></param>
		/// <returns>the ScheduledToastNotification if one was found, else returns null</returns>
		private static ScheduledToastNotification FindToastNotificationForID(string id)
		{
			IReadOnlyList<ScheduledToastNotification> scheduledToasts = toastNotifier.GetScheduledToastNotifications();
			// find the scheduled toast to cancel
			try
			{
				return scheduledToasts.First(toast => toast.Id == id && toast.Group == TOAST_GROUP && toast.Tag == id);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Removes from the OS's notification schedule the scheduled notification for the passed <paramref name="reminderToUnschedule"/>
		/// </summary>
		/// <param name="reminderToUnschedule"></param>
		public static void UnscheduleReminder(Reminder reminderToUnschedule)
		{
			ScheduledToastNotification foundNotification = FindToastNotificationForID(GetReminderNotificationID(reminderToUnschedule));
			if (foundNotification != null)
			{
				toastNotifier.RemoveFromSchedule(foundNotification);
			}
		}

		/// <summary>
		/// Creates and returns a <see cref="ToastContent"/> object with the passed alarm's details
		/// </summary>
		/// <param name="alarm"></param>
		/// <returns></returns>
		private static ToastContent CreateAlarmToast(Alarm alarm)
		{
			return new ToastContent()
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
		}

		/// <summary>
		/// Creates and returns a <see cref="ToastContent"/> object with the passed reminder's details
		/// </summary>
		/// <param name="alarm"></param>
		/// <returns></returns>
		private static ToastContent CreateReminderToast(Reminder reminder)
		{
			return new ToastContent()
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
		}

		private static string GetAlarmNotificationID(Alarm alarm)
		{
			return $"Bob_alarm{alarm.AlarmID}";
		}

		private static string GetReminderNotificationID(Reminder reminder)
		{
			return $"Bob_reminder{reminder.ReminderID}";
		}
	}
}
