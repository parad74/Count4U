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

namespace Count4U.Model.Count4U
{
	//ВНИМАНИЕ используется в 2 адаптерах
	public class ProductHamarotForGazitVerifoneParser1 : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductHamarotForGazitVerifoneParser1(IServiceLocator serviceLocator,
			ILog log)
			: base(serviceLocator, log)
		{
			base.Dtfi.ShortDatePattern = @"dd/MM/yyyy";
			base.Dtfi.ShortTimePattern = @"hh:mm:ss";
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
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("ExcelFileParser is null");

			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			base.ErrorBitList.Clear();

			string separator = "";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			MaskPackage maskPackage = parms.GetMaskPackageFromParms();

			bool  makatApplyMask = false;
			bool  barcodeApplyMask = false;
		   	GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);
			CatalogParserPoints gazitPoints = parms.GetCatalogParserPointsFromParm();
				
			//Field1: Barcode (col 1-13)
			//Field2: Item Code (col 14-29)

			//{0,13,29};
			//List<Point> startEndSubstring = new List<Point>();
			//startEndSubstring.Add(new Point(0, 12, 13));	 //12 - 0 + 1
			//startEndSubstring.Add(new Point(13, 28, 16));	//28 - 13 + 1

			//{0,13,29};
			List<Point> startEndSubstring = new List<Point>();
			startEndSubstring.Add(new Point
			{
				Start = gazitPoints.HamarotBarcodeStart,
				End = gazitPoints.HamarotBarcodeEnd,
				Length = gazitPoints.HamarotBarcodeEnd - gazitPoints.HamarotBarcodeStart  + 1
			});	//Point( 0, 12, 13));	 //12 - 0 + 1
			startEndSubstring.Add(new Point
			{
				Start = gazitPoints.HamarotItemCodeStart,
				End = gazitPoints.HamarotItemCodeEnd,
				Length = gazitPoints.HamarotItemCodeEnd - gazitPoints.HamarotItemCodeStart	+ 1
			});	  //Point( 13, 28, 16));	//28 - 13 + 1
			int count = 2;

			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }

				int len = rec.Length;

				//if (len < 29)
				//{
				if (len < gazitPoints.HamarotItemCodeEnd)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedLengthString, rec));
					continue;
				}

				String[] record = {"", ""};

			
				for (int i = 0; i < count; i++)
				{
					if (startEndSubstring[i].End < len)
					{
						record[i] = rec.Substring(startEndSubstring[i].Start, startEndSubstring[i].Length);
					}
				}

				ProductSimpleString newProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();
				//0						1
				//barcode	          makat
				//0098096672003218870-7        *//23
			
				string makat = record[1].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					base.Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.ParentMakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}

				newProductSimpleString.MakatOriginal = makat;

				if (makatApplyMask == true)
				{
					makat = maskPackage.MakatMaskTemplate.FormatString(
						makat, maskPackage.MakatMaskRecord.Value);
				}

				//ParentMakat Not Ecxist in DB
				if (productMakatDBDictionary.ContainsKey(makat) == false)
				{
					newProductSimpleString.Makat = makat;
					newProductSimpleString.Name = "Inserted from Hamarot File";
					newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
					newProductSimpleString.ParentMakat = "";
					newProductSimpleString.PriceString = "0.0";
					newProductSimpleString.PriceSale = "0.0";
					newProductSimpleString.SupplierCode = "";

					int retBit = newProductSimple.ValidateError(newProductSimpleString, this.Dtfi);
					if (retBit != 0)  //Error
					{
						base.ErrorBitList.Add(new BitAndRecord
						{
							Bit = retBit,
							Record = record.JoinRecord(separator),
							ErrorType = MessageTypeEnum.Error
						});
						continue;
					}

					retBit = newProductSimple.ValidateWarning(newProductSimpleString, this.Dtfi);

					if (retBit != 0)
					{
						base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
					}

					productMakatDBDictionary[makat] = null;	//newProductSimple.Makat
					//new ProductMakat(								  
					//{
					//    Makat = newProductSimple.Makat,
					//    Name = "Inserted from Hamarot File",
					//    ParentMakat = "",
					//    MakatOriginal = newProductSimple.MakatOriginal,
					//    TypeCode = TypeMakatEnum.M.ToString()
					//});

					newProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogBarcodeWithoutMakatParent;
					newProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();

					if (cancellationToken.IsCancellationRequested == true) break;
					k++;
					if (k % 100 == 0) countAction(k);

					yield return newProductSimple;
				}
			} //foreach
		}
	
	}
}
