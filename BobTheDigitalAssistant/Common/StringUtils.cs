namespace BobTheDigitalAssistant.Common
{
    public static class StringUtils
    {
        /// <summary>
        /// Checks if the passed string is null or blank, by checking if the string is equal to "" once it has been trimmed.
        /// </summary>
        /// <param name="ToCheck">The string to check if it's blank</param>
        /// <returns><code>true</code> if the string is null or blank, <code>false</code> otherwise</returns>
        public static bool IsBlank(string ToCheck)
        {
            bool isBlank = false;
            if (ToCheck is null || ToCheck.Trim().Equals(""))
            {
                isBlank = true;
            }
            return isBlank;
        }

        public static bool IsNotBlank(string ToCheck) => !IsBlank(ToCheck);

        public static bool StartsWith(string ToCheck, string startsWith) => ToCheck != null && ToCheck.ToLower().StartsWith(startsWith.ToLower());

        public static bool Contains(string MainString, string contains) => MainString != null && MainString.ToLower().Contains(contains.ToLower());

        public static bool AreEqual(string a, string b) => a != null && b != null && a.ToLower().Equals(b.ToLower());
    }
}