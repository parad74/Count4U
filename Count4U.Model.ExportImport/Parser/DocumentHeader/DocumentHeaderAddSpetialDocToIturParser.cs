using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Count4U.Model.Common;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class DocumentHeaderAddSpetialDocToIturParser : IDocumentHeaderParser
	{																														  
		private readonly IDocumentHeaderRepository _documentHeaderRepository;
		private readonly IIturRepository _iturRepository;
		private readonly IServiceLocator _serviceLocator;
		private readonly ILog _log;
		private Dictionary<string, DocumentHeader> _documentHeaderDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;

		public DocumentHeaderAddSpetialDocToIturParser(IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			IIturRepository iturRepository,
			ILog log)
		{
			if (documentHeaderRepository == null) throw new ArgumentNullException("DocumentHeaderRepository");

			this._documentHeaderRepository = documentHeaderRepository;
			this._iturRepository = iturRepository;
			this._serviceLocator = serviceLocator;

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

			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);

			//Iturs iturs = this._iturRepository.GetIturs(dbPath);
			//List<string> iturDBCodes = iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);

			DocumentHeaders documents = this._documentHeaderRepository.GetDocumentHeaders(dbPath);
			//Dictionary<string, string> iturCodesInDocumentHeaderFromDb =
			//	documents.Select(e => e.IturCode).Distinct().ToDictionary(k => k);

		   	 string[] iturDBCodes = this._iturRepository.GetIturCodes(dbPath);
	
			// добавляем в каждый Itur один DocumentHeader
			foreach (string iturCode in iturDBCodes)
			{
				if (string.IsNullOrWhiteSpace(iturCode) == true) continue;
				//if (iturCodesInDocumentHeaderFromDb.ContainsKey(iturCode) == false)
				//{
					string newDocumentCode = iturCode + "_99999";
					if (documentHeaderFromDBDictionary.ContainsKey(newDocumentCode) == true) continue;

					DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
					DocumentHeader newDocumentHeader = new DocumentHeader();
					newDocumentHeaderString.DocumentCode = newDocumentCode;
					newDocumentHeaderString.SessionCode = newSessionCode;				//in
					newDocumentHeaderString.CreateDate = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
					newDocumentHeaderString.WorkerGUID = "00001";
					newDocumentHeaderString.IturCode = iturCode;
					newDocumentHeaderString.Name = "00001";
					 
					int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
					if (retBitDocumentHeader != 0)  //Error
					{
						continue;
					}
					else //	Error  retBitSession == 0 
					{
						retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
						//iturCodesInDocumentHeaderFromDb[iturCode] = iturCode;
						newDocumentHeader.Approve = null;//false было  //first Document in Itur
						this._documentHeaderDictionary[newDocumentCode] = newDocumentHeader;
						documentHeaderFromDBDictionary[newDocumentCode] = newDocumentHeader;
					}
				//}
			}		 // Add DocumentHeader for Itur
	
			return this._documentHeaderDictionary;
		}
	}
}
