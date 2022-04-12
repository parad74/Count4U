using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using System.Data.Entity.Core.Objects;
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
	public class IturAnalyzesSourceReaderADORepository : BaseADORepository, IIturAnalyzesCaseSourceRepository
	{
		private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public IturAnalyzesSourceReaderADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository)
			: base(connection, dbSettings, log, serviceLocator)
		{
			this._makatRepository = makatRepository;

		}

		// ===================== InsertIturAnalyzes =========================================
		public void InsertIturAnalyzes(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null) //
		{
			IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesReader;

			//получаем данные от ридера - затем пишем в БД через ADO
			IIturAnalyzesReader iturAnalyzesReader =
			this._serviceLocator.GetInstance<IIturAnalyzesReader>(iturAnalyzesReaderEnum.ToString());

			//if (selectParms != null)
			//{
			//    string selectParmsString = selectParms.ToString();
			//}
			//if (refill == true)
			//{
			//    this.ClearIturAnalyzes(pathDB);
			//}
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

			// TO DO add insert IPValueInt1, IPValueFloat1, IPValueStr1, IPValueStr2 - for Report

			this.Log.Add(MessageTypeEnum.TraceRepository, "IturAnalyzesReaderADORepository");
			_logger.Info("IturAnalyzesReaderADORepository :: InsertIturAnalyzes");

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
		   "[Price]," +
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
		   "[LocationName], " +
				//"[IturName], " +
				//"[SessionCode], " +
				//"[SessionNum]" +
				  "[FamilyCode], " +
				  "[FamilyName], " +
				  "[FamilySize], " +
				  "[FamilyType], " +
				  "[FamilyExtra1], " +
				  "[FamilyExtra2], " +
				"[ValueChar]," +
				"[SubSessionCode]," +
				"[SessionName] ," +
				"[SubSessionName] " +
			") " +
	 "VALUES(" +
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
		   "@QuantityEdit, " +
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
		   "@Price," +
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
			"@LocationName," +
				//"@IturName," +
				//"@SessionCode," +
				//"@SessionNum" +
			  "@FamilyCode, " +
			  "@FamilyName, " +
			 "@FamilySize, " +
			  "@FamilyType, " +
			  "@FamilyExtra1, " +
			  "@FamilyExtra2, " +
				"@ValueChar, " +
				"@SubSessionCode, " +
				"@SessionName, " +
				"@SubSessionName " +
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
					"@Price", SqlDbType.Float));
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
				cmd.Parameters.Add(new SqlCeParameter(
				"@FamilyCode", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyName", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilySize", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyType", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyExtra1", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyExtra2", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueChar", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
				"@SubSessionCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SessionName", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SubSessionName", SqlDbType.NVarChar, 100));

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				//this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));

				IEnumerable<IturAnalyzes> iturAnalyzesEnumerable =
					iturAnalyzesReader.GetIturAnalyzes(pathDB, refill, refillCatalogDictionary, selectParms, parmsIn, false);


				foreach (IturAnalyzes iturAnalyzes in iturAnalyzesEnumerable)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
					k++;
					//==================== PDA =========================================
					cmd.Parameters["@Code"].Value = iturAnalyzes.Code;
					cmd.Parameters["@LocationCode"].Value = iturAnalyzes.LocationCode;
					cmd.Parameters["@DocumentHeaderCode"].Value = iturAnalyzes.DocumentHeaderCode;
					cmd.Parameters["@DocumentCode"].Value = iturAnalyzes.DocumentCode;
					cmd.Parameters["@IturCode"].Value = iturAnalyzes.IturCode;
					cmd.Parameters["@ERPIturCode"].Value = iturAnalyzes.ERPIturCode;
					cmd.Parameters["@Itur_Disabled"].Value = iturAnalyzes.Itur_Disabled;
					cmd.Parameters["@Itur_Publishe"].Value = iturAnalyzes.Itur_Publishe;
					cmd.Parameters["@Itur_StatusIturBit"].Value = iturAnalyzes.Itur_StatusIturBit;
					cmd.Parameters["@Itur_Number"].Value = iturAnalyzes.Itur_Number;
					cmd.Parameters["@Itur_NumberPrefix"].Value = iturAnalyzes.Itur_NumberPrefix;
					cmd.Parameters["@Itur_NumberSufix"].Value = iturAnalyzes.Itur_NumberSufix;
					cmd.Parameters["@Itur_LocationCode"].Value = iturAnalyzes.Itur_LocationCode;
					cmd.Parameters["@Itur_StatusIturGroupBit"].Value = iturAnalyzes.Itur_StatusIturGroupBit;
					cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = iturAnalyzes.Itur_StatusDocHeaderBit;
					cmd.Parameters["@Doc_Name"].Value = iturAnalyzes.Doc_Name;
					cmd.Parameters["@Doc_Approve"].Value = iturAnalyzes.Doc_Approve;
					//cmd.Parameters["@Doc_IturCode"].Value = iturAnalyzes.Doc_IturCode;
					cmd.Parameters["@Doc_WorkerGUID"].Value = iturAnalyzes.Doc_WorkerGUID;
					cmd.Parameters["@Doc_StatusDocHeaderBit"].Value = iturAnalyzes.Doc_StatusDocHeaderBit;
					cmd.Parameters["@Doc_StatusInventProductBit"].Value = iturAnalyzes.Doc_StatusInventProductBit;
					cmd.Parameters["@Doc_StatusApproveBit"].Value = iturAnalyzes.Doc_StatusApproveBit;
					cmd.Parameters["@DocNum"].Value = iturAnalyzes.DocNum;
					cmd.Parameters["@Barcode"].Value = iturAnalyzes.Barcode;
					cmd.Parameters["@QuantityDifference"].Value = iturAnalyzes.QuantityDifference;
					cmd.Parameters["@QuantityEdit"].Value = iturAnalyzes.QuantityEdit;
					cmd.Parameters["@QuantityInPackEdit"].Value = iturAnalyzes.QuantityInPackEdit;
					cmd.Parameters["@QuantityOriginal"].Value = iturAnalyzes.QuantityOriginal;
					//cmd.Parameters["@SerialNumber"].Value = iturAnalyzes.SerialNumber;
					//cmd.Parameters["@ShelfCode"].Value = iturAnalyzes.ShelfCode;
					cmd.Parameters["@ModifyDate"].Value = iturAnalyzes.ModifyDate;
					cmd.Parameters["@ProductName"].Value = iturAnalyzes.ProductName;
					cmd.Parameters["@PDA_StatusInventProductBit"].Value = iturAnalyzes.PDA_StatusInventProductBit;
					//cmd.Parameters["@PDA_DocumentHeaderCode"].Value = iturAnalyzes.PDA_DocumentHeaderCode;
					cmd.Parameters["@Makat"].Value = iturAnalyzes.Makat;
					cmd.Parameters["@InputTypeCode"].Value = iturAnalyzes.InputTypeCode;
					cmd.Parameters["@IPNum"].Value = iturAnalyzes.IPNum;
					cmd.Parameters["@BarcodeOriginal"].Value = iturAnalyzes.BarcodeOriginal;
					cmd.Parameters["@MakatOriginal"].Value = iturAnalyzes.MakatOriginal;
					cmd.Parameters["@PriceString"].Value = iturAnalyzes.PriceString;
					cmd.Parameters["@Price"].Value = iturAnalyzes.Price;
					cmd.Parameters["@PriceBuy"].Value = iturAnalyzes.PriceBuy;
					cmd.Parameters["@PriceSale"].Value = iturAnalyzes.PriceSale;
					cmd.Parameters["@PriceExtra"].Value = iturAnalyzes.PriceExtra;
					cmd.Parameters["@FromCatalogType"].Value = iturAnalyzes.FromCatalogType;
					cmd.Parameters["@SectionNum"].Value = iturAnalyzes.SectionNum;
					cmd.Parameters["@TypeCode"].Value = iturAnalyzes.TypeCode;
					cmd.Parameters["@TypeMakat"].Value = iturAnalyzes.TypeMakat;
					cmd.Parameters["@ValueBuyQriginal"].Value = iturAnalyzes.ValueBuyQriginal;
					cmd.Parameters["@ValueBuyEdit"].Value = iturAnalyzes.ValueBuyEdit;
					cmd.Parameters["@ValueBuyDifference"].Value = iturAnalyzes.ValueBuyDifference;
					cmd.Parameters["@PDA_ID"].Value = iturAnalyzes.PDA_ID;
					cmd.Parameters["@SectionCode"].Value = iturAnalyzes.SectionCode;
					cmd.Parameters["@SectionName"].Value = iturAnalyzes.SectionName;
					cmd.Parameters["@IsUpdateERP"].Value = iturAnalyzes.IsUpdateERP;
					cmd.Parameters["@ValueFloat"].Value = iturAnalyzes.ValueFloat;
					cmd.Parameters["@ValueChar"].Value = iturAnalyzes.ValueChar;
					cmd.Parameters["@QuantityOriginalERP"].Value = iturAnalyzes.QuantityOriginalERP;
					cmd.Parameters["@CountInParentPack"].Value = iturAnalyzes.CountInParentPack;
					cmd.Parameters["@BalanceQuantityPartialERP"].Value = iturAnalyzes.BalanceQuantityPartialERP;
					cmd.Parameters["@ValueOriginalERP"].Value = iturAnalyzes.ValueOriginalERP;
					cmd.Parameters["@QuantityDifferenceOriginalERP"].Value = iturAnalyzes.QuantityDifferenceOriginalERP;
					cmd.Parameters["@ValueDifferenceOriginalERP"].Value = iturAnalyzes.ValueDifferenceOriginalERP;
					//cmd.Parameters["@SupplierCode"].Value = iturAnalyzes.SupplierCode;
					//cmd.Parameters["@SupplierName"].Value = iturAnalyzes.SupplierName;
					cmd.Parameters["@LocationName"].Value = iturAnalyzes.LocationName;
					cmd.Parameters["@FamilyCode"].Value = iturAnalyzes.FamilyCode;
					cmd.Parameters["@FamilyName"].Value = iturAnalyzes.FamilyName;
					cmd.Parameters["@FamilySize"].Value = iturAnalyzes.FamilySize;
					cmd.Parameters["@FamilyType"].Value = iturAnalyzes.FamilyType;
					cmd.Parameters["@FamilyExtra1"].Value = iturAnalyzes.FamilyExtra1;
					cmd.Parameters["@FamilyExtra2"].Value = iturAnalyzes.FamilyExtra2;

					cmd.Parameters["@SubSessionCode"].Value = iturAnalyzes.SubSessionCode;
					cmd.Parameters["@SessionName"].Value = iturAnalyzes.SessionName;
					cmd.Parameters["@SubSessionName"].Value = iturAnalyzes.SubSessionName;
		
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
				_logger.ErrorException("InsertIturAnalyzes", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
		}



		// ====================== InsertIturAnalyzes ===============================================
		public void InsertIturAnalyzesSimple(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null)
		{

			IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesSimpleReader;
			//получаем данные от ридера - затем пишем в БД через ADO
			IIturAnalyzesReader iturAnalyzesReader =
			this._serviceLocator.GetInstance<IIturAnalyzesReader>(iturAnalyzesReaderEnum.ToString());

			//if (refill == true)
			//{
			//    this.ClearIturAnalyzes(pathDB);
			//}

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

			//IIturAnalyzesRepository iturAnalyzesRepository =
			//    this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			////Dictionary<string, Itur> iturDictionary = iturAnalyzesRepository.GetIturDictionary(pathDB, refill);
			////Dictionary<string, DocumentHeader> documentHeaderDictionary = iturAnalyzesRepository.GetDocumentHeaderDictionary(pathDB, refill);
			////Dictionary<string, ProductMakat> productMakatDictionary = iturAnalyzesRepository.GetProductMakatDictionary(pathDB, refill);
			//Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refill);

			this.Log.Add(MessageTypeEnum.TraceRepository, "IturAnalyzesReaderADORepository");
			_logger.Info("IturAnalyzesReaderADORepository :: InsertIturAnalyzesSimple");
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
		   "[Price]," +
		   "[PriceBuy]," +
		   "[PriceSale]," +
		   "[PriceExtra]," +
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
		   "@Price," +
		   "@PriceBuy," +
		   "@PriceSale," +
		   "@PriceExtra," +
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
					"@Price", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceBuy", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceSale", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceExtra", SqlDbType.Float));

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));

				IEnumerable<IturAnalyzes> iturAnalyzesEnumerable =
					iturAnalyzesReader.GetIturAnalyzes(pathDB, refill, refillCatalogDictionary, selectParms, parmsIn, false);

				foreach (IturAnalyzes iturAnalyzes in iturAnalyzesEnumerable)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}

					k++;
					cmd.Parameters["@SectionCode"].Value = DomainUnknownCode.UnknownSection;
					//cmd.Parameters["@LocationCode"].Value = iturAnalyzes.LocationCode;
					//cmd.Parameters["@DocumentHeaderCode"].Value = iturAnalyzes.DocumentHeaderCode;
					cmd.Parameters["@DocumentCode"].Value = iturAnalyzes.DocumentCode;
					cmd.Parameters["@IturCode"].Value = iturAnalyzes.IturCode;
					// cmd.Parameters["@ERPIturCode"].Value = iturAnalyzes.ERPIturCode;
					//cmd.Parameters["@Itur_Disabled"].Value = iturAnalyzes.Itur_Disabled;
					//cmd.Parameters["@Itur_Publishe"].Value = iturAnalyzes.Itur_Publishe;
					//cmd.Parameters["@Itur_StatusIturBit"].Value = iturAnalyzes.Itur_StatusIturBit;
					//cmd.Parameters["@Itur_Number"].Value = iturAnalyzes.Itur_Number;
					//cmd.Parameters["@Itur_NumberPrefix"].Value = iturAnalyzes.Itur_NumberPrefix;
					//cmd.Parameters["@Itur_NumberSufix"].Value = iturAnalyzes.Itur_NumberSufix;
					//cmd.Parameters["@Itur_LocationCode"].Value = iturAnalyzes.Itur_LocationCode;
					//cmd.Parameters["@Itur_StatusIturGroupBit"].Value = iturAnalyzes.Itur_StatusIturGroupBit;
					//cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = iturAnalyzes.Itur_StatusDocHeaderBit;
					//cmd.Parameters["@Doc_Name"].Value = iturAnalyzes.Doc_Name;
					//cmd.Parameters["@Doc_Approve"].Value = iturAnalyzes.Doc_Approve;
					//cmd.Parameters["@Doc_IturCode"].Value = iturAnalyzes.Doc_IturCode;
					//cmd.Parameters["@Doc_WorkerGUID"].Value = iturAnalyzes.Doc_WorkerGUID;
					//cmd.Parameters["@Doc_StatusDocHeaderBit"].Value = iturAnalyzes.Doc_StatusDocHeaderBit;
					//cmd.Parameters["@Doc_StatusInventProductBit"].Value = iturAnalyzes.Doc_StatusInventProductBit;
					//cmd.Parameters["@Doc_StatusApproveBit"].Value = iturAnalyzes.Doc_StatusApproveBit;
					// cmd.Parameters["@DocNum"].Value = iturAnalyzes.DocNum;
					cmd.Parameters["@Barcode"].Value = iturAnalyzes.Barcode;
					cmd.Parameters["@QuantityDifference"].Value = iturAnalyzes.QuantityDifference;
					cmd.Parameters["@QuantityEdit"].Value = iturAnalyzes.QuantityEdit;
					cmd.Parameters["@QuantityInPackEdit"].Value = iturAnalyzes.QuantityInPackEdit;
					cmd.Parameters["@QuantityOriginal"].Value = iturAnalyzes.QuantityOriginal;
					//cmd.Parameters["@SerialNumber"].Value = iturAnalyzes.SerialNumber;
					//cmd.Parameters["@ShelfCode"].Value = iturAnalyzes.ShelfCode;
					//cmd.Parameters["@ModifyDate"].Value = iturAnalyzes.ModifyDate;
					cmd.Parameters["@ProductName"].Value = iturAnalyzes.ProductName;
					//cmd.Parameters["@PDA_StatusInventProductBit"].Value = iturAnalyzes.PDA_StatusInventProductBit;
					//cmd.Parameters["@PDA_DocumentHeaderCode"].Value = iturAnalyzes.PDA_DocumentHeaderCode;
					cmd.Parameters["@Makat"].Value = iturAnalyzes.Makat;
					cmd.Parameters["@InputTypeCode"].Value = iturAnalyzes.InputTypeCode;
					cmd.Parameters["@IPNum"].Value = iturAnalyzes.IPNum;
					//cmd.Parameters["@BarcodeOriginal"].Value = iturAnalyzes.BarcodeOriginal;
					//cmd.Parameters["@MakatOriginal"].Value = iturAnalyzes.MakatOriginal;
					//cmd.Parameters["@PriceString"].Value = iturAnalyzes.PriceString;
					cmd.Parameters["@Price"].Value = iturAnalyzes.Price;
					cmd.Parameters["@PriceBuy"].Value = iturAnalyzes.PriceBuy;
					cmd.Parameters["@PriceSale"].Value = iturAnalyzes.PriceSale;
					cmd.Parameters["@PriceExtra"].Value = iturAnalyzes.PriceExtra;
					cmd.Parameters["@FromCatalogType"].Value = iturAnalyzes.FromCatalogType;
					cmd.Parameters["@SectionCode"].Value = iturAnalyzes.SectionCode;
					//cmd.Parameters["@SectionNum"].Value = iturAnalyzes.SectionNum;
					//cmd.Parameters["@TypeCode"].Value = iturAnalyzes.TypeCode;
					cmd.Parameters["@TypeMakat"].Value = iturAnalyzes.TypeMakat;
					cmd.Parameters["@ValueBuyQriginal"].Value = iturAnalyzes.ValueBuyQriginal;
					cmd.Parameters["@ValueBuyEdit"].Value = iturAnalyzes.ValueBuyEdit;
					cmd.Parameters["@ValueBuyDifference"].Value = iturAnalyzes.ValueBuyDifference;
					//cmd.Parameters["@PDA_ID"].Value = iturAnalyzes.PDA_ID;
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
		}


		// ============== InsertIturAnalyzesSumSimple ===================================
		public void InsertIturAnalyzesSumSimple(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, bool addResult = true, List<ImportDomainEnum> importType = null)
		{
			IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesSimpleSumReader;
			//получаем данные от ридера - затем пишем в БД через ADO
			IIturAnalyzesReader iturAnalyzesReader =
			this._serviceLocator.GetInstance<IIturAnalyzesReader>(iturAnalyzesReaderEnum.ToString());

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


			//if (inventProductList == null)
			//{
			//    inventProductList = iturAnalyzesRepository.GetIPSumQuantityByMakats(null, pathDB, true);
			//}

			//this.ClearIturAnalyzes(pathDB);

			//stopwatch = Stopwatch.StartNew();
			//Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refill);
			//this.ClearIturAnalyzes(pathDB);
			//stopwatch.Stop();
			//System.Diagnostics.Debug.Print(String.Format("GetProductSimpleDictionary: {0}", stopwatch.ElapsedTicks.ToString()));

			this.Log.Add(MessageTypeEnum.TraceRepository, "IturAnalyzesReaderADORepository");
			_logger.Info("IturAnalyzesReaderADORepository :: InsertIturAnalyzesSumSimple");
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
			"[Price]," +
			"[PriceBuy]," +
			"[PriceSale]," +
			"[PriceExtra]," +
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
			"[SupplierName], " +
			"[ResulteValue], " +
			"[ResulteDescription]," +
			"[IsResulte]" +
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
			"@Price," +
		   "@PriceBuy," +
			"@PriceSale," +
			"@PriceExtra," +
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
			"@SupplierName," +
			"@ResulteValue," +
			"@ResulteDescription," +
			"@IsResulte" +
				//"@PDA_ID" +
			")";

			stopwatch = Stopwatch.StartNew();

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
					"@Price", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceBuy", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceSale", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceExtra", SqlDbType.Float));
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
				cmd.Parameters.Add(new SqlCeParameter(
				"@ResulteValue", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ResulteDescription", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@IsResulte", SqlDbType.Bit));


				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));


				//все суммы IP 

				IEnumerable<IturAnalyzes> iturAnalyzesEnumerable =
					iturAnalyzesReader.GetIturAnalyzes(pathDB, refill, refillCatalogDictionary, selectParms, parmsIn, addResult);

				foreach (IturAnalyzes iturAnalyzes in iturAnalyzesEnumerable)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
					k++;
					//cmd.Parameters["@Code"].Value = iturAnalyzes.Code;
					//cmd.Parameters["@LocationCode"].Value = iturAnalyzes.LocationCode;
					//cmd.Parameters["@DocumentHeaderCode"].Value = iturAnalyzes.DocumentHeaderCode;
					//cmd.Parameters["@DocumentCode"].Value = iturAnalyzes.DocumentCode;
					//cmd.Parameters["@IturCode"].Value = iturAnalyzes.IturCode;
					//cmd.Parameters["@Itur_Disabled"].Value = iturAnalyzes.Itur_Disabled;
					//cmd.Parameters["@Itur_Publishe"].Value = iturAnalyzes.Itur_Publishe;
					//cmd.Parameters["@Itur_StatusIturBit"].Value = iturAnalyzes.Itur_StatusIturBit;
					//cmd.Parameters["@Itur_Number"].Value = iturAnalyzes.Itur_Number;
					//cmd.Parameters["@Itur_NumberPrefix"].Value = iturAnalyzes.Itur_NumberPrefix;
					//cmd.Parameters["@Itur_NumberSufix"].Value = iturAnalyzes.Itur_NumberSufix;
					//cmd.Parameters["@Itur_LocationCode"].Value = iturAnalyzes.Itur_LocationCode;
					//cmd.Parameters["@Itur_StatusIturGroupBit"].Value = iturAnalyzes.Itur_StatusIturGroupBit;
					//cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = iturAnalyzes.Itur_StatusDocHeaderBit;
					//cmd.Parameters["@Doc_Name"].Value = iturAnalyzes.Doc_Name;
					//cmd.Parameters["@Doc_Approve"].Value = iturAnalyzes.Doc_Approve;
					//cmd.Parameters["@Doc_IturCode"].Value = iturAnalyzes.Doc_IturCode;
					//cmd.Parameters["@Doc_WorkerGUID"].Value = iturAnalyzes.Doc_WorkerGUID;
					//cmd.Parameters["@Doc_StatusDocHeaderBit"].Value = iturAnalyzes.Doc_StatusDocHeaderBit;
					//cmd.Parameters["@Doc_StatusInventProductBit"].Value = iturAnalyzes.Doc_StatusInventProductBit;
					//cmd.Parameters["@Doc_StatusApproveBit"].Value = iturAnalyzes.Doc_StatusApproveBit;
					// cmd.Parameters["@DocNum"].Value = iturAnalyzes.DocNum;
					//cmd.Parameters["@Barcode"].Value = iturAnalyzes.Barcode;
					cmd.Parameters["@QuantityDifference"].Value = iturAnalyzes.QuantityDifference;
					cmd.Parameters["@QuantityEdit"].Value = iturAnalyzes.QuantityEdit;
					cmd.Parameters["@QuantityInPackEdit"].Value = iturAnalyzes.QuantityInPackEdit;
					cmd.Parameters["@QuantityOriginal"].Value = iturAnalyzes.QuantityOriginal;
					//cmd.Parameters["@SerialNumber"].Value = iturAnalyzes.SerialNumber;
					//cmd.Parameters["@ShelfCode"].Value = iturAnalyzes.ShelfCode;
					//cmd.Parameters["@ModifyDate"].Value = iturAnalyzes.ModifyDate;
					cmd.Parameters["@ProductName"].Value = iturAnalyzes.ProductName;
					//cmd.Parameters["@PDA_StatusInventProductBit"].Value = iturAnalyzes.PDA_StatusInventProductBit;
					//cmd.Parameters["@PDA_DocumentHeaderCode"].Value = iturAnalyzes.PDA_DocumentHeaderCode;
					cmd.Parameters["@Makat"].Value = iturAnalyzes.Makat;
					//cmd.Parameters["@InputTypeCode"].Value = iturAnalyzes.InputTypeCode;
					//cmd.Parameters["@IPNum"].Value = iturAnalyzes.IPNum;
					//cmd.Parameters["@BarcodeOriginal"].Value = iturAnalyzes.BarcodeOriginal;
					//cmd.Parameters["@MakatOriginal"].Value = iturAnalyzes.MakatOriginal;
					//cmd.Parameters["@PriceString"].Value = iturAnalyzes.PriceString;
					cmd.Parameters["@Price"].Value = iturAnalyzes.Price;
					cmd.Parameters["@PriceBuy"].Value = iturAnalyzes.PriceBuy;
					cmd.Parameters["@PriceSale"].Value = iturAnalyzes.PriceSale;
					cmd.Parameters["@PriceExtra"].Value = iturAnalyzes.PriceExtra;
					cmd.Parameters["@FromCatalogType"].Value = iturAnalyzes.FromCatalogType;
					cmd.Parameters["@SectionCode"].Value = iturAnalyzes.SectionCode;
					cmd.Parameters["@SectionName"].Value = iturAnalyzes.SectionName;
					//cmd.Parameters["@TypeCode"].Value = iturAnalyzes.TypeCode;
					cmd.Parameters["@TypeMakat"].Value = iturAnalyzes.TypeMakat;
					cmd.Parameters["@ValueBuyQriginal"].Value = iturAnalyzes.ValueBuyQriginal;
					cmd.Parameters["@ValueBuyEdit"].Value = iturAnalyzes.ValueBuyEdit;
					cmd.Parameters["@ValueBuyDifference"].Value = iturAnalyzes.ValueBuyDifference;
					cmd.Parameters["@Count"].Value = iturAnalyzes.Count;
					cmd.Parameters["@ResultCode"].Value = iturAnalyzes.ResultCode;
					cmd.Parameters["@IsUpdateERP"].Value = iturAnalyzes.IsUpdateERP;
					cmd.Parameters["@QuantityOriginalERP"].Value = iturAnalyzes.QuantityOriginalERP;
					cmd.Parameters["@CountInParentPack"].Value = iturAnalyzes.CountInParentPack;
					cmd.Parameters["@BalanceQuantityPartialERP"].Value = iturAnalyzes.BalanceQuantityPartialERP;
					cmd.Parameters["@ValueOriginalERP"].Value = iturAnalyzes.ValueOriginalERP;
					cmd.Parameters["@QuantityDifferenceOriginalERP"].Value = iturAnalyzes.QuantityDifferenceOriginalERP;
					cmd.Parameters["@ValueDifferenceOriginalERP"].Value = iturAnalyzes.ValueDifferenceOriginalERP;
					cmd.Parameters["@SupplierCode"].Value = iturAnalyzes.SupplierCode;
					cmd.Parameters["@SupplierName"].Value = iturAnalyzes.SupplierName;
					//cmd.Parameters["@PDA_ID"].Value = iturAnalyzes.PDA_ID;
					cmd.Parameters["@ResulteValue"].Value = iturAnalyzes.ResulteValue;
					cmd.Parameters["@ResulteDescription"].Value = iturAnalyzes.ResulteDescription;
					cmd.Parameters["@IsResulte"].Value = iturAnalyzes.IsResulte;
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
				_logger.ErrorException("InsertIturAnalyzesSumSimple", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			stopwatch.Stop();
			System.Diagnostics.Debug.Print(String.Format("productSimpleDictionary cmd.ExecuteNonQuery: {0}", stopwatch.ElapsedTicks.ToString()));

		}


		// ============== InsertIturAnalyzesFamily =================================================
		public void InsertIturAnalyzesFamily(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, bool addResult = true)
		{
			IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesReader;

			//получаем данные от ридера - затем пишем в БД через ADO
			IIturAnalyzesReader iturAnalyzesReader =
			this._serviceLocator.GetInstance<IIturAnalyzesReader>(iturAnalyzesReaderEnum.ToString());

			//if (selectParms != null)
			//{
			//    string selectParmsString = selectParms.ToString();
			//}
			//if (refill == true)
			//{
			//    this.ClearIturAnalyzes(pathDB);
			//}
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

			this.Log.Add(MessageTypeEnum.TraceRepository, "IturAnalyzesReaderADORepository");
			_logger.Info("IturAnalyzesReaderADORepository :: InsertIturAnalyzes");

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
		   "[Price]," +
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
		   "[LocationName], " +
				//"[IturName], " +
				//"[SessionCode], " +
				//"[SessionNum]" +
				  "[FamilyCode], " +
				  "[FamilyName], " +
				  "[FamilySize], " +
				  "[FamilyType], " +
				  "[FamilyExtra1], " +
				  "[FamilyExtra2], " +
			"[ValueChar] " +
			") " +
	 "VALUES(" +
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
		   "@QuantityEdit, " +
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
		   "@Price," +
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
			"@LocationName," +
				//"@IturName," +
				//"@SessionCode," +
				//"@SessionNum" +
			  "@FamilyCode, " +
			  "@FamilyName, " +
			 "@FamilySize, " +
			  "@FamilyType, " +
			  "@FamilyExtra1, " +
			  "@FamilyExtra2, " +
			"@ValueChar" +
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
					"@Price", SqlDbType.Float));
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
				cmd.Parameters.Add(new SqlCeParameter(
				"@FamilyCode", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyName", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilySize", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyType", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyExtra1", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyExtra2", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ValueChar", SqlDbType.NVarChar, 100));


				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				//this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));

				IEnumerable<IturAnalyzes> iturAnalyzesEnumerable =
					iturAnalyzesReader.GetIturAnalyzes(pathDB, refill, refillCatalogDictionary, selectParms, parmsIn, false);


				foreach (IturAnalyzes iturAnalyzes in iturAnalyzesEnumerable)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
					k++;
					//==================== PDA =========================================
					cmd.Parameters["@Code"].Value = iturAnalyzes.Code;
					cmd.Parameters["@LocationCode"].Value = iturAnalyzes.LocationCode;
					cmd.Parameters["@DocumentHeaderCode"].Value = iturAnalyzes.DocumentHeaderCode;
					cmd.Parameters["@DocumentCode"].Value = iturAnalyzes.DocumentCode;
					cmd.Parameters["@IturCode"].Value = iturAnalyzes.IturCode;
					cmd.Parameters["@ERPIturCode"].Value = iturAnalyzes.ERPIturCode;
					cmd.Parameters["@Itur_Disabled"].Value = iturAnalyzes.Itur_Disabled;
					cmd.Parameters["@Itur_Publishe"].Value = iturAnalyzes.Itur_Publishe;
					cmd.Parameters["@Itur_StatusIturBit"].Value = iturAnalyzes.Itur_StatusIturBit;
					cmd.Parameters["@Itur_Number"].Value = iturAnalyzes.Itur_Number;
					cmd.Parameters["@Itur_NumberPrefix"].Value = iturAnalyzes.Itur_NumberPrefix;
					cmd.Parameters["@Itur_NumberSufix"].Value = iturAnalyzes.Itur_NumberSufix;
					cmd.Parameters["@Itur_LocationCode"].Value = iturAnalyzes.Itur_LocationCode;
					cmd.Parameters["@Itur_StatusIturGroupBit"].Value = iturAnalyzes.Itur_StatusIturGroupBit;
					cmd.Parameters["@Itur_StatusDocHeaderBit"].Value = iturAnalyzes.Itur_StatusDocHeaderBit;
					cmd.Parameters["@Doc_Name"].Value = iturAnalyzes.Doc_Name;
					cmd.Parameters["@Doc_Approve"].Value = iturAnalyzes.Doc_Approve;
					//cmd.Parameters["@Doc_IturCode"].Value = iturAnalyzes.Doc_IturCode;
					cmd.Parameters["@Doc_WorkerGUID"].Value = iturAnalyzes.Doc_WorkerGUID;
					cmd.Parameters["@Doc_StatusDocHeaderBit"].Value = iturAnalyzes.Doc_StatusDocHeaderBit;
					cmd.Parameters["@Doc_StatusInventProductBit"].Value = iturAnalyzes.Doc_StatusInventProductBit;
					cmd.Parameters["@Doc_StatusApproveBit"].Value = iturAnalyzes.Doc_StatusApproveBit;
					cmd.Parameters["@DocNum"].Value = iturAnalyzes.DocNum;
					cmd.Parameters["@Barcode"].Value = iturAnalyzes.Barcode;
					cmd.Parameters["@QuantityDifference"].Value = iturAnalyzes.QuantityDifference;
					cmd.Parameters["@QuantityEdit"].Value = iturAnalyzes.QuantityEdit;
					cmd.Parameters["@QuantityInPackEdit"].Value = iturAnalyzes.QuantityInPackEdit;
					cmd.Parameters["@QuantityOriginal"].Value = iturAnalyzes.QuantityOriginal;
					//cmd.Parameters["@SerialNumber"].Value = iturAnalyzes.SerialNumber;
					//cmd.Parameters["@ShelfCode"].Value = iturAnalyzes.ShelfCode;
					cmd.Parameters["@ModifyDate"].Value = iturAnalyzes.ModifyDate;
					cmd.Parameters["@ProductName"].Value = iturAnalyzes.ProductName;
					cmd.Parameters["@PDA_StatusInventProductBit"].Value = iturAnalyzes.PDA_StatusInventProductBit;
					//cmd.Parameters["@PDA_DocumentHeaderCode"].Value = iturAnalyzes.PDA_DocumentHeaderCode;
					cmd.Parameters["@Makat"].Value = iturAnalyzes.Makat;
					cmd.Parameters["@InputTypeCode"].Value = iturAnalyzes.InputTypeCode;
					cmd.Parameters["@IPNum"].Value = iturAnalyzes.IPNum;
					cmd.Parameters["@BarcodeOriginal"].Value = iturAnalyzes.BarcodeOriginal;
					cmd.Parameters["@MakatOriginal"].Value = iturAnalyzes.MakatOriginal;
					cmd.Parameters["@PriceString"].Value = iturAnalyzes.PriceString;
					cmd.Parameters["@Price"].Value = iturAnalyzes.Price;
					cmd.Parameters["@PriceBuy"].Value = iturAnalyzes.PriceBuy;
					cmd.Parameters["@PriceSale"].Value = iturAnalyzes.PriceSale;
					cmd.Parameters["@PriceExtra"].Value = iturAnalyzes.PriceExtra;
					cmd.Parameters["@FromCatalogType"].Value = iturAnalyzes.FromCatalogType;
					cmd.Parameters["@SectionNum"].Value = iturAnalyzes.SectionNum;
					cmd.Parameters["@TypeCode"].Value = iturAnalyzes.TypeCode;
					cmd.Parameters["@TypeMakat"].Value = iturAnalyzes.TypeMakat;
					cmd.Parameters["@ValueBuyQriginal"].Value = iturAnalyzes.ValueBuyQriginal;
					cmd.Parameters["@ValueBuyEdit"].Value = iturAnalyzes.ValueBuyEdit;
					cmd.Parameters["@ValueBuyDifference"].Value = iturAnalyzes.ValueBuyDifference;
					cmd.Parameters["@PDA_ID"].Value = iturAnalyzes.PDA_ID;
					cmd.Parameters["@SectionCode"].Value = iturAnalyzes.SectionCode;
					cmd.Parameters["@SectionName"].Value = iturAnalyzes.SectionName;
					cmd.Parameters["@IsUpdateERP"].Value = iturAnalyzes.IsUpdateERP;
					cmd.Parameters["@ValueFloat"].Value = iturAnalyzes.ValueFloat;
					cmd.Parameters["@ValueChar"].Value = iturAnalyzes.ValueChar;
					cmd.Parameters["@QuantityOriginalERP"].Value = iturAnalyzes.QuantityOriginalERP;
					cmd.Parameters["@CountInParentPack"].Value = iturAnalyzes.CountInParentPack;
					cmd.Parameters["@BalanceQuantityPartialERP"].Value = iturAnalyzes.BalanceQuantityPartialERP;
					cmd.Parameters["@ValueOriginalERP"].Value = iturAnalyzes.ValueOriginalERP;
					cmd.Parameters["@QuantityDifferenceOriginalERP"].Value = iturAnalyzes.QuantityDifferenceOriginalERP;
					cmd.Parameters["@ValueDifferenceOriginalERP"].Value = iturAnalyzes.ValueDifferenceOriginalERP;
					//cmd.Parameters["@SupplierCode"].Value = iturAnalyzes.SupplierCode;
					//cmd.Parameters["@SupplierName"].Value = iturAnalyzes.SupplierName;
					cmd.Parameters["@LocationName"].Value = iturAnalyzes.LocationName;
					cmd.Parameters["@FamilyCode"].Value = iturAnalyzes.FamilyCode;
					cmd.Parameters["@FamilyName"].Value = iturAnalyzes.FamilyName;
					cmd.Parameters["@FamilySize"].Value = iturAnalyzes.FamilySize;
					cmd.Parameters["@FamilyType"].Value = iturAnalyzes.FamilyType;
					cmd.Parameters["@FamilyExtra1"].Value = iturAnalyzes.FamilyExtra1;
					cmd.Parameters["@FamilyExtra2"].Value = iturAnalyzes.FamilyExtra2;

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
				_logger.ErrorException("InsertIturAnalyzes", error);
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
