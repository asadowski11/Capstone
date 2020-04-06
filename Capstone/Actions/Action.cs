using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Capstone.Actions
{
    /// <summary>
    /// A base class for all actions that can be taken by the assistant
    /// </summary>
    public abstract class Action
    {
        /// <summary>
        /// This is the full string use to pick out the action. It can be used to provided context to the action
        /// </summary>
        protected string CommandString { get; set; }

        /// <summary>
        /// this is an object where visual elements of the action can be displayed
        /// </summary>
        protected RelativePanel DynamicArea { get; set; }

        /// <summary>
        /// Used in the event the action requires sending data to the computer speakers
        /// </summary>
        protected MediaElement MediaElement { get; set; }

        public abstract void PerformAction();

        /// <summary>
        /// sets our media element and performs our action, allowing the media element to be used as a speech output
        /// </summary>
        /// <param name="element"></param>
        public void PerformAction(MediaElement element)
        {
            this.MediaElement = element;
            this.PerformAction();
        }

        /// <summary>
        /// Performs our action and sets the media Element and the Dynamic Area for our action to use.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dynamicArea"></param>
        /// <seealso cref="PerformAction"/>
        /// <seealso cref="PerformAction(MediaElement)"/>
        public void PerformAction(MediaElement element, RelativePanel dynamicArea)
        {
            this.DynamicArea = dynamicArea;
            this.DynamicArea.Children.Clear();
            this.ShowLoading();
            this.PerformAction(element);
        }

        /// <summary>
        /// Clears out all children of our <see cref="DynamicArea"/>
        /// </summary>
        protected void ClearArea()
        {
            if (this.DynamicArea != null)
            {
                this.DynamicArea.Children.Clear();
            }
        }

        /// <summary>
        /// Shows a loading ring on our <see cref="DynamicArea"/>, clearing the area before displaying the ring
        /// </summary>
        protected void ShowLoading()
        {
            if (this.DynamicArea != null)
            {
                this.DynamicArea.Children.Clear();
                // show a spinning progress ring in the middle of the area
                ProgressRing theRing = new ProgressRing();
                theRing.Width = 100;
                theRing.Height = theRing.Width;
                RelativePanel.SetAlignHorizontalCenterWithPanel(theRing, true);
                RelativePanel.SetAlignVerticalCenterWithPanel(theRing, true);
                this.DynamicArea.Children.Add(theRing);
                theRing.IsActive = true;
            }
        }

        /// <summary>
        /// Displays the passed <paramref name="messageToShow"/> on our dynamic area
        /// </summary>
        /// <param name="messageToShow"></param>
        protected void ShowMessage(string messageToShow)
        {
            if (this.DynamicArea != null)
            {
                this.DynamicArea.Children.Clear();
                TextBlock textBlock = new TextBlock
                {
                    FontSize = 48,
                    Text = messageToShow,
                    TextWrapping = TextWrapping.Wrap
                };
                ScrollViewer scrollPanel = new ScrollViewer();
                this.DynamicArea.Children.Add(scrollPanel);
                RelativePanel.SetAlignVerticalCenterWithPanel(scrollPanel, true);
                RelativePanel.SetAlignHorizontalCenterWithPanel(scrollPanel, true);
                scrollPanel.Content = textBlock;
            }
        }
    }

}
