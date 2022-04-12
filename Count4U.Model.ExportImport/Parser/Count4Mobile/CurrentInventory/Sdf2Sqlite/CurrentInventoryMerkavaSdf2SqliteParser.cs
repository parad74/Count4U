using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Count4U.Model;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4U;
using Count4U.Model.Common;

namespace Count4U.Model.Count4Mobile
{							  
	public class CurrentInventoryMerkavaSdf2SqliteParser : BaseCatalogSOLiteParser, ICurrentInventorSQLiteParser
	{
		private  IInventProductRepository _inventProductRepository;
		private IIturRepository _iturRepository;
		private const string SerialKey = "SN";
		private const string QuantityKey = "Q";


		public CurrentInventoryMerkavaSdf2SqliteParser(IServiceLocator serviceLocator,
			ILog log)
			: base(serviceLocator, log)
		{
			this.Dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this.Dtfi.ShortTimePattern = @"hh:mm:ss";
  		}

		public Dictionary<string, CurrentInventory> CurrentInventoryDictionary
		{
			get { throw new NotImplementedException(); }
		}

		

		public IEnumerable<CurrentInventory> GetCurrentInventorys(string fromPathFile, 
			Encoding encoding, string[] separators, 
			int countExcludeFirstString, 
			Dictionary<string, string> productMakatDBDictionary, 
			List<ImportDomainEnum> importType, 
			Dictionary<ImportProviderParmEnum, object> parms = null)
	{
			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			base.ErrorBitList.Clear();

			string separator = ",";
			string separatorParms = parms.GetStringValueFromParm(ImportProviderParmEnum.Delimiter);
			if (string.IsNullOrWhiteSpace(separatorParms) == false) separator = separatorParms; 
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}
	
			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool withQuantityERP = parms.GetBoolValueFromParm(ImportProviderParmEnum.WithQuantityERP);
			bool invertLetter = false;
			bool rt2lf = false;
			if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
			{
				invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
				rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
			}

			this._iturRepository = ServiceLocator.GetInstance<IIturRepository>();

			Dictionary<string, Itur> iturDictionary = this._iturRepository.GetIturDictionary(fromPathFile, true);

			this._inventProductRepository = ServiceLocator.GetInstance<IInventProductRepository>();
			foreach (InventProduct inventProduct in this._inventProductRepository.GetInventProducts(fromPathFile))
			{
	

				if (inventProduct == null) continue;


				string makat = inventProduct.Makat;
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					continue;
				}

				CurrentInventory newCurrentInventory = new CurrentInventory();
				//===========Product=======================================

				//SerialNumberLocal				SerialNumberLocal
				//ItemCode								Makat
				//SerialNumberSupplier			SupplierCode  
				//Quantity								QuantityEdit

				//PropertyStr1		IPValueStr1 
				//PropertyStr2		IPValueStr2
				//PropertyStr3		 IPValueStr3 
				//PropertyStr4		 IPValueStr4
				//PropertyStr5		 IPValueStr5
				//PropertyStr6		IPValueStr6 
				//PropertyStr7		IPValueStr7 
				//PropertyStr8		IPValueStr8 
				//PropertyStr9		 IPValueStr9 
				//PropertyStr10		 IPValueStr10 
				//PropertyStr11		IPValueStr11 
				//PropertyStr12		IPValueStr12
				//PropertyStr13		 IPValueStr13 
				//PropertyStr14		 IPValueStr14
				//PropertyStr15		 IPValueStr15
				//PropertyStr16		IPValueStr16 
				//PropertyStr17		IPValueStr17 
				//PropertyStr18		IPValueStr18 
				//PropertyStr19		 IPValueStr19 
				//PropertyStr20		 IPValueStr20 

				
				//LocationCode		  ERPIturCode
				//DateModified	   ?   //DateModified.GetValueOrDefault().Ticks.ToString().GetValueOrDefaultString();
				//DateCreated			?			//DateModified.GetValueOrDefault().Ticks.ToString().GetValueOrDefaultString();
				//ItemStatus			0

				//SerialNumberLocal				SerialNumberLocal
				//ItemCode								Makat
				//SerialNumberSupplier			SupplierCode  
				//Quantity								QuantityEdit
				//LocationCode		  ERPIturCode
				string serialNumber = inventProduct.SerialNumber;
				string locationCode = "-1";//inventProduct.IturCode; //после добавить если нет в словаре
				if (iturDictionary.ContainsKey(inventProduct.IturCode) == true)
				{
					locationCode = iturDictionary[inventProduct.IturCode].ERPIturCode;
				}

				//	 В Меркаве - есть 2 типа айтемов - SN и Q. Так вот только для SN данные храняться в PreviuseInventory. =>
				//Для SN четвертая составляющая ключа везде пустая строка. И в PreviuseInventory и в currentInventory
				//Для айтемов типа "Q". Их нет в PreviuseInventory. Добавляя их в currentInventory четвертая строка в ключе = propertyStrKey8
				//!! считается что с PrevioseInventor приходит только SN и 	   inventProduct.IPValueStr8 должно быть всегда пустым для	  SN
				// поэтому и в каррент инветоре для SN пустая строка четвертая, я для Q   inventProduct.IPValueStr8

				  string[] uids = new string[] { serialNumber, makat, locationCode, inventProduct.IPValueStr8 };	// 4 - составной ключ	 Q
				  if (inventProduct.ImputTypeCodeFromPDA == SerialKey)
				  {
					  uids = new string[] { serialNumber, makat, locationCode, "" };	// 3 - составной ключ для SN
				  }
				string uid = uids.JoinRecord("|");
				newCurrentInventory.Uid = uid.CutLength(249); 
				newCurrentInventory.SerialNumberLocal = serialNumber;
				newCurrentInventory.ItemCode = makat;
				newCurrentInventory.LocationCode = locationCode;

				newCurrentInventory.SerialNumberLocal = inventProduct.SerialNumber;
				newCurrentInventory.SerialNumberSupplier = inventProduct.SupplierCode;
   				int quantity = 0;
				try { quantity = Convert.ToInt32(inventProduct.QuantityEdit); }
				catch { }
				newCurrentInventory.Quantity = quantity.ToString();


				newCurrentInventory.PropertyStr1 = inventProduct.IPValueStr1.CutLength(49);
				newCurrentInventory.PropertyStr2 = inventProduct.IPValueStr2.CutLength(49);
				newCurrentInventory.PropertyStr3 = inventProduct.IPValueStr3.CutLength(49);
				newCurrentInventory.PropertyStr4 = inventProduct.IPValueStr4.CutLength(49);
				newCurrentInventory.PropertyStr5 = inventProduct.IPValueStr5.CutLength(49);
				newCurrentInventory.PropertyStr6 = inventProduct.IPValueStr6.CutLength(49);
				newCurrentInventory.PropertyStr7 = inventProduct.IPValueStr7.CutLength(49);
				newCurrentInventory.PropertyStr8 = inventProduct.IPValueStr8.CutLength(49);
				newCurrentInventory.PropertyStr9 = inventProduct.IPValueStr9.CutLength(49);
				newCurrentInventory.PropertyStr10 = inventProduct.IPValueStr10.CutLength(49);

				newCurrentInventory.PropertyStr11 = inventProduct.IPValueStr11.CutLength(49);
				newCurrentInventory.PropertyStr12 = inventProduct.IPValueStr12.CutLength(49);
				newCurrentInventory.PropertyStr13 = inventProduct.IPValueStr13.CutLength(49);
				newCurrentInventory.PropertyStr14 = inventProduct.IPValueStr14.CutLength(49);
				newCurrentInventory.PropertyStr15 = inventProduct.IPValueStr15.CutLength(49);
				newCurrentInventory.PropertyStr16 = inventProduct.IPValueStr16.CutLength(49);
				newCurrentInventory.PropertyStr17 = inventProduct.IPValueStr17.CutLength(49);
				newCurrentInventory.PropertyStr18 = inventProduct.IPValueStr18.CutLength(49);
				newCurrentInventory.PropertyStr19 = inventProduct.IPValueStr19.CutLength(49);
				newCurrentInventory.PropertyStr20 = inventProduct.IPValueStr20.CutLength(49);

		
				//DateModified	   ?   //DateModified.GetValueOrDefault().Ticks.ToString().GetValueOrDefaultString();
				//DateCreated			?			//DateModified.GetValueOrDefault().Ticks.ToString().GetValueOrDefaultString();
				//ItemStatus			0


				string dateModified = inventProduct.ModifyDate == null ? "" : (Convert.ToDateTime(inventProduct.ModifyDate)).ConvertDateTimeToAndroid();
				newCurrentInventory.DateModified = dateModified;
				string dateCreated = inventProduct.CreateDate == null ? "" : (Convert.ToDateTime(inventProduct.CreateDate)).ConvertDateTimeToAndroid();
				newCurrentInventory.DateCreated = dateCreated;
				newCurrentInventory.ItemStatus = inventProduct.ItemStatus.GetValueOrDefaultString(); 

				newCurrentInventory.UnitTypeCode = inventProduct.ImputTypeCodeFromPDA;
				newCurrentInventory.ItemType = inventProduct.IPValueStr8;
	
				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

			yield return newCurrentInventory;

			} //foreach
		}


		
	}
}
