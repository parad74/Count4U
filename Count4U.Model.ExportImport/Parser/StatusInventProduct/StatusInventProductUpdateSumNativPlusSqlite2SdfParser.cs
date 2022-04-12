using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Common;
using Count4U.Model.Count4U.Validate;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U
{
	public class StatusInventProductUpdateSumNativPlusSqlite2SdfParser : InventProductParserBase, IStatusInventProductSimpleParser
	{
		protected Dictionary<string, DocumentHeader> _iturInFileDictionary; //key IturCode, DocumentHeader 
		protected DocumentHeaderString _rowDocumentHeader;

		public StatusInventProductUpdateSumNativPlusSqlite2SdfParser(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base(documentHeaderRepository, serviceLocator, log)
		{
			this._iturInFileDictionary = new Dictionary<string, DocumentHeader>();
			this._rowDocumentHeader = new DocumentHeaderString();
			this._fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
		}


		public IEnumerable<StatusInventProduct> GetStatusInventProducts(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null
		)
		{
			StatusInventProduct newStatusInventProduct = new StatusInventProduct();
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			int filesCount = parms.GetIntValueFromParm(ImportProviderParmEnum.Count);
			if (filesCount == 0 || filesCount == 1)
			{
				yield return newStatusInventProduct;
			}
			else
			{
 				IStatusInventProductRepository _importstatusInventProductRepository = this._serviceLocator.GetInstance<IStatusInventProductRepository>();

				Dictionary<string, StatusInventProduct> dictionrySumQuntety = _importstatusInventProductRepository.GetStatusInventProductSumBitDictionary(dbPath);
				foreach (KeyValuePair<string, StatusInventProduct> keyValuePair in dictionrySumQuntety)
				{
					StatusInventProduct sip = keyValuePair.Value;
					if (sip.Bit == filesCount) continue;			 //есть во всех файлах
					if (sip.Bit == 1) continue;							 //есть в одном файле
					//					this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
					Log.Add(MessageTypeEnum.TraceParser,
						String.Format(ParserFileErrorMessage.BarcodeExistInDB, sip.Code) + "   [" + sip.Bit + "] times");
				}

				yield return newStatusInventProduct;
			}
		}


	}
}
	

