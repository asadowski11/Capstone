using BobTheDigitalAssistant.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class ActionRouterTests
    {
        [TestMethod]
        public void TestFindKeywordReturnsMatchedKeyword()
        {
            const string keyword = "piano";
            string inputString = $"this is the input string, and the keyword is {keyword}";
            // create the dictionary of the values
            Dictionary<string, dynamic> inputDict = new Dictionary<string, dynamic>();
            inputDict.Add("notIt", "whatever");
            inputDict.Add("weather", "whatever");
            // the uppercase P tests case insensitivity
            inputDict.Add("Piano", "whatever");
            inputDict.Add("alarms", "whatever");
            inputDict.Add("reminders", "whatever");
            string matchingKeyword = ActionRouter.FindKeyword(inputString, inputDict);
            Assert.AreEqual(keyword, matchingKeyword.ToLower());
        }

        [TestMethod]
        public void TestFindKeywordReturnsNullIfKeywordIsNotFound()
        {
            const string keyword = "pianoforte";
            string inputString = $"this is the input string, and the keyword is {keyword}";
            // create the dictionary of the values
            Dictionary<string, dynamic> inputDict = new Dictionary<string, dynamic>();
            inputDict.Add("notIt", "whatever");
            inputDict.Add("weather", "whatever");
            // the uppercase P tests case insensitivity
            inputDict.Add("Piano", "whatever");
            inputDict.Add("alarms", "whatever");
            inputDict.Add("reminders", "whatever");
            string matchingKeyword = ActionRouter.FindKeyword(inputString, inputDict);
            Assert.IsNull(matchingKeyword);
        }

        [TestMethod]
        public void TestGetNextNodeInChainReturnsValueAttachedToFoundKeyword()
        {
            const string keyword = "piano";
            string inputString = $"this is the input string, and the keyword is {keyword}";
            // create the dictionary of the values, mimicking the case insensitivity the original dictionary has
            Dictionary<string, dynamic> inputDict = new Dictionary<string, dynamic>();
            inputDict.Add("notIt", "whatever");
            inputDict.Add("weather", "whatever");
            inputDict.Add("Piano", "correct value");
            inputDict.Add("alarms", "whatever");
            inputDict.Add("reminders", "whatever");
            var value = ActionRouter.GetNextNodeInChain(inputString, inputDict);
            Assert.AreEqual(value, "correct value");
        }

        [TestMethod]
        public void TestGetFunctionFromCommandStringReturnsExpectedFunction()
        {
            // setup the action router
            ActionRouter.SetUp();
            // the command string
            const string commandString = "Set an alarm for 5:30 A.M.";
            Func<string, BobTheDigitalAssistant.Actions.Action> returnedFunction = ActionRouter.GetFunctionFromCommandString(commandString);
            Assert.IsNotNull(returnedFunction);
            AlarmAction returnedAction = (AlarmAction)returnedFunction(commandString);
            // check the alarm action's values
            Assert.AreEqual(AlarmAction.AlarmActionTypes.CREATE, returnedAction.ActionType);
        }

        [TestMethod]
        public void TestGetFunctionFromCommandStringReturnsNullIfNoDesiredActionCanBeDetermined()
        {
            ActionRouter.SetUp();
            const string commandString = "Made you look!";
            Assert.IsNull(ActionRouter.GetFunctionFromCommandString(commandString));
        }

        [TestMethod]
        public void TestGetFunctionFromCommandStringReturnsExpectedFunctionWhenShallowFunction()
        {
            // tests that the GetFunctionFromCommandString function still works even if it's not working on a nested Dictionary
            ActionRouter.SetUp();
            const string commandString = "Get the weather for tomorrow";
            Func<string, BobTheDigitalAssistant.Actions.Action> returnedFunction = ActionRouter.GetFunctionFromCommandString(commandString);
            Assert.IsNotNull(returnedFunction);
            WeatherAction returnedAction = (WeatherAction)returnedFunction(commandString);
            Assert.IsNotNull(returnedAction);
        }
    }
}
