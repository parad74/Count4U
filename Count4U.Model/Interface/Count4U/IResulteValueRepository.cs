using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Count4U
{
	public interface IResulteValueRepository
	{
		ResulteValues GetResulteValues(string pathDB);
		ResulteValues GetResulteValues(SelectParams selectParams, string pathDB);
		string[] GetSectionCodes(string pathDB);
		ResulteValue GetResulteValueByCode(string code, string pathDB);
		void Delete(ResulteValue resulteValue, string pathDB);
		void Insert(ResulteValues resulteValues, string pathDB);
		Dictionary<string, ResulteValue> GetResulteValueDictionary(string pathDB, bool refill = false);
		void ClearResulteValueDictionary();
		void FillResulteValueDictionary(string pathDB);

	}
}
