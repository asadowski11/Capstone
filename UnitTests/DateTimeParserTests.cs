using Capstone.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class DateTimeParserTests
    {
        [TestMethod]
        public void TestParseWeekDayFromStringFindsCorrectDay()
        {
            Assert.AreEqual(DayOfWeek.Wednesday, DateTimeParser.ParseWeekDayFromString("The day of the week is WednesDay"));
        }

        [TestMethod]
        public void TestParseWeekDayFromStringThrowsErrorIfNoneMatched()
        {
            try
            {
                DateTimeParser.ParseWeekDayFromString("The day of the week is");
            }
            catch (DateParseException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.Fail("A general exception should not be thrown");
            }
        }

        [TestMethod]
        public void TestGetDateForNextWeekDayReturnsCorrectDate()
        {
            // we need to use a fixed date for this test to work. The date in this case is Sunday, March 1st 2020
            var today = new DateTime(2020, 3, 1);
            var nextDay = DayOfWeek.Wednesday;
            // the date we're expecting is Wednesday, March 11th 2020
            var expectedDate = new DateTime(2020, 3, 11);
            var actualDate = DateTimeParser.GetDateForNextWeekDay(nextDay, today);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestGetDateForThisWeekDayReturnsCorrectDate()
        {
            // we need to use a fixed date for this test to work. The date in this case is Sunday, March 1st 2020
            var today = new DateTime(2020, 3, 1);
            var nextDay = DayOfWeek.Wednesday;
            // the date we're expecting is Wednesday, March 4th 2020
            var expectedDate = new DateTime(2020, 3, 4);
            var actualDate = DateTimeParser.GetDateForThisWeekDay(nextDay, today);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestParseExactDateReturnsCorrectDateObject()
        {
            var dayBeforeMonth = "set an alarm for the 05th of March";
            var dayAfterMonth = "add a reminder for october 22nd";

            DateTime beforeMonthDate = DateTimeParser.ParseExactDate(dayBeforeMonth);
            DateTime afterMonthDate = DateTimeParser.ParseExactDate(dayAfterMonth);
            // the expected values
            var expectedBeforeMonth = new DateTime(2020, 3, 5);
            var expectedAfterMonth = new DateTime(2020, 10, 22);

            Assert.AreEqual(expectedBeforeMonth.Day, beforeMonthDate.Day);
            Assert.AreEqual(expectedBeforeMonth.Month, beforeMonthDate.Month);
            Assert.AreEqual(expectedAfterMonth.Day, afterMonthDate.Day);
            Assert.AreEqual(expectedAfterMonth.Month, afterMonthDate.Month);
        }

        [TestMethod]
        public void TestParseExactDateThrowsExceptionIfParseFails()
        {
            try
            {
                DateTimeParser.ParseExactDate("");
            }
            catch (DateParseException)
            {
                Assert.IsTrue(true); // just to have an assert
            }
            catch (Exception e)
            {
                Assert.Fail("A general exception means something else failed: " + e.Message);
            }
        }

        [TestMethod]
        public void TestParseExactDateShiftsYearIfMonthIsPassed()
        {
            // the "today" used to prevent this test from failing after the months have changed
            var today = new DateTime(2020, 6, 1);

            // another test handles the placement of the day, this one only needs to check for one day placement
            var input = "Set an alarm for the 1st of May";
            DateTime expectedDate = new DateTime(2021, 5, 1);
            DateTime actualDate = DateTimeParser.ParseExactDate(input, today);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestGetNextOccurrenceOfDateProperlyShiftsDate()
        {
            // the date we're using as the "now" for the test method
            var now = new DateTime(2020, 3, 19);
            var day = 18;
            var month = 3;
            Assert.AreEqual(new DateTime(2021, month, day), DateTimeParser.GetNextOccurrenceOfDate(day, month, now));
        }

        [TestMethod]
        public void TestGetNextOccurrenceOfDateDoesNotShiftDateIfItHasNotPassed()
        {
            var now = new DateTime(2020, 3, 19);
            var day = 20;
            var month = 3;
            Assert.AreEqual(new DateTime(2020, month, day), DateTimeParser.GetNextOccurrenceOfDate(day, month, now));
        }

        [TestMethod]
        public void TestParseSlashOrDashNotationDoesNotShiftYearIfSpecified()
        {
            var input = "Set an alarm for 5/5/2020";
            var expectedDate = new DateTime(2020, 5, 5);
            var actualDate = DateTimeParser.ParseSlashOrDashNotation(input);

            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestParseSlashOrDashNotationProvidesYearIfNotSpecified()
        {
            var input = "Set an alarm for 5-5";
            // the date used for the starting point so that the test is stable
            var now = new DateTime(2020, 3, 19);
            var expectedDate = new DateTime(2020, 5, 5);
            var actualDate = DateTimeParser.ParseSlashOrDashNotation(input, now);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestGetDateForRelativeOffsetReturnsCorrectlyOffsetDate()
        {
            var testDate = new DateTime(2020, 3, 22);
            var inputString = "add a reminder for 3 weeks from now";
            var expectedDate = new DateTime(2020, 4, 12);
            var actualDate = DateTimeParser.GetDateForRelativeOffset(inputString, testDate);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestGetTimePartOfStringFindsProperParts()
        {
            // test several different time formats
            Assert.AreEqual("7", DateTimeParser.GetTimePartOfString("Set an alarm for 7"));
            Assert.AreEqual("7:30", DateTimeParser.GetTimePartOfString("set an alarm for 7:30"));
            Assert.AreEqual("7:30 pm", DateTimeParser.GetTimePartOfString("set an alarm for 7:30 pm called do unit tests"));
            Assert.AreEqual("19", DateTimeParser.GetTimePartOfString("set an alarm for 19 o'clock"));
            Assert.AreEqual("5 am", DateTimeParser.GetTimePartOfString("set an alarm called take the 3 youngest kids to school at 5 am"));
            Assert.AreEqual("905 am", DateTimeParser.GetTimePartOfString("add a reminder for November 12th at 905 am"));
            Assert.AreEqual("12:00", DateTimeParser.GetTimePartOfString("set an alarm at 12:00 called hi"));
            Assert.AreEqual("2359 pm", DateTimeParser.GetTimePartOfString("2359 pm"));
            Assert.AreEqual("0000", DateTimeParser.GetTimePartOfString("0000"));
            Assert.AreEqual("5pm", DateTimeParser.GetTimePartOfString("set an alarm for 5pm on November 12"));
        }

        [TestMethod]
        public void TestFormatTimeFormatsTimeProperlyDependingOnNumberOfDigits()
        {
            Assert.AreEqual("7:45", DateTimeParser.FormatTime("745"));
            Assert.AreEqual("7:00 am", DateTimeParser.FormatTime("7 Am"));
            Assert.AreEqual("23:59", DateTimeParser.FormatTime("23 59"));
        }

        [TestMethod]
        public void TestParseTimeFromStringCorrectlyParsesTime()
        {
            var inputString = "set an alarm for 7";
            var startingDate = new DateTime(2020, 1, 1, 0, 0, 0);
            var expectedDate = new DateTime(2020, 1, 1, 7, 0, 0);
            var actualDate = DateTimeParser.ParseTimeFromString(inputString, startingDate);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestParseDateTimeFromTextAdds12HoursIfTimeHasPassed()
        {
            var inputString = "set an alarm for 7";
            var startingDate = new DateTime(2020, 1, 1, 7, 0, 0);
            var expectedDate = new DateTime(2020, 1, 1, 19, 0, 0);
            var actualDate = DateTimeParser.ParseDateTimeFromText(inputString, startingDate);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestReplaceDaytimeNamesWithTimeValueProperlyReplacesTimeName()
        {
            var inputString = "set an alarm for Monday MoRning.";
            var expectedValue = $"set an alarm for monday {DateTimeParser.MORNING}.";
            var actualValue = DateTimeParser.ReplaceDaytimeNamesWithTimeValue(inputString);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void TestReplaceDaytimeNamesWithTimeValueWorksOnAllOccurrences()
        {
            var inputString = "Monday Night and Tuesday Evening and wednesday in the after noon";
            var expectedValue = $"monday {DateTimeParser.NIGHT} and tuesday {DateTimeParser.EVENING} and wednesday in the {DateTimeParser.AFTERNOON}";
            var actualValue = DateTimeParser.ReplaceDaytimeNamesWithTimeValue(inputString);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void TestParseDateFromTextProperlyParsesSpecificDates()
        {
            var now = new DateTime(2020, 1, 1);
            // slash dash stuff
            var slashDashNotation = "set an alarm for 4/1";
            var slashDashExpectedDate = new DateTime(2020, 4, 1);
            var slashDashActualDate = DateTimeParser.ParseDateFromText(slashDashNotation, now);
            // specific date stuff
            var specificDateNotation = "set an alarm for the 1st of May";
            var specificDateExpectedDate = new DateTime(2020, 5, 1);
            var specificDateActualDate = DateTimeParser.ParseDateFromText(specificDateNotation, now);
            // "this" weekday dates
            var thisWeekDayNotaion = "set an alarm for this saturday at 11:30 pm";
            var thisWeekDayExpectedDate = new DateTime(2020, 1, 4);
            var thisWeekDayActualDate = DateTimeParser.ParseDateFromText(thisWeekDayNotaion, now);
            // "next" weekday dates
            var nextWeekDayNotaion = "set an alarm for next saturday at 11:30 pm";
            var nextWeekDayExpectedDate = new DateTime(2020, 1, 11);
            var nextWeekDayActualDate = DateTimeParser.ParseDateFromText(nextWeekDayNotaion, now);
            // relative dates (not times)
            var relativeDateNotation = "set an alarm for tomorrow from now at 12";
            var relativeDateExpectedDate = new DateTime(2020, 1, 2);
            var relativeDateActualDate = DateTimeParser.ParseDateFromText(relativeDateNotation, now);

            // test the expected dates vs the actual dates
            Assert.AreEqual(slashDashExpectedDate, slashDashActualDate);
            Assert.AreEqual(specificDateExpectedDate, specificDateActualDate);
            Assert.AreEqual(thisWeekDayExpectedDate, thisWeekDayActualDate);
            Assert.AreEqual(nextWeekDayExpectedDate, nextWeekDayActualDate);
            Assert.AreEqual(relativeDateExpectedDate, relativeDateActualDate);
        }

        [TestMethod]
        public void TestParseDateTimeFromTextDoesNotShiftDatesWithLateTimes()
        {
            // the specific string that found this bug was "get the weather for sunday night", so use that to write the test
            DateTime now = new DateTime(2020, 4, 4).AddHours(16).AddMinutes(39);
            string inputString = "get the weather for sunday night";
            DateTime expectedDate = new DateTime(2020, 4, 5).AddHours(20);
            DateTime actualDate = DateTimeParser.ParseDateTimeFromText(inputString, now);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestParseDateTimeFromTextProperlyParsesDateAndTime()
        {
            var today = new DateTime(2020, 1, 1);
            var inputText = "add a reminder for tomorrow morning that I should take out the trash";
            var expectedDate = new DateTime(2020, 1, 2, 8, 0, 0);
            var actualDate = DateTimeParser.ParseDateTimeFromText(inputText, today);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestParseDateTimeFromTextDoesNotShiftTimeIfUserSpecifiesAMOrPM()
        {
            var today = new DateTime(2020, 1, 1, 13, 0, 0); // january first at 1 pm
            var inputText = "set an alarm for 9 am";
            var expectedDate = new DateTime(2020, 1, 2, 9, 0, 0); // january 2nd at 9 am
            var actualDate = DateTimeParser.ParseDateTimeFromText(inputText, today);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [TestMethod]
        public void TestParseDateTimeFromTextShiftsTimeToFutureIfPassed()
        {
            var inputString = "set an alarm for 5";
            // since AM is assumed if PM isn't indicated, we need to ensure that an extra 24 hours is applied to a time that has passed 
            var startingDate = new DateTime(2020, 1, 1, 17, 0, 0);
            var expectedDate = new DateTime(2020, 1, 2, 5, 0, 0);
            var actualDate = DateTimeParser.ParseDateTimeFromText(inputString, startingDate);
            Assert.AreEqual(expectedDate, actualDate);
        }
    }
}
