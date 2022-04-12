using System;

namespace Count4U.Common.Helpers
{
    public static class UtilsItur
    {
        public static string SuffixFromNumber(int number)
        {
            return String.Format("{0:0000}", number);
        }

        public static string PrefixFromString(string prefix)
        {
            return String.Format("{0:0000}", Int32.Parse(prefix));
        }

        public static string CodeFromPrefixAndSuffix(string prefix, string suffix)
        {
            return prefix + suffix;
        }
    }
}