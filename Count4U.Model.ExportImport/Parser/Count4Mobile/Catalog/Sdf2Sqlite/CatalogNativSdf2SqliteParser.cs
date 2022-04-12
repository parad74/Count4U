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

namespace Count4U.Model.Count4Mobile
{
	public class CatalogNativSdf2SqliteParser : BaseCatalogSOLiteParser, ICatalogSQLiteParser
	{
		private  IProductRepository _productRepository;

		public CatalogNativSdf2SqliteParser(IServiceLocator serviceLocator,
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
		public IEnumerable<Catalog> GetCatalogs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> catalogMakatDBDictionary,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{

			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			base.ErrorBitList.Clear();

	
	
			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool withQuantityERP = parms.GetBoolValueFromParm(ImportProviderParmEnum.WithQuantityERP);
			bool invertLetter = false;
			bool rt2lf = false;
			if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
			{
				invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
				rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
			}

			Catalog productEmpty = new Catalog();
			productEmpty.Uid = Guid.NewGuid().ToString();
			productEmpty.ItemCode = "-1";
			productEmpty.ItemName = "Not in Catalog";
			productEmpty.ItemType = "Q";
			productEmpty.SectionCode = "-1";
			productEmpty.SectionName = "No Section";
			productEmpty.SubSectionCode = "-1";
			productEmpty.SubSectionName = "No SubSection";
			productEmpty.PriceBuy = "0";
			productEmpty.PriceSell = "0";

			yield return productEmpty;

			this._productRepository = ServiceLocator.GetInstance<IProductRepository>();
			foreach (Product product in this._productRepository.GetProducts(fromPathFile))
			{
				//Field1 goes into Makat		    0
				//Field2 goes into Name			1
				//Field3 goes into ItemType	    2
				//options
				//	FamilyCode
				//FamilyName 
				//SectionCode
				//SectionName 
				//SubSectionCode 
				//SubSectionName 
				//PriceBuy 
				//PriceSell 
				//SupplierCode 
				//SupplierName

				if (product == null) continue;


				string makat = product.Makat;
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					continue;
				}

				Catalog newProductSimple = new Catalog();
				//===========Product=======================================
				//Field1 goes into Makat		    0
				//Field2 goes into Name			1
				//Field3 goes into ItemType	    2

				newProductSimple.Uid = Guid.NewGuid().ToString();
				newProductSimple.ItemCode = makat;
				string name = product.Name.ReverseDosHebrew(invertLetter, rt2lf);
				newProductSimple.ItemName = name;
				if (string.IsNullOrWhiteSpace(product.UnitTypeCode) == true)
					product.UnitTypeCode = "Q";
				newProductSimple.ItemType = product.UnitTypeCode;

				newProductSimple.SectionCode = "-1";
				if (string.IsNullOrWhiteSpace(product.SectionCode) == false) newProductSimple.SectionCode = product.SectionCode;
				newProductSimple.SectionName = "No Section";
				if (string.IsNullOrWhiteSpace(product.SectionName) == false) newProductSimple.SectionName = product.SectionName;
				newProductSimple.SubSectionCode = "-1";
				if (string.IsNullOrWhiteSpace(product.SubSectionCode) == false) newProductSimple.SubSectionCode = product.SubSectionCode;
				newProductSimple.SubSectionName = "No SubSection";
				if (string.IsNullOrWhiteSpace(product.SubSectionName) == false) newProductSimple.SubSectionName = product.SubSectionName;

				newProductSimple.FamilyCode = "-1";
				if (string.IsNullOrWhiteSpace(product.FamilyCode) == false) newProductSimple.FamilyCode = product.FamilyCode;
				newProductSimple.FamilyName = "No Family";
				if (string.IsNullOrWhiteSpace(product.Family) == false) newProductSimple.FamilyName = product.Family;

				newProductSimple.SupplierCode = "-1";
				if (string.IsNullOrWhiteSpace(product.SupplierCode) == false) newProductSimple.SupplierCode = product.SupplierCode;
				newProductSimple.SupplierName = "No Supplier";
				if (string.IsNullOrWhiteSpace(product.SupplierName) == false) newProductSimple.SupplierName = product.SupplierName;

				newProductSimple.PriceBuy = product.PriceBuy.ToString();
				newProductSimple.PriceSell = product.PriceSale.ToString();

				//	FamilyCode
				//FamilyName 
				//SectionCode
				//SectionName 
				//SubSectionCode 
				//SubSectionName 
				//SupplierCode 
				//SupplierName

			
			//	catalogMakatDBDictionary[makat] = null;  
	
				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

			yield return newProductSimple;

			} //foreach
		}

		
	}
}
