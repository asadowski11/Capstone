using Capstone.Common;
using System;

namespace Capstone.Actions
{
    public class AlarmAction : Action
    {
        public string Title { get; set; }
        public DateTime ActivateDateAndTime { get; set; }
        public AlarmActionTypes ActionType { get; set; }

        public AlarmAction(AlarmActionTypes ActionType, string CommandString)
        {
            this.ActionType = ActionType;
            this.CommandString = CommandString;
            this.ActivateDateAndTime = RequestActivatedDateAndTime();
        }

        private string RequestTitle()
        {
            var title = "";
            // TODO speak out to request title, and listen for title input

            return title;
        }

        private DateTime RequestActivatedDateAndTime()
        {
            DateTime activatedDateTime;

            try
            {
                activatedDateTime = DateTimeParser.ParseDateTimeFromText(this.CommandString);
            }
            catch (DateParseException)
            {
                // TODO maybe log the exception
                // TODO ask for the date and time since it could not be parsed
                activatedDateTime = DateTime.Now;
            }

            return activatedDateTime;
        }

        public override void PerformAction()
        {
            // TODO
        }

        public enum AlarmActionTypes
        {
            CREATE = 0,
            EDIT = 1,
            DELETE = 2
        }
    }
}
