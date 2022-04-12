using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к StatusItur объектам
    /// </summary>
	public interface IStatusIturGroupRepository
	{
		Dictionary<string, StatusIturGroup> CodeStatusIturGroupDictionary { get; }
		Dictionary<string, StatusIturGroup> CodeStatusIturGroupWithNoneDictionary { get; }

		Dictionary<int, IturStatusGroupEnum> BitStatusIturGroupEnumDictionary { get; }
		Dictionary<int, IturStatusGroupEnum> BitStatusIturGroupEnumWithNoneDictionary { get; }

		void SetBackgroundColor(string pathDB, IturStatusGroupEnum iturStatusGroup, string backgroundColor);

		IturStatusGroupEnum GetIturStatusGroup(IturStatusEnum iturStatus);
		List<IturStatusEnum> GetIturStatusList(IturStatusGroupEnum iturStatusGroup);
		string FromIturStatusGroupBitToLocalizationCode(int statusGroupBit);
	}
}
