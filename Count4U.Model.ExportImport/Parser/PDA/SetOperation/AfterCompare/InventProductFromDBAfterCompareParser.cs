using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class InventProductFromDBAfterCompareParser : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;
		//private readonly ILog _log;
		//private Dictionary<string, Itur> _iturDictionary;
		//private List<BitAndRecord> _errorBitList;
		//public DateTimeFormatInfo _dtfi;

		public InventProductFromDBAfterCompareParser(
			IInventProductRepository inventProductRepository,
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			if (inventProductRepository == null) throw new ArgumentNullException("iturRepository");

			this._inventProductRepository = inventProductRepository;
		}

		/// <summary>
		/// Получение списка InventProduct  
		/// </summary>
		/// <returns></returns>
		public IEnumerable<InventProduct> GetInventProducts(
		string fromPathFile,
		Encoding encoding, string[] separators,
		int countExcludeFirstString, string sessionCodeIn, //Guid workerGUID,
		Dictionary<string, ProductMakat> productMakatDictionary,
		Dictionary<string, Itur> iturFromDBDictionary,
		List<ImportDomainEnum> importType,
		Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string suffix = parms.GetStringValueFromParm(ImportProviderParmEnum.Suffix);	

			InventProducts inventProductFromDB = this._inventProductRepository.GetInventProducts(fromPathFile);
			string statusInventProductCode = fromPathFile.Replace("Inventor", "").CutLength(49);
			foreach (var newInventProduct in inventProductFromDB)
			{
				newInventProduct.DocumentCode = newInventProduct.DocumentCode + suffix;

				newInventProduct.StatusInventProductCode = statusInventProductCode;

				newInventProduct.SessionCode = newSessionCode;
				newInventProduct.SessionNum = 1; 
				newInventProduct.ModifyDate = DateTime.Now;
				newInventProduct.Code = "";
				newInventProduct.IPValueInt1 = 0;
				newInventProduct.IPValueInt2 = 0;
				newInventProduct.IPValueInt3 = 0;
				newInventProduct.IPValueInt4 = 0;
				newInventProduct.IPValueStr1 = "";
				newInventProduct.IPValueStr2 = "";
				newInventProduct.IPValueStr3 = "";
				newInventProduct.IPValueStr4 = "";
				newInventProduct.IPValueStr5 = "";
				newInventProduct.IPValueStr11 = "";
				newInventProduct.IPValueStr12 = "";
				newInventProduct.IPValueStr13 = "";
				//	newInventProduct.IPValueInt1 = Convert.ToInt32(newInventProduct.QuantityEdit);
				//newInventProduct.QuantityEdit = newInventProduct.QuantityOriginal;
				newInventProduct.QuantityInPackEdit = 1;
				newInventProduct.StatusInventProductBit = 0;

				string barcode = newInventProduct.Barcode;
				string makat = newInventProduct.Makat;
				//if (makat == barcode) newInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
				//else newInventProduct.TypeMakat = TypeMakatEnum.B.ToString();
				//if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
				//{
				//	if (string.IsNullOrWhiteSpace(makat) == true)
				//	{
				//		newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
				//	}
				//}

				if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
				{
					newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
					makat = productMakatDictionary.GetParentMakatFromMakatDictionary(barcode, Log);
					if (string.IsNullOrWhiteSpace(makat) == false)
					{
						ProductMakat newProduct = productMakatDictionary[makat];
						if (makat == barcode) newInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
						else newInventProduct.TypeMakat = TypeMakatEnum.B.ToString();
						newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
						newInventProduct.Makat = makat;
						newInventProduct.ProductName = newProduct.Name;
						newInventProduct.SectionCode = newProduct.SectionCode;
						newInventProduct.ImputTypeCodeFromPDA = newProduct.UnitTypeCode;
						newInventProduct.SupplierCode = newProduct.SupplierCode;
					}
					else
					{	// TODO: проверить
						newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
						newInventProduct.Makat = barcode;
						newInventProduct.ProductName = "NotExistInCatalog";
						//newInventProduct.SectionCode = DomainUnknownCode.UnknownSection;
						newInventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
						newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
					}
				}		//ExistMakat
				else   //Not ExistMakat
				{
					newInventProduct.ProductName = "NotCheckInCatalog";
				}
			
				yield return newInventProduct;
			}
		}
	}
}
