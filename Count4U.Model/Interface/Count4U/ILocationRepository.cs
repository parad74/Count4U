using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к Location объектам
    /// </summary>
    public interface ILocationRepository
    {
        /// <summary>
        /// Получить список всех локейшенов
        /// </summary>
        /// <returns></returns>
		Locations GetLocations(string pathDB);
		Locations GetLocations(int topCount, string pathDB);

        /// <summary>
        /// Получить список локейшенов по параметрам выборки
        /// </summary>
        /// <param name="selectParams"></param>
        /// <returns></returns>
		Locations GetLocations(SelectParams selectParams, string pathDB);

        /// <summary>
        /// Получить локейшен по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		Location GetLocationByName(string name, string pathDB);

		/// <summary>
		/// Получить локейшен по коду
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		Location GetLocationByCode(string code, string pathDB);

        /// <summary>
        /// Удалить 
        /// </summary>
        /// <param name="location"></param>
		void Delete(Location location, string pathDB);

        /// <summary>
        /// Удалить все
        /// </summary>
		void DeleteAll(string pathDB);

        /// <summary>
        /// Вставить
        /// </summary>
        /// <param name="location"></param>
		void Insert(Location location, string pathDB);
		void Insert(List<Location> locations, string pathDB);

        /// <summary>
        /// Изменить
        /// </summary>
        /// <param name="location"></param>
		void Update(Location location, string pathDB);
		void Update(Locations locations, string pathDB);

		Dictionary<string, Location> GetLocationDictionary(string pathDB, 	bool refill = false);
		void ClearLocationDictionary();
		void AddLocationInDictionary(string code, Location location);
		void RemoveLocationFromDictionary(string code) ;
		bool IsExistLocationInDictionary(string code);
		Location GetLocationByCodeFromDictionary(string code);
		void FillLocationDictionary(string pathDB);
		List<string> GetLocationCodeList(string pathDB);
		List<string> GetLocationCodeListByTag(string pathDB, string tag);
		List<string> GetLocationCodeListIncludedTag(string pathDB, string tag);
		Locations GetLocationListByTag(string pathDB, string tag);
		void RepairCodeFromDB(string pathDB);
		string GetFistLocationCodeWithoutIturs(string pathDB);
		string GetFistLocationCodeWithoutIturs(Locations locations, string pathDB);
        string GetFistFromAllLocationCodeWithoutIturs(string pathDB);



    }
}
