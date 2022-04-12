using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	//old
	public class InventProductCsvParser : IInventProductParser
	{
		private readonly IFileParser _fileParser;
		private readonly ILog _log;
		private Dictionary<string, DocumentHeader> _documentHeaderDictionary;
		private Dictionary<string, Session> _sessionDictionary;
		public IServiceLocator _serviceLocator;

		public InventProductCsvParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString());

			this._log = log;
			this._documentHeaderDictionary = new Dictionary<string, DocumentHeader>();
			this._sessionDictionary = new Dictionary<string, Session>();
		}

		public ILog Log
		{
			get { return this._log; }
		} 

		public Dictionary<string, DocumentHeader> DocumentHeaderDictionary
		{
			get { return this._documentHeaderDictionary; }
		}

		public Dictionary<string, Session> SessionDictionary
		{
			get { return this._sessionDictionary; }
		}

		/// <summary>
		/// Получение списка Product для ADO запроса
		/// </summary>
		/// <returns></returns>
		public IEnumerable<InventProduct> GetInventProducts(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString, string sessionCodeIn)
		{
			bool firstStringH = true;
			CultureInfo culture = new CultureInfo("");
			culture.DateTimeFormat.ShortDatePattern = @"DD/MM/yyyy";
			culture.DateTimeFormat.ShortTimePattern = @"hh:mm:ss";
			
			foreach (String[] record in this._fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{
				if (record == null)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, fromPathFile));
					continue;
				}
				string objectType = Convert.ToString(record[0]).Trim(" ".ToCharArray());

				if (objectType == "H")
				{
					if (firstStringH == false) continue;
					if (firstStringH == true)
					{
						firstStringH = false;
						DocumentHeader newDocumentHeader = new DocumentHeader();
						try
						{
							newDocumentHeader.IturCode = Convert.ToString(record[3]).Trim(" ".ToCharArray());
						}
						catch { }

						try
						{
							newDocumentHeader.DocumentCode = Convert.ToString(record[4]).Trim(" ".ToCharArray());
						}
						catch { }

						if (this._documentHeaderDictionary.ContainsKey(newDocumentHeader.DocumentCode) == false)
						{
							this._documentHeaderDictionary.Add(newDocumentHeader.DocumentCode, newDocumentHeader);
						}

					}
				} // "H"

				// ============ InventProduct
				if (objectType == "B")
				{
					InventProduct newInventProduct = new InventProduct();
					try
					{
						newInventProduct.Makat = Convert.ToString(record[1]).Trim(" ".ToCharArray());
						newInventProduct.Barcode = newInventProduct.Makat;
					}
					catch { }
					try
					{
						newInventProduct.QuantityOriginal = Convert.ToDouble((record[2]).Trim(" ".ToCharArray()));
					}
					catch { }

					newInventProduct.Code = (newInventProduct.Makat + "^" + newInventProduct.Barcode).CutLength(299); 
					yield return newInventProduct;
				}
			}
		}
	}
}
