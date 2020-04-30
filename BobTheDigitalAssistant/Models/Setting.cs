using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    /// <summary>
    /// A Setting is an object representing a configurable option for the application
    /// </summary>
    public class Setting
    {
        public int SettingID { get; set; }
        public string DisplayName { get; set; }
        public List<SettingOption> Options { get; set; } = new List<SettingOption>();

        public Setting() : this(-1, "", new List<SettingOption> { })
        {

        }

        public Setting(int SettingID, string DisplayName, List<SettingOption> Options)
        {
            this.SettingID = SettingID;
            this.DisplayName = DisplayName;
            this.Options = Options;
        }

        public static Setting FromDataRow(SqliteDataReader reader)
        {
            var setting = new Setting
            {
                SettingID = int.Parse(reader["settingID"].ToString()),
                DisplayName = reader["Setting Name"].ToString()
            };
            string[] settingOptionList = (reader["options"].ToString()).Split(',');
            foreach (string optionString in settingOptionList)
            {
                setting.Options.Add(SettingOption.FromDataRow(optionString));
            }
            return setting;
        }

        public SettingOption GetSelectedOption()
        {
            return this.Options.Find(option => option.IsSelected);
        }

        public void SelectOption(string optionName)
        {
            // deselect all options except for the one with the passed name
            this.Options.ForEach(option => option.IsSelected = option.DisplayName == optionName);
        }
    }

    public class SettingOption
    {
        public int OptionID { get; set; }
        public string DisplayName { get; set; }
        public bool IsSelected { get; set; }

        public SettingOption() : this(-1, "", false)
        {

        }

        public SettingOption(int OptionID, string DisplayName, bool IsSelected)
        {
            this.OptionID = OptionID;
            this.DisplayName = DisplayName;
            this.IsSelected = IsSelected;
        }

        public static SettingOption FromDataRow(string optionData)
        {
            string[] splitData = optionData.Split(":");
            var option = new SettingOption
            {
                OptionID = int.Parse(splitData[0]),
                DisplayName = splitData[1],
                IsSelected = Convert.ToBoolean(int.Parse(splitData[2]))
            };

            return option;
        }
    }
}
