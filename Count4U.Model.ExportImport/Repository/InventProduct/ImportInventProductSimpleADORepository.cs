using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using System.Data.SqlServerCe;
using System.Data;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using Count4U.Model.Count4U.Validate;
using System.Threading;
using NLog;
using System.IO;

namespace Count4U.Model.Count4U
{
	public class ImportInventProductSimpleADORepository : BaseImportADORepository, IImportInventProductRepository
	{
		private readonly IMakatRepository _makatRepository;
		private readonly IIturRepository _iturRepository;
		public readonly List<IInventProductSimpleParser> _inventProductParserList;
		private IInventProductSimpleParser _inventProductParser;
		private IInventProductSimpleParser _inventProductAdvancedParser;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportInventProductSimpleADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository,
			IIturRepository iturRepository)
			: base(connection,dbSettings, log, serviceLocator)
        {
			if (makatRepository == null) throw new ArgumentNullException("makatRepository");
			if (iturRepository == null) throw new ArgumentNullException("iturRepository");

			this._makatRepository = makatRepository;
			this._iturRepository = iturRepository;
			
        }

		private Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "ProductMakatDictionary"));
				Dictionary<string, ProductMakat> productMakatDictionary =
				this._makatRepository.GetProductBarcodeDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "ProductMakatDictionary"));
				return productMakatDictionary;
			}
			else
			{
				return this._makatRepository.GetProductBarcodeDictionary(pathDB, false);
			}
		}

		private Dictionary<string, Itur> GetIturDictionary(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "IturDictionary"));
				Dictionary<string, Itur> iturFromDBDictionary =
					this._iturRepository.GetIturDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "IturDictionary"));
				return iturFromDBDictionary;
			}
			else
			{
				return this._iturRepository.GetIturDictionary(pathDB, false);
			}
		}

		private static void SetIPSimpleParamet(SqlCeCommand cmd, int k, InventProduct inventProduct, bool fillEmptyAdvancedParam = false)
		{
			//}
			cmd.Parameters["@documentHeaderCode"].Value = inventProduct.DocumentCode;
			cmd.Parameters["@documentCode"].Value = inventProduct.DocumentCode;
			cmd.Parameters["@SessionCode"].Value = inventProduct.SessionCode;
			cmd.Parameters["@iturCode"].Value = inventProduct.IturCode;
			cmd.Parameters["@makat"].Value = inventProduct.Makat;
			cmd.Parameters["@code"].Value = inventProduct.Code;
			cmd.Parameters["@barcode"].Value = inventProduct.Barcode;	 
			cmd.Parameters["@SerialNumber"].Value = inventProduct.SerialNumber;
			cmd.Parameters["@productName"].Value = inventProduct.ProductName;

			cmd.Parameters["@quantityOriginal"].Value = inventProduct.QuantityOriginal;
			cmd.Parameters["@QuantityEdit"].Value = inventProduct.QuantityOriginal;
			cmd.Parameters["@QuantityInPackEdit"].Value = inventProduct.QuantityInPackEdit;

			//cmd.Parameters["@ValueBuyQriginal"].Value = inventProduct.ValueBuyQriginal;
			//cmd.Parameters["@ValueBuyEdit"].Value = inventProduct.ValueBuyQriginal;
	   	
		//public string SectionCode { get; set; }
		//public string SectionName { get; set; }
		//public double PriceBuy { get; set; }
		//public double PriceSale { get; set; }	
		// public string WorkerID { get; set; }
		//public string SupplierCode { get; set; }
		//public string SupplierName { get; set; }
	   
			cmd.Parameters["@createDate"].Value = inventProduct.CreateDate;
			cmd.Parameters["@inputTypeCode"].Value = inventProduct.InputTypeCode;
			cmd.Parameters["@ImputTypeCodeFromPDA"].Value = inventProduct.ImputTypeCodeFromPDA;
			cmd.Parameters["@StatusInventProductBit"].Value = inventProduct.StatusInventProductBit;
			cmd.Parameters["@IPNum"].Value = k;
			cmd.Parameters["@FromCatalogType"].Value = inventProduct.FromCatalogType;
			cmd.Parameters["@SectionNum"].Value = inventProduct.SectionNum;
			cmd.Parameters["@SectionCode"].Value = inventProduct.SectionCode;
			cmd.Parameters["@DocNum"].Value = inventProduct.DocNum;
			cmd.Parameters["@TypeMakat"].Value = inventProduct.TypeMakat;
			cmd.Parameters["@WorkerID"].Value = inventProduct.WorkerID;
			cmd.Parameters["@ItemStatus"].Value = inventProduct.ItemStatus;
			cmd.Parameters["@ERPIturCode"].Value = inventProduct.ERPIturCode !=null ? inventProduct.ERPIturCode : "";
	
			
			if (fillEmptyAdvancedParam == true)
			{
				cmd.Parameters["@IPValueStr1"].Value = "";
				cmd.Parameters["@IPValueStr2"].Value = "";
				cmd.Parameters["@IPValueStr3"].Value = "";
				cmd.Parameters["@IPValueStr4"].Value = "";
				cmd.Parameters["@IPValueStr5"].Value = "";
				cmd.Parameters["@IPValueStr6"].Value = "";
				cmd.Parameters["@IPValueStr7"].Value = "";
				cmd.Parameters["@IPValueStr8"].Value = "";
				cmd.Parameters["@IPValueStr9"].Value = "";
				cmd.Parameters["@IPValueStr10"].Value = "";
				cmd.Parameters["@IPValueStr11"].Value = "";
				cmd.Parameters["@IPValueStr12"].Value = "";
				cmd.Parameters["@IPValueStr13"].Value = "";
				cmd.Parameters["@IPValueStr14"].Value = "";
				cmd.Parameters["@IPValueStr15"].Value = "";
				cmd.Parameters["@IPValueStr16"].Value = "";
				cmd.Parameters["@IPValueStr17"].Value = "";
				cmd.Parameters["@IPValueStr18"].Value = "";
				cmd.Parameters["@IPValueStr19"].Value = "";
				cmd.Parameters["@IPValueStr20"].Value = "";
			}
		}

		private static void SetIPAdvancedParamet(SqlCeCommand cmd, int k, InventProduct inventProduct)
		{
			cmd.Parameters["@IPValueStr1"].Value = inventProduct.IPValueStr1;
			cmd.Parameters["@IPValueStr2"].Value = inventProduct.IPValueStr2;
			cmd.Parameters["@IPValueStr3"].Value = inventProduct.IPValueStr3;
			cmd.Parameters["@IPValueStr4"].Value = inventProduct.IPValueStr4;
			cmd.Parameters["@IPValueStr5"].Value = inventProduct.IPValueStr5;
			cmd.Parameters["@IPValueStr6"].Value = inventProduct.IPValueStr6;
			cmd.Parameters["@IPValueStr7"].Value = inventProduct.IPValueStr7;
			cmd.Parameters["@IPValueStr8"].Value = inventProduct.IPValueStr8;
			cmd.Parameters["@IPValueStr9"].Value = inventProduct.IPValueStr9;
			cmd.Parameters["@IPValueStr10"].Value = inventProduct.IPValueStr10;
			cmd.Parameters["@IPValueStr11"].Value = inventProduct.IPValueStr11;
			cmd.Parameters["@IPValueStr12"].Value = inventProduct.IPValueStr12;
			cmd.Parameters["@IPValueStr13"].Value = inventProduct.IPValueStr13;
			cmd.Parameters["@IPValueStr14"].Value = inventProduct.IPValueStr14;
			cmd.Parameters["@IPValueStr15"].Value = inventProduct.IPValueStr15;
			cmd.Parameters["@IPValueStr16"].Value = inventProduct.IPValueStr16;
			cmd.Parameters["@IPValueStr17"].Value = inventProduct.IPValueStr17;
			cmd.Parameters["@IPValueStr18"].Value = inventProduct.IPValueStr18;
			cmd.Parameters["@IPValueStr19"].Value = inventProduct.IPValueStr19;
			cmd.Parameters["@IPValueStr20"].Value = inventProduct.IPValueStr20;

			cmd.Parameters["@IPValueFloat1"].Value = inventProduct.IPValueFloat1;
			cmd.Parameters["@IPValueFloat2"].Value = inventProduct.IPValueFloat2;
			cmd.Parameters["@IPValueFloat3"].Value = inventProduct.IPValueFloat3;
			cmd.Parameters["@IPValueFloat4"].Value = inventProduct.IPValueFloat4;
			cmd.Parameters["@IPValueFloat5"].Value = inventProduct.IPValueFloat5;

			cmd.Parameters["@IPValueInt1"].Value = inventProduct.IPValueInt1;
			cmd.Parameters["@IPValueInt2"].Value = inventProduct.IPValueInt2;
			cmd.Parameters["@IPValueInt3"].Value = inventProduct.IPValueInt3;
			cmd.Parameters["@IPValueInt4"].Value = inventProduct.IPValueInt4;
			cmd.Parameters["@IPValueInt5"].Value = inventProduct.IPValueInt5;

			cmd.Parameters["@IPValueBit1"].Value = inventProduct.IPValueBit1;
			cmd.Parameters["@IPValueBit2"].Value = inventProduct.IPValueBit2;
			cmd.Parameters["@IPValueBit3"].Value = inventProduct.IPValueBit3;
			cmd.Parameters["@IPValueBit4"].Value = inventProduct.IPValueBit4;
			cmd.Parameters["@IPValueBit5"].Value = inventProduct.IPValueBit5;
			}

		public void InsertInventProducts(string fromPathFile, string pathDB, 
			InventProductSimpleParserEnum inventProductSimpleParserEnum,
			Encoding encoding, string[] separators, 
			int countExcludeFirstString,  
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1059%"Cancel Insert [{0}] in DB [{1}] from file [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1059, "[InventProduct]", pathDB, fromPathFile));
				return;
			}
			this._inventProductParser =
						this._serviceLocator.GetInstance<IInventProductSimpleParser>(inventProductSimpleParserEnum.ToString());
			if (this._inventProductParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, inventProductSimpleParserEnum.ToString()));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}

			//this._inventProductAdvancedParser   =  
			//    this._serviceLocator.GetInstance<IInventProductSimpleParser>(inventProductAdvancedParser.ToString());

			Dictionary<string, Itur> iturFromDBDictionary = new Dictionary<string, Itur>();
			if (importType.Contains(ImportDomainEnum.ExistItur) == true)
			{
				iturFromDBDictionary = this.GetIturDictionary(pathDB, true);
			}
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, " ImportPDA Repository", "[ImportInventProductSimpleADORepository]"));
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "InventProductParser", inventProductSimpleParserEnum.ToString()));
  
			int i = 0;
			//while (this._makatRepository.ProductMakatDictionaryFill == false)
			//{
			//    i++;
			//    Thread.Sleep(100);
			//    if (i == 1000) this._makatRepository.ProductMakatDictionaryFill = true;
			//}

			Dictionary<string, ProductMakat> productMakatDictionary = new Dictionary<string, ProductMakat>();
			if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
			{
				bool refillMalat = false;
				if (importType.Contains(ImportDomainEnum.RefillMakatDictionary) == true) refillMalat = true;
				productMakatDictionary = this._makatRepository.ProductMakatDictionaryRefill(pathDB, refillMalat);
			}

			//Dictionary<string, ProductMakat> productBarcodeDictionary = new Dictionary<string, ProductMakat>();
			//if (importType.Contains(ImportDomainEnum.ExistBarcode) == true)
			//{
			//    productBarcodeDictionary = this.GetProductBarcodeDictionary(pathDB, true);
			//}

			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			//=================================[InventProduct]============================
			//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "InventProduct", pathDB));

	
			string sessionCodeIn = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string sqlAdvancedInto = ",[IPValueStr1],[IPValueStr2],[IPValueStr3], [IPValueStr4],[IPValueStr5] " +
											",[IPValueStr6],[IPValueStr7],[IPValueStr8], [IPValueStr9],[IPValueStr10] " +
											",[IPValueStr11],[IPValueStr12],[IPValueStr13], [IPValueStr14],[IPValueStr15] " +
											",[IPValueStr16],[IPValueStr17],[IPValueStr18], [IPValueStr19],[IPValueStr20] ";
			string sqlAdvancedValue = ", @IPValueStr1, @IPValueStr2, @IPValueStr3, @IPValueStr4, @IPValueStr5 " +
											 ", @IPValueStr6, @IPValueStr7, @IPValueStr8, @IPValueStr9, @IPValueStr10 "	+
											 ", @IPValueStr11, @IPValueStr12, @IPValueStr13, @IPValueStr14, @IPValueStr15 " +
											 ", @IPValueStr16, @IPValueStr17, @IPValueStr18, @IPValueStr19, @IPValueStr20 ";

			if (importType.Contains(ImportDomainEnum.ImportParentProductAdvanced) == true)
			{
				sqlAdvancedInto = ",[IPValueStr1],[IPValueStr2],[IPValueStr3], [IPValueStr4],[IPValueStr5] " +
											",[IPValueStr6],[IPValueStr7],[IPValueStr8], [IPValueStr9],[IPValueStr10] " +
											",[IPValueStr11],[IPValueStr12],[IPValueStr13], [IPValueStr14],[IPValueStr15] " +
											",[IPValueStr16],[IPValueStr17],[IPValueStr18], [IPValueStr19],[IPValueStr20] " +
											",[IPValueFloat1],[IPValueFloat2],[IPValueFloat3], [IPValueFloat4],[IPValueFloat5] " +
											",[IPValueInt1],[IPValueInt2],[IPValueInt3], [IPValueInt4],[IPValueInt5] " +
											",[IPValueBit1],[IPValueBit2],[IPValueBit3], [IPValueBit4],[IPValueBit5] ";

				sqlAdvancedValue = ", @IPValueStr1, @IPValueStr2, @IPValueStr3, @IPValueStr4, @IPValueStr5 " +
											 ", @IPValueStr6, @IPValueStr7, @IPValueStr8, @IPValueStr9, @IPValueStr10 " +
											 ", @IPValueStr11, @IPValueStr12, @IPValueStr13, @IPValueStr14, @IPValueStr15 " +
											 ", @IPValueStr16, @IPValueStr17, @IPValueStr18, @IPValueStr19, @IPValueStr20 " +
											 ", @IPValueFloat1, @IPValueFloat2, @IPValueFloat3, @IPValueFloat4, @IPValueFloat5 " +
											 ", @IPValueInt1, @IPValueInt2, @IPValueInt3, @IPValueInt4, @IPValueInt5 " +
											 ", @IPValueBit1, @IPValueBit2, @IPValueBit3, @IPValueBit4, @IPValueBit5 ";
			}

	
			string sql1 = "INSERT INTO [InventProduct]" +
			"([Makat],[Code],[Barcode],[SerialNumber],[DocumentHeaderCode],[DocumentCode],[IturCode],[ProductName]" +
			", [QuantityEdit],[QuantityOriginal], [QuantityInPackEdit]" + 
			//", [ValueBuyEdit],[ValueBuyQriginal]" +
			",[InputTypeCode],[ImputTypeCodeFromPDA],[CreateDate],[StatusInventProductBit],[IPNum]" +
			",[FromCatalogType],[SectionNum],[SectionCode],[SessionCode],[DocNum], [TypeMakat],[WorkerID],[ItemStatus],[ERPIturCode]" +
			 sqlAdvancedInto  +
			")" +
			" VALUES(@makat, @code, @barcode, @SerialNumber, @documentHeaderCode, @documentCode, @iturCode, @productName" +
			", @QuantityEdit, @quantityOriginal, @QuantityInPackEdit" + 
			//", @ValueBuyEdit, @ValueBuyQriginal" + 
			", @inputTypeCode, @ImputTypeCodeFromPDA, @createDate, @StatusInventProductBit,@IPNum " +
			", @FromCatalogType, @SectionNum, @SectionCode, @SessionCode, @DocNum, @TypeMakat, @WorkerID, @ItemStatus , @ERPIturCode " +
			sqlAdvancedValue +
			")";

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

 				cmd.Parameters.Add(new SqlCeParameter(
					"@documentHeaderCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@documentCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@iturCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@makat", SqlDbType.NVarChar, 300));
				cmd.Parameters.Add(new SqlCeParameter(
				"@code", SqlDbType.NVarChar, 300));
			cmd.Parameters.Add(new SqlCeParameter(
					"@SerialNumber", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@barcode", SqlDbType.NVarChar, 300));
				cmd.Parameters.Add(new SqlCeParameter(
					"@productName", SqlDbType.NVarChar, 100));

				cmd.Parameters.Add(new SqlCeParameter(
					 "@quantityOriginal", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityEdit", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@QuantityInPackEdit", SqlDbType.Int));
				//cmd.Parameters.Add(new SqlCeParameter(
				//     "@ValueBuyQriginal", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//     "@ValueBuyEdit", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					 "@createDate", SqlDbType.DateTime));
				cmd.Parameters.Add(new SqlCeParameter(
					"@inputTypeCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ImputTypeCodeFromPDA", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@StatusInventProductBit", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@IPNum", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FromCatalogType", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionNum", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SessionCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@DocNum", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@TypeMakat", SqlDbType.NVarChar, 10));
				cmd.Parameters.Add(new SqlCeParameter(
					"@WorkerID", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@ItemStatus", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@ERPIturCode", SqlDbType.NVarChar, 50));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr1", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr2", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr3", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr4", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr5", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr6", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr7", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr8", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr9", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr10", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr11", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr12", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr13", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr14", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr15", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr16", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr17", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr18", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr19", SqlDbType.NVarChar, 100));
					cmd.Parameters.Add(new SqlCeParameter(
					"@IPValueStr20", SqlDbType.NVarChar, 100));

					if (importType.Contains(ImportDomainEnum.ImportParentProductAdvanced) == true)
					{
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueFloat1", SqlDbType.Float));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueFloat2", SqlDbType.Float));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueFloat3", SqlDbType.Float));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueFloat4", SqlDbType.Float));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueFloat5", SqlDbType.Float));

					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueInt1", SqlDbType.Int));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueInt2", SqlDbType.Int));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueInt3", SqlDbType.Int));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueInt4", SqlDbType.Int));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueInt5", SqlDbType.Int));

					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueBit1", SqlDbType.Bit));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueBit2", SqlDbType.Bit));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueBit3", SqlDbType.Bit));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueBit4", SqlDbType.Bit));
					cmd.Parameters.Add(new SqlCeParameter(
						 "@IPValueBit5", SqlDbType.Bit));
				}

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "InventProduct", pathDB));

				Guid workerGUID = Guid.NewGuid();
				int k = 0;

				

  //IEnumerable<InventProduct> dataInventProduct = this._inventProductParser.GetInventProducts(
  //                    fromPathFile, encoding, separators,
  //                    countExcludeFirstString, sessionCodeIn,
  //                     productMakatDictionary, //productBarcodeDictionary, 
  //                     iturFromDBDictionary,
  //                     importType,
  //                     parms).AsEnumerable<InventProduct>();

  //bulkCopy.WriteToServer(dataInventProduct);
				//countAction(k);
				//if (inventProductAdvancedParser == InventProductAdvancedParserEnum.None)
				//{
				_logger.Info("start import PDA file : " + fromPathFile);
				foreach (InventProduct inventProduct in
						this._inventProductParser.GetInventProducts(
						fromPathFile, encoding, separators,
						countExcludeFirstString, sessionCodeIn,
						 productMakatDictionary, //productBarcodeDictionary, 
						 iturFromDBDictionary,
						 importType,
						 parms))
				{
					if (cancellationToken.IsCancellationRequested == true) { break; }
					k++;
					countAction(k);

					
					if (importType.Contains(ImportDomainEnum.ImportParentProductAdvanced) == true)
					{
						SetIPSimpleParamet(cmd, k, inventProduct);
						SetIPAdvancedParamet(cmd, k, inventProduct);
					}
					else
					{
						SetIPSimpleParamet(cmd, k, inventProduct, true);
					}
					cmd.ExecuteNonQuery();
				}
				//}
				//else
				//{
				//foreach (InventProduct inventProduct in
				//this._inventProductParser.GetInventProducts(
				//fromPathFile, encoding, separators,
				//countExcludeFirstString, sessionCodeIn,
				// productMakatDictionary, //productBarcodeDictionary, 
				// iturFromDBDictionary,
				// importType,
				// parms))
				//    {
				//        if (cancellationToken.IsCancellationRequested == true) { break; }
				//        k++;
				//        countAction(k);
				//        SetIPAdvancedParamet(cmd, k, inventProduct);
				//        cmd.ExecuteNonQuery();
				//    }
				//}
				//SetIPAdvancedParamet(cmd, k, inventProduct);
				countAction(k);
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "InventProduct", pathDB));

				//===========================[DocumentHeader] ===============================
				if (importType.Contains(ImportDomainEnum.ImportDocumentHeader) == true)
				{
					//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "DocumentHeader", pathDB));

					string sql2 = "INSERT INTO [DocumentHeader]" +
					"([DocumentCode],[Name],[IturCode],[WorkerGUID]" +  //,[Approve]
					",[SessionCode],[CreateDate])" +
					" VALUES(@documentHeaderCode, @name, @iturCode, @workerGUID" + //, @approve
					" , @sessionCode, @createDate)";

					var cmd2 = new SqlCeCommand(sql2, sqlCeConnection, tran);

					cmd2.Parameters.Add(new SqlCeParameter(
						"@name", SqlDbType.NVarChar, 50));
					//cmd2.Parameters.Add(new SqlCeParameter(
					//"@num", SqlDbType.Int));
					cmd2.Parameters.Add(new SqlCeParameter(
						"@documentHeaderCode", SqlDbType.NVarChar, 50));
					//cmd2.Parameters.Add(new SqlCeParameter(
					//    "@approve", SqlDbType.Bit));
					cmd2.Parameters.Add(new SqlCeParameter(
						"@iturCode", SqlDbType.NVarChar, 50));
					cmd2.Parameters.Add(new SqlCeParameter(
						"@workerGUID", SqlDbType.NVarChar, 50));
					cmd2.Parameters.Add(new SqlCeParameter(
						"@sessionCode", SqlDbType.NVarChar, 50));
					cmd2.Parameters.Add(new SqlCeParameter(
						"@createDate", SqlDbType.DateTime));

					k = 0;
					countAction(k);
					foreach (KeyValuePair<string, DocumentHeader> keyValuePair in
						this._inventProductParser.DocumentHeaderDictionary)
					{
						if (cancellationToken.IsCancellationRequested == true)
						{
							break;
						}
						k++;
						if (k % 10 == 0)
						{
							countAction(k);
							//tran.Commit();
							//tran = sqlCeConnection.BeginTransaction();
						}
						string key = keyValuePair.Key;	//DocumentCode
						DocumentHeader val = keyValuePair.Value; //DocumentHeader
						//cmd2.Parameters["@code"].Value = val.Code;
						cmd2.Parameters["@name"].Value = val.Name != "" ? val.Name : "9999";
						//cmd2.Parameters["@num"].Value = k;
						cmd2.Parameters["@documentHeaderCode"].Value = val.DocumentCode;
						//cmd2.Parameters["@approve"].Value = val.Approve;
						cmd2.Parameters["@iturCode"].Value = val.IturCode;
						cmd2.Parameters["@workerGUID"].Value = val.WorkerGUID;
						cmd2.Parameters["@sessionCode"].Value = sessionCodeIn;
						cmd2.Parameters["@createDate"].Value = val.CreateDate;

						cmd2.ExecuteNonQuery();

					}
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "DocumentHeader", pathDB));
				}
	//=================================[Itur]============================
				if (importType.Contains(ImportDomainEnum.ImportItur) == true)
				{
					//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Itur", pathDB));
					string sql4 = "INSERT INTO [Itur](" +
						//"[Name]" + 
						//",[Description]" + 
						//",[InitialQuantityMakatExpected]" + 
						"[IturCode]" +
						",[ERPIturCode]" +
						//",[Approve]" + 
						//",[Disabled]" + 
						  //",[StatusIturCode]" +
						//",[CreateDate]" + 
						//",[ModifyDate]" + 
						//",[Publishe]" + 
						",[StatusIturBit]" + 
						 ",[Number]" +
						",[NumberPrefix]" +
						",[NumberSufix]" +
						",[LocationCode]" +
						")" +
						" VALUES(" +
						// "@Name" + 
						//",@Description" + 
						//",@InitialQuantityMakatExpected" + 
	   "@IturCode" +
	   ",@ERPIturCode" +
						//",@Approve" + 
						//",@Disabled" + 
	  // ",@StatusIturCode" +
						//",@CreateDate" + 
						//",@ModifyDate" + 
						//",@Publishe" + 
						",@StatusIturBit" + 
	   ",@Number" +
	   ",@NumberPrefix" +
	   ",@NumberSufix" +
	   ",@LocationCode" +
		")";

					var cmd4 = new SqlCeCommand(sql4, sqlCeConnection, tran);

					//cmd4.Parameters.Add(new SqlCeParameter(
					//    "@Name", SqlDbType.NVarChar, 50));
					//cmd4.Parameters.Add(new SqlCeParameter(
					//    "@Description", SqlDbType.NVarChar, 100));
					//cmd4.Parameters.Add(new SqlCeParameter(
					//    "@InitialQuantityMakatExpected", SqlDbType.Float, 50));			//?
					cmd4.Parameters.Add(new SqlCeParameter(
						"@IturCode", SqlDbType.NVarChar, 50));
					cmd4.Parameters.Add(new SqlCeParameter(
						"@ERPIturCode", SqlDbType.NVarChar, 50));
					
					//cmd4.Parameters.Add(new SqlCeParameter(		   //?
					//    "@Approve", SqlDbType.Bit));
					//cmd4.Parameters.Add(new SqlCeParameter(		   //?
					//    "@Disabled", SqlDbType.Bit));
					//cmd4.Parameters.Add(new SqlCeParameter(
					//    "@StatusIturCode", SqlDbType.NVarChar, 50));
					//cmd4.Parameters.Add(new SqlCeParameter(
					//    "@CreateDate", SqlDbType.NVarChar, 50));
					//cmd4.Parameters.Add(new SqlCeParameter(
					//    "@ModifyDate", SqlDbType.NVarChar, 50));	 //?
					//cmd4.Parameters.Add(new SqlCeParameter(
					//    "@Publishe", SqlDbType.Bit));		 //?
					cmd4.Parameters.Add(new SqlCeParameter(
						"@StatusIturBit", SqlDbType.Int));
					cmd4.Parameters.Add(new SqlCeParameter(
						"@Number", SqlDbType.Int));
					cmd4.Parameters.Add(new SqlCeParameter(
						"@NumberPrefix", SqlDbType.NVarChar, 50));
					cmd4.Parameters.Add(new SqlCeParameter(
					"@NumberSufix", SqlDbType.NVarChar, 50));
					cmd4.Parameters.Add(new SqlCeParameter(
						"@LocationCode", SqlDbType.NVarChar, 50));


					k = 0;
					countAction(k);
					foreach (KeyValuePair<string, Itur> keyValuePair in
						this._inventProductParser.IturDictionary)
					{
						if (cancellationToken.IsCancellationRequested == true)
						{
							break;
						}
						k++;
						if (k % 10 == 0)
						{
							countAction(k);
							//tran.Commit();
							//tran = sqlCeConnection.BeginTransaction();
						}

						string key = keyValuePair.Key;
						Itur val = keyValuePair.Value;

						cmd4.Parameters["@IturCode"].Value = val.IturCode;
						cmd4.Parameters["@ERPIturCode"].Value = val.ERPIturCode != null ? val.ERPIturCode : "";
						//cmd4.Parameters["@Name"].Value = itur.Name;
						//cmd4.Parameters["@Description"].Value = itur.Description;
						//cmd4.Parameters["@InitialQuantityMakatExpected"].Value = itur.InitialQuantityMakatExpected;//?
						//cmd4.Parameters["@Approve"].Value = itur.Approve;		  //?
						//cmd4.Parameters["@Disabled"].Value = itur.Disabled;		  //?
						//cmd4.Parameters["@StatusIturCode"].Value = val.StatusIturCode != null ? val.StatusIturCode : DomainUnknownCode.UnknownStatus;
						//cmd4.Parameters["@CreateDate"].Value = itur.CreateDate;
						//cmd4.Parameters["@ModifyDate"].Value = DateTime.Now;
						//cmd4.Parameters["@Publishe"].Value = itur.Publishe;	   //?
						cmd4.Parameters["@StatusIturBit"].Value = val.StatusIturBit;
						cmd4.Parameters["@Number"].Value = val.Number;
						cmd4.Parameters["@NumberPrefix"].Value = val.NumberPrefix;
						cmd4.Parameters["@NumberSufix"].Value = val.NumberSufix;
						cmd4.Parameters["@LocationCode"].Value = val.LocationCode != null ? val.LocationCode : DomainUnknownCode.UnknownLocation;
						//this._locationDictionary.Add(product.Barcode,
						//    new ProductSimple { ProductID = 0, ProductName = product.Name });

						cmd4.ExecuteNonQuery();

					}
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Itur", pathDB));
				}

				//==================================================================
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportInventProductSimpleADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportInventProductSimpleADORepository"));
			}
			catch (Exception error)
			{
				_logger.ErrorException("InsertInventProducts", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}

			this.FillLogFromErrorBitList(this._inventProductParser.ErrorBitList);
		}

		


		public void ClearInventProducts(string pathDB)
		{
			string sql1 = "DELETE FROM  [InventProduct]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "InventProduct", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear {0} Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "InventProduct"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "InventProduct"));
				this.Log.Add(MessageTypeEnum.TraceRepository, "");
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearInventProducts", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
		}

		public void ClearDocumentHeaders(string pathDB)
		{
			string sql1 = "DELETE FROM  [DocumentHeader]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "DocumentHeader", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear {0} Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "DocumentHeader"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "DocumentHeader"));
				tran.Commit();
		
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearDocumentHeaders", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
		}

		public void ClearSession(string pathDB)
		{
			string sql1 = "DELETE FROM  [Session]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Session", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear {0} Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Session"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Session"));
				this.Log.Add(MessageTypeEnum.TraceRepository, "");
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearSession", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
		}

		public void ClearItur(string pathDB)
		{
			string sql1 = "DELETE FROM  [Itur]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Itur", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Itur"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Itur"));
				this.Log.Add(MessageTypeEnum.TraceRepository, "");
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearItur", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			//LogPrint();
		}

		public void FillLogFromErrorBitList(List<BitAndRecord> errorBitList)
		{
			if (errorBitList == null) return;
			if (errorBitList.Count == 0) return;
			//Log_TraceParser1001%"Parser Error And Message : "
			this.Log.Add(MessageTypeEnum.TraceParser, Localization.Resources.Log_TraceParser1001);
			foreach (BitAndRecord bitAndRecord in errorBitList)
			{
				int bit = bitAndRecord.Bit;
				string record = bitAndRecord.Record;
				MessageTypeEnum errorType = bitAndRecord.ErrorType;
				if (errorType == MessageTypeEnum.Error)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.Error, //bitAndRecord.ErrorType.ToString() + " : "	+
							 InventProductValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : "	+
							 InventProductValidate.ConvertDataErrorCode2WarningMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}


		
	}
}
