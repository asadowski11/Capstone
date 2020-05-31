using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;

namespace BobTheDigitalAssistant.Helpers
{
	public static class ExtendedExecutionHelper
	{
		private static ExtendedExecutionSession session = null;
		public static bool IsRunning => session != null;

		public static async Task<ExtendedExecutionResult> RequestExtendedSessionAsync()
		{
			StopCurrentSession();
			var newSession = new ExtendedExecutionSession();
			newSession.Reason = ExtendedExecutionReason.Unspecified;
			newSession.Description = "for background voice recognition and alarm/reminder tracking";
			newSession.Revoked += (sender, args) => StopCurrentSession();

			// ask for permission for extended execution
			ExtendedExecutionResult result = await newSession.RequestExtensionAsync();
			switch (result)
			{
				case ExtendedExecutionResult.Allowed:
					session = newSession;
					break;

				case ExtendedExecutionResult.Denied:
					newSession.Dispose();
					break;
			}
			return result;
		}

		private static void StopCurrentSession()
		{
			if (session != null)
			{
				session.Dispose();
				session = null;
			}
		}
	}
}
