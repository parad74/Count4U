
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
using System.Threading;

namespace Count4U.Model.Count4U
{
    public class ProductCatalogGazitVerifoneSteimaztzkyUpdateERPQuentetyDBParser : BaseProductCatalogParser, IProductSimpleParser
    {
        private readonly IProductRepository _productRepository;
        private readonly IImportCatalogADORepository _importCatalogSimpleRepository;

		public ProductCatalogGazitVerifoneSteimaztzkyUpdateERPQuentetyDBParser(IProductRepository productRepository,
            ILog log,
            IServiceLocator serviceLocator)
      		: base(serviceLocator, log)
		{
            if (productRepository == null) throw new ArgumentNullException("productRepository");
            this._productRepository = productRepository;
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

            Dictionary<string, Product> quantityERPDictionary = new Dictionary<string, Product>();

            CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();
            string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string erpCode = parms.GetStringValueFromParm(ImportProviderParmEnum.ERPNum);
			erpCode = erpCode.Trim();
			erpCode = erpCode.TrimStart('0');

            base.SupplierDictionary.Clear();
            base.ErrorBitList.Clear();

            string separator = ";";

            if (separators != null && separators.Count() > 0)
            {
                separator = separators[0];
            }

	           MaskPackage maskPackage = parms.GetMaskPackageFromParms();
            bool makatApplyMask = false;
            bool barcodeApplyMask = false;
            GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);

		

			int i = 0;
			bool ret = false;
			foreach (String row in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				i++;
				if (string.IsNullOrWhiteSpace(row) == true) { continue; }
	
				string[] recordSplit = row.Split('|');
				if (recordSplit == null) continue;

				if (recordSplit.Length < 13)
				{
					continue;
				}

				//Field1: Item Code 				   0
				//Field13: Quantity Expected	   12	   

				String[] record = { "", "", "", "", "", "", "", "", "", "", "", "", ""};

				record[0] =recordSplit[0].Trim(); 
				record[12] =recordSplit[12].Trim(); 				
	
				//string erpCode1 = record[0].Trim().TrimStart('0');
				//if (erpCode1 != erpCode)
				//{
				//	if (ret == false)
				//	{
				//		Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.Warning + " ERPCode ", record.JoinRecord(separator)));
				//	}
				//	ret = true;
				//}

                ProductSimpleString newProductSimpleString = new ProductSimpleString();
                Product newProductSimple = new Product();
                string makat = record[0].Trim();
                if (string.IsNullOrWhiteSpace(makat) == true)
                {
                    Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
                    continue;
               }

				//Field1: Item Code 				   0
				//Field13: Quantity Expected	   12	   

                if (makatApplyMask == true)
                {
                    makat = maskPackage.MakatMaskTemplate.FormatString(
                        makat, maskPackage.MakatMaskRecord.Value);
                }


                if (productMakatDBDictionary.ContainsKey(makat) == false)
                {
                    base.Log.Add(MessageTypeEnum.WarningParser, String.Format(ParserFileErrorMessage.MakatNotExistInDB, record.JoinRecord(separator)));
                    continue;
                }
                //===========Product=======================================
                newProductSimpleString.Makat = makat;
                newProductSimpleString.BalanceQuantityERP = record[12].Trim(' ');

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

                retBit = newProductSimple.ValidateUpdateQuantityERP(newProductSimpleString, this.Dtfi);
                if (retBit != 0)
                {
                    base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
                }

                quantityERPDictionary[makat] = newProductSimple;

            } //foreach


            IEnumerable<Product> productFromDBSimples = this._productRepository.GetProducts(dbPath);
            //this._productRepository.DeleteAll(dbPath);
            IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();
            provider.ClearProducts(dbPath);

            //IImportCatalogSimpleRepository
            foreach (Product productSimple in productFromDBSimples)
            {
                Product ps = new Product();		  //get from updateDictionary ProductSimple
                ProductSimple psPartial = new ProductSimple();		  //get from updateDictionary ProductSimple
                bool exists = quantityERPDictionary.TryGetValue(productSimple.Makat, out ps);	//update Dictionary

                if (exists == true)	   //если в файле апдейта есть Макат текущего  ProductSimple, то мы его обновляем
                {
                    productSimple.BalanceQuantityERP = ps.BalanceQuantityERP;
					//productSimple.PriceBuy = ps.PriceBuy;
					//productSimple.PriceSale = ps.PriceSale;
                    //productSimple.IsUpdateERP = true;
                }
                else	  //если нет - обнуляем количество, цену не трогаем
                {
                    productSimple.BalanceQuantityERP = 0.0;
					//productSimple.PriceBuy = 0.0;
					//productSimple.PriceSale = 0.0;
                    //productSimple.IsUpdateERP = false;
                }

                if (cancellationToken.IsCancellationRequested == true) break;
                k++;
                if (k % 100 == 0) countAction(k);

                yield return productSimple;
            } //foreach

        }


    }
}