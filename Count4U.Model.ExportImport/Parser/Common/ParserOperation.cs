using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public static class ParserOperation
	{
		public static string JoinRecord(this String[] records, string separator)
		{
			string ret = "";
			foreach (string record in records)
			{
				ret = ret + record + separator;
			}
			return ret;
		}

		public static string JoinRecordSplitEndSeparator(this String[] records, string separator)
		{
			string ret = "";
			int k = 0;
			foreach (string record in records)
			{
				if (k == 0) { ret = record; k++; }
				else { ret = ret + separator + record; }
			}
			return ret;
		}

		public static string JoinRecord(this String[] records, string separator, bool excludeEmptyString )
		{
			string ret = "";
			if (excludeEmptyString == false)
			{
				foreach (string record in records)
				{
					ret = ret + record + separator;
				}
			}
			else
			{
				foreach (string record in records)
				{
					if (string.IsNullOrWhiteSpace(record) == false)
					{
						ret = ret + record + separator;
					}
				}
			}
			ret = ret.TrimEnd(separator.ToCharArray());
			return ret;
		}

		public static string JoinRecord(this Dictionary<string, string> records, string separator)
		{
			string ret = "";
			foreach (KeyValuePair<string, string> keyValuePair in records)
			{
				ret = ret + keyValuePair.Key + separator + keyValuePair.Value;
			}
			return ret;
		}

		public static string JoinRecord(this List<string> records, string separator)
		{
			string ret = "";
			foreach (string record in records)
			{
				ret = ret + record + separator + " ";
			}
			ret = ret.Trim(' ');
			ret = ret.Trim(',');
			return ret;
		}
	}
}
