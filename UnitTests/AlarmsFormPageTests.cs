using Microsoft.VisualStudio.TestTools.UnitTesting;
using BobTheDigitalAssistant;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace UnitTests
{
    [TestClass]
    public class AlarmsFormPageTests
    {
        [UITestMethod]
        public void TestValidateTimeReturnsFalseForPastHour()
        {
            var page = new AlarmsFormPage();
            var alarmTime = System.DateTime.Now.AddHours(-1);
            page.AlarmToEdit = new BobTheDigitalAssistant.Models.Alarm(-1, "", alarmTime, false, false);
            page.PopulateFormFromAlarm();
            Assert.IsFalse(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateTimeReturnsFalseForPastMinute()
        {
            var page = new AlarmsFormPage();
            var alarmTime = System.DateTime.Now.AddMinutes(-1);
            page.AlarmToEdit = new BobTheDigitalAssistant.Models.Alarm(-1, "", alarmTime, true, false);
            page.PopulateFormFromAlarm();
            Assert.IsFalse(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateTimeReturnsTrueForFutureHour()
        {
            var page = new AlarmsFormPage();
            var alarmTime = System.DateTime.Now.AddHours(1);
            page.AlarmToEdit = new BobTheDigitalAssistant.Models.Alarm(-1, "", alarmTime, false, false);
            page.PopulateFormFromAlarm();
            Assert.IsTrue(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateTimeReturnsFalseForPastHourAndFutureMinute()
        {
            var page = new AlarmsFormPage();
            var alarmTime = System.DateTime.Now.AddHours(-1).AddMinutes(10);
            page.AlarmToEdit = new BobTheDigitalAssistant.Models.Alarm(-1, "", alarmTime, true, false);
            page.PopulateFormFromAlarm();
            Assert.IsFalse(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateTimeReturnsTrueIfDayIsSetForFuture()
        {
            var page = new AlarmsFormPage();
            // setting the day in the future but the time in the past to ensure the day overrides everything
            var alarmTime = System.DateTime.Now.AddDays(1).AddHours(-1).AddMinutes(-1);
            page.AlarmToEdit = new BobTheDigitalAssistant.Models.Alarm(-1, "", alarmTime, true, false);
            page.PopulateFormFromAlarm();
            Assert.IsTrue(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateFormReturnsTrueIfAllFieldsAreValid()
        {
            var page = new AlarmsFormPage();
            // make the time in the future so that that part passes
            var alarmTime = System.DateTime.Now.AddHours(1);
            page.AlarmToEdit = new BobTheDigitalAssistant.Models.Alarm(-1, "test title", alarmTime, true, false);
            page.PopulateFormFromAlarm();
            Assert.IsTrue(page.ValidateForm());
        }

    }
}
