using System;
using Microsoft.Data.Sqlite;

namespace BobTheDigitalAssistant.Models
{
	public class Alarm
	{
		public int AlarmID { get; set; }
		public string Title { get; set; }
		public DateTime ActivateDateAndTime { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsExpired { get; set; }

		public Alarm() : this(-1, "", DateTime.Now, false, false)
		{
		}

		/// <summary>
		/// Creates a new alarm, with the title, date, and deleted status
		/// </summary>
		/// <param name="AlarmID">The ID of the alarm in the database</param>
		/// <param name="Title">The title of the alarm that shows up on the UI</param>
		/// <param name="ActivateDateAndTime">The date and time that the alarm is set to go off</param>
		/// <param name="IsDeleted">whether or not the alarm is set to be deleted. If marked as true, it will not go off and will not appear on the UI</param>
		public Alarm(int AlarmID, string Title, DateTime ActivateDateAndTime, bool IsDeleted, bool IsExpired)
		{
			this.AlarmID = AlarmID;
			this.Title = Title;
			this.ActivateDateAndTime = ActivateDateAndTime;
			this.IsDeleted = IsDeleted;
			this.IsExpired = IsExpired;
		}

		public static Alarm FromDataRow(SqliteDataReader reader)
		{
			Alarm createdReminder = new Alarm(int.Parse(reader["alarmID"].ToString()), reader["alarmTitle"].ToString(), DateTime.Parse($"{reader["alarmDate"]} {reader["alarmTime"]}"), Convert.ToBoolean((long)reader["isDeleted"]), Convert.ToBoolean((long)reader["isExpired"]));
			return createdReminder;
		}
	}
}
