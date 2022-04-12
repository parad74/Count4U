using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class IturFromDBParser1 : IIturParser
	{
		private readonly IIturRepository _iturRepository;
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;

		public IturFromDBParser1(IIturRepository iturRepository,
			ILog log)
		{
			if (iturRepository == null) throw new ArgumentNullException("iturRepository");

			this._iturRepository = iturRepository;
			this._log = log;
			this._iturDictionary = new Dictionary<string, Itur>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Itur> IturDictionary
		{
			get { return this._iturDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// Получение списка Product 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, Itur> GetIturs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Itur> iturFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._iturDictionary.Clear();
			this._errorBitList.Clear();

			Iturs itursFromDB = this._iturRepository.GetIturs(fromPathFile);
			foreach (var iturFromDB in itursFromDB)
			{
				if (iturFromDBDictionary.IsDictionaryContainsKey(iturFromDB.IturCode) == false)
				{
					this._iturDictionary[iturFromDB.IturCode] = iturFromDB;
					iturFromDBDictionary[iturFromDB.IturCode] = null;
				}
			}

			return this._iturDictionary;
			
		}

		#region IIturParser Members


		public IEnumerable<Itur> GetItursEnumerable(string fromPathFile, 
			Encoding encoding, string[] separators, int countExcludeFirstString, 
			Dictionary<string, Itur> IturFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._iturDictionary.Clear();
			this._errorBitList.Clear();

			Iturs itursFromDB = this._iturRepository.GetIturs(fromPathFile);
			foreach (var iturFromDB in itursFromDB)
			{
				if (IturFromDBDictionary.IsDictionaryContainsKey(iturFromDB.IturCode) == false)
				{
					iturFromDB.Restore = fromPathFile.CutLength(99);
					iturFromDB.StatusIturBit = 0;
					iturFromDB.StatusDocHeaderBit = 0; 
					iturFromDB.StatusIturGroupBit = 0;
					this._iturDictionary[iturFromDB.IturCode] = iturFromDB;
					IturFromDBDictionary[iturFromDB.IturCode] = null;
				}
			}

			//come to end
			//itursFromDB.ClearIturs(toDBPath);

			foreach (KeyValuePair<string, Itur> keyValueItur in _iturDictionary)	   // источник db3
			{
				yield return keyValueItur.Value;
			}
		}

		#endregion

		#region IIturParser Members


		public List<Location> LocationToDBList
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
