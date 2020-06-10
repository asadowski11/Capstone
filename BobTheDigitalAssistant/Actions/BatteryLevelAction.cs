using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Devices.Power;
using BobTheDigitalAssistant.Common;

namespace BobTheDigitalAssistant.Actions
{
	class BatteryLevelAction : Action
	{
		public BatteryLevelAction(string CommandString)
		{
			this.CommandString = CommandString;
		}

		public override void PerformAction()
		{
			// TODO ensure this works on a computer without a battery
			BatteryReport report = Battery.AggregateBattery.GetReport();
			if (report.RemainingCapacityInMilliwattHours.HasValue && report.FullChargeCapacityInMilliwattHours.HasValue)
			{
				float capacity = (float)report.RemainingCapacityInMilliwattHours.Value / report.FullChargeCapacityInMilliwattHours.Value * 100f;
				int roundedCapacity = (int)capacity;
				this.CreateBatteryDisplay(roundedCapacity);
				TextToSpeechEngine.SpeakText(this.MediaElement, $"Your battery is currently at {roundedCapacity} percent.");
			}
			else
			{
				// tell the user that bob was unable to get battery percentage
				string message = "Sorry, but I was unable to get the status of your battery";
			}
		}

		private void CreateBatteryDisplay(int batteryPercent)
		{

		}
	}
}
