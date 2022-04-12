using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;
using System.IO;
using System.Xml.Linq;

namespace Count4U.Model.Count4U
{
	public class PropertyStrProfileXml2SdfParser : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStrProfileXml2SdfParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._propertyStrDictionary = new Dictionary<string, PropertyStr>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, PropertyStr> PropertyStrDictionary
		{
			get { return this._propertyStrDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, PropertyStr> GetPropertyStrs(string fromPathFile,
			DomainObjectTypeEnum domainObjectType,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, PropertyStr> propertyStrFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{

			this._propertyStrDictionary.Clear();
			this._errorBitList.Clear();

			string separator = "|";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			bool invertLetter = false;
			bool rt2lf = false;
			if (parms != null)
			{
				if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
				{
					invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
					rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
				}
			}

			Dictionary<string, string> mInvToCount4UNameDictionary = FillmInvToCount4UNameDictionary();

			if (File.Exists(fromPathFile) == false)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsNotExist, fromPathFile));
				return this._propertyStrDictionary;
			}
			XDocument profileXDocument = XDocument.Load(fromPathFile);
			if (profileXDocument == null)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.Error, fromPathFile));
				return this._propertyStrDictionary;
			}

			string strtest = profileXDocument.ToString();
			if (string.IsNullOrWhiteSpace(strtest) == true)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, fromPathFile));
				return this._propertyStrDictionary;
			}

			string UIDKey = "";
			XElement proxyXElement = profileXDocument.Descendants("UIDKey").FirstOrDefault();
			if (proxyXElement != null)
			{
				UIDKey = (string)proxyXElement.Value;
			}
			else
			{
				Log.Add(MessageTypeEnum.Warning, String.Format(ParserFileErrorMessage.KeyNotFind, "UIDKey"));
			}

			// <UIDKey>SerialNumberLocal|ItemCode|LocationCode|PropertyStr13</UIDKey>

			string[] UIDKeys = UIDKey.Split(separator[0]);

			//	//XPath				0
			//	//Index 				1
			//	//Property_mINV				2
			//	//Property_Count4U			3

			//	//Name =						//XPath								0		"UIDKey";
			//	//TypeCode = 				//Index 								1
			//	//Code =						//Property_Count4U			2		"Makat";   - fromCount4U
			//	//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
			//	//DomainObject = "Profile";

			Log.Add(MessageTypeEnum.Trace, "Get from Profile UIDKey = " + UIDKey);

			int index = 0;
			foreach (string uid in UIDKeys)
			{

				String[] recordEmpty = { "", "", "", "" };

				PropertyStr newPropertyStr = new PropertyStr();
				PropertyStr propertyStr = new PropertyStr();


				//	//Name =						//XPath								0		"UIDKey";
				//	//TypeCode = 				//Index 								1
				//	//Code =						//Property_Count4U			2		"Makat";   - fromCount4U
				//	//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
				//	//DomainObject = "Profile";


				string xPath = "UIDKey";
				string propertyNameCount4U = "";
				string propertyNameInv = uid.Trim();
				if (string.IsNullOrWhiteSpace(propertyNameInv) == true)
				{
					Log.Add(MessageTypeEnum.Warning, String.Format(ParserFileErrorMessage.KeyIsEmpty, propertyNameInv));
				}
				else 	if (mInvToCount4UNameDictionary.ContainsKey(uid.Trim().ToLower()) == true)	//	propertyNameInv
				{
					propertyNameCount4U = mInvToCount4UNameDictionary[uid.Trim().ToLower()];//TODO
				}
				else
				{
					Log.Add(MessageTypeEnum.Warning, String.Format(ParserFileErrorMessage.KeyNotFind, uid));
				}
			

				propertyStr.Name = xPath; 	 //	 xPath			   "UIDKey";
				propertyStr.TypeCode = index.ToString();//	 Index
				propertyStr.Code = propertyNameCount4U;			//2		"Makat";   - fromCount4U
				propertyStr.PropertyStrCode = propertyNameInv; 	//3		"ItemCode"; - from PDA
				propertyStr.DomainObject = DomainObjectTypeEnum.Profile.ToString();

				index++;

				int retBit = newPropertyStr.ValidateProfileError(propertyStr);
				if (retBit != 0)  //Error
				{
					this._errorBitList.Add(new BitAndRecord
					{
						Bit = retBit,
						Record = uid,
						ErrorType = MessageTypeEnum.Error
					});
					continue;
				}
				else //	Error  retBit == 0 
				{
					retBit = newPropertyStr.ValidateWarning(propertyStr); //Warning
					if (propertyStrFromDBDictionary.ContainsKey(newPropertyStr.PropertyStrCode) == false)
					{
						this._propertyStrDictionary[newPropertyStr.PropertyStrCode] = newPropertyStr;
						propertyStrFromDBDictionary[newPropertyStr.PropertyStrCode] = null;
					}
					if (retBit != 0)
					{
						this._errorBitList.Add(new BitAndRecord
						{
							Bit = retBit,
							Record = uid,
							ErrorType = MessageTypeEnum.WarningParser
						});
					}
				}
			}

			return this._propertyStrDictionary;
		}



		public Dictionary<string, string> FillmInvToCount4UNameDictionary()
		{
			Dictionary<string, string> dictionaryName = new Dictionary<string, string>();

	
			//PropertyStr1		IPValueStr1 
			dictionaryName["PropertyStr1".ToLower()] = "IPValueStr1";
			//PropertyStr2		IPValueStr2
			dictionaryName["PropertyStr2".ToLower()] = "IPValueStr2";
			//PropertyStr3		 IPValueStr3 
			dictionaryName["PropertyStr3".ToLower()] = "IPValueStr3";
			//PropertyStr4		 IPValueStr4
			dictionaryName["PropertyStr4".ToLower()] = "IPValueStr4";
			//PropertyStr5		 IPValueStr5
			dictionaryName["PropertyStr5".ToLower()] = "IPValueStr5";
			//PropertyStr6		IPValueStr6 
			dictionaryName["PropertyStr6".ToLower()] = "IPValueStr6";
			//PropertyStr7		IPValueStr7 
			dictionaryName["PropertyStr7".ToLower()] = "IPValueStr7";
			//PropertyStr8		IPValueStr8 
			dictionaryName["PropertyStr8".ToLower()] = "IPValueStr8";
			//PropertyStr9		 IPValueStr9 
			dictionaryName["PropertyStr9".ToLower()] = "IPValueStr9";
			//PropertyStr10		 IPValueStr10 
			dictionaryName["PropertyStr10".ToLower()] = "IPValueStr10";
			//PropertyStr11		IPValueStr11 
			dictionaryName["PropertyStr11".ToLower()] = "IPValueStr11";
			//PropertyStr12		IPValueStr12
			dictionaryName["PropertyStr12".ToLower()] = "IPValueStr12";
			//PropertyStr13		 IPValueStr13 
			dictionaryName["PropertyStr13".ToLower()] = "IPValueStr13";
			//PropertyStr14		 IPValueStr14
			dictionaryName["PropertyStr14".ToLower()] = "IPValueStr14";
			//PropertyStr15		 IPValueStr15
			dictionaryName["Makat".ToLower()] = "IPValueStr15";
			//PropertyStr16		IPValueStr16 
			dictionaryName["PropertyStr16".ToLower()] = "IPValueStr16";
			//PropertyStr17		IPValueStr17 
			dictionaryName["PropertyStr17".ToLower()] = "IPValueStr17";
			//PropertyStr18		IPValueStr18 
			dictionaryName["PropertyStr18".ToLower()] = "IPValueStr18";
			//PropertyStr19		 IPValueStr19 
			dictionaryName["PropertyStr19".ToLower()] = "IPValueStr19";
			//PropertyStr20		 IPValueStr20 
			dictionaryName["PropertyStr20".ToLower()] = "IPValueStr20";


			//LocationCode		  ERPIturCode
			dictionaryName["LocationCode".ToLower()] = "ERPIturCode";
			//SerialNumberLocal		SerialNumber
			dictionaryName["SerialNumberLocal".ToLower()] = "SerialNumber";
			//ItemCode								Makat
			dictionaryName["ItemCode".ToLower()] = "Makat";
			//SerialNumberSupplier			SupplierCode  
			dictionaryName["SerialNumberSupplier".ToLower()] = "SupplierCode";
			//Quantity								QuantityEdit
			dictionaryName["Quantity".ToLower()] = "QuantityEdit";
			return dictionaryName;
		}
	}
}


