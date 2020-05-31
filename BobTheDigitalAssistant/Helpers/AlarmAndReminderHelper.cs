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
        private static readonly ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();

        /// <summary>
        /// schedules a toast notification for the passed alarm
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
            ScheduledToastNotification foundNotification = FindToastNotification(GetAlarmNotificationID(alarmToUnschedule));
            if (foundNotification != null)
            {
                toastNotifier.RemoveFromSchedule(foundNotification);
            }
        }

        private static ScheduledToastNotification FindToastNotification(string id)
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


        public static void UnscheduleReminder(Reminder reminderToUnschedule)
        {
            ScheduledToastNotification foundNotification = FindToastNotification(GetReminderNotificationID(reminderToUnschedule));
            if (foundNotification != null)
            {
                toastNotifier.RemoveFromSchedule(foundNotification);
            }
        }

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
