using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U.Mapping;

namespace Count4U.Model.Count4U
{
    public class IturRepository : IIturRepository
    {
 
        private Iturs _iturList;

        public IturRepository()
        {
            
        }

        #region IIturRepository Members

		public Iturs GetIturs(string pathDB)
		{
			if (this._iturList == null)
			{
				this._iturList = new Iturs {
		                   new Itur {IturCode="IturCode1", Name="Itur1",  LocationCode = "LocationCode1", StatusIturCode = "StatusItur1", Number=16},
		       
		               };
			}
			return this._iturList;
		}

		//public DomainObjects<Itur> DomainObjects(SelectParams selectParams)
		//{
		//    return null;
		//}

        public int[] GetIturNumbers(string pathDB)
        {
            throw new NotImplementedException();
        }

        public int GetItursTotal(string pathDB)
        {
            throw new NotImplementedException();
        }

        public Iturs GetIturs(SelectParams selectParams, string pathDB)
        {
            return null;
        }

		public Iturs GetItursByLocation(Location location, string pathDB)
		{
			return this.GetItursByLocationCode(location.Code, pathDB);
		}

		public Iturs GetItursByLocationCode(string locationCode, string pathDB)
		{
			var domainObjects = this.GetIturs(pathDB).Where(e => e.LocationCode.CompareTo(locationCode) == 0)
                                          .ToList().Select(e => e.ToDomainObject());
            return Iturs.FromEnumerable(domainObjects);
		}

		//public Iturs GetItursByLocationID(long locationID, string pathDB)
		//{
		//    var domainObjects = this.GetIturs(pathDB).Where(e => e.LocationID == locationID)
		//                                  .ToList().Select(e => e.ToDomainObject());
		//    return Iturs.FromEnumerable(domainObjects);
		//}

		public Iturs GetItursByDocumentHeader(DocumentHeaders documentHeader, string pathDB)
        {
            // TODO: невозможно реализовать метод,
            // так как недоступен репозиторий IDocumentHeaderRepository.
            throw new NotImplementedException();
        }

		public Iturs GetItursByDocumentHeaderID(long documentHeaderID, string pathDB)
        {
            // TODO: невозможно реализовать метод,
            // так как недоступен репозиторий IDocumentHeaderRepository.
            throw new NotImplementedException();
        }

		//public Iturs GetItursByStatusID(long statusID, string pathDB)
		//{
		//    var domainObjects = this.GetIturs(pathDB).Where(e => e.StatusIturID == statusID)
		//                                  .ToList().Select(e => e.ToDomainObject());
		//    return Iturs.FromEnumerable(domainObjects);
		//}

		public Iturs GetItursByStatus(string statusCode, string pathDB)
		{
			var domainObjects = this.GetIturs(pathDB).Where(e => e.StatusIturCode.CompareTo(statusCode) == 0)
                                          .ToList().Select(e => e.ToDomainObject());
            return Iturs.FromEnumerable(domainObjects);
		}

		//public Itur Clone(Itur itur)
		//{
		//    var domainObject = itur.Clone();
		//    domainObject.ID = 0;
		//    return domainObject;
		//}

		public void Delete(Itur itur, string pathDB)
		{
			var entity = this.GetEntityByCode(itur.IturCode, pathDB);
			if (entity == null) return;
			this.GetIturs(pathDB).Remove(entity);
		}

		public void DeleteAllByLocationCode(string locationCode, string pathDB)
        {
			this.GetIturs(pathDB).RemoveAll(e => e.LocationCode == locationCode);
        }

		//public void DeleteAllByDocumentHeaderID(long documentHeaderID, string pathDB)
		//{
		//             throw new NotImplementedException();
		//}

		public void Insert(Itur itur, DocumentHeader documentHeader, string pathDB)
        {
            // TODO: невозможно реализовать метод,
            // так как недоступен репозиторий IDocumentHeaderRepository.
            throw new NotImplementedException();
        }

		public void Insert(Itur itur, string pathDB)
        {
			if (itur == null) return;
            var entity = itur.ToEntity();
			this.GetIturs(pathDB).Add(entity);
        }

		public void Insert(Itur itur, Location location, string pathDB)
        {
			if (itur == null) return;
			if (location == null) return;
            var entity = itur.ToEntity();
           entity.LocationCode = location.Code;
		   entity.Name1 = location.Name;
			this.GetIturs(pathDB).Add(entity);
        }

		public void Insert(Itur itur, long locationID, string pathDB)
        {
			if (itur == null) return;
            var entity = itur.ToEntity();
 			this.GetIturs(pathDB).Add(entity);
        }

		public void Update(Itur itur, string pathDB)
        {
			if (itur == null) return;
			var entity = this.GetEntityByCode(itur.IturCode, pathDB);
			if (entity == null) return;
            entity.ApplyChanges(itur);
        }

		public Iturs 
			GetItursByNumber(string number, string pathDB)
        {
			var entities = this.GetIturs(pathDB).Where(e => e.Number.CompareTo(number) == 0)
                                     .ToList().Select(e => e.ToDomainObject());
            return Iturs.FromEnumerable(entities);
        }

		public Iturs GetItursByDate(DateTime createDate, string pathDB)
        {
           throw new NotImplementedException();
        }
	

		public Itur GetIturByCode(string iturCode, string pathDB)
		{
			var entity = this.GetEntityByCode(iturCode, pathDB);
			if (entity == null) return null;
            return entity.ToDomainObject();		    
		}

		public Itur GetIturByErpIturCode(string erpIturCode, string pathDB, bool nativ = false)
		{
			throw new NotImplementedException();
		}
		private Itur GetEntityByCode(string iturCode, string pathDB)
        {
			var entity = this.GetIturs(pathDB).First(e => e.IturCode.CompareTo(iturCode) == 0);
            return entity;
        }
		public Iturs GetItursByStatusCode(string statusCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public Itur GetIturByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Insert(Itur itur, string locationCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public System.Collections.Generic.Dictionary<string, Itur> GetIturDictionary(string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public void ClearIturDictionary()
		{
			throw new NotImplementedException();
		}

		public void AddIturInDictionary(string code, Itur itur)
		{
			throw new NotImplementedException();
		}

		public void RemoveIturFromDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public bool IsExistIturInDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public Itur GetIturByCodeFromDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public void FillIturDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public Locations GetLocationList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public StatusIturs GetStatusIturList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public System.Collections.BitArray GetResultStatusBitAndByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public int GetResultStatusIntOrByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public System.Collections.BitArray GetResultStatusBitOrByIturCode(string code, string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public int GetResultStatusIntOrByIturCode(string code, string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		//public int RefillStatusBitByIturCode(string code, string pathDB)
		//{
		//    throw new NotImplementedException();
		//}

		//public void RefillStatusBit(string pathDB)
		//{
		//    throw new NotImplementedException();
		//}

		public void ClearStatusBit(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public int RefillApproveStatusBitByIturCode(string iturCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public void RefillApproveStatusBit(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void RefillApproveStatusBit(string iturCode, List<string> docCodes, string pathDB)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IIturRepository Members


		//public void FillFirstApproveStatusBit(string pathDB)
		//{
		//    throw new NotImplementedException();
		//}

		//public int FillFirstApproveStatusBitByIturCode(string code, string pathDB)
		//{
		//    throw new NotImplementedException();
		//}

		#endregion

		#region IIturRepository Members


		public string[] GetIturCodes(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Insert(Iturs iturs, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public void RefillApproveStatusBit(Iturs iturs, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

        public Dictionary<int, int> GetIturTotalGroupByStatuses(string pathDB)
        {
            return null;
        }

		#region IIturRepository Members


		public void SetDisabledStatusBitByIturCode(Iturs iturs, bool disabled, string pathDB)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IIturRepository Members


		public void Update(Iturs iturs, string pathDB)
		{
			throw new NotImplementedException();
		}
		
		public void DeleteAllIturs(Iturs iturs, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public bool RefillApproveStatus
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IIturRepository Members


		public void RefillApproveStatusBit(List<string> docCodes, List<string> sessionCodeList, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void RefillApproveStatusBit( List<string> sessionCodeList, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public void DeleteOnlyEmpty(Iturs iturs, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void DeleteHierarchical(Itur itur, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void ClearIturHierarchical(Itur itur, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public List<string> GetIturCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		public List<string> GetLocationCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public void RepairCodeFromDB(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public Iturs GetItursByUnitPlanCode(string unitPlanCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public List<string> GetIturCodesUnitPlanCode(string unitPlanCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public double GetIturTotalDone(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public double GetIturTotalDone(Iturs iturs, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public Iturs GetItursAndLocationName(string pathDB)
		{
			throw new NotImplementedException();
		}

		public Iturs GetItursAndLocationName(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion


		public List<string> GetIturCodesForLocationCode(string[] list, string pathDB)
		{
			throw new NotImplementedException();
		}


		public void RefillApproveStatusBitByIturCodeTest(List<string> iturCodes, string pathDB)
		{
			throw new NotImplementedException();
		}


		public void UpdateIturCode(Iturs iturs, string pathDB)
		{
			throw new NotImplementedException();
		}


		IEnumerable<Itur> IIturRepository.GetItursByNumber(int number, string pathDB)
		{
			throw new NotImplementedException();
		}


		public IEnumerable<Itur> GetItursByNumberInLocation(int number, string locationCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void UpdatePrefix(string prefixNew, string pathDB)
		{
			throw new NotImplementedException();
		}


		public void UpdateIturCode(string pathDB)
		{
			throw new NotImplementedException();
		}


		public void RefillIturStatistic(string pathDB)
		{
			throw new NotImplementedException();
		}


		public List<string> GetIturCodesWithInventProduct(string pathDB)
		{
			throw new NotImplementedException();
		}


		public Dictionary<string, Itur> GetERPIturDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#region IIturRepository Members


		public int GetMaxNumber(string prefix, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public List<string> GetIturCodesForLocationCodes(List<string> locationCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public List<string> GetIturCodesForLocationCode(string locationCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public List<string> GetIturCodesForLocationCodes(string[] locationCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public List<string> GetIturCodeListByTag(string pathDB, string tag)
		{
			throw new NotImplementedException();
		}

		public List<string> GetIturCodeListIncludedTag(string pathDB, string tag)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public List<string> GetTagList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturRepository Members


		public List<string> GetIturCodeListWithAnyDocument(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Delete(string iturCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public Iturs GetItursByPrefix(string prefix, string pathDB)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, int> GetIturTotalGroupByLocationCode(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void DeleteAllIturs(string pathDB)
		{
			throw new NotImplementedException();
		}

		public Iturs SwitchDisabledStatusBitByIturCode(Iturs iturs, bool disabled, string pathDB)
		{
			throw new NotImplementedException();
		}

		public List<string> RefillApproveStatusBitByStep1Docs(List<string> sessionCodeList, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void RefillApproveStatusBitByStep2Iturs(List<string> iturCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void RefillApproveStatusBitByStep(List<string> docCodes, List<string> sessionCodeList, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
