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
	public class CatalogNativPlusMISSdf2SqliteParser : BaseCatalogSOLiteParser, ICatalogSQLiteParser
	{
		private  IProductRepository _productRepository;

		public CatalogNativPlusMISSdf2SqliteParser(IServiceLocator serviceLocator,
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

	
																																			
			string pathDB = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			
			ISectionRepository sectionRepository = ServiceLocator.GetInstance<ISectionRepository>();
			Dictionary<string, Section> sectionDictionary = new Dictionary<string, Section>();
			sectionDictionary =    sectionRepository.GetSectionDictionary(pathDB, true);

			IFamilyRepository familyRepository = ServiceLocator.GetInstance<IFamilyRepository>();
			Dictionary<string, Family> familyDictionary = new Dictionary<string, Family>();
			familyDictionary = familyRepository.GetFamilyDictionary(pathDB, true);

			ISupplierRepository supplierRepository = ServiceLocator.GetInstance<ISupplierRepository>();
			Dictionary<string, Supplier> supplierDictionary = new Dictionary<string, Supplier>();
			supplierDictionary = supplierRepository.GetSupplierDictionary(pathDB, true);
												
														 
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
			//Every code need to get Type = Q
			productEmpty.ItemType = "Q";
			productEmpty.SectionCode = "-1";
			productEmpty.SectionName = "No Section";
			productEmpty.SubSectionCode = "-1";
			productEmpty.SubSectionName = "No SubSection";
			productEmpty.PriceBuy = "0";
			productEmpty.PriceSell = "0";

			yield return productEmpty;

			IMakatRepository makatRepository = ServiceLocator.GetInstance<IMakatRepository>();
			//Dictionary<string, ProductMakat> productMakatDictionary = new Dictionary<string, ProductMakat>();
			//productMakatDictionary = makatRepository.ProductMakatDictionaryRefill(fromPathFile, true);

			this._productRepository = ServiceLocator.GetInstance<IProductRepository>();
			Products products = this._productRepository.GetProducts(fromPathFile);
			Dictionary<string, Product> productMakatDictionary = products.Select(e => e).Distinct().ToDictionary(x => x.Makat);

			foreach (Product product in products)
			{
				//Field1 goes into Makat		    0
				//Field2 goes into Name			1
				//Field3 goes into ItemType	    2

				if (product == null) continue;


				string makat = product.Makat;
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					continue;
				}
				//if (makat.Contains("888888893172"))
				//{
				//	makat = makat;
				//}

				string parentMakat = product.ParentMakat;
				string typeCode = product.TypeCode;
				string name = product.Name.ReverseDosHebrew(invertLetter, rt2lf);
				string product_SectionCode	= product.SectionCode;
				string product_SectionName = "No Section";
				string product_SubSectionCode = product.SubSectionCode;
				string product_SubSectionName = "No SubSection";
				string product_FamilyCode = product.FamilyCode;
				string product_Family = "No Family";
				string product_SupplierCode = product.SupplierCode;
				string product_SupplierName = "No Supplier";
				//============  Price
				string priceBuy = product.PriceBuy.ToString();
				string priceSell = product.PriceSale.ToString();

				if (typeCode == TypeMakatEnum.B.ToString())
				{
					if (string.IsNullOrWhiteSpace(parentMakat) == false)
					{
						if (productMakatDictionary.ContainsKey(parentMakat) == true)
						{
							Product productMakat = productMakatDictionary[parentMakat];

							name = productMakat.Name.ReverseDosHebrew(invertLetter, rt2lf);
							product_SectionCode = productMakat.SectionCode;
							product_FamilyCode = productMakat.FamilyCode;
							product_SupplierCode = productMakat.SupplierCode;
							product_SubSectionCode = productMakat.SubSectionCode;
							priceBuy = productMakat.PriceBuy.ToString();
							priceSell = productMakat.PriceSale.ToString();

						}
					}
				}

				Catalog newProductSimple = new Catalog();
				//===========Product=======================================
				//Field1 goes into Makat		    0
				//Field2 goes into Name			1
				//Field3 goes into ItemType	    2
		
				newProductSimple.Uid = Guid.NewGuid().ToString();
				newProductSimple.ItemCode = makat;
				
				newProductSimple.ItemName = name;

				newProductSimple.PriceBuy = priceBuy;
				newProductSimple.PriceSell = priceSell;


				newProductSimple.ItemType = "Q";
				if (product.UnitTypeCode == "SN")
				{
					newProductSimple.ItemType = product.UnitTypeCode;
				}

				//================	 Section ============
				newProductSimple.SectionCode = "-1";
				newProductSimple.SectionName = "";
				if (string.IsNullOrWhiteSpace(product_SectionCode) == false)       // не  пустой код
				{
					product_SectionName = "";
					if (product_SectionCode == "none")
					{
						product_SectionName = "";
					}
					else if (sectionDictionary.ContainsKey(product_SectionCode) == true)
					{
						product_SectionName = sectionDictionary[product_SectionCode].Name;
					}
					else
					{
						product_SectionName = product_SectionCode;
					}
		
					newProductSimple.SectionCode = product_SectionCode == "none" ? "" : product_SectionCode;
					newProductSimple.SectionName = product_SectionName;
				}
				
				

				//================	 SubSection ============
				newProductSimple.SubSectionCode = "-1";
				newProductSimple.SubSectionName = "";
				if (string.IsNullOrWhiteSpace(product_SubSectionCode) == false)
				{
					if (sectionDictionary.ContainsKey(product_SubSectionCode) == true)
					{
						product_SubSectionName = sectionDictionary[product_SubSectionCode].Name;
					}
					else
					{
						product_SubSectionName = product_SubSectionCode;
					}
					newProductSimple.SubSectionCode = product_SubSectionCode;
					newProductSimple.SubSectionName = product_SubSectionName;
				}
				

				//newProductSimple.SectionCode = "-1";
				//if (string.IsNullOrWhiteSpace(product.SectionCode) == false) newProductSimple.SectionCode = product.SectionCode;
				//newProductSimple.SectionName = "No Section";
				//if (string.IsNullOrWhiteSpace(product.SectionName) == false) newProductSimple.SectionName = product.SectionName;
				//newProductSimple.SubSectionCode = "-1";
				//if (string.IsNullOrWhiteSpace(product.SubSectionCode) == false) newProductSimple.SubSectionCode = product.SubSectionCode;
				//newProductSimple.SubSectionName = "No SubSection";
				//if (string.IsNullOrWhiteSpace(product.SubSectionName) == false) newProductSimple.SubSectionName = product.SubSectionName;


				//================	 Family ============
				newProductSimple.FamilyCode = "-1";
				newProductSimple.FamilyName = "";
				if (string.IsNullOrWhiteSpace(product_FamilyCode) == false)    // не  пустой код
				{
					product_Family = "";
					if (familyDictionary.ContainsKey(product_FamilyCode) == true)
					{
						product_Family = familyDictionary[product_FamilyCode].Name;
					}
					else
					{
						product_Family = product_FamilyCode;
					}
					newProductSimple.FamilyCode = product_FamilyCode;
					newProductSimple.FamilyName = product_Family;
				}
			
				

				//newProductSimple.FamilyCode = "-1";
				//if (string.IsNullOrWhiteSpace(product.FamilyCode) == false) newProductSimple.FamilyCode = product.FamilyCode;
				//newProductSimple.FamilyName = "No Family";
				//if (string.IsNullOrWhiteSpace(product.Family) == false) newProductSimple.FamilyName = product.Family;


				//================	 Supplier ============
				newProductSimple.SupplierCode = "-1";
				newProductSimple.SupplierName = "";
				if (string.IsNullOrWhiteSpace(product_SupplierCode) == false)     // не  пустой код
				{
					product_SupplierName = "";
					if (product_SupplierCode == "none")
					{
						product_SupplierName = "";
					}
					else	if (supplierDictionary.ContainsKey(product_SupplierCode) == true)
					{
						product_SupplierName = supplierDictionary[product_SupplierCode].Name;
					}
					else
					{
						product_SupplierName = product_SupplierCode;
					}
					newProductSimple.SupplierCode = product_SupplierCode == "none" ? "" : product_SupplierCode;
					newProductSimple.SupplierName = product_SupplierName;
				}


				//newProductSimple.SupplierCode = "-1";
				//if (string.IsNullOrWhiteSpace(product.SupplierCode) == false) newProductSimple.SupplierCode = product.SupplierCode;
				//newProductSimple.SupplierName = "No Supplier";
				//if (string.IsNullOrWhiteSpace(product.SupplierName) == false) newProductSimple.SupplierName = product.SupplierName;


	
				//newProductSimple.SectionCode = "-1";
				//newProductSimple.SectionName = "No Section";
				//newProductSimple.SubSectionCode = "-1";
				//newProductSimple.SubSectionName = "No SubSection";
				//newProductSimple.PriceBuy = "0";
				//newProductSimple.PriceSell = "0";


				//------------
				newProductSimple.Description = typeCode + "|" + parentMakat;
			//	catalogMakatDBDictionary[makat] = null;  
	
				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

			yield return newProductSimple;

			} //foreach
		}

		
	}
}
