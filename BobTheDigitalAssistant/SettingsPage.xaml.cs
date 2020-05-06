using System;
using System.Collections.Generic;
using Capstone.Common;
using Capstone.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Capstone
{
    /// <summary>
    /// The page that displays all the settings that the user can change
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private List<Setting> Settings = new List<Setting>();

        public SettingsPage()
        {
            this.InitializeComponent();
            this.Settings = this.GetSettingsFromDatabase();
            this.PopulateScreenWithSettings();
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            ConfirmCancel();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ConfirmCancel();
        }

        private async void ConfirmCancel()
        {
            MessageDialog dialog = new MessageDialog("You're about to go back without saving your changes. Are you sure?");
            dialog.Commands.Add(new UICommand("No"));
            dialog.Commands.Add(new UICommand("Yes", (command) => UIUtils.GoToMainPage(this)));
            dialog.DefaultCommandIndex = 1;
            await dialog.ShowAsync();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // for each setting, save its state in the database
            this.Settings.ForEach(setting =>
            {
                var settingID = setting.SettingID;
                var selectedOptionID = setting.GetSelectedOption().OptionID;
                StoredProcedures.SelectOption(settingID, selectedOptionID);
            });
            // now go to the main page
            UIUtils.GoToMainPage(this);
        }

        private ComboBox CreateComboBoxForSetting(Setting setting)
        {
            ComboBox box = new ComboBox();
            // for each option in the setting, add its item to the combo box
            setting.Options.ForEach(option => box.Items.Add(option.DisplayName));
            // if the setting has a selected item, select that item in the box
            if (setting.GetSelectedOption() != null)
            {
                box.SelectedItem = setting.GetSelectedOption().DisplayName;
            }
            box.SelectionChanged += (sender, args) => setting.SelectOption(args.AddedItems[0].ToString());
            box.Header = setting.DisplayName;
            box.Margin = new Thickness(25);
            return box;
        }

        private List<Setting> GetSettingsFromDatabase()
        {
            return StoredProcedures.QuerySettingsForSettingScreen();
        }

        private void PopulateScreenWithSettings()
        {
            this.Settings.ForEach(setting => this.SettingsStackPanel.Children.Add(this.CreateComboBoxForSetting(setting)));
        }
    }
}