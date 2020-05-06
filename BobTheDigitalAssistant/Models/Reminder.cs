using System;
using Microsoft.Data.Sqlite;

namespace Capstone.Models
{
    public class Reminder
    {
        public int ReminderID { get; set; }
        public string Title { get; set; }
        public DateTime ActivateDateAndTime { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsExpired { get; set; }

        public Reminder() : this(-1, "", DateTime.Now, "", false, false)
        {
        }

        /// <summary>
        /// Creates a new reminder, with the title, date, and deleted status
        /// </summary>
        /// <param name="ReminderID">The ID in the database for the reminder</param>
        /// <param name="Title">The title of the reminder that shows up on the UI</param>
        /// <param name="ActivateDateAndTime">The date and time that the reminder is set to go off</param>
        /// <param name="Description">The description of the reminder to show on the UI</param>
        /// <param name="IsDeleted">whether or not the reminder is set to be deleted. If marked as true, it will not go off and will not appear on the UI</param>
        public Reminder(int ReminderID, string Title, DateTime ActivateDateAndTime, string Description, bool IsDeleted, bool IsExpired)
        {
            this.ReminderID = ReminderID;
            this.Title = Title;
            this.ActivateDateAndTime = ActivateDateAndTime;
            this.Description = Description;
            this.IsDeleted = IsDeleted;
            this.IsExpired = IsExpired;
        }

        public static Reminder FromDataRow(SqliteDataReader reader)
        {
            Reminder createdReminder = new Reminder(int.Parse(reader["reminderID"].ToString()), reader["reminderTitle"].ToString(), DateTime.Parse($"{reader["reminderDate"]} {reader["reminderTime"]}"), reader["reminderDescription"].ToString(), Convert.ToBoolean((long)reader["isDeleted"]), Convert.ToBoolean((long)reader["isExpired"]));
            return createdReminder;
        }
    }
}