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
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U
{
	public class ProductCatalogNativMISSqlite2SdfParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogNativMISSqlite2SdfParser(IServiceLocator serviceLocator,
			ILog log)
			: base(serviceLocator, log)
		{
			this.Dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this.Dtfi.ShortTimePattern = @"hh:mm:ss";
  		}

	
		/// <summary>
		/// Получение списка Product
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Product> GetProducts(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, ProductMakat> productMakatDBDictionary,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();

			string separator = ",";
			string separatorParms = parms.GetStringValueFromParm(ImportProviderParmEnum.Delimiter);
			if (string.IsNullOrWhiteSpace(separatorParms) == false) separator = separatorParms; 
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}
	
			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);

			ITemporaryInventoryRepository temporaryInventoryRepository = this._serviceLocator.GetInstance<ITemporaryInventoryRepository>();
			Dictionary<string, TemporaryInventory> dictionaryTemporaryInsertCatalog
			= temporaryInventoryRepository.GetDictionaryTemporaryInventorys(toDBPath, "Product", "INSERT");

			IImportCatalogSQLiteADORepository importCatalogSQLiteADORepository = this._serviceLocator.GetInstance<IImportCatalogSQLiteADORepository>();

		//	Dictionary<string, Catalog> catalogDictionry = importCatalogSQLiteADORepository.GetCatalogMobileDictionary(encoding, fromPathFile);

			foreach (KeyValuePair<string, TemporaryInventory> valueTemporaryInventory in dictionaryTemporaryInsertCatalog)	   // источник db3
			{
				string makat = valueTemporaryInventory.Key;
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, makat));
					continue;
				}

				if (productMakatDBDictionary.ContainsKey(makat) == true)
				{
					base.Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatExistInDB, makat));
					continue;
				}

				//Catalog newCatalog = importCatalogSQLiteADORepository.GetCatalogMobileByItemCode(itemCode, fromPathFile);
				//if (newCatalog == null) continue;
				Catalog newCatalog = new Catalog();
				string tag = valueTemporaryInventory.Value.Tag;
				//ItemType|ItemCode|ItemName
				string[] catalogs = new string[] { "Q", "", "NotExistInCatalog" };
				string[] tags = tag.Split('|');
				int len = 3;
				if (tags.Length < 3) len = tags.Length;
				for (int i = 0; i < len; i++ )
				{
					if (string.IsNullOrWhiteSpace(tags[i]) == false)
					{
						catalogs[i] = tags[i];
					}
				}

				//ItemType|ItemCode|ItemName
				newCatalog.ItemType = catalogs[0];
				newCatalog.ItemName = catalogs[2];

				ProductSimpleString newProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();
				
				//===========Product=======================================
				//ItemType|ItemCode|ItemName
				newProductSimpleString.MakatOriginal = makat;
				newProductSimpleString.Makat = makat;
				string name = newCatalog.ItemName.CutLength(99);
 				newProductSimpleString.Name = name;
				newProductSimpleString.UnitTypeCode = newCatalog.ItemType.Trim().CutLength(49);
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				newProductSimpleString.BalanceQuantityERP = "0";
		
				int retBit = newProductSimple.ValidateError(newProductSimpleString, this.Dtfi);
				if (retBit != 0)  //Error
				{
					base.ErrorBitList.Add(new BitAndRecord
					{
						Bit = retBit,
						Record = makat,
						ErrorType = MessageTypeEnum.Error
					});
					continue;
				}


				retBit = newProductSimple.ValidateWarning(newProductSimpleString, this.Dtfi);
  				if (retBit != 0)
				{
					base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = makat, ErrorType = MessageTypeEnum.WarningParser });
				}

				newProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod; // не уверена, что правильно, и как добавить в каталог
				newProductSimple.InputTypeCode = InputTypeCodeEnum.K.ToString();
		
				productMakatDBDictionary[makat] = new ProductMakat()
				{
					Makat = newProductSimple.Makat,
					UnitTypeCode = newProductSimple.UnitTypeCode,
					//BalanceQuantityERP = newProductSimple.BalanceQuantityERP,
					//MakatOriginal = newProductSimple.MakatOriginal,
					Name = newProductSimple.Name,
					//ParentMakat = newProductSimple.ParentMakat,
					//SectionCode = newProductSimple.SectionCode,
					////SupplierCode = newProductSimple.SupplierCode,
					//TypeCode = newProductSimple.TypeCode
				};

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

			yield return newProductSimple;

			} //foreach
		}
	}
}
