using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface IInventProductParser
	{
		//int CountExcludeFirstString { get; set; }
		Dictionary<string, DocumentHeader> DocumentHeaderDictionary { get; }
		//string FromPathFile { get; set; }
		IEnumerable<InventProduct> GetInventProducts(
			string fromPathFile, Encoding encoding, string[] separators,
			int countExcludeFirstString, string sessionCodeIn);
		Dictionary<string, Session> SessionDictionary { get; }
	}
}
