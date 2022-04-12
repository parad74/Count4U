using System;
using Count4U.Model.Count4U;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Count4U
{
	/// <summary>
	/// Интерфейс репозитория для доступа к Section объектам
	/// </summary>
	public interface ISectionRepository
	{
		/// <summary>
		/// Получить все секции
		/// </summary>
		/// <returns></returns>
		Sections GetSections(string pathDB);
		Sections GetSections(SelectParams selectParams, string pathDB);
		string[] GetSectionCodes(string pathDB);
		Section GetSectionByCode(string sectionCode, string pathDB);
		List<string> GetSectionCodeListByTag(string pathDB, string tag);
		List<string> GetSectionCodeListIncludedTag(string pathDB, string tag);
		void Delete(Section section, string pathDB);
		void Insert(Section section, string pathDB);
		void Insert(Sections sections, string pathDB);
		void Update(Section section, string pathDB);
		void Update(Sections sections, string pathDB);
		Dictionary<string, Section> GetSectionDictionary(string pathDB, bool refill = false);
		Dictionary<string, Section> GetSectionDictionary_DescriptionKey(string pathDB);
		Dictionary<string, Section> GetSectionDictionary_NameKey(string pathDB);
		void ClearSectionDictionary();
		void FillSectionDictionary(string pathDB);
		List<string> GetSectionCodeList(string pathDB);
		void RepairCodeFromDB(string pathDB);

	}
}
