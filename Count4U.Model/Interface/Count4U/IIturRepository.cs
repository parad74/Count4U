using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using System.Collections;

namespace Count4U.Model.Interface.Count4U
{

	public interface IIturRepository
	{
		Iturs GetIturs(string pathDB);
		Iturs GetItursAndLocationName(string pathDB)	 ;
		Iturs GetItursAndLocationName(SelectParams selectParams, string pathDB);

		Locations GetLocationList(string pathDB);
		StatusIturs GetStatusIturList(string pathDB);

		List<string> GetIturCodeListByTag(string pathDB, string tag) ;
		List<string> GetIturCodeListIncludedTag(string pathDB, string tag);
		List<string> GetTagList(string pathDB);

		BitArray GetResultStatusBitOrByIturCode(string code, string pathDB, bool refill = false);
		int GetResultStatusIntOrByIturCode(string code, string pathDB, bool refill = false);
		//int RefillStatusBitByIturCode(string code, string pathDB) ;
		//void RefillStatusBit(string pathDB);
		void RefillApproveStatusBit(string pathDB);
		//void FillFirstApproveStatusBit(string pathDB);
		void RefillApproveStatusBit(string iturCode, List<string> docCodes, string pathDB);
		List<string> RefillApproveStatusBitByStep1Docs(List<string> sessionCodeList, string pathDB);
		 void RefillApproveStatusBitByStep2Iturs(List<string> iturCodes, string pathDB);
		void ClearStatusBit(string pathDB);

		int RefillApproveStatusBitByIturCode(string iturCode, string pathDB);
	//	void RefillApproveStatusBitByIturCodeTest(List<string> iturCodes, string pathDB);
		void SetDisabledStatusBitByIturCode(Iturs iturs, bool disabled, string pathDB);
		Iturs SwitchDisabledStatusBitByIturCode(Iturs iturs, bool disabled, string pathDB);
		void RefillApproveStatusBit(Iturs iturs, string pathDB);
		void RefillApproveStatusBit(List<string> docCodes, List<string> sessionCodeList, string pathDB);
		void RefillApproveStatusBitByStep(List<string> docCodes, List<string> sessionCodeList, string pathDB);
		bool RefillApproveStatus { get; set; }
		//int FillFirstApproveStatusBitByIturCode(string code, string pathDB);

		string[] GetIturCodes(string pathDB);
		List<string> GetIturCodesWithInventProduct(string pathDB);
	    int[] GetIturNumbers(string pathDB);
	    int GetItursTotal(string pathDB);
		Iturs GetItursByUnitPlanCode(string unitPlanCode, string pathDB) ;
		List<string> GetIturCodesUnitPlanCode(string unitPlanCode, string pathDB);
		List<string> GetIturCodesForLocationCode(string locationCode, string pathDB);
		List<string> GetIturCodesForLocationCodes(string[] locationCodes, string pathDB);
		void RefillIturStatistic(string pathDB);


		Iturs GetIturs(SelectParams selectParams, string pathDB);

		Itur GetIturByCode(string iturCode, string pathDB);
		Itur GetIturByErpIturCode(string erpIturCode, string pathDB, bool nativ = false);
		Iturs GetItursByLocation(Location location, string pathDB);
		Iturs GetItursByLocationCode(string locationCode, string pathDB);
		Itur GetIturByDocumentCode(string documentCode, string pathDB);
		int GetMaxNumber(string prefix, string pathDB);
	
		Iturs GetItursByStatusCode(string statusCode, string pathDB);

		Iturs GetItursByNumber(string number, string pathDB);
		Iturs GetItursByPrefix(string prefix, string pathDB);
		IEnumerable<Itur> GetItursByNumber(int number, string pathDB);
		IEnumerable<Itur> GetItursByNumberInLocation(int number, string locationCode, string pathDB);
		Iturs GetItursByDate(DateTime createDate, string pathDB);

		void Delete(Itur itur, string pathDB);
		void Delete(string iturCode, string pathDB);
		void DeleteOnlyEmpty(Iturs iturs, string pathDB);
		void DeleteHierarchical(Itur itur, string pathDB);
		void ClearIturHierarchical(Itur itur, string pathDB);

		void DeleteAllByLocationCode(string locationCode, string pathDB);

		void Insert(Itur itur, DocumentHeader documentHeader, string pathDB);

		
		void Insert(Itur itur, string pathDB);

		void Insert(Iturs iturs, string pathDB);

		
		void Insert(Itur itur, Location location, string pathDB);

		
		void Insert(Itur itur, string locationCode, string pathDB);

		void Update(Itur itur, string pathDB);
		void Update(Iturs iturs, string pathDB);
		void DeleteAllIturs(string pathDB);
		void UpdateIturCode(Iturs iturs, string pathDB);

		void UpdatePrefix(string prefixNew, string pathDB);
		void UpdateIturCode(string pathDB);


		Dictionary<string, Itur> GetIturDictionary(string pathDB, bool refill = false);
		Dictionary<string, Itur> GetERPIturDictionary(string pathDB);
		void ClearIturDictionary();
		void AddIturInDictionary(string code, Itur itur);
		void RemoveIturFromDictionary(string code);
		bool IsExistIturInDictionary(string code);

		Itur GetIturByCodeFromDictionary(string code);
		void FillIturDictionary(string pathDB);

		Dictionary<int, int> GetIturTotalGroupByStatuses(string pathDB);
		Dictionary<string, int> GetIturTotalGroupByLocationCode(string pathDB);
		List<string> GetIturCodeList(string pathDB);
		List<string> GetIturCodeListWithAnyDocument(string pathDB);
		List<string> GetLocationCodeList(string pathDB);
		void RepairCodeFromDB(string pathDB);
		double GetIturTotalDone(SelectParams selectParams, string pathDB);
		double GetIturTotalDone(Iturs iturs, string pathDB);
	}
}