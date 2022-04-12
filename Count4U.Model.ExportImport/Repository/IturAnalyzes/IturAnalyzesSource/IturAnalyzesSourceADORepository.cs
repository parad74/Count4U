using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using System.Data.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.SelectionParams;
using NLog;
using System.Diagnostics;
using System.Threading;

namespace Count4U.Model.Count4U
{
	public class IturAnalyzesSourceADORepository : BaseADORepository, IIturAnalyzesCaseSourceRepository
	{
		private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public IturAnalyzesSourceADORepository(
			IConnectionADO connection,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository)
			: base(connection, log, serviceLocator)
        {
			this._makatRepository = makatRepository;
		
	    }

		public void InsertIturAnalyzes(string pathDB, bool refill = true, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null)
		{
			if (selectParms != null)
			{
				string selectParmsString = selectParms.ToString();
			}
			if (refill == true)
			{
				this.ClearIturAnalyzes(pathDB);
			}
			Action<long> countAction = null;
			CancellationToken cancellationToken = CancellationToken.None;
			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
 			if (parmsIn != null)
			{
				parms = parmsIn.ConvertToImportProviderParmEnum();
				cancellationToken = parms.GetCancellationTokenFromParm();
			}
			if (cancellationToken == CancellationToken.None)	//throw new ArgumentNullException("CancellationToken.None");
			{
				var c = new CancellationTokenSource();
				cancellationToken = c.Token;
			}
 			countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}
		

			IIturAnalyzesRepository iturAnalyzesRepository =
				this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			Dictionary<string, Itur> iturDictionary = iturAnalyzesRepository.GetIturDictionary(pathDB, refill);
			Dictionary<string, DocumentHeader> documentHeaderDictionary = iturAnalyzesRepository.GetDocumentHeaderDictionary(pathDB, refill);
			Dictionary<string, Location> locationDictionary = iturAnalyzesRepository.GetLocationDictionary(pathDB, refill);
			ISectionRepository sectionRepository = this._serviceLocator.GetInstance<ISectionRepository>();
			Dictionary<string, Section> sectionFromDBDictionary = sectionRepository.GetSectionDictionary(pathDB, true);
			//Dictionary<int, IturStatusEnum> statusIturDictionary = iturAnalyzesRepository.GetStatusIturDictionary();
			//Dictionary<int, IturStatusGroupEnum> statusIturGroupDictionary = iturAnalyzesRepository.GetStatusIturGroupDictionary();
			Dictionary<string, ProductMakat> productMakatDictionary = iturAnalyzesRepository.GetProductMakatDictionary(pathDB, refill);
			Dictionary<string, ProductSimple>  productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refill );

			this.Log.Add(MessageTypeEnum.TraceRepository, "IturAnalyzesADORepository");
			string sql1 = "INSERT INTO [IturAnalyzes] ( " +
			"[Code], " +
		   "[LocationCode], " +
	       "[DocumentHeaderCode], " +
			"[DocumentCode], " +
			"[IturCode], " +
			"[ERPIturCode], " +
		   "[Itur_Disabled], " +
		   "[Itur_Publishe], " +
		   "[Itur_StatusIturBit], " +
		   "[Itur_Number], " +
		   "[Itur_NumberPrefix], " +
		   "[Itur_NumberSufix], " +
		   "[Itur_LocationCode], " +
		   "[Itur_StatusIturGroupBit], " +
		   "[Itur_StatusDocHeaderBit], " +
		   "[Doc_Name], " +
		   "[Doc_Approve], " +
		   //"[Doc_IturCode], " +
		   "[Doc_WorkerGUID], " +
		   "[Doc_StatusDocHeaderBit], " +
		   "[Doc_StatusInventProductBit], " +
		   "[Doc_StatusApproveBit], " +
		   "[DocNum], " +
		   "[Barcode], " +
           "[QuantityDifference], " +
           "[QuantityEdit], " +
		   "[QuantityInPackEdit]," +
           "[QuantityOriginal], " +
		   //"[SerialNumber], " +
		   //"[ShelfCode], " +
           "[ModifyDate], " +
           "[ProductName], " +
           "[PDA_StatusInventProductBit], " +
           //"[DocumentHeaderCode], " +
           "[Makat], " +
           "[InputTypeCode], " +
		   "[IPNum]," +
		   "[BarcodeOriginal]," +
           "[MakatOriginal]," +
           "[PriceString]," +
           "[PriceBuy]," +
           "[PriceSale]," +
		   "[PriceExtra]," +
		   "[FromCatalogType]," +
	       "[SectionNum]," +
		   "[TypeCode]," +
		   "[TypeMakat]," +
		   "[ValueBuyQriginal]," +
		   "[ValueBuyEdit]," +
		   "[ValueBuyDifference]," +
		   "[PDA_ID]," +
		   "[SectionCode], " +
		   "[SectionName], " +
		   "[IsUpdateERP], " +
		   "[ValueFloat]," +
		   "[QuantityOriginalERP], " +
		   "[CountInParentPack], " +
		   "[BalanceQuantityPartialERP], " +
		   "[ValueOriginalERP], " +
		   "[QuantityDifferenceOriginalERP], " +
		   "[ValueDifferenceOriginalERP]," +
		   //"[SupplierCode], " +
		   //"[SupplierName], " +
		   "[LocationName] " +
		   //"[IturName], " +
		   //"[SessionCode], " +
		   //"[SessionNum]" +
			") " + 
     "VALUES("  +
			 "@Code, " +
		   "@LocationCode, " +
           "@DocumentHeaderCode, " +
			"@DocumentCode, " +
		   "@IturCode, " +
		   "@ERPIturCode, " +
		   "@Itur_Disabled, " +
		   "@Itur_Publishe, " +
		   "@Itur_StatusIturBit, " +
		   "@Itur_Number, " +
		   "@Itur_NumberPrefix, " +
		   "@Itur_NumberSufix, " +
		   "@Itur_LocationCode, " +
		   "@Itur_StatusIturGroupBit, " +
		   "@Itur_StatusDocHeaderBit, " +
		   "@Doc_Name, " +
		   "@Doc_Approve, " +
		   //"@Doc_IturCode, " +
		   "@Doc_WorkerGUID, " +
		   "@Doc_StatusDocHeaderBit, " +
		   "@Doc_StatusInventProductBit, " +
		   "@Doc_StatusApproveBit, " +
			"@DocNum, " +
           "@Barcode, " +
		   "@QuantityDifference, " +
		   "@QuantityEdit, "  +
		   "@QuantityInPackEdit, " +
           "@QuantityOriginal, " +
		   //"@SerialNumber, " +
		   //"@ShelfCode, " +
           "@ModifyDate, " +
           "@ProductName, " +
           "@PDA_StatusInventProductBit, " +
           //"@PDA_DocumentHeaderCode	, " +
           "@Makat, " +
		   "@InputTypeCode, " +
           "@IPNum, " +
		   "@BarcodeOriginal," +
           "@MakatOriginal," +
           "@PriceString," +
           "@PriceBuy," +
           "@PriceSale," +
		   "@PriceExtra," +
		   "@FromCatalogType," +
		   "@SectionNum," +
		   "@TypeCode," +
			"@TypeMakat," +
		   "@ValueBuyQriginal," +
			"@ValueBuyEdit," +
			"@ValueBuyDifference," +
			"@PDA_ID," +
			"@SectionCode," +
			"@SectionName," +
			"@IsUpdateERP," +
			"@ValueFloat," +
			"@QuantityOriginalERP," +
			"@CountInParentPack," +
			"@BalanceQuantityPartialERP, " +
			"@ValueOriginalERP," +
			"@QuantityDifferenceOriginalERP," +
			"@ValueDifferenceOriginalERP," +
			//"@SupplierCode," +
			//"@SupplierName," +
			"@LocationName" +
			//"@IturName," +
			//"@SessionCode," +
			//"@SessionNum" +
			")";
			SqlCeTransaction tran = null;

			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
			int k = 0;

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();

				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				cmd.Parameters.Add(new SqlCeParameter(
					"@Code", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@LocationCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@DocumentHeaderCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@DocumentCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@IturCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ERPIturCode", SqlDbType.NVarChar, 50));
	//====================Itur==================================	
	       		cmd.Parameters.Add(new SqlCeParameter(		   //?
					"@Itur_Disabled", SqlDbType.Bit));
			 	cmd.Parameters.Add(new SqlCeParameter(
					"@Itur_Publishe", SqlDbType.Bit));		 //?
				cmd.Parameters.Add(new SqlCeParameter(
					"@Itur_StatusIturBit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Itur_Number", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Itur_NumberPrefix", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Itur_NumberSufix", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Itur_LocationCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Itur_StatusIturGroupBit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Itur_StatusDocHeaderBit", SqlDbType.Int));
 //=================== Doc ====================================
		   		cmd.Parameters.Add(new SqlCeParameter(
					"@Doc_Name", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(		   //?
					"@Doc_Approve", SqlDbType.Bit));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Doc_StatusDocHeaderBit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Doc_StatusInventProductBit", SqlDbType.Int, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Doc_StatusApproveBit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Doc_WorkerGUID", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_IturCode", SqlDbType.NVarChar));
				cmd.Parameters.Add(new SqlCeParameter(
					"@DocNum", SqlDbType.BigInt));
//=================== PDA ====================================
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_DocumentHeaderCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Makat", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Barcode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ProductName", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityEdit", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@QuantityInPackEdit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityDifference", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@IsUpdateERP", SqlDbType.Bit));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@ModifyDate", SqlDbType.DateTime));
				cmd.Parameters.Add(new SqlCeParameter(
					"@InputTypeCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PDA_StatusInventProductBit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@IPNum", SqlDbType.BigInt));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FromCatalogType", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionNum", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@TypeCode", SqlDbType.NVarChar, 10));
				cmd.Parameters.Add(new SqlCeParameter(
					"@TypeMakat", SqlDbType.NVarChar, 10));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyQriginal", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyEdit", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyDifference", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PDA_ID", SqlDbType.BigInt));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueFloat", SqlDbType.Float));  //  QuantityDifference из IP
				
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_ShelfCode", SqlDbType.NVarChar, 10));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_SerialNumber", SqlDbType.NVarChar, 50));
			//============Product ===================
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityOriginal", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
				 "@BarcodeOriginal", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
				 "@MakatOriginal", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceString", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceBuy", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceSale", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceExtra", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionName", SqlDbType.NVarChar, 100));

				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityOriginalERP", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@CountInParentPack", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@BalanceQuantityPartialERP", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@ValueOriginalERP", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityDifferenceOriginalERP", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@ValueDifferenceOriginalERP", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@SupplierCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@SupplierName", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@LocationName", SqlDbType.NVarChar, 100));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@IturName", SqlDbType.NVarChar, 100));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@SessionCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@SessionNum", SqlDbType.Int));
	

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));

				InventProducts inventProductList = null;
				
				if (selectParms == null)
				{
					selectParms = new SelectParams();
				}
				
				
				if (selectParms.FilterStringListParams.ContainsKey("TypeMakat") == false)
				{
					List<string> typeMakatList = new List<string>();
					typeMakatList.Add(TypeMakatEnum.M.ToString());
					typeMakatList.Add(TypeMakatEnum.B.ToString());
					typeMakatList.Add(TypeMakatEnum.W.ToString());
					selectParms.FilterStringListParams.Add("TypeMakat", new FilterStringListParam()
					{
						Values = typeMakatList
					});
				}

				//
				//if (exportProviderEnum == ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider1)
				//{
				//    typeMakatList.Add(TypeMakatEnum.W.ToString());
				//}
				//else
				//{
				//    typeMakatList.Add(TypeMakatEnum.M.ToString());
				//    typeMakatList.Add(TypeMakatEnum.B.ToString());		//??
				//    typeMakatList.Add(TypeMakatEnum.W.ToString());
				//}
				//selectParams.FilterStringListParams.Add("TypeMakat", new FilterStringListParam()
				//{
				//    Values = typeMakatList
				//});
				//

				try
				{
					inventProductList = iturAnalyzesRepository.
						GetInventProductList(selectParms, pathDB);
				}
				catch { }

				if (inventProductList == null)
				{
					return;
					//inventProductList = iturAnalyzesRepository.
					//      GetInventProductList(null, pathDB);
				}

				foreach (InventProduct inventProduct in inventProductList)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
						k++;
						//==================== PDA =========================================
						cmd.Parameters["@DocumentHeaderCode"].Value = inventProduct.DocumentHeaderCode;
						cmd.Parameters["@DocumentCode"].Value = inventProduct.DocumentCode;
						cmd.Parameters["@IturCode"].Value = inventProduct.IturCode;
						cmd.Parameters["@Barcode"].Value = inventProduct.Barcode;
						//cmd.Parameters["@QuantityDifference"].Value =  inventProduct.QuantityEdit - inventProduct.QuantityOriginal;
						cmd.Parameters["@ValueFloat"].Value = inventProduct.QuantityOriginal - inventProduct.QuantityEdit;

						double quantityEdit = inventProduct.QuantityEdit != null ? Convert.ToDouble(inventProduct.QuantityEdit) : 0;
						double quantityOriginal = inventProduct.QuantityOriginal != null ? Convert.ToDouble(inventProduct.QuantityOriginal) : 0;
						int quantityInPackEdit = inventProduct.QuantityInPackEdit != null ? Convert.ToInt32(inventProduct.QuantityInPackEdit) : 0;

						cmd.Parameters["@QuantityEdit"].Value = quantityEdit;// +quantityInPackEdit;	//?? +	 inventProduct.QuantityInPackEdit
						cmd.Parameters["@QuantityInPackEdit"].Value = quantityInPackEdit;// inventProduct.QuantityInPackEdit;
						cmd.Parameters["@QuantityOriginal"].Value = quantityOriginal;
						cmd.Parameters["@QuantityDifference"].Value = quantityEdit- quantityOriginal;

						cmd.Parameters["@ValueBuyQriginal"].Value = 0;
						cmd.Parameters["@ValueBuyEdit"].Value = 0;
						cmd.Parameters["@ValueBuyDifference"].Value = 0;
	
						cmd.Parameters["@IsUpdateERP"].Value = false;
						//cmd.Parameters["@QuantityOriginal"].Value = inventProduct.QuantityOriginal;
						//cmd.Parameters["@SerialNumber"].Value = inventProduct.SerialNumber != null ? inventProduct.SerialNumber : "";
						//cmd.Parameters["@ShelfCode"].Value = inventProduct.ShelfCode != null ? inventProduct.ShelfCode : "";
						cmd.Parameters["@ModifyDate"].Value = inventProduct.ModifyDate;
						cmd.Parameters["@ProductName"].Value = inventProduct.ProductName;
						cmd.Parameters["@PDA_StatusInventProductBit"].Value = inventProduct.StatusInventProductBit;
						//cmd.Parameters["@PDA_DocumentHeaderCode"].Value = inventProduct.DocumentHeaderCode;
						cmd.Parameters["@Makat"].Value = inventProduct.Makat;
						cmd.Parameters["@InputTypeCode"].Value = inventProduct.InputTypeCode;
						cmd.Parameters["@IPNum"].Value = inventProduct.IPNum;
						cmd.Parameters["@PDA_ID"].Value = inventProduct.ID;
					   //===========Catalog =============
					
						cmd.Parameters["@MakatOriginal"].Value = inventProduct.Makat;			 //W 
						cmd.Parameters["@BarcodeOriginal"].Value = inventProduct.Barcode;	  //W
						cmd.Parameters["@PriceString"].Value ="";
						cmd.Parameters["@PriceBuy"].Value = 0;
						cmd.Parameters["@PriceSale"].Value = 0;
						cmd.Parameters["@PriceExtra"].Value = 0;
						cmd.Parameters["@CountInParentPack"].Value = 1;
						cmd.Parameters["@SectionCode"].Value = DomainUnknownCode.UnknownSection;
						cmd.Parameters["@SectionName"].Value = DomainUnknownName.UnknownSection;
						cmd.Parameters["@FromCatalogType"].Value = inventProduct.FromCatalogType;
						cmd.Parameters["@SectionNum"].Value = inventProduct.SectionNum != null ? inventProduct.SectionNum : 0;
						cmd.Parameters["@TypeCode"].Value = "";
						cmd.Parameters["@TypeMakat"].Value = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";

						cmd.Parameters["@QuantityOriginalERP"].Value = 0;
						cmd.Parameters["@BalanceQuantityPartialERP"].Value = 0;
						cmd.Parameters["@QuantityDifferenceOriginalERP"].Value = 0;
						cmd.Parameters["@ValueOriginalERP"].Value = 0;
						cmd.Parameters["@ValueDifferenceOriginalERP"].Value = 0;

		
					if (productMakatDictionary.ContainsKey(inventProduct.Makat) == true)
						{
							ProductMakat productMakat = productMakatDictionary[inventProduct.Makat];
							cmd.Parameters["@MakatOriginal"].Value = productMakat.MakatOriginal;
							//cmd.Parameters["@BarcodeOriginal"].Value = "";// productMakat.MakatOriginal;//?
							cmd.Parameters["@TypeCode"].Value = productMakat.TypeCode;
						}

						if (String.IsNullOrWhiteSpace(inventProduct.Barcode) == false
							&& inventProduct.Barcode != inventProduct.Makat)
						{
							if (productMakatDictionary.ContainsKey(inventProduct.Barcode) == true)
							{
								ProductMakat productMakat = productMakatDictionary[inventProduct.Barcode];
								cmd.Parameters["@BarcodeOriginal"].Value = productMakat.MakatOriginal;
								cmd.Parameters["@TypeCode"].Value = productMakat.TypeCode;
							}
						}
						if (productSimpleDictionary.ContainsKey(inventProduct.Makat) == true)
						{
							ProductSimple productSimple = productSimpleDictionary[inventProduct.Makat];
							//double quantityOriginalERP = productSimple.BalanceQuantityERP != null ? Convert.ToDouble(productSimple.BalanceQuantityERP) : 0;
							double priceBuy = productSimple.PriceBuy != null ? Convert.ToDouble(productSimple.PriceBuy) : 0;
							cmd.Parameters["@FromCatalogType"].Value = productSimple.FromCatalogType;
							cmd.Parameters["@PriceString"].Value = productSimple.PriceString;
							cmd.Parameters["@PriceBuy"].Value = priceBuy;
							int countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;
							cmd.Parameters["@CountInParentPack"].Value = countInParentPack;
							double quantityEditAndPartial = quantityEdit;
							if(countInParentPack != 0) quantityEditAndPartial = quantityEdit + quantityInPackEdit / countInParentPack;
							cmd.Parameters["@QuantityEdit"].Value = quantityEditAndPartial;
							//cmd.Parameters["@QuantityOriginal"].Value = productSimple.BalanceQuantityERP;		//??
							cmd.Parameters["@IsUpdateERP"].Value = productSimple.IsUpdateERP;
							//cmd.Parameters["@QuantityDifference"].Value = inventProduct.QuantityEdit - inventProduct.QuantityOriginal ;
							cmd.Parameters["@ValueBuyQriginal"].Value = quantityOriginal * priceBuy;
							double valueEditAndPartial = quantityEditAndPartial * priceBuy;
							cmd.Parameters["@ValueBuyEdit"].Value = valueEditAndPartial; //quantityEdit * priceBuy;

							//-------- new 23.04.2013
							double balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP;	   
							if (countInParentPack != 0) balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP +
								productSimple.BalanceQuantityPartialERP / countInParentPack;
							cmd.Parameters["@QuantityOriginalERP"].Value = balanceQuantityERPAndPartial;//balanceQuantityERP;

							double valueBuyQriginalERP = priceBuy * balanceQuantityERPAndPartial;
							cmd.Parameters["@ValueOriginalERP"].Value = valueBuyQriginalERP;
							//-------------

							string sectionCode = string.IsNullOrWhiteSpace(productSimple.SectionCode) == false ? productSimple.SectionCode : DomainUnknownCode.UnknownSection;
							cmd.Parameters["@SectionCode"].Value = sectionCode;
							if (sectionFromDBDictionary.ContainsKey(sectionCode) == true
							&& sectionFromDBDictionary[sectionCode] != null)
							{
								Section section = sectionFromDBDictionary[sectionCode];
								cmd.Parameters["@SectionName"].Value = section.Name;
							}

							//cmd.Parameters["@ValueOriginalERP"].Value = quantityOriginalERP * priceBuy;
							//cmd.Parameters["@QuantityOriginalERP"].Value = quantityOriginalERP;
							//cmd.Parameters["@QuantityDifferenceOriginalERP"].Value = quantityEdit- quantityOriginalERP;
							//cmd.Parameters["@ValueDifferenceOriginalERP"].Value = 0;
						}
									   
					   //===================== Doc ========================================
						if (documentHeaderDictionary.ContainsKey(inventProduct.DocumentCode) == true)
						{
							DocumentHeader docHeader = documentHeaderDictionary[inventProduct.DocumentCode];
							string iturCode = docHeader.IturCode != null ? docHeader.IturCode : "";
							cmd.Parameters["@Code"].Value = iturCode;
							cmd.Parameters["@Doc_Name"].Value = docHeader.Name != null ? docHeader.Name : "";
							cmd.Parameters["@Doc_Approve"].Value = docHeader.Approve != null ? docHeader.Approve : false;
							//cmd.Parameters["@Doc_IturCode"].Value = docHeader.IturCode;
							cmd.Parameters["@Doc_WorkerGUID"].Value = docHeader.WorkerGUID != null ? docHeader.WorkerGUID : "";
							cmd.Parameters["@Doc_StatusDocHeaderBit"].Value = docHeader.StatusDocHeaderBit;
							cmd.Parameters["@Doc_StatusInventProductBit"].Value = docHeader.StatusInventProductBit;
							cmd.Parameters["@Doc_StatusApproveBit"].Value = docHeader.StatusApproveBit;
							cmd.Parameters["@DocNum"].Value = docHeader.ID;
							//==================== Itur ==================================
							if (iturDictionary.ContainsKey(iturCode) == true 
								&& iturDictionary[iturCode] != null)
							{
								Itur itur = iturDictionary[iturCode];
								string locationCode = itur.LocationCode != null ? itur.LocationCode : DomainUnknownCode.UnknownLocation;
								cmd.Parameters["@ERPIturCode"].Value = itur.ERPIturCode;
								cmd.Parameters["@LocationCode"].Value =locationCode;
								cmd.Parameters["@LocationName"].Value = locationCode;
								cmd.Parameters["@Itur_Disabled"].Value = itur.Disabled != null ? itur.Disabled : false;
								cmd.Parameters["@Itur_Publishe"].Value = itur.Publishe != null ? itur.Publishe : false;
								cmd.Parameters["@Itur_StatusIturBit"].Value = itur.StatusIturBit != null ? itur.StatusIturBit : 0;
								cmd.Parameters["@Itur_Number"].Value = itur.Number != null ? itur.Number : 0;
								cmd.Parameters["@Itur_NumberPrefix"].Value = itur.NumberPrefix != null ? itur.NumberPrefix : "";
								cmd.Parameters["@Itur_NumberSufix"].Value = itur.NumberSufix != null ? itur.NumberSufix : "";
								cmd.Parameters["@Itur_LocationCode"].Value = locationCode;
								cmd.Parameters["@Itur_StatusIturGroupBit"].Value = itur.StatusIturGroupBit != null ? itur.StatusIturGroupBit : 0;
								cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = itur.StatusDocHeaderBit != null ? itur.StatusDocHeaderBit : 0;
								//cmd.Parameters["@SectionCode"].Value = DomainUnknownCode.UnknownSection;
								if (locationDictionary.ContainsKey(locationCode) == true
								&& locationDictionary[locationCode] != null)
								{
									Location location = locationDictionary[locationCode];
									cmd.Parameters["@LocationName"].Value = location.Name;
								}
						}
							else
							{
								cmd.Parameters["@ERPIturCode"].Value = "";
								cmd.Parameters["@LocationCode"].Value = DomainUnknownCode.UnknownLocation;
								cmd.Parameters["@Itur_Disabled"].Value = false;
								cmd.Parameters["@Itur_Publishe"].Value = false;
								cmd.Parameters["@Itur_StatusIturBit"].Value = 0;
								cmd.Parameters["@Itur_Number"].Value = 0;
								cmd.Parameters["@Itur_NumberPrefix"].Value = "";
								cmd.Parameters["@Itur_NumberSufix"].Value = "";
								cmd.Parameters["@Itur_LocationCode"].Value = DomainUnknownCode.UnknownLocation;
								//cmd.Parameters["@SectionCode"].Value = DomainUnknownCode.UnknownSection;
								cmd.Parameters["@Itur_StatusIturGroupBit"].Value = 0;
								cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = 0;
								cmd.Parameters["@LocationName"].Value = DomainUnknownName.UnknownLocation;
							}
						}
						else
						{
							cmd.Parameters["@Code"].Value = "None";
							cmd.Parameters["@Doc_Name"].Value = "";
							cmd.Parameters["@Doc_Approve"].Value = false;
							//cmd.Parameters["@Doc_IturCode"].Value = "";
							cmd.Parameters["@Doc_WorkerGUID"].Value = "";
							cmd.Parameters["@Doc_StatusDocHeaderBit"].Value = 0;
							cmd.Parameters["@Doc_StatusInventProductBit"].Value = 0;
							cmd.Parameters["@Doc_StatusApproveBit"].Value = 0;
							cmd.Parameters["@DocNum"].Value = 0;
							cmd.Parameters["@ERPIturCode"].Value = "";
							cmd.Parameters["@LocationCode"].Value = DomainUnknownCode.UnknownLocation;
							cmd.Parameters["@Itur_Disabled"].Value = false;
							cmd.Parameters["@Itur_Publishe"].Value = false;
							cmd.Parameters["@Itur_StatusIturBit"].Value = 0;
							cmd.Parameters["@Itur_Number"].Value = 0;
							cmd.Parameters["@Itur_NumberPrefix"].Value = "";
							cmd.Parameters["@Itur_NumberSufix"].Value = "";
							cmd.Parameters["@Itur_LocationCode"].Value = DomainUnknownCode.UnknownLocation;
							cmd.Parameters["@Itur_StatusIturGroupBit"].Value = 0;
							cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = 0;
							//cmd.Parameters["@SectionCode"].Value =DomainUnknownCode.UnknownSection;
							cmd.Parameters["@LocationName"].Value = DomainUnknownName.UnknownLocation;
						}
						cmd.ExecuteNonQuery();
					}
				if (cancellationToken.IsCancellationRequested == false)
				{
					tran.Commit();
				}
				else
				{
					tran.Rollback();
					inventProductList = null;
				}
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "IturAnalyzes", pathDB));
			}
			catch (Exception error)
			{
				_logger.ErrorException("InsertIturAnalyzes", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}

			iturDictionary.Clear();
			documentHeaderDictionary.Clear(); 
			locationDictionary.Clear(); 
			sectionFromDBDictionary.Clear(); 
			productMakatDictionary.Clear();
			productSimpleDictionary.Clear();
			GC.Collect();
		}



		public void ClearIturAnalyzes(string pathDB)
		{
			//this.Log.Clear();
			string sql1 = "DELETE FROM  [IturAnalyzes]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Product", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "IturAnalyzes"));
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "IturAnalyzes"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearIturAnalyzes", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			//LogPrint();
		}

		public void InsertIturAnalyzesSimple(string pathDB, bool refill = true, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null)
		{
			if (refill == true)
			{
				this.ClearIturAnalyzes(pathDB);
			}

			Action<long> countAction = null;
			CancellationToken cancellationToken = CancellationToken.None;
			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
			if (parmsIn != null)
			{
				parms = parmsIn.ConvertToImportProviderParmEnum();
				cancellationToken = parms.GetCancellationTokenFromParm();
			}
			if (cancellationToken == CancellationToken.None)	//throw new ArgumentNullException("CancellationToken.None");
			{
				var c = new CancellationTokenSource();
				cancellationToken = c.Token;
			}
			countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}

			IIturAnalyzesRepository iturAnalyzesRepository =
				this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			//Dictionary<string, Itur> iturDictionary = iturAnalyzesRepository.GetIturDictionary(pathDB, refill);
			//Dictionary<string, DocumentHeader> documentHeaderDictionary = iturAnalyzesRepository.GetDocumentHeaderDictionary(pathDB, refill);
			//Dictionary<string, ProductMakat> productMakatDictionary = iturAnalyzesRepository.GetProductMakatDictionary(pathDB, refill);
			Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refill);

			this.Log.Add(MessageTypeEnum.TraceRepository, "IturAnalyzesADORepository");
			string sql1 = "INSERT INTO [IturAnalyzes] ( " +
		   // "[Code], " +
		   //"[LocationCode], " +
		   //"[DocumentHeaderCode], " +
			"[DocumentCode], " +
			"[IturCode], " +
			//"[ERPIturCode], " +
		   //"[Itur_Disabled], " +
		   //"[Itur_Publishe], " +
		   //"[Itur_StatusIturBit], " +
		   //"[Itur_Number], " +
		   //"[Itur_NumberPrefix], " +
		   //"[Itur_NumberSufix], " +
		   //"[Itur_LocationCode], " +
		   //"[Itur_StatusIturGroupBit], " +
		   //"[Itur_StatusDocHeaderBit], " +
		   //"[Doc_Name], " +
		   //"[Doc_Approve], " +
				//"[Doc_IturCode], " +
		   //"[Doc_WorkerGUID], " +
		   //"[Doc_StatusDocHeaderBit], " +
		   //"[Doc_StatusInventProductBit], " +
		   //"[Doc_StatusApproveBit], " +
		   //"[DocNum], " +
		   "[Barcode], " +
		   "[QuantityDifference], " +
		   "[QuantityEdit], " +
		   "[QuantityInPackEdit]," +
		   "[QuantityOriginal], " +
				//"[SerialNumber], " +
				//"[ShelfCode], " +
		   //"[ModifyDate], " +
		   "[ProductName], " +
		   //"[PDA_StatusInventProductBit], " +
				//"[DocumentHeaderCode], " +
		   "[Makat], " +
		   "[InputTypeCode], " +
		   "[IPNum]," +
		   //[BarcodeOriginal]," +
		   //"[MakatOriginal]," +
		   //"[PriceString]," +
		   "[PriceBuy]," +
		   //"[PriceSale]," +
		   //"[PriceExtra]," +
		   "[FromCatalogType]," +
		   "[SectionCode]," +
		   //"[SectionNum]," +
		   //"[TypeCode]," +
		   "[TypeMakat]," +
		   "[ValueBuyQriginal]," +
		   "[ValueBuyEdit]," +
		   "[ValueBuyDifference]" +
		   //"[PDA_ID]" +
			") " +
	 "VALUES(" +
			 //"@Code, " +
		   //"@LocationCode, " +
		   //"@DocumentHeaderCode, " +
			"@DocumentCode, " +
		   "@IturCode, " +
		  // "@ERPIturCode, " +
		   //"@Itur_Disabled, " +
		   //"@Itur_Publishe, " +
		   //"@Itur_StatusIturBit, " +
		   //"@Itur_Number, " +
		   //"@Itur_NumberPrefix, " +
		   //"@Itur_NumberSufix, " +
		   //"@Itur_LocationCode, " +
		   //"@Itur_StatusIturGroupBit, " +
		   //"@Itur_StatusDocHeaderBit, " +
		   //"@Doc_Name, " +
		   //"@Doc_Approve, " +
				//"@Doc_IturCode, " +
		   //"@Doc_WorkerGUID, " +
		   //"@Doc_StatusDocHeaderBit, " +
		   //"@Doc_StatusInventProductBit, " +
		   //"@Doc_StatusApproveBit, " +
		   // "@DocNum, " +
		   "@Barcode, " +
		   "@QuantityDifference, " +
		   "@QuantityEdit, " +
		   "@QuantityInPackEdit," +
		   "@QuantityOriginal, " +
				//"@SerialNumber, " +
				//"@ShelfCode, " +
		   //"@ModifyDate, " +
		   "@ProductName, " +
		   //"@PDA_StatusInventProductBit, " +
				//"@PDA_DocumentHeaderCode	, " +
		   "@Makat, " +
		   "@InputTypeCode, " +
		   "@IPNum, " +
		   //"@BarcodeOriginal," +
		   //"@MakatOriginal," +
		   //"@PriceString," +
		   "@PriceBuy," +
		   //"@PriceSale," +
		   //"@PriceExtra," +
		   "@FromCatalogType," +
		   "@SectionCode," +
		   //"@SectionNum," +
		   //"@TypeCode," +
			"@TypeMakat," +
		   "@ValueBuyQriginal," +
			"@ValueBuyEdit," +
			"@ValueBuyDifference" +
			//"@PDA_ID" +
			")";

			SqlCeTransaction tran = null;

			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
			int k = 0;

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();

				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Code", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@LocationCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@DocumentHeaderCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@DocumentCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@IturCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@ERPIturCode", SqlDbType.NVarChar, 50));
				//====================Itur==================================	
				//cmd.Parameters.Add(new SqlCeParameter(		   //?
				//    "@Itur_Disabled", SqlDbType.Bit));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_Publishe", SqlDbType.Bit));		 //?
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_StatusIturBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_Number", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_NumberPrefix", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_NumberSufix", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_LocationCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_StatusIturGroupBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_StatusDocHeaderBit", SqlDbType.Int));
				////=================== Doc ====================================
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_Name", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(		   //?
				//    "@Doc_Approve", SqlDbType.Bit));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_StatusDocHeaderBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_StatusInventProductBit", SqlDbType.Int, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_StatusApproveBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_WorkerGUID", SqlDbType.NVarChar, 50));
				////cmd.Parameters.Add(new SqlCeParameter(
				////    "@Doc_IturCode", SqlDbType.NVarChar));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@DocNum", SqlDbType.BigInt));
				////=================== PDA ====================================
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_DocumentHeaderCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Makat", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Barcode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ProductName", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
				     "@QuantityOriginal", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityEdit", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityInPackEdit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
				     "@QuantityDifference", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//     "@ModifyDate", SqlDbType.DateTime));
				cmd.Parameters.Add(new SqlCeParameter(
					"@InputTypeCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_StatusInventProductBit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@IPNum", SqlDbType.BigInt));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FromCatalogType", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@SectionNum", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@TypeCode", SqlDbType.NVarChar, 10));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionCode", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@TypeMakat", SqlDbType.NVarChar, 10));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyQriginal", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyEdit", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyDifference", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_ID", SqlDbType.BigInt));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_ShelfCode", SqlDbType.NVarChar, 10));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_SerialNumber", SqlDbType.NVarChar, 50));
				//============Product ===================
				//cmd.Parameters.Add(new SqlCeParameter(
				// "@BarcodeOriginal", SqlDbType.NVarChar, 100));
				//cmd.Parameters.Add(new SqlCeParameter(
				// "@MakatOriginal", SqlDbType.NVarChar, 100));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PriceString", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceBuy", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PriceSale", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PriceExtra", SqlDbType.Float));

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));

				InventProducts inventProductList = null;
				if (selectParms == null)
				{
					selectParms = new SelectParams();
				}
				if (selectParms.FilterStringListParams.ContainsKey("TypeMakat") == false)
				{
					List<string> typeMakatList = new List<string>();
					typeMakatList.Add(TypeMakatEnum.M.ToString());
					typeMakatList.Add(TypeMakatEnum.B.ToString());
					typeMakatList.Add(TypeMakatEnum.W.ToString());
					selectParms.FilterStringListParams.Add("TypeMakat", new FilterStringListParam()
					{
						Values = typeMakatList
					});
				}

				try
				{
					inventProductList = iturAnalyzesRepository.
						GetInventProductList(selectParms, pathDB);
				}
				catch { }

				if (inventProductList == null)
				{
					return;
					//inventProductList = iturAnalyzesRepository.
					//      GetInventProductList(null, pathDB);
				}

				foreach (InventProduct inventProduct in inventProductList)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}

					k++;
					//==================== PDA =========================================
					//cmd.Parameters["@DocumentHeaderCode"].Value = inventProduct.DocumentHeaderCode;
					//string locationCode = "";
					//if (iturDictionary.ContainsKey(inventProduct.IturCode) == true)
					//{
					//    Itur itur = iturDictionary[inventProduct.IturCode];
					//    locationCode = itur.IturCode;
					//}
					cmd.Parameters["@SectionCode"].Value = DomainUnknownCode.UnknownSection;
					cmd.Parameters["@DocumentCode"].Value = inventProduct.DocumentCode;
					cmd.Parameters["@IturCode"].Value = inventProduct.IturCode;
					//cmd.Parameters["@ERPIturCode"].Value = "";		//TODO ?
					//cmd.Parameters["@LocationCode"].Value = locationCode;
					cmd.Parameters["@Barcode"].Value = inventProduct.Barcode;
					cmd.Parameters["@QuantityEdit"].Value = inventProduct.QuantityEdit;// +inventProduct.QuantityInPackEdit;
					cmd.Parameters["@QuantityInPackEdit"].Value = inventProduct.QuantityInPackEdit;
					cmd.Parameters["@QuantityDifference"].Value = inventProduct.QuantityEdit - inventProduct.QuantityOriginal;
					cmd.Parameters["@QuantityOriginal"].Value = inventProduct.QuantityOriginal;
					//cmd.Parameters["@SerialNumber"].Value = inventProduct.SerialNumber != null ? inventProduct.SerialNumber : "";
					//cmd.Parameters["@ShelfCode"].Value = inventProduct.ShelfCode != null ? inventProduct.ShelfCode : "";
					//cmd.Parameters["@ModifyDate"].Value = inventProduct.ModifyDate;
					cmd.Parameters["@ProductName"].Value = inventProduct.ProductName;
					//cmd.Parameters["@PDA_StatusInventProductBit"].Value = inventProduct.StatusInventProductBit;
					//cmd.Parameters["@PDA_DocumentHeaderCode"].Value = inventProduct.DocumentHeaderCode;
					cmd.Parameters["@Makat"].Value = inventProduct.Makat;
					cmd.Parameters["@InputTypeCode"].Value = inventProduct.InputTypeCode;
					cmd.Parameters["@IPNum"].Value = inventProduct.IPNum;
					//cmd.Parameters["@PDA_ID"].Value = inventProduct.ID;
					//===========Catalog =============

					//cmd.Parameters["@MakatOriginal"].Value = inventProduct.Makat;			 //W 
					//cmd.Parameters["@BarcodeOriginal"].Value = inventProduct.Barcode;	  //W
					//cmd.Parameters["@PriceString"].Value = "";
					cmd.Parameters["@PriceBuy"].Value = 0;
					//cmd.Parameters["@PriceSale"].Value = 0;
					//cmd.Parameters["@PriceExtra"].Value = 0;
					cmd.Parameters["@ValueBuyQriginal"].Value = 0;
					cmd.Parameters["@ValueBuyEdit"].Value = 0;
					cmd.Parameters["@ValueBuyDifference"].Value = 0;
					cmd.Parameters["@FromCatalogType"].Value = inventProduct.FromCatalogType;
					//cmd.Parameters["@SectionNum"].Value = inventProduct.SectionNum != null ? inventProduct.SectionNum : 0;
					//cmd.Parameters["@TypeCode"].Value = "";
					cmd.Parameters["@TypeMakat"].Value = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";

					//if (productMakatDictionary.ContainsKey(inventProduct.Makat) == true)
					//{
					//    ProductMakat productMakat = productMakatDictionary[inventProduct.Makat];
					//    cmd.Parameters["@MakatOriginal"].Value = productMakat.MakatOriginal;
					//    cmd.Parameters["@BarcodeOriginal"].Value = productMakat.MakatOriginal;//?
					//    cmd.Parameters["@TypeCode"].Value = productMakat.TypeCode;
					//}
					//if (inventProduct.Barcode != inventProduct.Makat)
					//{
					//    if (productMakatDictionary.ContainsKey(inventProduct.Barcode) == true)
					//    {
					//        ProductMakat productMakat = productMakatDictionary[inventProduct.Barcode];
					//        cmd.Parameters["@BarcodeOriginal"].Value = productMakat.MakatOriginal;
					//        cmd.Parameters["@TypeCode"].Value = productMakat.TypeCode;
					//    }
					//}
					if (productSimpleDictionary.ContainsKey(inventProduct.Makat) == true)
					{
						ProductSimple productSimple = productSimpleDictionary[inventProduct.Makat];
						cmd.Parameters["@FromCatalogType"].Value = productSimple.FromCatalogType;
						//cmd.Parameters["@PriceString"].Value = productSimple.PriceString;
						cmd.Parameters["@PriceBuy"].Value = productSimple.PriceBuy;
						//cmd.Parameters["@ValueBuyQriginal"].Value = productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityOriginal);
						int countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;
						double quantityEditAndPartial = inventProduct.QuantityEdit;
						if (countInParentPack != 0) quantityEditAndPartial = inventProduct.QuantityEdit + inventProduct.QuantityInPackEdit / countInParentPack;
						cmd.Parameters["@QuantityEdit"].Value = quantityEditAndPartial;
						cmd.Parameters["@ValueBuyEdit"].Value = productSimple.PriceBuy * quantityEditAndPartial;// productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityEdit);

						cmd.Parameters["@QuantityDifference"].Value = quantityEditAndPartial - inventProduct.QuantityOriginal;
														//inventProduct.QuantityEdit - inventProduct.QuantityOriginal;
						//cmd.Parameters["@ValueBuyDifference"].Value = productSimple.PriceBuy *
						//    (Convert.ToDouble(inventProduct.QuantityOriginal) - Convert.ToDouble(inventProduct.QuantityEdit));
					}

					//===================== Doc ========================================
					//if (documentHeaderDictionary.ContainsKey(inventProduct.DocumentCode) == true)
					//{
					//    DocumentHeader docHeader = documentHeaderDictionary[inventProduct.DocumentCode];
					//    cmd.Parameters["@Code"].Value = docHeader.IturCode;
					//    cmd.Parameters["@Doc_Name"].Value = docHeader.Name != null ? docHeader.Name : "";
					//    cmd.Parameters["@Doc_Approve"].Value = docHeader.Approve != null ? docHeader.Approve : false;
					//    //cmd.Parameters["@Doc_IturCode"].Value = docHeader.IturCode;
					//    cmd.Parameters["@Doc_WorkerGUID"].Value = docHeader.WorkerGUID != null ? docHeader.WorkerGUID : "";
					//    cmd.Parameters["@Doc_StatusDocHeaderBit"].Value = docHeader.StatusDocHeaderBit;
					//    cmd.Parameters["@Doc_StatusInventProductBit"].Value = docHeader.StatusInventProductBit;
					//    cmd.Parameters["@Doc_StatusApproveBit"].Value = docHeader.StatusApproveBit;
					//    cmd.Parameters["@DocNum"].Value = docHeader.ID;
						//==================== Itur ==================================
						//if (iturDictionary.ContainsKey(docHeader.IturCode) == true)
						//{
						//    Itur itur = iturDictionary[docHeader.IturCode];
						//    cmd.Parameters["@LocationCode"].Value = itur.LocationCode;
						//    cmd.Parameters["@Itur_Disabled"].Value = itur.Disabled != null ? itur.Disabled : false;
						//    cmd.Parameters["@Itur_Publishe"].Value = itur.Publishe != null ? itur.Publishe : false;
						//    cmd.Parameters["@Itur_StatusIturBit"].Value = itur.StatusIturBit;
						//    cmd.Parameters["@Itur_Number"].Value = itur.Number;
						//    cmd.Parameters["@Itur_NumberPrefix"].Value = itur.NumberPrefix;
						//    cmd.Parameters["@Itur_NumberSufix"].Value = itur.NumberSufix;
						//    cmd.Parameters["@Itur_LocationCode"].Value = itur.LocationCode;
						//    cmd.Parameters["@Itur_StatusIturGroupBit"].Value = itur.StatusIturGroupBit;
						//    cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = itur.StatusDocHeaderBit;
						//}
					//}
					//else
					//{
					//    cmd.Parameters["@Code"].Value = "";
					//    cmd.Parameters["@Doc_Name"].Value = "";
					//    cmd.Parameters["@Doc_Approve"].Value = false;
					//    //cmd.Parameters["@Doc_IturCode"].Value = "";
					//    cmd.Parameters["@Doc_WorkerGUID"].Value = "";
					//    cmd.Parameters["@Doc_StatusDocHeaderBit"].Value = 0;
					//    cmd.Parameters["@Doc_StatusInventProductBit"].Value = 0;
					//    cmd.Parameters["@Doc_StatusApproveBit"].Value = 0;
					//    cmd.Parameters["@DocNum"].Value = 0;
					//    cmd.Parameters["@LocationCode"].Value = "";
					//    cmd.Parameters["@Itur_Disabled"].Value = false;
					//    cmd.Parameters["@Itur_Publishe"].Value = false;
					//    cmd.Parameters["@Itur_StatusIturBit"].Value = 0;
					//    cmd.Parameters["@Itur_Number"].Value = 0;
					//    cmd.Parameters["@Itur_NumberPrefix"].Value = "";
					//    cmd.Parameters["@Itur_NumberSufix"].Value = "";
					//    cmd.Parameters["@Itur_LocationCode"].Value = "";
					//    cmd.Parameters["@Itur_StatusIturGroupBit"].Value = 0;
					//    cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = 0;
					//}
					cmd.ExecuteNonQuery();
				}
				if (cancellationToken.IsCancellationRequested == false)
				{
					tran.Commit();
				}
				else
				{
					tran.Rollback();
				}
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "IturAnalyzes", pathDB));
			}
			catch (Exception error)
			{
				_logger.ErrorException("InsertIturAnalyzesSimple", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);

				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			productSimpleDictionary.Clear();
			GC.Collect();
		}


		public void InsertIturAnalyzesSumSimple(string pathDB, bool refill = true, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, bool addResult = true)
		{
			if (refill == true)
			{
				this.ClearIturAnalyzes(pathDB);
			}

			Action<long> countAction = null;
			CancellationToken cancellationToken = CancellationToken.None;
			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
			if (parmsIn != null)
			{
				parms = parmsIn.ConvertToImportProviderParmEnum();
				cancellationToken = parms.GetCancellationTokenFromParm();
			}
			if (cancellationToken == CancellationToken.None)	//throw new ArgumentNullException("CancellationToken.None");
			{
				var c = new CancellationTokenSource();
				cancellationToken = c.Token;
			}
			countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}

			Stopwatch stopwatch;
			IIturAnalyzesRepository iturAnalyzesRepository =  this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			ISectionRepository sectionRepository = this._serviceLocator.GetInstance<ISectionRepository>();
			ISupplierRepository supplierRepository = this._serviceLocator.GetInstance<ISupplierRepository>();
			Dictionary<string, Section> sectionFromDBDictionary = sectionRepository.GetSectionDictionary(pathDB, true);
			Dictionary<string, Supplier> supplierFromDBDictionary = supplierRepository.GetSupplierDictionary(pathDB, true);

			//Dictionary<string, Itur> iturDictionary = iturAnalyzesRepository.GetIturDictionary(pathDB, refill);
			//Dictionary<string, DocumentHeader> documentHeaderDictionary = iturAnalyzesRepository.GetDocumentHeaderDictionary(pathDB, refill);
			//Dictionary<string, ProductMakat> productMakatDictionary = iturAnalyzesRepository.GetProductMakatDictionary(pathDB, refill);

			if (selectParms == null)
			{
				selectParms = new SelectParams();
			}
			if (selectParms.FilterStringListParams.ContainsKey("TypeMakat") == false)
			{
				List<string> typeMakatList = new List<string>();
				typeMakatList.Add(TypeMakatEnum.M.ToString());
				typeMakatList.Add(TypeMakatEnum.B.ToString());
				typeMakatList.Add(TypeMakatEnum.W.ToString());
				selectParms.FilterStringListParams.Add("TypeMakat", new FilterStringListParam()
				{
					Values = typeMakatList
				});
			}

			stopwatch = Stopwatch.StartNew();
			IEnumerable<IturAnalyzesSimple> inventProductList = null;
			try
			{
				//inventProductList = iturAnalyzesRepository.GetIPSumQuantityByMakats(selectParms, pathDB, true);  test
				inventProductList = iturAnalyzesRepository.GetIPSumQuantityEditDifferenceByMakats(selectParms, pathDB);  
			}
			catch { }
			stopwatch.Stop();
			System.Diagnostics.Debug.Print(String.Format("GetIPSumQuantityEditDifferenceByMakats: {0}", stopwatch.ElapsedTicks.ToString()));

			//if (inventProductList == null)
			//{
			//    inventProductList = iturAnalyzesRepository.GetIPSumQuantityByMakats(null, pathDB, true);
			//}

  			//this.ClearIturAnalyzes(pathDB);

			stopwatch = Stopwatch.StartNew();
			Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refill);
			this.ClearIturAnalyzes(pathDB);
			stopwatch.Stop();
			System.Diagnostics.Debug.Print(String.Format("GetProductSimpleDictionary: {0}", stopwatch.ElapsedTicks.ToString()));

			this.Log.Add(MessageTypeEnum.TraceRepository, "IturAnalyzesADORepository");
			string sql1 = "INSERT INTO [IturAnalyzes] ( " +
				// "[Code], " +
				//"[LocationCode], " +
				//"[DocumentHeaderCode], " +
			//"[DocumentCode], " +
			//"[IturCode], " +
				//"[Itur_Disabled], " +
				//"[Itur_Publishe], " +
				//"[Itur_StatusIturBit], " +
				//"[Itur_Number], " +
				//"[Itur_NumberPrefix], " +
				//"[Itur_NumberSufix], " +
				//"[Itur_LocationCode], " +
				//"[Itur_StatusIturGroupBit], " +
				//"[Itur_StatusDocHeaderBit], " +
				//"[Doc_Name], " +
				//"[Doc_Approve], " +
				//"[Doc_IturCode], " +
				//"[Doc_WorkerGUID], " +
				//"[Doc_StatusDocHeaderBit], " +
				//"[Doc_StatusInventProductBit], " +
				//"[Doc_StatusApproveBit], " +
				//"[DocNum], " +
		    //"[Barcode], " +
			"[QuantityDifference], " +
		    "[QuantityEdit], " +
			"[QuantityInPackEdit]," +
			"[QuantityOriginal], " +
				//"[SerialNumber], " +
				//"[ShelfCode], " +
				//"[ModifyDate], " +
		   "[ProductName], " +
				//"[PDA_StatusInventProductBit], " +
				//"[DocumentHeaderCode], " +
		   "[Makat], " +
		   //"[InputTypeCode], " +
		   //"[IPNum]," +
			//"[BarcodeOriginal]," +
			//"[MakatOriginal]," +
				//"[PriceString]," +
		    "[PriceBuy]," +
			"[PriceSale]," +
				//"[PriceExtra]," +
			"[FromCatalogType]," +
			"[SectionCode]," +
			"[SectionName]," +
			//"[SectionNum]," +	
				//"[TypeCode]," +
			"[TypeMakat]," +
			"[ValueBuyQriginal]," +
			"[ValueBuyEdit]," +
			"[ValueBuyDifference]," +
			"[Count]," +
			"[ResultCode]," +
			"[IsUpdateERP]," +
		   "[QuantityOriginalERP], " +
		   "[CountInParentPack], " +
		   "[BalanceQuantityPartialERP], " +
		   "[ValueOriginalERP], " +
		   "[QuantityDifferenceOriginalERP], " +
		   "[ValueDifferenceOriginalERP], " +
		   "[SupplierCode], " +
			"[SupplierName] " +
			//"[PDA_ID]" +
			") " +
	 "VALUES(" +
				//"@Code, " +
				//"@LocationCode, " +
				//"@DocumentHeaderCode, " +
			//"@DocumentCode, " +
			//"@IturCode, " +
				//"@Itur_Disabled, " +
				//"@Itur_Publishe, " +
				//"@Itur_StatusIturBit, " +
				//"@Itur_Number, " +
				//"@Itur_NumberPrefix, " +
				//"@Itur_NumberSufix, " +
				//"@Itur_LocationCode, " +
				//"@Itur_StatusIturGroupBit, " +
				//"@Itur_StatusDocHeaderBit, " +
				//"@Doc_Name, " +
				//"@Doc_Approve, " +
				//"@Doc_IturCode, " +
				//"@Doc_WorkerGUID, " +
				//"@Doc_StatusDocHeaderBit, " +
				//"@Doc_StatusInventProductBit, " +
				//"@Doc_StatusApproveBit, " +
				// "@DocNum, " +
		   //"@Barcode, " +
		   "@QuantityDifference, " +
		   "@QuantityEdit, " +
		   "@QuantityInPackEdit," +
			"@QuantityOriginal, " +
				//"@SerialNumber, " +
				//"@ShelfCode, " +
				//"@ModifyDate, " +
		   "@ProductName, " +
				//"@PDA_StatusInventProductBit, " +
				//"@PDA_DocumentHeaderCode	, " +
		   "@Makat, " +
		   //"@InputTypeCode, " +
		   //"@IPNum, " +
			//"@BarcodeOriginal," +
			//"@MakatOriginal," +
				//"@PriceString," +
		   "@PriceBuy," +
			"@PriceSale," +
				//"@PriceExtra," +
			"@FromCatalogType," +
			"@SectionCode," +
			"@SectionName," +
				//"@TypeCode," +
			"@TypeMakat," +
			"@ValueBuyQriginal," +
			"@ValueBuyEdit," +
			"@ValueBuyDifference," +
			"@Count," +
			"@ResultCode," +
			"@IsUpdateERP," +
			"@QuantityOriginalERP, " +
			"@CountInParentPack, " +
		   "@BalanceQuantityPartialERP, " +
		   "@ValueOriginalERP, " +
		   "@QuantityDifferenceOriginalERP, " +
		   "@ValueDifferenceOriginalERP," +
			"@SupplierCode," +
			"@SupplierName" +
			//"@PDA_ID" +
			")";

			SqlCeTransaction tran = null;

			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
			int k = 0;

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();

				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Code", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@LocationCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@DocumentHeaderCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@DocumentCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@IturCode", SqlDbType.NVarChar, 50));
				//====================Itur==================================	
				//cmd.Parameters.Add(new SqlCeParameter(		   //?
				//    "@Itur_Disabled", SqlDbType.Bit));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_Publishe", SqlDbType.Bit));		 //?
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_StatusIturBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_Number", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_NumberPrefix", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_NumberSufix", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_LocationCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_StatusIturGroupBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Itur_StatusDocHeaderBit", SqlDbType.Int));
				////=================== Doc ====================================
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_Name", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(		   //?
				//    "@Doc_Approve", SqlDbType.Bit));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_StatusDocHeaderBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_StatusInventProductBit", SqlDbType.Int, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_StatusApproveBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Doc_WorkerGUID", SqlDbType.NVarChar, 50));
				////cmd.Parameters.Add(new SqlCeParameter(
				////    "@Doc_IturCode", SqlDbType.NVarChar));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@DocNum", SqlDbType.BigInt));
				////=================== PDA ====================================
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_DocumentHeaderCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Makat", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Barcode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ProductName", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityOriginal", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityEdit", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityInPackEdit", SqlDbType.Int));

				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityDifference", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ResultCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				 "@IsUpdateERP", SqlDbType.Bit));
				//cmd.Parameters.Add(new SqlCeParameter(
				//     "@ModifyDate", SqlDbType.DateTime));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@InputTypeCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_StatusInventProductBit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@IPNum", SqlDbType.BigInt));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FromCatalogType", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionCode", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionName", SqlDbType.NVarChar, 100));

				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@TypeCode", SqlDbType.NVarChar, 10));
				cmd.Parameters.Add(new SqlCeParameter(
					"@TypeMakat", SqlDbType.NVarChar, 10));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyQriginal", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyEdit", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueBuyDifference", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_ID", SqlDbType.BigInt));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_ShelfCode", SqlDbType.NVarChar, 10));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PDA_SerialNumber", SqlDbType.NVarChar, 50));
				//============Product ===================
				//cmd.Parameters.Add(new SqlCeParameter(
				// "@BarcodeOriginal", SqlDbType.NVarChar, 100));
				//cmd.Parameters.Add(new SqlCeParameter(
				// "@MakatOriginal", SqlDbType.NVarChar, 100));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PriceString", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceBuy", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceSale", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@PriceExtra", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Count", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@QuantityOriginalERP", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@CountInParentPack", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@BalanceQuantityPartialERP", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@ValueOriginalERP", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityDifferenceOriginalERP", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@ValueDifferenceOriginalERP", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SupplierCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SupplierName", SqlDbType.NVarChar, 100));


				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));


				//все суммы IP 
				stopwatch = Stopwatch.StartNew();
				foreach (IturAnalyzesSimple inventProduct in inventProductList)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
					k++;
					//==================== SumQuantityByMakats =========================================
					//cmd.Parameters["@DocumentHeaderCode"].Value = inventProduct.DocumentHeaderCode;
					//cmd.Parameters["@DocumentCode"].Value = inventProduct.DocumentCode;
					//cmd.Parameters["@IturCode"].Value = inventProduct.IturCode;
					//cmd.Parameters["@Barcode"].Value = inventProduct.Barcode;
					//cmd.Parameters["@QuantityDifference"].Value = inventProduct.QuantityEdit - inventProduct.QuantityOriginal ;
					//double quantityEdit = Convert.ToDouble(inventProduct != null ? inventProduct.QuantityEdit : 0);
					//cmd.Parameters["@QuantityOriginal"].Value = inventProduct.QuantityOriginal;
					//cmd.Parameters["@SerialNumber"].Value = inventProduct.SerialNumber != null ? inventProduct.SerialNumber : "";
					//cmd.Parameters["@ShelfCode"].Value = inventProduct.ShelfCode != null ? inventProduct.ShelfCode : "";
					//cmd.Parameters["@ModifyDate"].Value = inventProduct.ModifyDate;
					//cmd.Parameters["@ProductName"].Value = inventProduct.ProductName;
					//cmd.Parameters["@PDA_StatusInventProductBit"].Value = inventProduct.StatusInventProductBit;
					//cmd.Parameters["@PDA_DocumentHeaderCode"].Value = inventProduct.DocumentHeaderCode;
					cmd.Parameters["@Makat"].Value = inventProduct.Makat;
					//cmd.Parameters["@InputTypeCode"].Value = inventProduct.InputTypeCode;
					//cmd.Parameters["@IPNum"].Value = inventProduct.IPNum;
					//cmd.Parameters["@PDA_ID"].Value = inventProduct.ID;
					cmd.Parameters["@Count"].Value = inventProduct.Count;
					
					//===========Catalog =============

					//cmd.Parameters["@MakatOriginal"].Value = inventProduct.Makat;			 //W 
					//cmd.Parameters["@BarcodeOriginal"].Value = inventProduct.Barcode;	  //W
					//cmd.Parameters["@PriceString"].Value = "";
					cmd.Parameters["@PriceBuy"].Value = 0;
					cmd.Parameters["@PriceSale"].Value = 0;
					//cmd.Parameters["@PriceExtra"].Value = 0;
					cmd.Parameters["@ValueBuyQriginal"].Value = 0;
					cmd.Parameters["@ValueBuyEdit"].Value = 0;
					cmd.Parameters["@ValueBuyDifference"].Value = 0;
					cmd.Parameters["@ValueOriginalERP"].Value = 0;
					cmd.Parameters["@ValueDifferenceOriginalERP"].Value = 0;
					cmd.Parameters["@CountInParentPack"].Value = 1;
					cmd.Parameters["@BalanceQuantityPartialERP"].Value = 0;

					//cmd.Parameters["@QuantityEdit"].Value = quantityEdit;
					//cmd.Parameters["@QuantityDifference"].Value = quantityDifference; // 0;// - quantityEdit;
					cmd.Parameters["@QuantityOriginal"].Value = 0;	  //quantityOriginal

					cmd.Parameters["@SectionCode"].Value = DomainUnknownCode.UnknownSection;
					cmd.Parameters["@SupplierCode"].Value = DomainUnknownCode.UnknownSupplier;
					cmd.Parameters["@SupplierName"].Value = DomainUnknownName.UnknownSupplier;
					cmd.Parameters["@ProductName"].Value = "NotCheckInCatalog";
					//cmd.Parameters["@MakatOriginal"].Value = "";
					//cmd.Parameters["@FromCatalogType"].Value = inventProduct.FromCatalogType;
					//cmd.Parameters["@SectionNum"].Value = inventProduct.SectionNum != null ? inventProduct.SectionNum : 0;
					//cmd.Parameters["@TypeCode"].Value = "";
					cmd.Parameters["@ResultCode"].Value = "XXX";
					cmd.Parameters["@TypeMakat"].Value = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";
					cmd.Parameters["@FromCatalogType"].Value = FromCatalogTypeEnum.Unknown;
					double balanceQuantityERP = 0;
					double balanceQuantityPartialERP = 0;
					//double quantityDifference = 0;
					double priceBuy = 0;
					double priceSale = 0;
					bool isUpdateERP = false;
					string sectionName = DomainUnknownName.UnknownSection;
					string supplierName = DomainUnknownName.UnknownSupplier;
	
					//if (productMakatDictionary.ContainsKey(inventProduct.Makat) == true)
					//{
					//    ProductMakat productMakat = productMakatDictionary[inventProduct.Makat];
					//    cmd.Parameters["@MakatOriginal"].Value = productMakat.MakatOriginal;
					//    cmd.Parameters["@BarcodeOriginal"].Value = productMakat.MakatOriginal;//?
					//    cmd.Parameters["@TypeCode"].Value = productMakat.TypeCode;
					//}
					//if (inventProduct.Barcode != inventProduct.Makat)
					//{
					//    if (productMakatDictionary.ContainsKey(inventProduct.Barcode) == true)
					//    {
					//        ProductMakat productMakat = productMakatDictionary[inventProduct.Barcode];
					//        cmd.Parameters["@BarcodeOriginal"].Value = productMakat.MakatOriginal;
					//        cmd.Parameters["@TypeCode"].Value = productMakat.TypeCode;
					//    }
					//}
					double quantityEdit = Convert.ToDouble(inventProduct.QuantityEdit != null ? inventProduct.QuantityEdit : 0);
					double quantityInPackEdit = inventProduct.QuantityInPackEdit;
					
					
					//double quantityDifference = Convert.ToDouble(inventProduct.QuantityDifference != null ? inventProduct.QuantityDifference : 0);

					int countInParentPack = 1;
					if (productSimpleDictionary.ContainsKey(inventProduct.Makat) == true)
					{
						ProductSimple productSimple = productSimpleDictionary[inventProduct.Makat];
						string sectionCode = string.IsNullOrWhiteSpace(productSimple.SectionCode) == false ? productSimple.SectionCode : DomainUnknownCode.UnknownSection;
						string supplierCode = string.IsNullOrWhiteSpace(productSimple.SupplierCode) == false ? productSimple.SupplierCode : DomainUnknownCode.UnknownSupplier;

						productSimpleDictionary[inventProduct.Makat].FromCatalogTypeWithoutInventProduct = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
						cmd.Parameters["@FromCatalogType"].Value = productSimple.FromCatalogType;
						//cmd.Parameters["@FromCatalogType"].Value = productSimple.FromCatalogType;
						//cmd.Parameters["@PriceString"].Value = productSimple.PriceString;
						priceBuy = productSimple.PriceBuy;
						priceSale = productSimple.PriceSale;
						balanceQuantityERP = productSimple.BalanceQuantityERP;
						balanceQuantityPartialERP = productSimple.BalanceQuantityPartialERP;
						countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;
						cmd.Parameters["@CountInParentPack"].Value = countInParentPack; 
						cmd.Parameters["@BalanceQuantityPartialERP"].Value = productSimple.BalanceQuantityPartialERP;
						//cmd.Parameters["@MakatOriginal"].Value = productSimple.MakatOriginal;
						cmd.Parameters["@ProductName"].Value = productSimple.Name;
						cmd.Parameters["@SectionCode"].Value = sectionCode;
						cmd.Parameters["@SupplierCode"].Value = supplierCode;
						if (sectionFromDBDictionary.ContainsKey(sectionCode) == true)
						{
							sectionName = sectionFromDBDictionary[sectionCode].Name;
						}

						if (supplierFromDBDictionary.ContainsKey(supplierCode) == true)
						{
							supplierName = supplierFromDBDictionary[supplierCode].Name;
						}
						isUpdateERP = productSimple.IsUpdateERP;
						//cmd.Parameters["@ValueBuyQriginal"].Value = productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityOriginal);
						//cmd.Parameters["@ValueBuyEdit"].Value = productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityEdit);
						//cmd.Parameters["@ValueBuyDifference"].Value = productSimple.PriceBuy *
						//    (Convert.ToDouble(inventProduct.QuantityOriginal) - Convert.ToDouble(inventProduct.QuantityEdit));
					}

					double balanceQuantityERPAndPartial = balanceQuantityERP;
					if (countInParentPack != 0) balanceQuantityERPAndPartial = balanceQuantityERP + balanceQuantityPartialERP / countInParentPack;
					cmd.Parameters["@QuantityOriginalERP"].Value = balanceQuantityERPAndPartial;//balanceQuantityERP;
					

					double quantityEditAndPartial = quantityEdit;
					if (countInParentPack != 0) quantityEditAndPartial = quantityEdit + quantityInPackEdit / countInParentPack;
					cmd.Parameters["@QuantityEdit"].Value = quantityEditAndPartial;
					//cmd.Parameters["@QuantityEdit"].Value = quantityEdit;
					cmd.Parameters["@QuantityInPackEdit"].Value = quantityInPackEdit;
					double quantityDifference = quantityEditAndPartial - inventProduct.QuantityOriginal;
					cmd.Parameters["@QuantityDifference"].Value = quantityDifference;
					cmd.Parameters["@QuantityDifferenceOriginalERP"].Value = quantityEditAndPartial - balanceQuantityERPAndPartial;//quantityEdit - balanceQuantityERP;

					cmd.Parameters["@PriceBuy"].Value = priceBuy;
					cmd.Parameters["@PriceSale"].Value = priceSale;
					double valueBuyEdit = priceBuy * quantityEditAndPartial; //priceBuy * quantityEdit;
					double valueBuyDifference = priceBuy * quantityDifference;
					double valueBuyQriginalERP = priceBuy * balanceQuantityERPAndPartial;//priceBuy * balanceQuantityERP;

					cmd.Parameters["@ValueOriginalERP"].Value = valueBuyQriginalERP;
					cmd.Parameters["@ValueBuyEdit"].Value = valueBuyEdit;
					cmd.Parameters["@ValueBuyDifference"].Value = valueBuyDifference;
					cmd.Parameters["@ValueDifferenceOriginalERP"].Value = valueBuyEdit - valueBuyQriginalERP;

					cmd.Parameters["@SectionName"].Value = sectionName;
					cmd.Parameters["@SupplierName"].Value = supplierName;
					cmd.Parameters["@ResultCode"].Value = "XXX";
					cmd.Parameters["@IsUpdateERP"].Value = isUpdateERP;

					cmd.ExecuteNonQuery();
				}
				stopwatch.Stop();
				System.Diagnostics.Debug.Print(String.Format("inventProductList cmd.ExecuteNonQuery: {0}", stopwatch.ElapsedTicks.ToString()));

				inventProductList = null;
				GC.Collect();

				stopwatch = Stopwatch.StartNew();
				foreach (KeyValuePair<string, ProductSimple> keyValuePair in productSimpleDictionary)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
					ProductSimple productSimple = keyValuePair.Value;
					if (productSimple.FromCatalogTypeWithoutInventProduct == (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct)
					{
						//    keyValuePair.Key.ToString();
						if (productSimple.BalanceQuantityERP != 0.0)
						{
							k++;

							double priceBuy = productSimple.PriceBuy;
							double priceSale = productSimple.PriceSale;
							int countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;
							double balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP;	   //??
							if (countInParentPack != 0) balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP + 
								productSimple.BalanceQuantityPartialERP / countInParentPack;
							cmd.Parameters["@QuantityOriginalERP"].Value = balanceQuantityERPAndPartial;

							//double balanceQuantityERP = productSimple.BalanceQuantityERP;
							//string sectionCode = string.IsNullOrWhiteSpace(productSimple.SectionCode) == false ? productSimple.SectionCode : DomainUnknownCode.UnknownSection;
							cmd.Parameters["@FromCatalogType"].Value = productSimple.FromCatalogType;
							cmd.Parameters["@BalanceQuantityPartialERP"].Value = productSimple.BalanceQuantityPartialERP;
							cmd.Parameters["@QuantityEdit"].Value = 0;
							cmd.Parameters["@QuantityInPackEdit"].Value = 0;
							cmd.Parameters["@QuantityOriginalERP"].Value = balanceQuantityERPAndPartial;//balanceQuantityERP;
							cmd.Parameters["@QuantityDifferenceOriginalERP"].Value = 0 - balanceQuantityERPAndPartial;//balanceQuantityERP;
							//===
							cmd.Parameters["@QuantityDifference"].Value = 0;// quantityEdit;
							cmd.Parameters["@QuantityOriginal"].Value = 0;	  //- quantityOriginal
							//=====
							cmd.Parameters["@Makat"].Value = productSimple.Makat;
							cmd.Parameters["@Count"].Value = 1;
							cmd.Parameters["@PriceBuy"].Value = priceBuy;
							cmd.Parameters["@PriceSale"].Value = priceSale;
							cmd.Parameters["@ValueOriginalERP"].Value = balanceQuantityERPAndPartial * priceBuy;//balanceQuantityERP * priceBuy;
							cmd.Parameters["@ValueBuyEdit"].Value = 0;
							cmd.Parameters["@ValueDifferenceOriginalERP"].Value = 0 - balanceQuantityERPAndPartial * priceBuy;//0 - balanceQuantityERP * priceBuy;
							//===
							cmd.Parameters["@ValueBuyQriginal"].Value = 0;
							cmd.Parameters["@ValueBuyDifference"].Value = 0;
							//===
							//cmd.Parameters["@SectionCode"].Value = sectionCode;
							cmd.Parameters["@ProductName"].Value = productSimple.Name;
							//cmd.Parameters["@MakatOriginal"].Value = productSimple.MakatOriginal;
							cmd.Parameters["@TypeMakat"].Value = "";
							cmd.Parameters["@ResultCode"].Value = "XXX";
							cmd.Parameters["@isUpdateERP"].Value = false;

							//string sectionName = "";
							//if (sectionFromDBDictionary.ContainsKey(sectionCode) == true)
							//{
							//    sectionName = sectionFromDBDictionary[sectionCode].Name;
							//}
							//cmd.Parameters["@SectionName"].Value = sectionName;

							cmd.ExecuteNonQuery();
						}
					}
				}

				stopwatch.Stop();
				System.Diagnostics.Debug.Print(String.Format("productSimpleDictionary cmd.ExecuteNonQuery: {0}", stopwatch.ElapsedTicks.ToString()));

				stopwatch = Stopwatch.StartNew();
				productSimpleDictionary.Clear();
				sectionFromDBDictionary.Clear();
				supplierFromDBDictionary.Clear();
				GC.Collect();

				if (cancellationToken.IsCancellationRequested == false)
				{
					tran.Commit();
					stopwatch.Stop();

					System.Diagnostics.Debug.Print(String.Format("Commit: {0}", stopwatch.ElapsedTicks.ToString()));

					if (addResult == true)
					{
						string countItemsInsertFromBarcode = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountItems_InsertFromBarcode, pathDB);
						iturAnalyzesRepository.SetResulteValue(AnalezeValueTypeEnum.CountItems_InsertFromBarcode, countItemsInsertFromBarcode, "Items - Insert from Barcode", pathDB);

						string countItemsInsertManually = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountItems_InsertManually, pathDB);
						iturAnalyzesRepository.SetResulteValue(AnalezeValueTypeEnum.CountItems_InsertManually, countItemsInsertManually, "Items - Insert Manually", pathDB);

						//string countItemsTotal = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountItems_Total, pathDB);
						//iturAnalyzesRepository.SetResulteValue(AnalezeValueTypeEnum.CountItems_Total, countItemsTotal, "Items Total", pathDB);

						string countPDAMakatsTotal = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountPDAMakats_Total, pathDB);
						iturAnalyzesRepository.SetResulteValue(AnalezeValueTypeEnum.CountPDAMakats_Total, countPDAMakatsTotal, "Total Makat (Counted)", pathDB);

						string countERPMakatsTotal = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountERPMakats_Total, pathDB);
						iturAnalyzesRepository.SetResulteValue(AnalezeValueTypeEnum.CountERPMakats_Total, countERPMakatsTotal, "Total Makat (in Report)", pathDB);
					}
					//string ret = iturAnalyzesRepository.GetResulteValue(AnalezeValueTypeEnum.CountItems_Total,  pathDB);
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "IturAnalyzes", pathDB));
				}
				else
				{
					tran.Rollback();
				}
			}
			catch (Exception error)
			{
				_logger.ErrorException("InsertIturAnalyzesSimple", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);

				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			
		}

		private void CountLong(long count)
		{
		}

	}
}
