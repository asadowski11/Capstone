using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace BobTheDigitalAssistant.Common
{
	public static class DateTimeParser
	{
		private enum DaysOfWeek
		{
			// used to determine if there's an error
			NONE = -1,

			SUNDAY = DayOfWeek.Sunday,
			MONDAY = DayOfWeek.Monday,
			TUESDAY = DayOfWeek.Tuesday,
			WEDNESDAY = DayOfWeek.Wednesday,
			THURSDAY = DayOfWeek.Thursday,
			FRIDAY = DayOfWeek.Friday,
			SATURDAY = DayOfWeek.Saturday
		}

		private enum Months
		{
			// used to indicate an error
			NONE = -1,

			JANUARY = 1,
			FEBRUARY = 2,
			MARCH = 3,
			APRIL = 4,
			MAY = 5,
			JUNE = 6,
			JULY = 7,
			AUGUST = 8,
			SEPTEMBER = 9,
			OCTOBER = 10,
			NOVEMBER = 11,
			DECEMBER = 12
		}

		/// <summary>
		/// Represents supported units of time for this parser class
		/// </summary>
		private enum Units
		{
			// used to represent an error
			NONE = -1,

			SECOND = 0,
			MINUTE = 1,
			HOUR = 2,
			DAY = 3,
			WEEK = 4,
			MONTH = 5,
			YEAR = 6,

			// this isn't really a unit, but putting it here is the easiest because of where this enum is used
			TOMORROW = 7
		}

		// constant values for names assigned to times of day
		public static readonly string MORNING = "8:00 am";

		public static readonly string NOON = "12:00 pm";
		public static readonly string DAY = "1:00 pm";
		public static readonly string AFTERNOON = "4:00 pm";
		public static readonly string EVENING = "6:00 pm";
		public static readonly string NIGHT = "8:00 pm";
		public static readonly string MIDNIGHT = "tomorrow 12:00 am";

		/// <summary>
		/// Formats a time-like piece of string to be in a parseable format by <see cref="DateTime.Parse(string)"/>
		/// The rules for parsing are as follows:
		/// <list type="bullet">
		/// <item>If the time has 1 or 2 digits, those digits are used as the hour and 00 is set as the minutes</item>
		/// <item>If the time has exactly 3 digits, the first digit is used as the hour and the other 2 are used as the minutes</item>
		/// <item>If the time has exactly 4 digits, the first 2 digits are used as the hour and the other 2 are used as the minutes</item>
		/// <item>if the original time has either am/pm in it, am/pm will be added to the end of the time respectively</item>
		/// </list>
		/// This method does not validate that the time is ok, only that it's formatted correctly
		/// </summary>
		/// <param name="time">The time-like string to be properly formatted</param>
		/// <returns></returns>
		public static string FormatTime(string time)
		{
			// a copy of the passed time so that we don't screw up the original for later
			var cleanRegex = new Regex("(am|pm)|[ :]");
			string timeCopy = cleanRegex.Replace(time.ToLower(), "").Trim();
			// the length of the string changes where we need to insert characters
			if (timeCopy.Length == 1 || timeCopy.Length == 2)
			{
				// it's the hour; need to add ":00"
				timeCopy += ":00";
			}
			else if (timeCopy.Length == 3)
			{
				// full time with a single-digit hour; add a colon after the first character
				timeCopy = timeCopy.Substring(0, 1) + ":" + timeCopy.Substring(1);
			}
			else if (timeCopy.Length == 4)
			{
				// full time with a double-digit hour; add a colon after the second character
				timeCopy = timeCopy.Substring(0, 2) + ":" + timeCopy.Substring(2);
			}
			// re-add the am/pm if the original time has it
			if (time.ToLower().Contains("am"))
			{
				timeCopy += " am";
			}
			else if (time.ToLower().Contains("pm"))
			{
				timeCopy += " pm";
			}
			return timeCopy;
		}

		/// <summary>
		///  Checks if the passed text has a date or a time in it, and returns a boolean corresponding to if either a date or a time was found
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool DoesStringHaveDateOrTime(string text)
		{
			bool hasDateOrTime = true;
			try
			{
				DateTimeParser.ParseDateFromText(text);
			}
			catch (DateParseException)
			{
				// if we get here then we know that there is no date, so check for the time
				hasDateOrTime = StringUtils.IsNotBlank(DateTimeParser.GetTimePartOfString(text));
			}
			return hasDateOrTime;
		}

		/// <summary>
		/// Returns a date that is on the specified <paramref name="nextDay"/> 1 week after the passed <paramref name="date"/>
		/// </summary>
		/// <param name="nextDay">The weekday to get the date for</param>
		/// <param name="date">The date to get the weekday for relative to</param>
		/// <returns>a new date 1 week after the passed date on the passed nextDay</returns>
		public static DateTime GetDateForNextWeekDay(System.DayOfWeek nextDay, DateTime date)
		{
			// get the number of days in between the date's day of the week and the passed nextDay
			DayOfWeek today = date.DayOfWeek;
			// formula to get the number of days until the specified next day = 7 - (today - nextDay)
			int numberOfDaysUntilNextDay = 7 - (today - nextDay);
			// create a new DateTime from the passed date so that we don't modify the passed date
			DateTime newDate = new DateTime(date.Ticks).AddDays(numberOfDaysUntilNextDay);
			return newDate;
		}

		/// <summary>
		/// Gets a date from the passed text that contains a relative date (e.g. "set an alarm for 5 days from now") and returns it
		/// </summary>
		/// <param name="text"></param>
		/// <param name="onlyUsedForTests">the date used as the starting point to create the returned date from</param>
		/// <returns></returns>
		/// <exception cref="DateParseException" />
		public static DateTime GetDateForRelativeOffset(string text, DateTime onlyUsedForTests)
		{
			DateTime offsetDateTime;
			var now = onlyUsedForTests;
			Units specifiedUnit = ParseUnitFromText(text);
			if (specifiedUnit == Units.NONE)
			{
				throw new DateParseException($"could not find a unit in the passed string [{text}]");
			}
			// create a regex to parse out the number before the returned unit
			var regexString = $"(?i)[0-9]{{1,3}}(?= {specifiedUnit.ToString()})|tomorrow(?-i)";
			var regex = new Regex(regexString);
			var match = regex.Match(text);
			if (match.Success)
			{
				int parsedNumber = match.Value.ToLower().Equals("tomorrow") ? 1 : int.Parse(match.Value);
				// figure out which unit to increment
				switch (specifiedUnit)
				{
					case Units.SECOND:
						offsetDateTime = now.AddSeconds(parsedNumber);
						break;

					case Units.MINUTE:
						offsetDateTime = now.AddMinutes(parsedNumber);
						break;

					case Units.HOUR:
						offsetDateTime = now.AddHours(parsedNumber);
						break;

					case Units.DAY:
						offsetDateTime = now.AddDays(parsedNumber);
						break;

					case Units.WEEK:
						offsetDateTime = now.AddDays(7 * parsedNumber);
						break;

					case Units.MONTH:
						offsetDateTime = now.AddMonths(parsedNumber);
						break;

					case Units.YEAR:
						offsetDateTime = now.AddYears(parsedNumber);
						break;

					case Units.TOMORROW:
						offsetDateTime = now.AddDays(1);
						break;

					case Units.NONE:
					default:
						throw new DateParseException("Something went wrong with adding the unit to the date");
				}
			}
			else
			{
				throw new DateParseException($"could not find number of unit for text [{text}]");
			}
			return offsetDateTime;
		}

		/// <summary>
		/// Gets the date for a specific day that has yet to occur during this week. To get the next date for a day that has already happened, see <see cref="GetDateForNextWeekDay"/>
		/// </summary>
		/// <param name="nextDay">The day of the week to get the date for</param>
		/// <param name="date">the date that the day of the week should be determined starting from</param>
		/// <returns></returns>
		/// <exception cref="DateParseException" />
		public static DateTime GetDateForThisWeekDay(System.DayOfWeek nextDay, DateTime date)
		{
			// get the number of days in between the date's day of the week and the passed nextDay
			DayOfWeek today = date.DayOfWeek;
			// if today is after the passed nextDay, then get the date for the next occurence of that day
			if (today > nextDay)
			{
				return GetDateForNextWeekDay(nextDay, date);
			}
			// formula to get the number of days until the specified next day = Math.Abs(today - nextDay)
			int numberOfDaysUntilNextDay = Math.Abs(today - nextDay);
			// create a new DateTime from the passed date so that we don't modify the passed date
			DateTime newDate = new DateTime(date.Ticks).AddDays(numberOfDaysUntilNextDay);
			return newDate;
		}

		private static DateTime GetDateForThisWeekDay(System.DayOfWeek nextDay)
		{
			return GetDateForThisWeekDay(nextDay, DateTime.Now);
		}

		public static DateTime GetNextOccurrenceOfDate(int day, int month, DateTime onlyUsedForTests)
		{
			var now = onlyUsedForTests;
			var year = (int)month < now.Month || (int)month == now.Month && day < now.Day ? now.Year + 1 : now.Year;
			return new DateTime(year, month, day);
		}

		/// <summary>
		/// Finds the first instance of text in the string that matches a time-like format. Examples of this format are:
		/// <list type="bullet">
		/// <item>7 am</item>
		/// <item>7:30 pm</item>
		/// <item>2359</item>
		/// <item>19 o'clock</item>
		/// </list>
		/// This method does not ensure that the time is in proper format to be parsed by <see cref="System.DateTime.Parse(string)"/>
		/// </summary>
		/// <param name="text">the string to find a time-like structure in</param>
		/// <returns>the found time if it was found, else an empty string</returns>
		public static string GetTimePartOfString(string text)
		{
			string timePart = "";
			text = text.ToLower();
			// use a regex to parse out the time. This regex can handle partial times, full times, am, pm, and o'clock formats.
			var timeRegex = new Regex("([0-2]?[0-9][: ]?[0-5][0-9]( ?[ap]m)?)|([0-9]{1,4} ?[ap]m)|([0-9]{1,2}(?=( ?)o([' -]?)clock))|([0-9]{1,4}$|[0-9]{1,2}[: ][0-9]{1,2}$)");
			var match = timeRegex.Match(text);
			if (match.Success)
			{
				timePart = match.Value;
			}
			return timePart;
		}

		public static DateTime ParseDateTimeFromText(string text, DateTime onlyUsedForTests)
		{
			var now = onlyUsedForTests;
			// format time values
			text = ReplaceDaytimeNamesWithTimeValue(text);
			DateTime time;
			DateTime date;
			// will be factored into the final date. This will add to the number of days based on if the user specifies am or pm in the time and a couple other factors
			var dayOffset = 0;
			try
			{
				time = ParseTimeFromString(text);
			}
			catch (Exception)
			{
				// if time isn't in the string, then assume that they mean now (use case would be a string that only contains "today")
				time = now;
			}
			try
			{
				date = ParseDateFromText(text, onlyUsedForTests);
			}
			catch (DateParseException)
			{
				// this might mean that the date wasn't specified, and it means "today" is implied"
				date = new DateTime(now.Year, now.Month, now.Day);
			}

			// if date is today, and the time has already passed, shift the time 12 hours forward to hit the next time that time would happen
			string timePart = GetTimePartOfString(text).ToLower();
			if (now.Year == date.Year && now.Month == date.Month && now.Day == date.Day && now.TimeOfDay >= time.TimeOfDay)
			{
				// if the user specified am or pm, then move the day forward. Else assume they mean the next occurence of the time and move the time forward 12 hours
				if (!StringUtils.Contains(timePart, "am") && !StringUtils.Contains(timePart, "pm"))
				{
					time = time.AddHours(12);
					// if time is still less than now, then add another 12 hours
					if (now.TimeOfDay >= time.TimeOfDay)
					{
						time = time.AddHours(12);
						dayOffset = 1;
					}
				}
				else
				{
					dayOffset = 1;
				}
			}

			return (new System.DateTime(date.Year, date.Month, date.Day).AddDays(dayOffset)) + time.TimeOfDay;
		}

		public static DateTime ParseDateTimeFromText(string text)
		{
			return ParseDateTimeFromText(text, DateTime.Now);
		}

		/// <summary>
		/// Parses a date from the passed text. The date within the text can be of different formats (e.g. 3/1, 1st of March, 3 days from now, next sunday, this wednesday, tomorrow)
		/// </summary>
		/// <param name="text"></param>
		/// <param name="onlyUsedForTests"></param>
		/// <returns></returns>
		public static DateTime ParseDateFromText(string text, DateTime onlyUsedForTests)
		{
			DateTime parsedDateTime;
			// regexes used to determine which type of date we need to parse
			var slashDashRegex = new Regex(@"[0-9]?[0-9][/\-. ][0-9]{1,2}([/\-. ][0-9]{4})?");
			// create a regex string for each month and day of the week
			string monthsList = Utils.JoinEnum(typeof(Months), " ?| ?");
			var dayOfMonthRegex = new Regex($"(?i)[0-9]{{1,2}}(?=(th|rd|st|nd))|( ?({monthsList}) ?[0-9]{{1,2}})(?-i)");
			string daysList = Utils.JoinEnum(typeof(DaysOfWeek), "|");
			var thisWeekDayRegex = new Regex($"(?i)((this )?({daysList}))(?-i)");
			var nextWeekDayRegex = new Regex($"(?i)next ({daysList})(?-i)");
			// relative dates
			var relativeList = Utils.JoinEnum(typeof(Units), "|");
			var relativeDateRegex = new Regex($"(?i)from (now|today|this day)|{relativeList}(?-i)");

			// check each regex and determine which method to call based on the type
			if (slashDashRegex.IsMatch(text))
			{
				parsedDateTime = ParseSlashOrDashNotation(text, onlyUsedForTests);
			}
			else if (dayOfMonthRegex.IsMatch(text))
			{
				parsedDateTime = ParseExactDate(text, onlyUsedForTests);
			}
			else if (nextWeekDayRegex.IsMatch(text))
			{
				parsedDateTime = GetDateForNextWeekDay(ParseWeekDayFromString(text), onlyUsedForTests);
			}
			else if (thisWeekDayRegex.IsMatch(text))
			{
				parsedDateTime = GetDateForThisWeekDay(ParseWeekDayFromString(text), onlyUsedForTests);
			}
			else if (relativeDateRegex.IsMatch(text))
			{
				parsedDateTime = GetDateForRelativeOffset(text, onlyUsedForTests);
			}
			else
			{
				throw new DateParseException($"Failed to parse general date! [{text}]");
			}

			return parsedDateTime;
		}

		public static DateTime ParseDateFromText(string text)
		{
			return ParseDateFromText(text, DateTime.Now);
		}

		public static DateTime ParseExactDate(string text)
		{
			return ParseExactDate(text, DateTime.Now);
		}

		/// <summary>
		/// Parses an exact date (e.g. "March 4th 2014", "March 4th", "4th of March", etc.) from a string of text and returns it
		/// </summary>
		/// <param name="text">The text to parse the date from</param>
		/// <param name="startingDate">Today's date, used to determine if the date references next year or not. Passed as a parameter to allow for unit testing</param>
		/// <returns></returns>
		/// <exception cref="DateParseException" />
		public static DateTime ParseExactDate(string text, DateTime startingDate)
		{
			// get the first occurrence of a month within the text
			Months referencedMonth = GetMonthFromString(text);
			if (referencedMonth == Months.NONE)
			{
				throw new DateParseException("Month not found in text!");
			}
			// the datetime to return
			DateTime dateTime;
			// get the index of the month in the string and determine where the day of the month is using a regex
			string pattern = $"[0-9]{{1,2}}(?=(th|rd|st|nd) of {referencedMonth.ToString().ToLower()})|(?<={referencedMonth.ToString().ToLower()} )[0-9]{{1,2}}";
			Regex dayOfMonthRegex = new Regex(pattern);
			var match = dayOfMonthRegex.Match(text.ToLower());
			if (match.Success)
			{
				// check if the month has already passed. If so, we need to shift the year forward
				var matchedDay = int.Parse(match.Value);
				dateTime = GetNextOccurrenceOfDate(matchedDay, (int)referencedMonth, startingDate);
			}
			else
			{
				throw new DateParseException("Regex match could not find the month's day!");
			}
			return dateTime;
		}

		public static DateTime ParseSlashOrDashNotation(string text)
		{
			return ParseSlashOrDashNotation(text, DateTime.Now);
		}

		/// <summary>
		/// Parses a date from a piece of text in a format with a separator and returns it. The allowed separators are:
		/// <list type="bullet">
		/// <item>slash</item>
		/// <item>period</item>
		/// <item>space</item>
		/// <item>dash</item>
		/// </list>
		/// </summary>
		/// <param name="text">The text to parse the date from</param>
		/// <param name="onlyUsedForTests">The date used to determine if the date refers to next year, if no year is specified. This is passed as a parameter to allow for unit tests to work</param>
		/// <returns></returns>
		/// <exception cref="DateParseException" />
		public static DateTime ParseSlashOrDashNotation(string text, DateTime onlyUsedForTests)
		{
			DateTime parsedDate;
			// matches a month/day/year pattern using either dashes, slashes, spaces, or periods as the separator
			var withYear = new Regex("[0-9]{1,2}[-/ .][0-9]{1,2}[-/ .][0-9]{4}");
			var withoutYear = new Regex("[0-9]{1,2}[-/ .][0-9]{1,2}");
			var withYearMatch = withYear.Match(text);
			var withoutYearMatch = withoutYear.Match(text);
			if (withYearMatch.Success)
			{
				// parse the date outright, there's no need to determine if the year needs to be shifted as it was specified
				parsedDate = DateTime.Parse(withYearMatch.Value);
			}
			else if (withoutYearMatch.Success)
			{
				// use our method to get the next occurrence of that date
				var dateWithWrongYear = DateTime.Parse(withoutYearMatch.Value);
				parsedDate = GetNextOccurrenceOfDate(dateWithWrongYear.Day, dateWithWrongYear.Month, onlyUsedForTests);
			}
			else
			{
				throw new DateParseException("A date could not be parsed from " + text);
			}
			return parsedDate;
		}

		/// <summary>
		/// parses the first occurrence of a time-like structure from the passed text and returns it as a <see cref="DateTime"/>
		/// </summary>
		/// <param name="text">the string to find and parse the time from</param>
		/// <returns>the parsed time</returns>
		/// <exception cref="FormatException">if the time cannot be parsed due to an error in the numbers (e.g. 23:61 isn't a valid time but can determined as the time-like structure since it's shaped like the time)</exception>
		/// <exception cref="ArgumentNullException">if <paramref name="text"/> is null</exception>
		public static DateTime ParseTimeFromString(string text)
		{
			return ParseTimeFromString(text, DateTime.Now);
		}

		public static DateTime ParseTimeFromString(string text, DateTime onlyUsedForTests)
		{
			var now = onlyUsedForTests;
			// get the formatted time from the text
			string timePart = GetTimePartOfString(text);
			DateTime parsedDate;
			DateTime parsedTime;
			try
			{
				string formattedTimePart = FormatTime(timePart);
				parsedTime = DateTime.Parse(formattedTimePart);
			}
			catch (Exception)
			{
				// check if there are any time units in the string, if there are use that to get the time
				var unitRegex = new Regex("(?i)(hour|minute|second)(?-i)");
				if (unitRegex.IsMatch(text))
				{
					parsedTime = GetDateForRelativeOffset(text, now);
				}
				else
				{
					parsedTime = DateTime.Now;
				}
			}
			parsedDate = now.Date + parsedTime.TimeOfDay;
			return parsedDate;
		}

		/// <summary>
		/// scans the pased text for one of the days of the week and returns which <see cref="System.DayOfWeek"/> matches the string. The check is case-insensitive.
		/// <br />
		/// If no day of week was matched, a <see cref="DateParseException"/> will be thrown
		/// </summary>
		/// <param name="text">the text to check</param>
		/// <returns>the matching day of week.</returns>
		/// <exception cref="DateParseException" />
		public static System.DayOfWeek ParseWeekDayFromString(string text)
		{
			text = text.ToLower();
			DaysOfWeek matchedDay = DaysOfWeek.NONE;
			var dayOfWeekList = Enum.GetValues(typeof(DaysOfWeek));
			foreach (DaysOfWeek day in dayOfWeekList)
			{
				string dayName = day.ToString().ToLower();
				if (text.Contains(dayName))
				{
					matchedDay = day;
				}
			}
			// figure out which System.DayOfWeek to return
			switch (matchedDay)
			{
				case DaysOfWeek.SUNDAY:
					return DayOfWeek.Sunday;

				case DaysOfWeek.MONDAY:
					return DayOfWeek.Monday;

				case DaysOfWeek.TUESDAY:
					return DayOfWeek.Tuesday;

				case DaysOfWeek.WEDNESDAY:
					return DayOfWeek.Wednesday;

				case DaysOfWeek.THURSDAY:
					return DayOfWeek.Thursday;

				case DaysOfWeek.FRIDAY:
					return DayOfWeek.Friday;

				case DaysOfWeek.SATURDAY:
					return DayOfWeek.Saturday;

				case DaysOfWeek.NONE:
				default:
					throw new DateParseException($"The string [{text}] does not contain a day of the week!");
			}
		}

		public static string ReplaceDaytimeNamesWithTimeValue(string text)
		{
			text = text.ToLower().Replace("this after noon", "afternoon").Replace("this afternoon", "afternoon").Replace("tonight", "night").Replace("after noon", "afternoon");
			// create a regex to match where the daytime words should be
			var regex = new Regex(@"(?i)(?<=(in the |at |during the |monday |tuesday |wednesday |thursday |friday |saturday |sunday |tomorrow |this ))[a-zA-Z]{3,}(?-i)|afternoon|midnight|night|noon");
			// make sure that "after noon" gets scrunched down to "afternoon" for the regex to work
			while (regex.IsMatch(text))
			{
				var match = regex.Match(text);
				var value = match.Value.ToLower();
				var startIndex = match.Index;
				var length = match.Length;
				// split the string based on the match index and length
				string[] splitText = { text.Substring(0, startIndex), text.Substring(startIndex + length) };
				// find the matching time value
				string replacedTimeValue = "";
				switch (value.ToLower())
				{
					case "morning":
						replacedTimeValue = MORNING;
						break;

					case "noon":
						replacedTimeValue = NOON;
						break;

					case "afternoon":
						replacedTimeValue = AFTERNOON;
						break;

					case "day":
						replacedTimeValue = DAY;
						break;

					case "evening":
						replacedTimeValue = EVENING;
						break;

					case "night":
						replacedTimeValue = NIGHT;
						break;

					case "midnight":
						replacedTimeValue = MIDNIGHT;
						break;
				}
				// squish the string together with the replaced time
				text = splitText[0] + replacedTimeValue + splitText[1];
			}
			return text;
		}

		private static int FindFirstIndexOfMonth(string text, Months month)
		{
			text = text.ToLower();
			int matchingIndex = text.ToLower().IndexOf(month.ToString().ToLower());
			return matchingIndex;
		}

		private static DateTime GetDateForNextWeekDay(System.DayOfWeek nextDay)
		{
			return GetDateForNextWeekDay(nextDay, DateTime.Now);
		}

		private static DateTime GetDateForRelativeOffset(string text)
		{
			return GetDateForRelativeOffset(text, DateTime.Now);
		}

		private static Months GetMonthFromString(string text)
		{
			text = text.ToLower();
			Months foundMonth = Months.NONE;
			// get an array of the months
			var months = Enum.GetValues(typeof(Months));
			foreach (Months month in months)
			{
				if (text.Contains(month.ToString().ToLower()))
				{
					foundMonth = month;
					break;
				}
			}
			return foundMonth;
		}

		/// <summary>
		/// takes a piece of text and replaces all references of full month names with their numeric counterparts (e.g. January becomes 1, and December becomes 12). The replacing is done ignoring case.
		/// </summary>
		/// <param name="text">the text to have all month occurrences replaced in</param>
		/// <returns></returns>
		private static DateTime GetNextOccurrenceOfDate(int day, int month)
		{
			return GetNextOccurrenceOfDate(day, month, DateTime.Now);
		}

		private static Units ParseUnitFromText(string text)
		{
			Units parsedUnit = Units.NONE;
			var unitsArray = Enum.GetValues(typeof(Units));
			foreach (Units unit in unitsArray)
			{
				var lowercaseUnit = unit.ToString().ToLower();
				if (text.ToLower().Contains(lowercaseUnit))
				{
					parsedUnit = unit;
					break;
				}
			}
			return parsedUnit;
		}

		private static string ReplaceMonthNamesWithMonthNumber(string text)
		{
			string result = text;
			// for each month, create a case-insensitive regex for it and use it to replace the text
			var months = Enum.GetValues(typeof(Months));
			foreach (Months month in months)
			{
				var monthRegex = new Regex($"(?i){month.ToString()}(?-i)");
				result = monthRegex.Replace(result, ((int)month).ToString());
			}
			return result;
		}
	}

	public class DateParseException : Exception
	{
		public DateParseException()
		{
		}

		public DateParseException(string message) : base(message)
		{
		}

		public DateParseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DateParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
