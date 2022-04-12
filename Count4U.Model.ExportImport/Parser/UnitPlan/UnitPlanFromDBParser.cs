using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class UnitPlanFromDBParser : IUnitPlanParser
	{
		private readonly IUnitPlanRepository _unitPlanRepository;
		private readonly ILog _log;
		private Dictionary<string, UnitPlan> _unitPlanDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public UnitPlanFromDBParser(IUnitPlanRepository unitPlanRepository,
			ILog log)
		{
			if (unitPlanRepository == null) throw new ArgumentNullException("unitPlanRepository");

			this._unitPlanRepository = unitPlanRepository;
			this._log = log;
			this._unitPlanDictionary = new Dictionary<string, UnitPlan>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, UnitPlan> UnitPlanDictionary
		{
			get { return this._unitPlanDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, UnitPlan> GetUnitPlans(string fromPathFile,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<string, UnitPlan> unitPlanFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._unitPlanDictionary.Clear();
			this._errorBitList.Clear();

			UnitPlans unitPlansFromDB = this._unitPlanRepository.GetUnitPlans(fromPathFile);
			foreach (var unitPlanFromDB in unitPlansFromDB)
			{
				if (unitPlanFromDBDictionary.ContainsKey(unitPlanFromDB.UnitPlanCode) == false)
				{
					//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
					this._unitPlanDictionary[unitPlanFromDB.UnitPlanCode] = unitPlanFromDB;
					unitPlanFromDBDictionary[unitPlanFromDB.UnitPlanCode] = null;
				}
			}

			//this._locationDictionary = this._locationRepository.GetLocationDictionary(fromPathFile, true);
			return this._unitPlanDictionary;
		}


	}
}
