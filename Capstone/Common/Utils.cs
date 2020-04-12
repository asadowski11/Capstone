using System;
using Windows.UI.Core;

namespace Capstone.Common
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
    }
}
