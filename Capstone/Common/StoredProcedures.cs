using Capstone.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace Capstone.Common
{
    class StoredProcedures
    {
        public static async Task CreateDatabase()
        {
            StorageFolder localStateFolder = ApplicationData.Current.LocalFolder;
            StorageFolder createdFolder;
            if (!Directory.Exists(Path.Combine(localStateFolder.Path, "Database")))
            {
                createdFolder = await localStateFolder.CreateFolderAsync("Database");
            }
            else
            {
                createdFolder = await localStateFolder.GetFolderAsync("Database");
            }
            string targetDbPath = Path.Combine(createdFolder.Path, "BobDB.db");
            if (!File.Exists(targetDbPath))
            {
                SqliteConnection connection = new SqliteConnection($"Data Source={targetDbPath};");
                connection.Open();
                string initialScript = File.ReadAllText($"{Windows.ApplicationModel.Package.Current.InstalledLocation.Path}\\Database\\InitialScript.sql");
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = initialScript;
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
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
            Title = EscapeSingleTicks(Title);
            Description = EscapeSingleTicks(Description);
            // gives the hour:minute [AP]m format
            string strTime = ReminderDateAndTime.ToString("HH:mm");
            // gives the month/day/year format
            string strDate = ReminderDateAndTime.ToString("yyyy-MM-dd");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"INSERT INTO TReminders(reminderTitle, reminderTime, isDeleted, reminderDescription, isExpired) Values('{Title}','{strTime}', 0, '{Description}', 0);";
            command.ExecuteNonQuery();
            command.CommandText = $"INSERT INTO TReminderDates(reminderID, reminderDate) Select MAX(reminderID), '{strDate}' FROM TReminders;";
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static void UpdateReminder(int ID, string Title, DateTime ReminderDateAndTime, string Description, bool isExpired)
        {
            // escape the single ticks
            Title = EscapeSingleTicks(Title);
            Description = EscapeSingleTicks(Description);
            int intID = ID;
            // gives the hour:minute [AP]m format
            string strTime = ReminderDateAndTime.ToString("HH:mm");
            // gives the month/day/year format
            string strDate = ReminderDateAndTime.ToString("yyyy-MM-dd");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Update TReminders Set reminderTime = '{strTime}', reminderTitle = '{Title}', reminderDescription = '{Description}', isExpired = {(isExpired ? 1 : 0)} Where reminderID = {intID};";
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
            command.CommandText = $"Select TReminders.reminderID, TReminders.reminderTitle, TReminders.reminderDescription, TReminders.reminderTime, TReminderDates.reminderDate, TReminders.isExpired, TReminders.isDeleted From TReminders, TReminderDates Where TReminders.reminderID = TReminderDates.reminderID and TReminders.reminderID = COALESCE({intID}, TReminders.reminderID); ";
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
            string query = @"SELECT TReminders.reminderID, TReminders.reminderTitle, TReminders.reminderTime, TReminders.reminderDescription, TReminders.isDeleted, TReminders.isExpired, TReminderDates.reminderDate 
                            FROM TReminders, TReminderDates
                            WHERE TReminderDates.reminderID = TReminders.reminderID AND TReminders.isDeleted <> 1
                            ORDER BY TReminderDates.reminderDate,TReminders.reminderTime;";
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

        public static List<Reminder> QueryAllUnexpiredReminders()
        {
            List<Reminder> reminders = new List<Reminder>();
            string query = @"SELECT TReminders.reminderID, TReminders.reminderTitle, TReminders.reminderTime, TReminders.reminderDescription, TReminders.isDeleted, TReminders.IsExpired, TReminderDates.reminderDate 
                            FROM TReminders, TReminderDates
                            WHERE TReminderDates.reminderID = TReminders.reminderID AND TReminders.isDeleted <> 1 AND TReminders.isExpired <> 1
                            ORDER BY TReminderDates.reminderDate,TReminders.reminderTime;";
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

        public static void ExpireReminder(int ID)
        {
            using (SqliteConnection connection = OpenDatabase())
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"update TReminders set isExpired = 1 where reminderID = {ID}";
                    command.ExecuteNonQuery();
                }
            }
        }

        public static Reminder QueryLatestReminder()
        {
            Reminder queriedReminder = null;
            using (SqliteConnection conn = OpenDatabase())
            {
                conn.Open();
                using (SqliteCommand latestIDCommand = conn.CreateCommand())
                {
                    latestIDCommand.CommandText = "SELECT MAX(reminderID) as reminderID from TReminders";
                    SqliteDataReader reader = latestIDCommand.ExecuteReader();
                    reader.Read();
                    int id = int.Parse(reader["reminderID"].ToString());
                    queriedReminder = QueryReminder(id);
                    reader.Close();
                }
            }
            return queriedReminder;
        }

        public static void DeleteLatestReminder()
        {
            using (SqliteConnection conn = OpenDatabase())
            {
                conn.Open();
                using (SqliteCommand maxIDCommand = conn.CreateCommand())
                using (SqliteCommand deleteLatestCommand = conn.CreateCommand())
                {
                    maxIDCommand.CommandText = "SELECT MAX(reminderID) as reminderID FROM TReminders";
                    SqliteDataReader reader = maxIDCommand.ExecuteReader();
                    reader.Read();
                    int reminderID = int.Parse(reader["reminderID"].ToString());
                    deleteLatestCommand.CommandText = $"UPDATE TReminders SET isDeleted = 1 WHERE reminderID = {reminderID}";
                    deleteLatestCommand.ExecuteNonQuery();
                    reader.Close();
                }
            }
        }

        public static void CreateAlarm(string Title, DateTime AlarmDateTime)
        {
            // escape the single ticks
            Title = EscapeSingleTicks(Title);
            string strTime = AlarmDateTime.ToString("HH:mm");
            string strDate = AlarmDateTime.ToString("yyyy-MM-dd");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"INSERT INTO TAlarms(AlarmTitle, AlarmTime, isDeleted, isExpired) Values('{Title}','{strTime}', 0, 0);";
            command.ExecuteNonQuery();
            command.CommandText = $"INSERT INTO TAlarmDates(AlarmID, AlarmDate) Select MAX(AlarmID), '{strDate}' FROM TAlarms;";
            command.ExecuteNonQuery();
            conn.Close();
        }

        public static void UpdateAlarm(int ID, string Title, DateTime AlarmDateTime, bool isExpired)
        {
            // escape the single ticks
            Title = EscapeSingleTicks(Title);
            int intID = ID;
            string strTime = AlarmDateTime.ToString("HH:mm");
            string strDate = AlarmDateTime.ToString("yyyy-MM-dd");
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Update TAlarms Set AlarmTime = '{strTime}', AlarmTitle = '{Title}', isExpired = {(isExpired ? 1 : 0)} Where AlarmID = {intID};";
            command.ExecuteNonQuery();
            command.CommandText = $"Update TAlarmDates Set AlarmDate = '{strDate}' Where AlarmID = {intID};";
            command.ExecuteNonQuery();
            conn.Close();
        }

        public static void ExpireAlarm(int ID)
        {
            using (SqliteConnection connection = OpenDatabase())
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"update TAlarms set isExpired = 1 where alarmID = {ID}";
                    command.ExecuteNonQuery();
                }
            }
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

        public static Alarm QueryLatestAlarm()
        {
            Alarm queriedAlarm = null;
            using (SqliteConnection conn = OpenDatabase())
            {
                conn.Open();
                using (SqliteCommand latestIDCommand = conn.CreateCommand())
                {
                    latestIDCommand.CommandText = "SELECT MAX(alarmID) as alarmID from TAlarms";
                    SqliteDataReader reader = latestIDCommand.ExecuteReader();
                    reader.Read();
                    int id = int.Parse(reader["alarmID"].ToString());
                    queriedAlarm = QueryAlarm(id);
                    reader.Close();
                }
            }
            return queriedAlarm;
        }

        public static void DeleteLatestAlarm()
        {
            using (SqliteConnection conn = OpenDatabase())
            {
                conn.Open();
                using (SqliteCommand maxIDCommand = conn.CreateCommand())
                using (SqliteCommand deleteLatestCommand = conn.CreateCommand())
                {
                    maxIDCommand.CommandText = "SELECT MAX(alarmID) as alarmID FROM TAlarms";
                    SqliteDataReader reader = maxIDCommand.ExecuteReader();
                    reader.Read();
                    int alarmID = int.Parse(reader["alarmID"].ToString());
                    deleteLatestCommand.CommandText = $"UPDATE TAlarms SET isDeleted = 1 WHERE alarmID = {alarmID}";
                    deleteLatestCommand.ExecuteNonQuery();
                    reader.Close();
                }
            }
        }

        public static Alarm QueryAlarm(int ID = -1)
        {
            Alarm alarm = new Alarm();
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TAlarms.AlarmID, TAlarms.AlarmTitle, TAlarms.AlarmTime, TAlarmDates.AlarmDate, TAlarms.isDeleted, TAlarms.isExpired From TAlarms, TAlarmDates Where TAlarms.AlarmID = TAlarmDates.AlarmID and TAlarms.AlarmID = COALESCE({ID}, TAlarms.AlarmID); ";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                alarm = Alarm.FromDataRow(reader);
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
            string query = @"SELECT TAlarms.alarmID, TAlarms.alarmTitle, TAlarms.isDeleted, TAlarms.alarmTime, TAlarms.isExpired, TAlarmDates.alarmDate
                            FROM TAlarms, TAlarmDates 
                            WHERE TAlarmDates.alarmID = TAlarms.alarmID AND TAlarms.isDeleted <> 1
                            ORDER BY TAlarmDates.alarmDate,TAlarms.alarmTime";
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

        public static List<Alarm> QueryAllUnexpiredAlarms()
        {
            List<Alarm> alarms = new List<Alarm>();
            string query = @"SELECT TAlarms.alarmID, TAlarms.alarmTitle, TAlarms.isDeleted, TAlarms.alarmTime, TAlarms.isExpired, TAlarmDates.alarmDate 
                            FROM TAlarms, TAlarmDates 
                            WHERE TAlarmDates.alarmID = TAlarms.alarmID AND TAlarms.isDeleted <> 1 AND TAlarms.isExpired <> 1
                            ORDER BY TAlarmDates.alarmDate,TAlarms.alarmTime";
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


        public static List<VoiceMemo> QueryAllVoiceMemos()
        {
            List<VoiceMemo> voiceMemos = new List<VoiceMemo>();
            string query = @"Select voiceMemoID, fileName, displayName, recordingDuration, filePath, recordDate, recordTime From TVoiceMemos
                            ORDER BY recordDate, recordTime;";
            using (SqliteConnection connection = OpenDatabase())
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        voiceMemos.Add(VoiceMemo.FromDataRow(reader));
                    }
                }
            }
            return voiceMemos;
        }

        public static VoiceMemo QueryLatestVoiceMemo()
        {
            VoiceMemo memo = new VoiceMemo();
            using (SqliteConnection connection = OpenDatabase())
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM TVoiceMemos WHERE voiceMemoID = (SELECT MAX(voiceMemoID) From TVoiceMemos)";
                    SqliteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    memo = VoiceMemo.FromDataRow(reader);
                    reader.Close();
                }
            }
            return memo;
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
            using (SqliteDataReader reader = command.ExecuteReader())
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

        public static Setting QuerySettingByName(string settingName)
        {
            Setting setting = null;
            using (var connection = OpenDatabase())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT TSettings.settingID, TSettings.settingDisplayName AS 'Setting Name', group_concat((TSettingOptions.settingOptionID || ':' || TSettingOptions.optionDisplayName || ':' || TSettingOptions.isSelected)) AS 'options'
                                            FROM TSettings,TSettingOptions 
                                            WHERE TSettings.settingID = TSettingOptions.settingID
	                                              AND TSettings.settingDisplayName = '{DisplayName}'
                                            GROUP BY TSettings.settingID".Replace("{DisplayName}", settingName);
                    var reader = command.ExecuteReader();
                    reader.Read();
                    setting = Setting.FromDataRow(reader);
                    reader.Close();
                }
            }
            return setting;
        }

        /// <summary>
        /// Gets all the settings from the database that are to be used for the settings screen. A setting is marked as for the settings screen if its setting name does not start with an underscore
        /// </summary>
        /// <returns></returns>
        public static List<Setting> QuerySettingsForSettingScreen()
        {
            List<Setting> settings = new List<Setting>();
            using (var connection = OpenDatabase())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT TSettings.settingID, TSettings.settingDisplayName AS 'Setting Name', group_concat((TSettingOptions.settingOptionID || ':' || TSettingOptions.optionDisplayName || ':' || TSettingOptions.isSelected)) AS 'options'
                                            FROM TSettings,TSettingOptions 
                                            WHERE TSettings.settingID = TSettingOptions.settingID
	                                              AND TSettings.settingDisplayName NOT LIKE '\_%' ESCAPE '\'
                                            GROUP BY TSettings.settingID";
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        settings.Add(Setting.FromDataRow(reader));
                    }
                    reader.Close();
                }
            }

            return settings;
        }

        public static void SelectOption(int settingID, int selectedOptionID)
        {
            using (var connection = OpenDatabase())
            {
                connection.Open();
                using (var resetAllOptionsCommand = connection.CreateCommand())
                using (var selectOptionCommand = connection.CreateCommand())
                {
                    // first mark all options as deselected
                    resetAllOptionsCommand.CommandText = @"UPDATE TSettingOptions
                                                           SET isSelected = 0
                                                           WHERE settingID = {settingID}".Replace("{settingID}", settingID.ToString());
                    resetAllOptionsCommand.ExecuteNonQuery();
                    // next step is to select the option with the passed id
                    selectOptionCommand.CommandText = @"UPDATE TSettingOptions
                                                        SET isSelected = 1
                                                        WHERE settingOptionID = {optionID}".Replace("{optionID}", selectedOptionID.ToString());
                    selectOptionCommand.ExecuteNonQuery();
                }
            }
        }

        public static WeatherProvider QueryWeatherProvider(string ProviderName = "")
        {
            WeatherProvider provider = null;
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = @"SELECT TWeatherProviders.weatherProviderID, TWeatherProviders.weatherProviderName, TWeatherProviderURLS.weatherProviderURL AS 'baseURL', group_concat(TWeatherProviderURLParts.weatherProviderURLPartURLString,'###') AS 'urlParts', TWeatherProviderAccessTypes.weatherProviderAccessType AS 'type'
                                    FROM TWeatherProviders, TWeatherProviderURLS, TWeatherProviderURLParts, TWeatherProviderAccessTypes
                                    WHERE TWeatherProviders.weatherProviderID = TWeatherProviderURLS.weatherProviderID AND TWeatherProviders.weatherProviderID = TWeatherProviderURLParts.weatherProviderID
                                    AND TWeatherProviders.weatherProviderName = '{providerName}';".Replace("{providerName}", ProviderName);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    provider = WeatherProvider.FromDataRow(reader);
                }
            }
            conn.Close();
            return provider;
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


        public static SearchEngine QuerySearchEngineByName(string SearchEngineName)
        {
            // escape the single ticks
            SearchEngineName = EscapeSingleTicks(SearchEngineName);
            SearchEngine searchEngine = new SearchEngine();
            SqliteConnection conn = OpenDatabase();
            conn.Open();
            SqliteCommand command = conn.CreateCommand();
            command.CommandText = $"Select TSearchEngines.searchEngineID, TSearchEngines.searchEngineName, TSearchEngines.searchEngineBaseURL, TSearchEngines.searchEngineQueryString From TSearchEngines Where TSearchEngines.searchEngineName = '{SearchEngineName}';";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    searchEngine = SearchEngine.FromDataRow(reader);
                }
            }
            conn.Close();
            return searchEngine;
        }

        public static List<SearchableWebsite> QueryAllSearchableWebsites()
        {
            List<SearchableWebsite> searchableWebsites = new List<SearchableWebsite>();
            string query = @"Select searchableWebsitesID, searchableWebsiteName, searchableWebsiteBaseURL, searchableWebsiteQueryString From TSearchableWebsites;";
            using (SqliteConnection connection = OpenDatabase())
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        searchableWebsites.Add(SearchableWebsite.FromDataRow(reader));
                    }
                }
            }
            return searchableWebsites;
        }

            /// <summary>
            /// Pulls a random joke from the database and returns it
            /// </summary>
            /// <returns></returns>
            public static Joke QueryRandomJoke()
            {
                Joke joke = null;
                using (SqliteConnection connection = OpenDatabase())
                {
                    connection.Open();
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM TJokes WHERE jokeID IN (SELECT jokeID FROM TJokes ORDER BY RANDOM() LIMIT 1)";
                        SqliteDataReader reader = command.ExecuteReader();
                        reader.Read();
                        joke = Joke.FromDataRow(reader);
                        reader.Close();
                    }
                }
                return joke;
            }

            private static string EscapeSingleTicks(string text)
            {
                var tickRegex = new Regex("'");
                return tickRegex.Replace(text, "''");
            }
        }
    }
