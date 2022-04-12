using System;
using System.Collections.Generic;
using Count4U.Model.Main;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Main;
using Count4U.Model;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.UnityExtensions;
using Count4U.Model.SelectionParams;
using Count4U.Model.ServiceContract;
using Common.Utility.Constant;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;

namespace Count4U.Model.Service
{
    public class IturRepositoryService : IIturWcfRepository
	{
	
		public List<Itur> GetIturList()
		{
			Tuple<IIturRepository, string>  iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return null;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return null;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			Iturs iturs = itruRepository.GetIturs(dbPath);
			List<Itur> iturList = new List<Itur>();
			foreach (Itur itur in iturs)
			{
				iturList.Add(itur);
			}
			return iturList;
		}

		
		public Iturs GetIturs()
		{
			Tuple<IIturRepository, string> iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return null;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return null;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			Iturs iturs = itruRepository.GetIturs(dbPath);
			return iturs;

		}

		public Itur GetItur(string iturCode)
		{
			if (string.IsNullOrWhiteSpace(iturCode) == true) return null;

			Tuple<IIturRepository, string> iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return null;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return null;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			Itur itur = itruRepository.GetIturByCode(iturCode, dbPath);
			return itur;
		}

		public Itur GetOrCreateIturByERPIturCode(string erpIturCode)
		{
			if (string.IsNullOrWhiteSpace(erpIturCode) == true) return null;

			Tuple<IIturRepository, string> iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return null;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return null;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			Itur itur = itruRepository.GetIturByErpIturCode(erpIturCode, dbPath, true);
			return itur;
		}

		public bool Insert(Itur itur)
		{
			if (itur == null) return false;

			Tuple<IIturRepository, string> iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return false;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return false;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			try
			{
				itruRepository.Insert(itur, dbPath);
			}
			catch { return false; }
			return true;
		}

		public Itur InsertAndReturn(Itur itur)
		{
			if (itur == null) return null;

			Tuple<IIturRepository, string> iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return null;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return null;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			try
			{
				itruRepository.Insert(itur, dbPath);
			}
			catch { return null; }
			Itur iturreturn = itruRepository.GetIturByCode(itur.IturCode, dbPath);
			return iturreturn;

		}

		public bool Update(Itur itur)
		{
			if (itur == null) return false;

			Tuple<IIturRepository, string> iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return false;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return false;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			try
			{
				itruRepository.Update(itur, dbPath);
			}
			catch { return false; }
			return true;
		}

		public Itur UpdateAndReturn(Itur itur)
		{
			if (itur == null) return null;

			Tuple<IIturRepository, string> iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return null;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return null;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			try
			{
				itruRepository.Update(itur, dbPath);
			}
			catch { return null; }
			Itur iturreturn = itruRepository.GetIturByCode(itur.IturCode, dbPath);
			return iturreturn;

		}

		public bool Delete(string iturCode)
		{
			if (string.IsNullOrWhiteSpace(iturCode) == true) return true;

			Tuple<IIturRepository, string> iturRep = GetIturRepository();
			if (iturRep.Item1 == null) return false;
			if (string.IsNullOrWhiteSpace(iturRep.Item2) == true) return false;

			IIturRepository itruRepository = iturRep.Item1;
			string dbPath = iturRep.Item2;

			try
			{
				itruRepository.Delete(iturCode, dbPath);
			}
			catch { return false; }
			return true;
		}


		private Tuple<IIturRepository, string> GetIturRepository()
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			IIturRepository itruRepository = serviceLocator.GetInstance<IIturRepository>();
			IContextCBIRepository contextCBIRepository = serviceLocator.GetInstance<IContextCBIRepository>();
			AuditConfig mainAuditConfig = contextCBIRepository.GetProcessCBIConfig(CBIContext.History);
			Inventor currentInventor = contextCBIRepository.GetCurrentInventor(mainAuditConfig);
			string dbPath = contextCBIRepository.GetDBPath(currentInventor);
			Tuple<IIturRepository, string> ret = new Tuple<IIturRepository, string>(itruRepository, dbPath);
			return ret;
		}

 	}
}
