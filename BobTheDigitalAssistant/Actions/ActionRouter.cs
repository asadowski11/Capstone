using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capstone.Actions
{
    public static class ActionRouter
    {
        // a case-insensitive dictionary. Due to runtime binding the second argument needs to be set to "dynamic". In practice, this value is  Dictionary<string, dynamic>. The dynamic keeps going until a value of type Func<string, Action> is reached
        private static Dictionary<string, dynamic> actionTree;

        public static bool IsSetup { get; private set; } = false;

        /// <summary>
        /// Sets up the different routes for a voice command. The route order matters, and the routes that are added before others will have more precedence over the other routes
        /// </summary>
        public static void SetUp()
        {
            // initialize the action tree
            actionTree = new Dictionary<string, dynamic>();
            SetUpInternetSearchBranches();
            // add all the main keys to the dictionary
            SetUpAlarmBranches();
            SetUpReminderBranches();
            SetUpWeatherBranches();
            SetUpVoiceNoteBranches();
            SetUpJokeBranches();
            SetUpDirectionBranches();
            SetUpMiscBranches();
            SetUpAgeBranches();
            SetUpTimeBranches();
            SetUpThankYouBranches();
            SetUpDirectBobQuestionBranches();
            SetUpGreetingsBranch();

            IsSetup = true;
        }

        private static void SetUpWeatherBranches()
        {
            // add the keys to the main dict because it's easier for the user to speak less
            Func<string, Action> getWeatherFunction = (commandText) => new WeatherAction(commandText);
            actionTree.Add("weather", getWeatherFunction);
        }

        private static void SetUpVoiceNoteBranches()
        {
            var voiceNoteDict = new Dictionary<string, dynamic>();
            Func<string, Action> recordVoiceNote = (commandText) => new VoiceMemoAction(commandText);
            voiceNoteDict.Add("note", recordVoiceNote);
            voiceNoteDict.Add("record", recordVoiceNote);
            voiceNoteDict.Add("recording", recordVoiceNote);
            actionTree.Add("voice", voiceNoteDict);
            actionTree.Add("record", voiceNoteDict);
            actionTree.Add("recording", voiceNoteDict);
        }

        private static void SetUpInternetSearchBranches()
        {
            Func<string, Action> internetSearchDict = (commandText) => new WebsiteSearchAction(commandText);

            actionTree.Add("internet", internetSearchDict);
            actionTree.Add("search", internetSearchDict);
        }

        private static void SetUpThankYouBranches()
        {
            Func<string, Action> thankYouDict = (commandText) => new ThankYouAction(commandText);

            actionTree.Add("thank", thankYouDict);
            actionTree.Add("thanks", thankYouDict);
        }

        private static void SetUpGreetingsBranch()
        {
            Func<string, Action> greetingDict = (commandText) => new GreetingAction(commandText);

            actionTree.Add("hello", greetingDict);
            actionTree.Add("hi", greetingDict);
            actionTree.Add("howdy", greetingDict);
            actionTree.Add("morning", greetingDict);
            actionTree.Add("afternoon", greetingDict);
            actionTree.Add("evening", greetingDict);
            actionTree.Add("night", greetingDict);
        }

        private static void SetUpTimeBranches()
        {
            Func<string, Action> getTimeAndDate = (commandText) => new TimeAction(commandText);
            // set directly to the action tree as saying "what time is it" is more natural than saying something else
            actionTree.Add("date", getTimeAndDate);
            actionTree.Add("time", getTimeAndDate);
        }

        private static void SetUpReminderBranches()
        {
            // a dictionary that takes a string for the key and returns a lambda that takes a string for the command and returns an ReminderAction
            var reminderDict = new Dictionary<string, dynamic>();
            // the different types of actions that can be taken for an reminder
            Func<string, Action> createReminder = (commandText) => new ReminderAction(ReminderAction.ReminderActionTypes.CREATE, commandText);
            Func<string, Action> deleteReminder = (commandText) => new ReminderAction(ReminderAction.ReminderActionTypes.DELETE, commandText);
            Func<string, Action> editReminder = (commandText) => new ReminderAction(ReminderAction.ReminderActionTypes.EDIT, commandText);
            /* add the different key phrases for the reminder creation, update, and deletion*/
            // create reminder ====================
            reminderDict.Add("create", createReminder);
            reminderDict.Add("set", createReminder);
            reminderDict.Add("new", createReminder);
            reminderDict.Add("add", createReminder);
            // edit reminder ======================
            reminderDict.Add("edit", editReminder);
            reminderDict.Add("update", editReminder);
            reminderDict.Add("change", editReminder);
            reminderDict.Add("alter", editReminder);
            // delete reminder ====================
            reminderDict.Add("delete", deleteReminder);
            reminderDict.Add("remove", deleteReminder);
            reminderDict.Add("clear", deleteReminder);
            reminderDict.Add("void", deleteReminder);
            reminderDict.Add("cancel", deleteReminder);
            // add the reminder dict to the main one
            actionTree.Add("reminder", reminderDict);
        }

        private static void SetUpAlarmBranches()
        {
            // a dictionary that takes a string for the key and returns a lambda that takes a string for the command and returns an AlarmAction
            var alarmDict = new Dictionary<string, dynamic>();
            // the different types of actions that can be taken for an alarm
            Func<string, Action> createAlarm = (commandText) => new AlarmAction(AlarmAction.AlarmActionTypes.CREATE, commandText);
            Func<string, Action> deleteAlarm = (commandText) => new AlarmAction(AlarmAction.AlarmActionTypes.DELETE, commandText);
            Func<string, Action> editAlarm = (commandText) => new AlarmAction(AlarmAction.AlarmActionTypes.EDIT, commandText);
            /* add the different key phrases for the alarm creation, update, and deletion*/
            // create alarm ====================
            alarmDict.Add("create", createAlarm);
            alarmDict.Add("set", createAlarm);
            alarmDict.Add("new", createAlarm);
            alarmDict.Add("add", createAlarm);
            // edit alarm ======================
            alarmDict.Add("edit", editAlarm);
            alarmDict.Add("update", editAlarm);
            alarmDict.Add("change", editAlarm);
            alarmDict.Add("alter", editAlarm);
            // delete alarm ====================
            alarmDict.Add("delete", deleteAlarm);
            alarmDict.Add("remove", deleteAlarm);
            alarmDict.Add("clear", deleteAlarm);
            alarmDict.Add("void", deleteAlarm);
            alarmDict.Add("cancel", deleteAlarm);
            // add the alarm dict to the main one
            actionTree.Add("alarm", alarmDict);
        }

        private static void SetUpDirectionBranches()
        {
            // add the keys to the main dict because it's easier for the user to speak less
            Func<string, Action> getDirectionsFunction = (commandText) => new DirectionsAction(commandText);
            Dictionary<string, dynamic> otherPhrases = new Dictionary<string, dynamic>()
            {
                // for phrases like "how do I get to", and "get me to"
                {"get", getDirectionsFunction },
                // for phrases like "take me to"
                {"take", getDirectionsFunction }
            };
            actionTree.Add("directions", getDirectionsFunction);
            actionTree.Add("to", otherPhrases);
        }

        public static void SetUpJokeBranches()
        {
            Func<string, Action> jokeAction = (commandText) => new JokeAction();
            var jokeDict = new Dictionary<string, dynamic>();
            // listen for the phrases "joke" and "another"
            jokeDict.Add("joke", jokeAction);
            jokeDict.Add("another", jokeAction);
            // root is "tell", but sometimes user's could say other phrases so stick with "joke"
            actionTree.Add("tell", jokeDict);
            actionTree.Add("joke", jokeDict);
        }

        private static void SetUpAgeBranches()
        {
            Func<string, Action> getAge = (commandText) => new AgeAction(commandText);

            actionTree.Add("age", getAge);
            actionTree.Add("born", getAge);
            actionTree.Add("old", getAge);
        }

        private static void SetUpDirectBobQuestionBranches()
        {
            Func<string, Action> whatCanYouDoFunction = (commandText) => new WhatCanYouDoAction(commandText);
            // it's messy, but easier to do than to create multiple dictionaries
            var whatCanYouDoDict = new Dictionary<string, dynamic>()
            {
                {"can", whatCanYouDoFunction }
            };
            Dictionary<string, dynamic> directQuestions = new Dictionary<string, dynamic>()
            {
                {"do", whatCanYouDoDict }
            };
            actionTree.Add("you", directQuestions);
        }

        private static void SetUpMiscBranches()
        {
            Func<string, Action> repeatAfterMeAction = (commandText) => new RepeatAfterMeAction(commandText);
            var repeatAfterMeDict = new Dictionary<string, dynamic>()
            {
                {"me", repeatAfterMeAction}
            };
            actionTree.Add("repeat", repeatAfterMeDict);
        }

        /// <summary>
        /// Searches through the passed <paramref name="DictToCheck"/> for a keyword that most matches a word in the passed <paramref name="inputString"/>
        /// </summary>
        /// <param name="inputString">A string that contains any amount of text. Each space-separated word is used in an attemptto match against a key in the dictionary</param>
        /// <param name="DictToCheck">The dictionary whose keys will be checked for a keyword</param>
        /// <returns>The found keyword if one was found, otherwise null is returned</returns>
        public static string FindKeyword(string inputString, Dictionary<string, dynamic> DictToCheck)
        {
            string foundKeyword = null;
            // split the inputString into tokens and get rid of anything that's not a letter
            Regex specialCharRegex = new Regex(@"[^\w ]|[_]");
            string[] tokens = specialCharRegex.Replace(inputString.ToLower(), "").Split(" ");
            List<string> keys = DictToCheck.Keys.ToList();
            // iterate through the keys to find the keyword
            foreach (string key in keys)
            {
                if (tokens.Contains(key.ToLower()))
                {
                    foundKeyword = key;
                    break;
                }
            }
            return foundKeyword;
        }

        /// <summary>
        /// Finds a keyword from the <paramref name="inputString"/> that exists within the key set of the <paramref name="DictToCheck"/>, and then returns the value assigned to that key
        /// </summary>
        /// <param name="inputString">a phrase that may contain a keyword that the DictToCheck has</param>
        /// <param name="DictToCheck">The dictionary whose keys are being checked to see if the keyword exists</param>
        /// <returns>a dynamic value, but the type will either be a Dictionary&lt;string, Func&gt; or a Func object</returns>
        public static dynamic GetNextNodeInChain(string inputString, Dictionary<string, dynamic> DictToCheck)
        {
            dynamic value = null;
            // get the keyword from the inputString
            var keyword = FindKeyword(inputString, DictToCheck);
            if (keyword != null)
            {
                value = DictToCheck[keyword];
            }
            return value;
        }

        /// <summary>
        /// Gets the function for the passed <paramref name="commandString"/> and returns it
        /// </summary>
        /// <param name="commandString">the command you want bob to do (e.g. "Set an alarm at 5:30 A.M.")</param>
        /// <returns></returns>
        public static Func<string, Action> GetFunctionFromCommandString(string commandString)
        {
            dynamic currentNode = actionTree;
            // quasi-recursively traverse the tree and update our currentNode until it's either null or a function
            while (currentNode != null && currentNode.GetType() != typeof(Func<string, Action>))
            {
                currentNode = GetNextNodeInChain(commandString, currentNode);
            }

            // at this point it should be a func or null
            return currentNode;
        }
    }
}