using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Capstone.Common
{
    static class UIUtils
    {
        /// <summary>
        /// Navigates from the passed CurrentPage to the main screen. Useful for something like a back button
        /// </summary>
        /// <param name="CurrentPage">the instance of the page the user is currently on. must not be null</param>
        public static void GoToMainPage(Page CurrentPage)
        {
            if (CurrentPage is null)
            {
                throw new ArgumentNullException(nameof(CurrentPage));
            }
            CurrentPage.Frame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Colors the background of a <see cref="Control"/> the passed <paramref name="backgroundColor"/>. If no background color is passed, the control's background is colored yellow
        /// </summary>
        /// <param name="control">The control to color</param>
        /// <param name="backgroundColor">the color to set the control's background to, defaults to <see cref="Colors.Yellow"/> if nothing is passed</param>
        public static void HighlightUIElement(Control control, Color? backgroundColor = null)
        {
            Brush backgroundBrush = new SolidColorBrush(backgroundColor.GetValueOrDefault(Colors.Yellow));
            control.Background = backgroundBrush;
        }

        /// <summary>
        /// Attempts to go back to the previous page, defaulting to the passed <paramref name="DefaultPageIfCannotGoBack"/> if the frame cannot go back
        /// </summary>
        /// <param name="CurrentPage"></param>
        /// <param name="DefaultPageIfCannotGoBack"></param>
        public static void GoBack(Page CurrentPage, Type DefaultPageIfCannotGoBack)
        {
            if (CurrentPage.Frame.CanGoBack)
            {
                CurrentPage.Frame.GoBack();
            }
            else
            {
                CurrentPage.Frame.Navigate(DefaultPageIfCannotGoBack);
            }
        }

        public static async void MinimizeWindow()
        {
            IList<AppDiagnosticInfo> infos = await AppDiagnosticInfo.RequestInfoForAppAsync();
            IList<AppResourceGroupInfo> resourceInfos = infos[0].GetResourceGroups();
            await resourceInfos[0].StartSuspendAsync();
        }

        public static void ShowMessageOnRelativePanel(RelativePanel panel, string messageToShow)
        {
            panel.Children.Clear();
            TextBlock textBlock = new TextBlock
            {
                FontSize = 48,
                Text = messageToShow,
                TextWrapping = TextWrapping.Wrap
            };
            ScrollViewer scrollPanel = new ScrollViewer();
            panel.Children.Add(scrollPanel);
            RelativePanel.SetAlignVerticalCenterWithPanel(scrollPanel, true);
            RelativePanel.SetAlignHorizontalCenterWithPanel(scrollPanel, true);
            scrollPanel.Content = textBlock;
        }

    }
}
