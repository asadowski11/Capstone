using System;
using System.Threading;
using BobTheDigitalAssistant.Models;
using Windows.ApplicationModel;
using Windows.UI.Core;

namespace BobTheDigitalAssistant.Common
{
    public static class Utils
    {
        public static string JoinEnum(Type e, string joinString)
        {
            string joined = "";
            var enumValues = Enum.GetValues(e);
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (i > 0)
                {
                    joined += joinString;
                }
                joined += enumValues.GetValue(i).ToString();
            }
            return joined;
        }

        public static async void RunOnMainThread(Action actionToRun)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => actionToRun.Invoke());
        }

        public static bool IsListeningSettingEnabled()
        {
            Setting voiceRecognitionSetting = StoredProcedures.QuerySettingByName("Voice Activation");
            SettingOption chosenSetting = voiceRecognitionSetting.GetSelectedOption();
            return chosenSetting != null && chosenSetting.DisplayName == "Enabled";
        }

        public static void RunOnSeparateThread(Action action, bool isBackground = false)
        {
            Thread thread = new Thread(new ThreadStart(action));
            thread.IsBackground = isBackground;
            thread.Start();
        }

        public static string GetAppPackagePath()
        {
            return Package.Current.InstalledLocation.Path;
        }
    }
}