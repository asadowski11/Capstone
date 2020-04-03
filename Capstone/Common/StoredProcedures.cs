using Capstone.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace Capstone.Common
{
    class StoredProcedures
    {
        public static void CreateDatabase()
        {
            var appData = Windows.Storage.ApplicationData.Current.LocalFolder.Path;

            if (!File.Exists($"{appData}\\Database\\BobDB.sqlite"))
            {
                try
                {

                    SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={appData}\\Database\\BobDB.sqlite;Version=3;");
                    m_dbConnection.Open();

                    string script = File.ReadAllText($"{Windows.ApplicationModel.Package.Current.InstalledLocation.Path}\\Database\\InitialScript.sql");

                    string sql = script;

                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();

                    m_dbConnection.Close();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                    throw;
                }

            }

        }

        public static SqliteConnection OpenDatabase()
        {
            string targetDbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Database\\BobDB.db");
            SqliteConnection conn = new SqliteConnection(@"Data Source = " + targetDbPath);
            return conn;
        }

        public static void CreateReminder(string Title, DateTime ReminderDateAndTime, string Description)
        {

            // gives the hour:minute [AP]m format
            string strTime = ReminderDateAndTime.ToString("t");
            // gives the month/day/year format
            string strDate = ReminderDateAndTime.ToString("d");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"INSERT INTO TReminders(reminderTitle, reminderTime, isDeleted, reminderDescription) Values('{Title}','{strTime}', 0, '{Description}');";
            command.ExecuteNonQuery();
            command.CommandText = $"INSERT INTO TReminderDates(reminderID, reminderDate) Select MAX(reminderID), '{strDate}' FROM TReminders;";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static void UpdateReminder(int ID, string Title, DateTime ReminderDateAndTime, string Description)
        {
            int intID = ID;
            // gives the hour:minute [AP]m format
            string strTime = ReminderDateAndTime.ToString("t");
            // gives the month/day/year format
            string strDate = ReminderDateAndTime.ToString("d");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Update TReminders Set reminderTime = '{strTime}', reminderTitle = '{Title}', reminderDescription = '{Description}' Where reminderID = {intID};";
            command.ExecuteNonQuery();
            command.CommandText = $"Update TReminderDates Set reminderDate = '{strDate}' Where reminderID = {intID};";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static void DeleteReminder(int ID)
        {
            int intID = ID;
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Update TReminders Set isDeleted = 1 Where reminderID = {intID};";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static Reminder QueryReminder(int ID = -1)
        {
            Reminder reminder = new Reminder();
            string intID;
            if (ID == -1)
            {
                intID = "null";
            }
            else
            {
                intID = ID.ToString();
            }
            
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TReminders.reminderID, TReminders.reminderTitle, TReminders.reminderDescription, TReminders.reminderTime, TReminderDates.reminderDate, TReminders.isDeleted From TReminders, TReminderDates Where TReminders.reminderID = TReminderDates.reminderID and TReminders.reminderID = COALESCE({intID}, TReminders.reminderID); ";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    reminder = Reminder.FromDataRow(reader);
                }
            }
            conn.Close();
            return reminder;
        }

       /// <summary>
       /// queries a list of all non-deleted reminders from the database and returns them.
       /// </summary>
       /// <returns>the list of queried reminders</returns>
       /// <exception cref="SqliteException">If there' an error executing the sql statement</exception>
        public static List<Reminder> QueryAllReminders()
        {
            List<Reminder> reminders = new List<Reminder>();
            string query = "SELECT TReminders.reminderID, TReminders.reminderTitle, TReminders.reminderTime, TReminders.reminderDescription, TReminders.isDeleted, TReminderDates.reminderDate FROM TReminders, TReminderDates WHERE TReminderDates.reminderID = TReminders.reminderID AND TReminders.isDeleted <> 1";
            using (SqliteConnection connection = OpenDatabase())
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reminders.Add(Reminder.FromDataRow(reader));
                    }
                }
            }
            return reminders;
        }

        public static void CreateAlarm(string Title, DateTime AlarmDateTime)
        {

            string strTime = AlarmDateTime.ToString("t");
            string strDate = AlarmDateTime.ToString("d");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"INSERT INTO TAlarms(AlarmTitle, AlarmTime, isDeleted) Values('{Title}','{strTime}', 0);";
            command.ExecuteNonQuery();
            command.CommandText = $"INSERT INTO TAlarmDates(AlarmID, AlarmDate) Select MAX(AlarmID), '{strDate}' FROM TAlarms;";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static void UpdateAlarm(int ID, string Title, DateTime AlarmDateTime)
        {
            int intID = ID;
            string strTime = AlarmDateTime.ToString("t");
            string strDate = AlarmDateTime.ToString("d");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Update TAlarms Set AlarmTime = '{strTime}', AlarmTitle = '{Title}' Where AlarmID = {intID};";
            command.ExecuteNonQuery();
            command.CommandText = $"Update TAlarmDates Set AlarmDate = '{strDate}' Where AlarmID = {intID};";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static void DeleteAlarm(int ID)
        {
            int intID = ID;
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Update TAlarms Set isDeleted = 1 Where AlarmID = {intID};";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static Alarm QueryAlarm(int ID = -1)
        {
            Alarm alarm = new Alarm();
            string intID;
            if (ID == -1)
            {
                intID = "null";
            }
            else
            {
                intID = ID.ToString();
            }

            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TAlarms.AlarmID, TAlarms.AlarmTitle, TAlarms.AlarmTime, TAlarmDates.AlarmDate, TAlarms.isDeleted From TAlarms, TAlarmDates Where TAlarms.AlarmID = TAlarmDates.AlarmID and TAlarms.AlarmID = COALESCE({intID}, TAlarms.AlarmID); ";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int intAlarmID = int.Parse(reader["AlarmID"].ToString());
                    alarm.AlarmID = intAlarmID;
                    alarm.Title = reader["AlarmTitle"].ToString();
                    var date = DateTime.Parse($"{reader["AlarmDate"]} {reader["AlarmTime"]}");
                    alarm.ActivateDateAndTime = date;
                    alarm.IsDeleted = Convert.ToBoolean((long)reader["isDeleted"]);
                }
            }
            conn.Close();
            return alarm;
        }

        /// <summary>
        /// queries a list of all non-deleted alarms from the database and returns them.
        /// </summary>
        /// <returns>the list of queried alarms</returns>
        /// <exception cref="SqliteException">If there' an error executing the sql statement</exception>
        public static List<Alarm> QueryAllAlarms()
        {
            List<Alarm> alarms = new List<Alarm>();
            string query = "SELECT TAlarms.alarmID, TAlarms.alarmTitle, TAlarms.isDeleted, TAlarms.alarmTime, TAlarmDates.alarmDate FROM TAlarms, TAlarmDates WHERE TAlarmDates.alarmID = TAlarms.alarmID AND TAlarms.isDeleted <> 1";
            using (SqliteConnection connection = OpenDatabase())
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        alarms.Add(Alarm.FromDataRow(reader));
                    }
                }
            }
            return alarms;
        }

        public static void CreateVoiceNote(string FileName, string DsiplayName, int RecordingDuration, string FilePath, DateTime RecordDate, DateTime RecordTime)
        {

            string strRecordTime = RecordTime.ToString("HH:mm");
            string strRecordDate = RecordDate.ToString("yyyy-MM-dd HH:mm");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"INSERT INTO TVoiceMemos(fileName,displayName,recordingDuration,filePath,recordDate,recordTime) Values('{FileName}', '{DsiplayName}', '{RecordingDuration}', '{FilePath}', '{strRecordDate}', '{strRecordTime}'); ";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static void DeleteVoiceNote(int ID)
        {
            int intID = ID;
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Delete From TVoiceMemos Where TVoiceMemos.voiceMemoID = {intID};";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static void UpdateVoiceNote(int ID, string Title)
        {
            int intID = ID;
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Update TVoiceMemos Set displayName = '{Title}' Where voiceMemoID = {intID};";
            command.ExecuteNonQuery();
            conn.Close();
        }

        public static VoiceMemo QueryVoiceMemo(int ID = -1)
        {
            VoiceMemo voiceMemo = new VoiceMemo();
            string intID;
            if (ID == -1)
            {
                intID = "null";
            }
            else
            {
                intID = ID.ToString();
            }

            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select voiceMemoID, fileName, displayName, recordingDuration, filePath, recordDate, recordTime From TVoiceMemos Where voiceMemoID = COALESCE({intID}, voiceMemoID);";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int intVoiceMemoID = int.Parse(reader["voiceMemoID"].ToString());
                    voiceMemo.VoiceMemoID = intVoiceMemoID;
                    voiceMemo.FileName = reader["fileName"].ToString();
                    voiceMemo.DisplayName = reader["displayName"].ToString();
                    voiceMemo.RecordingDuration = (int)reader["recordingDuration"];
                    voiceMemo.FullFilePath = reader["filePath"].ToString();
                    voiceMemo.DateRecorded = (DateTime)reader["recordDate"];
                }
            }
            conn.Close();
            return voiceMemo;
        }

        public static void UpdateSettings(int ID, bool IsSelected)
        {
            int intID = ID;
            bool boolIsSelected = IsSelected;
            int intIsSelected = boolIsSelected ? 1 : 0;
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Update TSettingOptions Set isSelected = {intIsSelected} Where settingOptionID = {intID};";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static Setting QuerySetting(int ID = -1)
        {
            Setting setting = new Setting();
            List<SettingOption> SettingOptionList = new List<SettingOption>();
            string intID;
            if (ID == -1)
            {
                intID = "null";
            }
            else
            {
                intID = ID.ToString();
            }

            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TSettings.settingID, TSettings.settingDisplayName,TSettingOptions.settingOptionID, TSettingOptions.optionDisplayName,TSettingOptions.isSelected From TSettings, TSettingOptions Where TSettings.settingID = COALESCE({intID}, TSettings.settingID) and TSettings.settingID = TSettingOptions.settingID;";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    SettingOption settingOption = new SettingOption();
                    int intSettingOptionID = int.Parse(reader["settingOptionID"].ToString());
                    settingOption.OptionID = intSettingOptionID;
                    settingOption.DisplayName = reader["optionDisplayName"].ToString();
                    settingOption.IsSelected = Convert.ToBoolean((long)reader["isSelected"]);
                    SettingOptionList.Add(settingOption);
                }
            }
            command.CommandText = $"Select TSettings.settingID, TSettings.settingDisplayName, TSettingOptions.optionDisplayName,TSettingOptions.isSelected From TSettings, TSettingOptions Where TSettings.settingID = COALESCE({intID}, TSettings.settingID) and TSettings.settingID = TSettingOptions.settingID;";
            using(SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int intSettingID = int.Parse(reader["settingID"].ToString());
                    setting.SettingID = intSettingID;
                    setting.DisplayName = reader["settingDisplayName"].ToString();
                    setting.Options = SettingOptionList;
                }
            }
            conn.Close();
            return setting;
        }
        public static WeatherProvider QueryWeatherProvider(int ID = -1)
        {
            WeatherProvider weatherProvider = new WeatherProvider();
            string intID;
            if (ID == -1)
            {
                intID = "null";
            }
            else
            {
                intID = ID.ToString();
            }

            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TWeatherProviders.weatherProviderID, TWeatherProviders.weatherProviderName,TWeatherProviderURLS.weatherProviderURL,TWeatherProviderURLParts.weatherProviderURLPartURLString,TWeatherProviderAccessTypes.weatherProviderAccessType From TWeatherProviders, TWeatherProviderURLS, TWeatherProviderURLParts, TWeatherProviderAccessTypes Where TWeatherProviders.weatherProviderID = COALESCE({intID}, TWeatherProviders.weatherProviderID) and TWeatherProviders.weatherProviderID = TWeatherProviderURLS.weatherProviderID and TWeatherProviders.weatherProviderID = TWeatherProviderURLParts.weatherProviderID and TWeatherProviders.weatherProviderID = TWeatherProviderAccessTypes.weatherProviderID; ";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int intWeatherProviderID = int.Parse(reader["weatherProviderID"].ToString());
                    weatherProvider.WeatherProviderID = intWeatherProviderID;
                    weatherProvider.Name = reader["weatherProviderName"].ToString();
                    weatherProvider.BaseURL = reader["weatherProviderURL"].ToString();
                    //weatherProvider.AccessType = reader["weatherProviderAccessType"];
                    //weatherProvider.APIKey = reader[""];
                }
            }
            conn.Close();
            return weatherProvider;
        }
        public static MapProvider QueryMapProvider(int ID = -1)
        {
            MapProvider mapProvider = new MapProvider();
            string intID;
            if (ID == -1)
            {
                intID = "null";
            }
            else
            {
                intID = ID.ToString();
            }

            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TmapProviders.mapProviderID, TmapProviders.mapProviderName,TMapProvidersURLS.mapProviderURL,TMapProvidersURLParts.mapProviderURLPartType,TMapProvidersURLParts.mapProviderURLPartURL,TmapProviderAccessTypes.mapProviderAccessType From TmapProviders, TMapProvidersURLS, TmapProvidersURLParts, TmapProviderAccessTypes Where TmapProviders.mapProviderID = COALESCE({intID}, TmapProviders.mapProviderID) and TmapProviders.mapProviderID = TmapProvidersURLS.mapProviderID and TmapProviders.mapProviderID = TMapProvidersURLParts.mapProviderID and TmapProviders.mapProviderID = TmapProviderAccessTypes.mapProviderID;";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string AccessType = reader["mapProviderAccessType"].ToString();
                    int intMapProviderID = int.Parse(reader["mapProviderID"].ToString());
                    mapProvider.MapProviderID = intMapProviderID;
                    mapProvider.Name = reader["mapProviderName"].ToString();
                    mapProvider.BaseURL = reader["mapProviderURL"].ToString();
                    //mapProvider.AccessType = AccessType;
                    //mapProvider.APIKey = reader[""];
                }
            }
            conn.Close();
            return mapProvider;
        }
        public static SearchableWebsite QuerySearchableWebsite(int ID = -1)
        {
            SearchableWebsite searchableWebsite = new SearchableWebsite();
            string intID;
            if (ID == -1)
            {
                intID = "null";
            }
            else
            {
                intID = ID.ToString();
            }

            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TSearchableWebsites.searchableWebsitesID, TSearchableWebsites.searchableWebsiteName, TSearchableWebsites.searchableWebsiteBaseURL, TSearchableWebsites.searchableWebsiteQueryString From TSearchableWebsites Where TSearchableWebsites.searchableWebsitesID = COALESCE({intID}, TSearchableWebsites.searchableWebsitesID);";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
            while (reader.Read())
            {
                int intSearchableWebsitesID = int.Parse(reader["searchableWebsitesID"].ToString());
                searchableWebsite.SearchableWebsiteID = intSearchableWebsitesID;
                searchableWebsite.Name = reader["searchableWebsiteName"].ToString();
                searchableWebsite.BaseURL = reader["searchableWebsiteBaseURL"].ToString();
                searchableWebsite.QueryString = reader["searchableWebsiteQueryString"].ToString();
                }
            }
            conn.Close();
            return searchableWebsite;
        }
        public static SearchEngine QuerySearchEngine(int ID = -1)
        {
            SearchEngine searchEngine = new SearchEngine();
            string intID;
            if (ID == -1)
            {
                intID = "null";
            }
            else
            {
                intID = ID.ToString();
            }

            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TSearchEngines.searchEngineID, TSearchEngines.searchEngineName, TSearchEngines.searchEngineBaseURL, TSearchEngines.searchEngineQueryString From TSearchEngines Where TSearchEngines.searchEngineID = COALESCE({intID}, TSearchEngines.searchEngineID);";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int intSearchEngineID = int.Parse(reader["searchEngineID"].ToString());
                    searchEngine.SearchEngineID = intSearchEngineID;
                    searchEngine.Name = reader["searchEngineName"].ToString();
                    searchEngine.BaseURL = reader["searchEngineBaseURL"].ToString();
                    searchEngine.QueryString = reader["searchEngineQueryString"].ToString();
                }
            }
            conn.Close();
            return searchEngine;
        }

    }
}
