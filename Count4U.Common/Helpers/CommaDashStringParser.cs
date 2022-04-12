using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Common.Helpers
{
    public class CommaDashStringParser
    {
        public static List<int> Parse(string input)
        {
            if (!IsValid(input))
                return null;

            if (String.IsNullOrEmpty(input))
                return null;

            List<int> result = new List<int>();

            string[] raw = input.Split(new[] { ',' }).Where(r => !String.IsNullOrEmpty(r)).ToArray();
            foreach (string element in raw)
            {
                string trimmed = element.Trim();
                if (trimmed.Contains("-"))
                {
                    string[] dashRaw = trimmed.Split(new[] { '-' });
                    string[] dashTrimmed = dashRaw.Where(r => !String.IsNullOrEmpty(r)).Select(r => r.Trim()).ToArray();
                    if (dashTrimmed.Count() != 2)
                        continue;

                    int from = Int32.Parse(dashTrimmed[0]);
                    int to = Int32.Parse(dashTrimmed[1]);

                    to++;

                    Enumerable.Range(from, to - from).ToList().ForEach(result.Add);
                }
                else
                {
                    result.Add(Int32.Parse(trimmed));
                }
            }

            return result.Distinct().ToList();
        }

        public static bool IsValid(string input)
        {
            if (String.IsNullOrEmpty(input))
                return true;

            char[] valid = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '-' };

            foreach (char character in input)
            {
                if (!valid.Contains(character))
                    return false;
            }

            string[] raw = input.Split(new[] { ',' }).Where(r => !String.IsNullOrEmpty(r)).ToArray();

            if (raw.Count() == 0)
                return false;

            foreach (string element in raw)
            {
                string trimmed = element.Trim();
                if (trimmed.Contains("-"))
                {
                    string[] dashRaw = trimmed.Split(new[] { '-' });
                    string[] dashTrimmed = dashRaw.Where(r => !String.IsNullOrEmpty(r)).Select(r => r.Trim()).ToArray();
                    if (dashTrimmed.Count() != 2)
                        return false;

                    int from = Int32.Parse(dashTrimmed[0]);
                    int to = Int32.Parse(dashTrimmed[1]);

                    if (from == 0 || to == 0 || to <= from)
                        return false;
                }
            }

            return true;
        }

        public static string Reverse(List<int> input)
        {
            if (input == null || input.Count == 0)
                return String.Empty;

            StringBuilder result = new StringBuilder();

            List<int> ordered = input.Distinct().OrderBy(r => r).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                if (i == 0)
                    result.AppendFormat("{0}", ordered[i]);
                else
                {
                    if (i < ordered.Count - 1 && ordered[i] == ordered[i - 1] + 1 && ordered[i + 1] == ordered[i] + 1) //begin of sequence or in the middle of sequence
                    {
                        if (result[result.Length - 1] != '-')
                            result.Append('-');
                    }
                    else
                    {
                        if (result[result.Length - 1] == '-') //end of sequence
                            result.AppendFormat("{0}", ordered[i]);
                        else
                            result.AppendFormat(",{0}", ordered[i]);
                    }
                }
            }

            string ret = result.ToString();
            return ret;
        }
    }
}