using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Malam;

namespace Count4U.Model.Count4U
{
	public class InventProductToMalamXMLParser : InventProductParserBase, IInventProductToObjectParser
	{
		protected Dictionary<string, string> _iturInFileDictionary; //key IturCode, DocumentCode - для файла
		protected DocumentHeaderString _rowDocumentHeader;

		public InventProductToMalamXMLParser(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			this._iturInFileDictionary = new Dictionary<string, string>();
			this._rowDocumentHeader = new DocumentHeaderString();
		}


		public object GetMyObject(
				string fromPathFile,
				Encoding encoding, string[] separators,
				int countExcludeFirstString,
				List<ImportDomainEnum> importType,
				Dictionary<ImportProviderParmEnum, object> parms = null)
		{

			Records records = new Records();
			string newIturCode = "00010001";
			this._errorBitList.Clear();

			char[] separator = "|".ToCharArray();

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0].ToCharArray();
			}

			//H|000001|4454f65a-2359-4436-9411-f2d2aff41b5|06/07/2015|15:28:18
			//0			1			  2														  3					 4		 
//	Header:
//H								0
//PDA Num					1
//C4U Inventor UID	2
//Date							3
//Time							4
			//B|29270001|A01-01|111|28|M|05/07/2015|22:59:03
			//0   1                 2		 3     4   5     6					7
//Record:
//B																	0
//Field1: Itur Code											1
//Field2: Itur Code ERP									2
//Field3: Code Input from PDA						3
//Field4: Quantity											4
//Field5: Manual\Barcode – How inserted		5
//Field6: Date													6
//Field7: Time													7

//		File sample:
//H|010|202f8bae-866a-4466-879e-f501981b1533|23/04/2017|12:03:34
//B|20160002|999|100001-1|1|K|23/04/2017|12:05:24

//XML fields for first record:
//B|20160002|999|100001-1|1|K|23/04/2017|12:05:24
//ballot_box = 100001
//item_type = 1
//pda_num = 010
//location = 999
//datetime = 23/04/2017 12:05:24


			Records recordList = new Records();
			foreach (String recordString in this._fileParser.GetRecords(fromPathFile, 
				encoding, 
				countExcludeFirstString))
			{
				if (recordString == null) continue;
				string[] record = recordString.Split(separator);

				int countRecord = record.Length;
				if (countRecord < 5)
				{
					Log.Add(MessageTypeEnum.Error,
						String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, recordString));
					continue;
				}

				string objectType = Convert.ToString(record[0]).Trim(" ".ToCharArray());
	
				if (objectType == "H")
				{
					string idPDA = record[1].Trim(" ".ToCharArray());
					this._rowDocumentHeader.Name = idPDA;
					//this._rowDocumentHeader.WorkerGUID = idPDA.PadLeft(9, '0');
					//this._rowDocumentHeader.CreateDate = record[3].Trim(" ".ToCharArray());
					//this._rowDocumentHeader.CreateTime = record[4].Trim(" ".ToCharArray());

				} // "H"


				//===========================InventProduct==============================
				//B|29270001|A01-01|111|28|M|05/07/2015|22:59:03
				//0   1                 2		 3     4   5     6					7
				//Record:
				//B																	0
				//Field1: Itur Code											1
				//Field2: Itur Code ERP									2
				//Field3: Code Input from PDA						3
				//Field4: Quantity											4
				//Field5: Manual\Barcode – How inserted		5
				//Field6: Date													6
				//Field7: Time													7
				else if (objectType == "B")
				{
					if (countRecord < 7)
					{
						Log.Add(MessageTypeEnum.Error,
							String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, recordString));
						continue;
					}
					
					string barcode = Convert.ToString(record[3]).Trim(" ".ToCharArray());
					string[] barcodes = barcode.Split('-');
					string iturERPcode = Convert.ToString(record[2]).Trim(" ".ToCharArray());
					Record newRecord = new Record();
					newRecord.text = recordString;
					newRecord.ballotbox = barcode;
					newRecord.itemtype = "";
					if (barcodes.Length > 1)
					{
						newRecord.ballotbox = barcodes[0];//0
						newRecord.itemtype = barcodes[1];//1
					}
					newRecord.pdanum  =this._rowDocumentHeader.Name ;
					newRecord.location =  iturERPcode;
					newRecord.insertdate =   record[6] + " " +  record[7]; 
					recordList.Add(newRecord);

				}
				else // != 'H' !='B' 
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedMarker, recordString));
					continue;
				}

			} //foreach record from file

			//retObject.records = recordList.ToArray();
			return recordList; 
		
		}



		
	}
}
