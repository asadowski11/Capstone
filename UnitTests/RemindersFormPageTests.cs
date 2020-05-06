using Microsoft.VisualStudio.TestTools.UnitTesting;
using BobTheDigitalAssistant;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace UnitTests
{
    [TestClass]
    public class RemindersFormPageTests
    {
        [UITestMethod]
        public void TestValidateTimeReturnsFalseForPastHour()
        {
            var page = new RemindersFormPage();
            var ReminderTime = System.DateTime.Now.AddHours(-1);
            page.ReminderToEdit = new BobTheDigitalAssistant.Models.Reminder(-1, "", ReminderTime, "", false, false);
            page.PopulateFormFromReminder();
            Assert.IsFalse(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateTimeReturnsFalseForPastMinute()
        {
            var page = new RemindersFormPage();
            var ReminderTime = System.DateTime.Now.AddMinutes(-1);
            page.ReminderToEdit = new BobTheDigitalAssistant.Models.Reminder(-1, "", ReminderTime, "", true, false);
            page.PopulateFormFromReminder();
            Assert.IsFalse(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateTimeReturnsTrueForFutureHour()
        {
            var page = new RemindersFormPage();
            var ReminderTime = System.DateTime.Now.AddHours(1);
            page.ReminderToEdit = new BobTheDigitalAssistant.Models.Reminder(-1, "", ReminderTime, "", false, false);
            page.PopulateFormFromReminder();
            Assert.IsTrue(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateTimeReturnsFalseForPastHourAndFutureMinute()
        {
            var page = new RemindersFormPage();
            var ReminderTime = System.DateTime.Now.AddHours(-1).AddMinutes(10);
            page.ReminderToEdit = new BobTheDigitalAssistant.Models.Reminder(-1, "", ReminderTime, "", true, false);
            page.PopulateFormFromReminder();
            Assert.IsFalse(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateFormReturnsFalseForReminderWithoutTitle()
        {
            var page = new RemindersFormPage();
            // make the time in the future so that that part passes
            var ReminderTime = System.DateTime.Now.AddHours(1);
            page.ReminderToEdit = new BobTheDigitalAssistant.Models.Reminder(-1, "", ReminderTime, "", false, false);
            page.PopulateFormFromReminder();
            Assert.IsFalse(page.ValidateForm());
        }

        [UITestMethod]
        public void TestValidateTimeReturnsTrueIfDayIsSetForFuture()
        {
            var page = new RemindersFormPage();
            // setting the day in the future but the time in the past to ensure the day overrides everything
            var ReminderTime = System.DateTime.Now.AddDays(1).AddHours(-1).AddMinutes(-1);
            page.ReminderToEdit = new BobTheDigitalAssistant.Models.Reminder(-1, "", ReminderTime, "", true, false);
            page.PopulateFormFromReminder();
            Assert.IsTrue(page.ValidateTime());
        }

        [UITestMethod]
        public void TestValidateFormReturnsTrueIfAllFieldsAreValid()
        {
            var page = new RemindersFormPage();
            // make the time in the future so that that part passes
            var ReminderTime = System.DateTime.Now.AddHours(1);
            page.ReminderToEdit = new BobTheDigitalAssistant.Models.Reminder(-1, "test title", ReminderTime, "", true, false);
            page.PopulateFormFromReminder();
            Assert.IsTrue(page.ValidateForm());
        }

    }
}
