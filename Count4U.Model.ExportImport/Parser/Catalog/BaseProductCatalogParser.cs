using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Count4U.Model;
using Microsoft.Practices.ServiceLocation;


namespace Count4U.Model.Count4U
{
	public abstract class BaseProductCatalogParser
	{
		//protected  IFileParser _fileParser;

		protected readonly ILog _log;
		protected Dictionary<string, ProductSimple> _productDictionary;
		protected Dictionary<string, Supplier> _supplierDictionary;
		protected Dictionary<string, ProductMakat> _productParentMakatDictionary;
		protected List<BitAndRecord> _errorBitList;
		//protected List<ProductSimple> _productSimpleList;
		private DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public BaseProductCatalogParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			//this._fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString());

			this._log = log;
			this._productDictionary = new Dictionary<string, ProductSimple>();
			this._supplierDictionary = new Dictionary<string, Supplier>();
			//this._productSimpleList = new List<ProductSimple>();
			this._productParentMakatDictionary = new Dictionary<string, ProductMakat>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
		}

		protected IServiceLocator ServiceLocator
		{
			get { return _serviceLocator; }
		}

		//protected IFileParser FileParser
		//{
		//	get { return _fileParser; }
		//	set { _fileParser = value; }
		//} 

		protected DateTimeFormatInfo Dtfi
		{
			get { return _dtfi; }
			set { _dtfi = value; }
		}


		public Dictionary<string, ProductSimple> ProductDictionary
		{
			get { return this._productDictionary; }
		}


		public Dictionary<string, Supplier> SupplierDictionary
		{
			get { return this._supplierDictionary; }
		}

		public Dictionary<string, ProductMakat> ProductParentMakatDictionary
		{
			get { return this._productParentMakatDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		//public List<ProductSimple> ProductList
		//    {
		//        get { return this._productSimpleList; }
		//}

		protected static void GetApplyMasks(List<ImportDomainEnum> importType, MaskPackage maskPackage,
		out bool makatApplyMask, out bool barcodeApplyMask)
		{
			makatApplyMask = false;
			barcodeApplyMask = false;

			if (importType.Contains(ImportDomainEnum.MakatApplyMask) == true)
				if (maskPackage.MakatMaskTemplate != null
					&& maskPackage.MakatMaskRecord != null
					&& string.IsNullOrWhiteSpace(maskPackage.MakatMaskRecord.Value) == false)
				{
					makatApplyMask = true;
				}

			if (importType.Contains(ImportDomainEnum.BarcodeApplyMask) == true)
				if (maskPackage.BarcodeMaskTemplate != null
					&& maskPackage.BarcodeMaskRecord != null
					&& string.IsNullOrWhiteSpace(maskPackage.BarcodeMaskRecord.Value) == false)
				{
					barcodeApplyMask = true;
				}
		}

		protected void AddToProductParentMakatDictionary(
			Dictionary<string, ProductMakat> productMakatDictionary,
			string makat, string parentMakat, string makatType,
			string record)
		{
			//Barcode appears more than once on items.asp file: 
			//Ignore the second Barcode, 
			//Keep the Makat on that line (Without the barcode) 
			//write into the log file the line which contains the second appearance of the exists Barcode.

			if (productMakatDictionary.ContainsKey(makat) == true)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.BarcodeExistInDB, record));
			}
			else
			{
				ProductMakat newProductMakat = new ProductMakat();
				newProductMakat.Makat = makat;
				newProductMakat.TypeCode = makatType;
				newProductMakat.ParentMakat = parentMakat;
				newProductMakat.Name = "";
				if (this._productParentMakatDictionary.ContainsKey(makat) == false)
				{
					this._productParentMakatDictionary.Add(makat, newProductMakat);
				}
				productMakatDictionary.Add(makat, newProductMakat);
			}
		}

		protected void AddToProductDictionary(
			Dictionary<string, ProductSimple> productDictionary,
			string makat, ProductSimple productSimple,
			string record)
		{
			if (productDictionary.ContainsKey(makat) == true)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.BarcodeExistInDB, record));
			}
			else
			{
				productDictionary.Add(makat, productSimple);
			}
		}
	}

	
}
