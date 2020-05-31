using System.Collections.Generic;
using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.Models;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BobTheDigitalAssistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstTimeSetup : Page
    {
        private List<Setting> PageSettings = new List<Setting>();

        public FirstTimeSetup()
        {
            this.InitializeComponent();
            PopulateComboBoxesWithSettings();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (this.Validate())
            {
                // for each setting, select the appropriate option in the database
                PageSettings.ForEach(setting =>
                {
                    int settingID = setting.SettingID;
                    int selectedOptionID = setting.GetSelectedOption().OptionID;
                    StoredProcedures.SelectOption(settingID, selectedOptionID);
                });
                // mark the first time setup page as passed in the database
                Setting firstTimeSetupSetting = StoredProcedures.QuerySettingByName("_FirstTimeSetupPassed");
                firstTimeSetupSetting.SelectOption("true");
                StoredProcedures.SelectOption(firstTimeSetupSetting.SettingID, firstTimeSetupSetting.GetSelectedOption().OptionID);
                UIUtils.GoToMainPage(this);
            }
        }

        private void PopulateComboBoxesWithSettings()
        {
            // get the settings and store them in the list
            var searchEngineSetting = StoredProcedures.QuerySettingByName("Search Engine");
            var voiceActivationSetting = StoredProcedures.QuerySettingByName("Voice Activation");
            var mapProviderSetting = StoredProcedures.QuerySettingByName("Map Provider");
            PageSettings.Add(searchEngineSetting);
            PageSettings.Add(voiceActivationSetting);
            PageSettings.Add(mapProviderSetting);
            // populate the search engine settings dropdown
            searchEngineSetting.Options.ForEach(option => this.SearchEngineOptionBox.Items.Add(option.DisplayName));
            voiceActivationSetting.Options.ForEach(option => this.VoiceDetectionOptionBox.Items.Add(option.DisplayName));
            mapProviderSetting.Options.ForEach(option => this.MapProviderOptionBox.Items.Add(option.DisplayName));
        }

        private void SearchEngineOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // find the search engine option, and select its choice based on the combo box's option
            PageSettings.Find(setting => setting.DisplayName == "Search Engine").SelectOption(e.AddedItems[0].ToString());
            UIUtils.HighlightUIElement(this.SearchEngineOptionBox, Colors.Transparent);
        }

        private void VoiceDetectionOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // find the search engine option, and select its choice based on the combo box's option
            PageSettings.Find(setting => setting.DisplayName == "Voice Activation").SelectOption(e.AddedItems[0].ToString());
            UIUtils.HighlightUIElement(this.VoiceDetectionOptionBox, Colors.Transparent);
        }

        private void MapProviderOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PageSettings.Find(setting => setting.DisplayName == "Map Provider").SelectOption(e.AddedItems[0].ToString());
            UIUtils.HighlightUIElement(this.MapProviderOptionBox, Colors.Transparent);
        }

        private bool Validate()
        {
            bool isValid = true;
            // check to make sure each setting is set
            if (this.SearchEngineOptionBox.SelectedIndex == -1)
            {
                UIUtils.HighlightUIElement(this.SearchEngineOptionBox);
                isValid = false;
            }
            if (this.VoiceDetectionOptionBox.SelectedIndex == -1)
            {
                UIUtils.HighlightUIElement(this.VoiceDetectionOptionBox);
                isValid = false;
            }
            if (this.VoiceDetectionOptionBox.SelectedIndex == -1)
            {
                UIUtils.HighlightUIElement(this.MapProviderOptionBox);
                isValid = false;
            }
            return isValid;
        }
    }
}