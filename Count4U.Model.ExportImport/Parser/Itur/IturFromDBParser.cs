using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;

namespace Count4U.Model.Count4U
{
	public class IturDBParser : IIturParser
	{
		private readonly IIturRepository _iturRepository;
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;

		public IturDBParser(IIturRepository iturRepository,
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
				//if (iturFromDBDictionary.IsDictionaryContainsKey(iturFromDB.IturCode) == false)
				//{
					this._iturDictionary[iturFromDB.IturCode] = iturFromDB;
					//iturFromDBDictionary[iturFromDB.IturCode] = null;
					//this._iturDictionary.AddToDictionary(newItur.Code, newItur, record.JoinRecord(separator), Log);
				//}
			}

			//this._iturDictionary =  this._iturRepository.GetIturDictionary(fromPathFile, true);
			return this._iturDictionary;
			
		}

		#region IIturParser Members


		public IEnumerable<Itur> GetItursEnumerable(string fromPathFile, Encoding encoding, string[] separators, int countExcludeFirstString, Dictionary<string, Itur> IturFromDBDictionary, Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			throw new NotImplementedException();
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
