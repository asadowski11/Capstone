using System;
using Microsoft.Data.Sqlite;

namespace BobTheDigitalAssistant.Models
{
    public class VoiceMemo
    {
        public int VoiceMemoID { get; set; }

        /// <summary>
        /// Contains the name of the file, which isn't necessarily what gets displayed to the user
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// This is what gets displayed to the user through the UI
        /// </summary>
        public string DisplayName { get; set; }

        public int RecordingDuration { get; set; }
        public string FullFilePath { get; set; }
        public DateTime DateRecorded { get; set; }

        public VoiceMemo() : this(-1, "", "", 0, "", DateTime.MaxValue)
        {
        }

        public VoiceMemo(int VoiceMemoID, string FileName, string DisplayName, int RecordingDuration, string FullFilePath, DateTime DateRecorded)
        {
            this.VoiceMemoID = VoiceMemoID;
            this.FileName = FileName;
            this.DisplayName = DisplayName;
            this.RecordingDuration = RecordingDuration;
            this.FullFilePath = FullFilePath;
            this.DateRecorded = DateRecorded;
        }

        public static VoiceMemo FromDataRow(SqliteDataReader reader)
        {
            VoiceMemo createdVoiceMemo = new VoiceMemo(int.Parse(reader["voiceMemoID"].ToString()), reader["fileName"].ToString(), reader["displayName"].ToString(),
              int.Parse(reader["recordingDuration"].ToString()), reader["filePath"].ToString(), DateTime.Parse(reader["recordDate"].ToString()));
            return createdVoiceMemo;
        }
    }
}