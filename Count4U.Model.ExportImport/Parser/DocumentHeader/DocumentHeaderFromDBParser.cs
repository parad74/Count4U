using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class DocumentHeaderFromDBParser : IDocumentHeaderParser
	{																														  
		private readonly IDocumentHeaderRepository _documentHeaderRepository;
		private readonly ILog _log;
		private Dictionary<string, DocumentHeader> _documentHeaderDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;

		public DocumentHeaderFromDBParser(IDocumentHeaderRepository documentHeaderRepository,
			ILog log)
		{
			if (documentHeaderRepository == null) throw new ArgumentNullException("DocumentHeaderRepository");

			this._documentHeaderRepository = documentHeaderRepository;
			this._log = log;
			this._documentHeaderDictionary = new Dictionary<string, DocumentHeader>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, DocumentHeader> DocumentHeaderDictionary
		{
			get { return this._documentHeaderDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, DocumentHeader> GetDocumentHeaders(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, DocumentHeader> documentHeaderFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._documentHeaderDictionary.Clear();
			this._errorBitList.Clear();
			bool approve = parms.GetBoolValueFromParm(ImportProviderParmEnum.Approve);
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);

			string suffix = parms.GetStringValueFromParm(ImportProviderParmEnum.Suffix);	

			DocumentHeaders documentHeadersFromDB = this._documentHeaderRepository.GetDocumentHeaders(fromPathFile);
			foreach (var documentHeaderFromDB in documentHeadersFromDB)
			{
				documentHeaderFromDB.DocumentCode = documentHeaderFromDB.DocumentCode + suffix;
				documentHeaderFromDB.Code = documentHeaderFromDB.DocumentCode + suffix;

				if (documentHeaderFromDBDictionary.ContainsKey(documentHeaderFromDB.DocumentCode) == false)
				{
					//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
					documentHeaderFromDB.Restore = fromPathFile.CutLength(99);
					documentHeaderFromDB.SessionCode = newSessionCode;
					documentHeaderFromDB.Approve = approve;
					this._documentHeaderDictionary[documentHeaderFromDB.DocumentCode] = documentHeaderFromDB;
					documentHeaderFromDBDictionary[documentHeaderFromDB.DocumentCode] = null;
				}
			}

	
			return this._documentHeaderDictionary;
		}
	}
}
