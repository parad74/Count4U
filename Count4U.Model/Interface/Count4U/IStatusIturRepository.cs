using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к StatusItur объектам
    /// </summary>
	public interface IStatusIturRepository
	{
		Dictionary<string, StatusItur> CodeStatusIturDictionary { get; }

		Dictionary<int, IturStatusEnum> BitStatusIturEnumDictionary { get; }

		void SetBackgroundColor(string pathDB, IturStatusEnum iturStatus, string backgroundColor);
	}
}
