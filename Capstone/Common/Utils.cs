using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common
{
    public static class Utils
    {
        public static string JoinEnum(Type e, string joinString)
        {
            string joined = "";
            var enumValues = Enum.GetValues(e);
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (i > 0)
                {
                    joined += joinString;
                }
                joined += enumValues.GetValue(i).ToString();
            }
            return joined;
        }
    }
}
