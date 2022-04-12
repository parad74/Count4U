using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public enum LocationParserEnum
	{
		LocationParser = 0,
		LocationFromDBParser = 1,
		LocationEmulationParser = 2,
		LocationXlsxParser,
		LocationUpdateTagParser,
		LocationMerkavaXslx2SdfParser,
		LocationUpdateMerkavaSqlite2SdfParser,
		LocationUpdateClalitSqlite2SdfParser,
		LocationClalitXslx2SdfParser,
		LocationNativXslx2SdfParser,
		LocationUpdateNativSqlite2SdfParser,
		LocationYesXlsxParserSN,
		LocationYesXlsxParserQ,
		LocationNativPlusLadpcParser,
		LocationMultyCsvParser,
	}


	public enum DocumentHeaderParseEnum
	{
		DocumentHeaderFromDBParser,
		DocumentHeaderAddFristDocToIturParser,
		DocumentHeaderAddSpetialDocToIturParser

	}

	public enum UnitPlanParserEnum
	{
		UnitPlanParser,
		UnitPlanFromDBParser
	
	}

	public enum SectionParserEnum
	{
		SectionNetPOSSuperPharmParser = 0,
		SectionDBParser = 1,
		SectionGeneralCSVParser,
		SectionMikramSonolParser,
		SectionNimrodAvivParser,
		SectionH_MParser,
		SectionFRSVisionMirkamParser,
		SectionMPLParser,
		SectionTafnitMatrixParser,
		SectionGeneralXLSXParser,
		SectionSapb1ZometsfarimParser,
		SectionSapb1ZometsfarimParser1,
		SectionGazitVerifoneSteimaztzkyParser,
		SectionGazitVerifoneSteimaztzkyParser1,
		SectionAS400MegaParser,
		SectionAS400HamashbirParser,
		SectionH_M_NewParser,
		SectionNativXslx2SdfParser,
		SectionNativXslx2SdfParser1,
		SectionGazitGlobalXlsxParser,
	}

	public enum SupplierParserEnum
	{
		SupplierDefaultParser,
		SupplierDBParser,
		SupplierAvivPOSParser,
		SupplierComaxASPParser,
		SupplierMade4NetParser,
		SupplierNibitParser,
		SupplierAS400AprilParser,
		SupplierFRSVisionMirkamParser,
		SupplierGeneralXLSXParser,
		SupplierSapb1ZometsfarimParser,
		SupplierAS400MegaParser,
		SupplierNativXslx2SdfParser
	}


	public enum FamilyParserEnum
	{
		FamilyPriorityRenuarParser,
		FamilyLadyComfortParser,
		FamilyH_MParser,
		FamilyPriorityKedsShowRoomParser,
		FamilyTafnitMatrixParser,
		FamilyDefaultParser,
		FamilyAS400HamashbirParser,
		FamilyGeneralXLSXParser,
		FamilyH_M_NewParser,
		FamilyNativXslx2SdfParser
	}

	public enum IturAnalyzesReaderEnum
	{
		IturAnalyzesReader = 0,
		IturAnalyzesSimpleReader = 1,
		IturAnalyzesSimpleSumReader = 2,
		IturAnalyzesFamilyReader
	}

	public enum CurrentInventoryAdvancedReaderEnum
	{
		CurrentInventoryAdvancedReader = 0
	}

	public enum IturAnalyzesFamilyReaderEnum
	{
		FamilyAnalyzesReader = 0,
		FamilyAnalyzesSortLocationIturMakatReader = 0,
	}

	public enum IturParserEnum
	{
		IturParser = 0,
		IturDBParser = 1,
		IturERPParser = 2,
		IturFacingParser,
		IturMerkavaSqliteXslxParser,
		IturMerkavaXslx2SdfParser,
		IturUpdateMerkavaSqlite2SdfParser,
		IturUpdateClalitSqlite2SdfParser,
		IturClalitXslx2SdfParser,
		IturUpdateNativSqlite2SdfParser,
		IturNativXslx2SdfParser,
		IturFromDBParser1,
		IturUpdateNativSimpleSqlite2SdfParser,
		IturUpdateNativXslx2SdfParser,
		IturNativXslx2SdfUpdateUpLevelParser,
		IturMerkavaXslx2SdfUpdateUpLevelParser,
		IturYesXlsxParserQ,
		IturYesXlsxParserSN,
		IturNativPlusLadpcParser1,
		IturNativPlusLadpcParser9999,
		IturMultiCsv2SdfParser,
		IturMultiDelete9999999Parser,
		IturMultiCsv2SdfParser1,
	}

	public enum TemporaryInventoryParserEnum
	{
		TemporaryInventoryMerkavaSqlite2SdfParser
	}

	public enum PropertyStrParserEnum
	{
		PropertyStr6MerkavaXslx2SdfParser,
		PropertyStr7MerkavaXslx2SdfParser,
		PropertyStr7MerkavaXslxParser1,
		PropertyStrBuildingConfigMerkavaXslx2SdfParser,
		PropertyStr6MerkavaXslx2SdfParser1,
		PropertyStrBuildingConfigClalitXslx2SdfParser,
		PropertyStrClalitXslx2SdfParser1,
		PropertyStrClalitXslx2SdfParser2,
		PropertyStrClalitXslx2SdfParser3,
		PropertyStrClalitXslx2SdfParser4,
		PropertyStrClalitXslx2SdfParser5,
		PropertyStrBuildingConfigNativXslx2SdfParser,
		PropertyStr1NativXslx2SdfParser,
		PropertyStr2NativXslx2SdfParser,
		PropertyStr3NativXslx2SdfParser,
		PropertyStr18MerkavaXslx2SdfParser,
		PropertyStr4NativXslx2SdfParser,
		PropertyStr5NativXslx2SdfParser,
		PropertyStr6NativXslx2SdfParser,
		PropertyStr7NativXslx2SdfParser,
		PropertyStr9NativXslx2SdfParser,
		PropertyStrNativSqlite2SdfParser,
		PropertyStr8NativXslx2SdfParser,
		PropertyStr10NativXslx2SdfParser,
		PropertyStrPropertyDecoratorNativXslx2SdfParser,
		PropertyStrProfileNativXslx2SdfParser,
		PropertyStrProfileXml2SdfParser,
		PropertyStrPropertyDecoratorNativExportErpParser1,
		PropertyStrPropertyDecoratorNativExportErpParser2,
		PropertyStrPropertyDecoratorNativExportErpParser3,
		PropertyStrPropertyDecoratorNativExportErpParser4,
	}

	public enum FileParserEnum
	{
		CsvFileParser = 0,
		ExcelFileParser = 1,
		SqliteFileParser,
		ExcelMacrosFileParser
	}

	public enum BranchParserEnum
	{
		BranchDefaultParser = 0,
		//BranchDBParser = 1,
		BranchGazitVerifoneParser = 2,
		BranchXtechMeuhedetParser = 3,
		BranchMikramSonolParser,
		BranchAS400LeumitParser,
		BranchAS400HonigmanParser,
		BranchPriorityCastroParser,
		BranchDefaultXlsxParser,
		BranchXtechMeuhedetXlsxParser
	}



	public enum CatalogSQLiteParserEnum
	{
		CatalogMerkavaXslx2SqliteParser,
		CatalogMerkavaSdf2SqliteParser,
		CatalogClalitSdf2SqliteParser,
		CatalogNativSdf2SqliteParser,
		CatalogNativPlusMISSdf2SqliteParser,
	}

	public enum CurrentInventorSQLiteParserEnum
	{
		//CurrentInventorMerkavaSqliteXslxParser,
		CurrentInventoryMerkavaSdf2SqliteParser,
		CurrentInventoryClalitSdf2SqliteParser,
		CurrentInventoryNativSdf2SqliteParser,
		CurrentInventoryNativPlusSdf2SqliteParser,
	}


	public enum LocationSQLiteParserEnum
	{
		LocationMerkavaXslx2SqliteParser,
		LocationMerkavaSdf2SqliteParser,
		LocationMerkavaSqlite2SdfParser,
		IturMerkavaSdf2SqliteParser,
		LocationClalitSdf2SqliteParser,
		LocationNativSdf2SqliteParser,
		LocationNativPlusMISSdf2SqliteParser,
		LocationNativPlusMISSdf2SqliteParserERP,
		LocationNativPlusMISSdf2SqliteParserIturCode,
	}

	public enum BuildingConfigParserEnum
	{
		//BuildingConfigMerkavaSqliteXslxParser,
		BuildingConfigMerkavaXslxParser,
		BuildingConfigMerkavaSdf2SqliteParser,
		BuildingConfigClalitSdf2SqliteParser,
		BuildingConfigNativSdf2SqliteParser,
		BuildingConfigNativPlusMISSdf2SqliteParser,
	}

	public enum PreviousInventorySQLiteParserEnum
	{
		PreviousInventoryMerkavaXslx2SqliteParser,
		PreviousInventoryMerkavaSdf2SqliteParser,
		PreviousInventoryMerkavaXslx2DbSetParser,
		PreviousInventoryClalitSdf2SqliteParser,
		PreviousInventoryClalitXslx2DbSetParser,
		PreviousInventoryNativXslx2DbSetParser,
		PreviousInventoryNativSdf2SqliteParser,
		PreviousInventoryNativPlusXslx2DbSetParser,
		PreviousInventoryMerkavaXslx2DictiontyParser,
		PreviousInventoryNativPlusYesXlsxDbSetParserQ,
		PreviousInventoryNativPlusYesXlsxDbSetParserSN,
		PreviousInventoryNativPlusLadpcDbSetParser,
		PreviousInventoryNativPlusLadpc2DbSetParser
	}

	public enum TemporaryInventorySQLiteParserEnum
	{
		TemporaryInventoryFromDeletedInventorySqlite2SdfParser,
		TemporaryInventoryFromAddedInventorySqlite2SdfParser,
		//TemporaryInventoryClalitSqlite2SdfParser,
		//TemporaryInventoryNativSqlite2SdfParser

	}

	public enum TemplateInventorySQLiteParserEnum
	{
		TemplateInventoryFromTemplateSqlite2SdfParser,
		TemplateInventoryNativPlusXslx2DbSetParser	,
		TemplateInventoryNativSdf2SqliteParser

	}

	public enum PropertyStrListSQLiteParserEnum
	{
		PropertyStrListDefaultXslx2SqliteParser,
		PropertyStrListMerkavaSdf2SqliteParser,
		PropertyStrListClalitSdf2SqliteParser,
		PropertyStrListNativSdf2SqliteParser,
	}

	public enum PropertyStrListSQLiteTableNameEnum
	{
		PropertyStr1List,
		PropertyStr2List,
		PropertyStr3List,
		PropertyStr4List,
		PropertyStr5List,
		PropertyStr6List,
		PropertyStr7List,
		PropertyStr8List,
		PropertyStr9List,
		PropertyStr10List,
		PropertyStr11List,
		PropertyStr12List,
		PropertyStr13List,
		PropertyStr14List,
		PropertyStr15List,
		PropertyStr16List,
		PropertyStr17List,
		PropertyStr18List,
		PropertyStr19List,
		PropertyStr20List
	}

	public enum PropStrNameEnum
	{
		PropStr1Name,
		PropStr2Name,
		PropStr3Name,
		PropStr4Name,
		PropStr5Name,
		PropStr6Name,
		PropStr7Name,
		PropStr8Name,
		PropStr9Name,
		PropStr10Name,
		PropStr11Name,
		PropStr12Name,
		PropStr13Name,
		PropStr14Name,
		PropStr15Name,
		PropStr16Name,
		PropStr17Name,
		PropStr18Name,
		PropStr19Name,
		PropStr20Name,
	}
	public enum PropStrCodeEnum
	{
		PropStr1Code,
		PropStr2Code,
		PropStr3Code,
		PropStr4Code,
		PropStr5Code,
		PropStr6Code,
		PropStr7Code,
		PropStr8Code,
		PropStr9Code,
		PropStr10Code,
		PropStr11Code,
		PropStr12Code,
		PropStr13Code,
		PropStr14Code,
		PropStr15Code,
		PropStr16Code,
		PropStr17Code,
		PropStr18Code,
		PropStr19Code,
		PropStr20Code
	}

	public enum ProductSimpleParserEnum
	{
		ProductCatalogForUnizagParser = 0,
		ProductCatalogForUnizagParser1 = 1,
		ProductCatalogForComaxASPParser = 2,
		ProductCatalogForComaxASPParser1 = 3,
		ProductCatalogForGazitVerifoneParser = 4,
		ProductHamarotForGazitVerifoneParser1 = 5,
		ProductHamarotForGazitVerifoneParser2 = 6,
		ProductCatalogForPriorityRenuarParser = 7,
		ProductCatalogForPriorityRenuarParser1 = 8,
		ProductFromDBParser = 9,
		ProductCatalogForNetPOSSuperPharmParser = 10,
		ProductCatalogForNetPOSSuperPharmParser1 = 11,
		ProductNetPOSSuperPharmParserUpdateERPQuentetyDBParser = 12,
		ProductFromDBUpdateERPQuentetyParser = 13,
		ProductCatalogForXtechMeuhedetParser1 = 14,
		ProductCatalogForXtechMeuhedetParser2 = 15,
		ProductCatalogForXtechMeuhedetUpdateERPQuentetyDBParser = 16,
		ProductCatalogForYarpaParser,
		ProductCatalogForYarpaUpdateERPQuentetyDBParser1,
		ProductCatalogForYarpaUpdateERPQuentetyDBParser2,
		ProductCatalogForYarpaParser1,
		ProductCatalogForYarpaUpdateERPQuentetyWindowsDBParser1,
		ProductCatalogForYarpaUpdateERPQuentetyWindowsDBParser2,
		ProductCatalogForYarpaWindowsParser,
		ProductCatalogForYarpaWindowsParser1,
		ProductCatalogForGeneralCSVUpdateERPQuentetyDBParser1,
		ProductCatalogForGeneralCSVParser1,
		ProductCatalogForGeneralCSVParser,
		ProductCatalogAvivPOSParser,
		ProductCatalogAvivPOSParser1,
		ProductCatalogAvivPOSUpdateERPQuentetyDBParser1,
		ProductCatalogAvivMultiParser,
		ProductCatalogAvivMultiParser1,
		ProductCatalogAvivMultiUpdateERPQuentetyDBParser1,
		ProductCatalogMikramSonolParser,
		ProductCatalogMikramSonolParser1,
		ProductCatalogMikramSonolUpdateERPQuentetyDBParser1,
		ProductCatalogPriorityKedsUpdateERPQuentetyDBParser1,
		ProductCatalogPriorityKedsParser1,
		ProductCatalogPriorityKedsParser,
		ProductCatalogAvivPOSParser2,
		ProductNetPOSSuperPharmParser,
		ProductCatalogAvivPOSParser3,
		ProductRetalixPosHOParserUpdateERPQuentetyDBParser,
		ProductRetalixPosHOParser,
		ProductCatalogAS400LeumitParser,
		ProductCatalogAS400LeumitParser1,
		ProductCatalogAS400LeumitUpdateERPQuentetyDBParser1,
		ProductCatalogMiniSoftParser,
		ProductCatalogMiniSoftParser1,
		ProductCatalogRetalixNextParser1,
		ProductCatalogRetalixNextParser,
		ProductRetalixNextUpdateERPQuentetyDBParser,
		ProductRetalixNextParser,
		ProductCatalogOne1UpdateERPQuentetyDBParser1,
		ProductCatalogOne1Parser1,
		ProductCatalogOne1Parser,
		ProductCatalogAS400AmericanEagleParser,
		ProductCatalogAS400AmericanEagleParser1,
		ProductCatalogMaccabiPharmSAPParser,
		ProductCatalogMaccabiPharmSAPParser1,
		ProductCatalogMaccabiPharmSAPUpdateERPQuentetyParser,
		ProductCatalogMaccabiPharmSAPParser2,
		ProductCatalogComaxASPMultiBarcodeParser,
		ProductCatalogComaxASPMultiBarcodeParser1,
		ProductCatalogComaxASPMultiBarcodeParser2,
		ProductCatalogMikiKupotParser,
		ProductCatalogLadyComfortParser,
		ProductCatalogMade4NetParser,
		ProductCatalogMade4NetParser1,
		ProductCatalogAS400JaforaParser,
		ProductCatalogNibitParser,
		ProductCatalogNibitParser1,
		ProductCatalogNibitUpdateERPQuentetyDBParser,
		ProductCatalogOrenOriginalsParser,
		ProductCatalogOrenOriginalsParser1,
		ProductCatalogNimrodAvivParser,
		ProductCatalogNimrodAvivParser1,
		ProductCatalogNimrodAvivParser2,
		ProductCatalogH_MParser,
		ProductCatalogH_MParser1,
		ProductCatalogH_MParser2,
		ProductCatalogH_MParser3,
		ProductCatalogAS400AprilParser,
		ProductCatalogAS400AprilParser1,
		ProductCatalogAS400AprilUpdateERPQuentetyParser,
		ProductCatalogNesherParser,
		ProductCatalogPriorityKedsShowRoomParser,
		ProductCatalogHashParser,
		ProductCatalogHashParser1,
		ProductCatalogNibitParser0,
		ProductCatalogAS400MangoParser1,
		ProductCatalogAS400MangoParser,
		ProductCatalogFRSVisionMirkamParser1,
		ProductCatalogFRSVisionMirkamParser,
		ProductCatalogForUnizagParser2,
		ProductCatalogForUnizagParser3,
		ProductCatalogAS400HonigmanParser,
		ProductCatalogAS400HonigmanParser1,
		ProductCatalogAS400HonigmanParser2,
		ProductCatalogMPLUpdateERPQuentetyDBParser1,
		ProductCatalogMPLParser2,
		ProductCatalogMPLParser1,
		ProductCatalogMPLParser,
		ProductCatalogAS400AmericanEagleParser2,
		ProductCatalogAS400AmericanEagleParser3,
		ProductCatalogTafnitMatrixParser1,
		ProductCatalogTafnitMatrixParser,
		ProductCatalogTafnitMatrixUpdateERPQuentetyDBParser,
		ProductCatalogTafnitMatrixParser2,
		ProductCatalogPriorityCastroParser,
		ProductCatalogPriorityCastroParser1,
		ProductCatalogForGeneralXLSXParser,
		ProductCatalogForGeneralXLSXParser1,
		ProductCatalogForGeneralXLSXUpdateERPQuentetyDBParser1,
		ProductCatalogOrenOriginalsParser2,
		ProductCatalogOrenOriginalsUpdateERPQuentetyDBParser1,
		ProductCatalogOtechUpdateERPQuentetyDBParser1,
		ProductCatalogOtechParser1,
		ProductCatalogOtechParser,
		ProductCatalogOtechParser2,
		ProductCatalogNikeIntParser,
		ProductCatalogNikeIntParser1,
		ProductCatalogNikeIntParser2,
		ProductCatalogNikeIntUpdateERPQuentetyDBParser,
		ProductCatalogWarehouseXslxParser,
		ProductCatalogWarehouseXslxParser1,
		ProductCatalogWarehouseXslxParser3,
		ProductCatalogWarehouseXslxParser2,
		ProductCatalogSapb1XslxParser,
		ProductCatalogSapb1XslxParser1,
		ProductCatalogSapb1XslxParser2,
		ProductCatalogSapb1XslxUpdateERPQuentetyDBParser1,
		ProductCatalogMerkavaSqliteXslxParser,
		ProductCatalogMerkavaXslx2SdfParser,
		ProductCatalogMerkavaXslx2SdfParser1,
		ProductCatalogMerkavaXslx2SdfUpdateERPQuentetyDBParser1,
		ProductCatalogSapb1ZometsfarimParser,
		ProductCatalogSapb1ZometsfarimParser1,
		ProductCatalogSapb1ZometsfarimParser2,
		ProductCatalogSapb1ZometsfarimParser3,
		ProductCatalogSapb1ZometsfarimUpdateERPQuentetyDBParser,
		ProductCatalogGazitVerifoneSteimaztzkyParser,
		ProductCatalogGazitVerifoneSteimaztzkyParser1,
		ProductCatalogGazitVerifoneSteimaztzkyUpdateERPQuentetyDBParser,
		ProductCatalogClalitXslx2SdfParser,
		ProductCatalogAS400MegaParser,
		ProductCatalogAS400MegaParser1,
		ProductCatalogAS400MegaUpdateERPQuentetyParser,
		ProductCatalogNativXslx2SdfParser,
		ProductCatalogNativSqlite2SdfParser,
		ProductCatalogNativXslx2SdfUpdateERPQuentetyDBParser1,
		ProductCatalogPriorityAldoParser1,
		ProductCatalogPriorityAldoParser,
		ProductCatalogPrioritySweetGirlXLSXParser,
		ProductCatalogAS400HamashbirParser2,
		ProductCatalogAS400HamashbirParser1,
		ProductCatalogAS400HamashbirParser,
		ProductCatalogAS400HamashbirUpdateERPQuentetyParser,
		ProductCatalogAS400HamashbirUpdateERPQuentetyParser2,
		ProductCatalogAS400HamashbirUpdateERPQuentetyParser1,
		ProductCatalogGazitAlufHaSportXlsxParser,
		ProductCatalogAS400HoParser,
		ProductCatalogAS400HoParser1,
		ProductCatalogAS400HoParser2_2,
		ProductCatalogAS400HoParser2_1,
		ProductCatalogYesXslxParserSN,
		ProductCatalogYesXslxParserQ,
		ProductCatalogNativMISSqlite2SdfParser,
		ProductCatalogXtechMeuhedetXLSXParser,
		ProductCatalogXtechMeuhedetXLSXParser1,
		ProductCatalogXtechMeuhedetXLSXUpdateERPQuentetyDBParser1,
		ProductCatalogXtechMeuhedetXLSXParser2,
		ProductCatalogH_M_NewParser3,
		ProductCatalogH_M_NewParser2,
		ProductCatalogH_M_NewParser1,
		ProductCatalogH_M_NewParser,
		ProductCatalogPrioritytEsteeLouderXslxParser,
		ProductCatalogForGeneralXLSXUpdateERPQuentetyDBParser2,
		ProductCatalogGazitLeeCooperParser1,
		ProductCatalogGazitLeeCooperParser,
		ProductCatalogNativPlusLadpcParser,
		ProductCatalogGazitGlobalXlsxParser,
		ProductCatalogPrioritytEsteeLouderUpdateERPQuentetyDBParser,
		ProductCatalogOrenMutagimParser,
		ProductCatalogOrenMutagimParser1,
		ProductCatalogYtungXlsxParser,
		ProductCatalogAutosoftParser,
		ProductCatalogAutosoftParser1,
		ProductCatalogAutosoftParser2
	}


	public enum InventProductSimpleParserEnum
	{
		InventProductSimpleParser = 0,
		InventProductParser = 1,
		InventProductFromDBParser = 2,
		InventProductFromRepository = 3,
		InventProductSimpleYarpaParser,
		InventProductMisParser,
		InventProductWarehouseParser,
		InventProductDB3Parser,
		InventProductMerkavaSqlite2SdfParser,
		InventProductClalitSqlite2SdfParser,
		InventProductNativSqlite2SdfParser,
		InventProductDefaultBackupParser,
		InventProductToMalamXMLParser,
		InventProductToMalamDataSetParser,
		InventProductUpdateDBParser,
		InventProductUpdateDBParser2,
		InventProductUpdateDBParser3,
		InventProductMinusByMakatFromDBParser,
		InventProductUpdate2SumByIturMakatFromDBParser,
		InventProductUpdate2SumByIturDocMakatFromDBParser,
		InventProductNativPlusSqlite2SdfParser,
		ImportInventProductUpdateCompare2SumByIturMakatDbBulkProvider,
		InventProductUpdateCompare2SumByIturMakatFromDBParser,
		InventProductUpdateCompare2SumByIturMakatFromDBParser2,
		InventProductUpdateCompare2SumByIturMakatFromDBParser1,
		InventProductUpdateBarcodeFromDBParser,
		InventProductUpdate2MakatAndSNFromDBParser,
		InventProductUpdateMakat2BarcodeDBParser,
		InventProductUpdate2SumByIturMakatSNumberFromDBParser,
		InventProductUpdate2SumByIturBarcodeSNumberFromDBParser,
		InventProductUpdate2BarcodeAndSNFromDBParser,
		InventProductUpdate2SumByIturBarcodeFromDBParser,
		InventProductYesXlsxParserQ,
		InventProductYesXlsxParserSN,
		InventProductNativPlusMISSqlite2SdfParser,
		InventProductFromDBAfterCompareParser,
		InventProductUpdateDBParser4,
		InventProductMerkavaXlsxParserSN,
		InventProductMerkavaXlsxParserQ,
		InventProductFromNativDBParser,
		InventProductUpdate2SumByIturBarcodeSNumberProp10FromDBParser,
		InventProductUpdate2SumByIturMakatSNumberProp10FromDBParser,
		InventProductMultiCsvParser,
		InventProductMISSqlite2SdfParser,
	}

	public enum StatusInventProductSimpleParserEnum
	{
		StatusInventProductNativPlusSqlite2SdfParser,
		StatusInventProductUpdateSumNativPlusSqlite2SdfParser
	}

	public enum ImportShelfParserEnum
	{
		ShelfFacingParser 
	}

	public enum InventProductAdvancedParserEnum
	{
		None,
		InventProductAdvancedParser,
		InventProductAdvancedYarpaParser,

	}

	public enum SeparatorEnum
	{
		Comma = 0,
		Equal = 1,
		Dot = 2,
		DotComma = 3
	}

	public static class SeparatorField
	{
		public const string Comma = ",";
		public const string Equal = "=";
		public const string Dot = ".";
		public const string DotComma = ";";
		public const string Empty = " ";
		public const string Cr = "^";
		public const string Tab = "\t";
		public const string I = "|";
		public const string Diez = "#";
	}


	//public enum ImportDomainEnum
	//{
	//    ImportInventProduct = 0,
	//    ImportDocumentHeader = 1,
	//    ImportMakat = 2,
	//    ImportItur = 3,
	//    ImportSession = 4,
	//    ImportLocation = 5,
	//    ImportCatalog = 6,
	//    ImportSupplier = 7,
	//    ImportParentMakat = 8,
	//    ImportParentCatalog = 9,
	//    ExistMakat = 10,
	//    ExistItur = 11,
	//    ExistBarcode = 12,
	//    ImportDBCatalog = 13,
	//    ImportParentProduct = 14,
	//    MakatApplyMask = 15,
	//    BarcodeApplyMask = 16,
	//    ExportCatalog = 18,
	//    ExportItur = 19,
	//    ExportUserIni = 20,
	//    ExportCustomerConfig = 21,
	//    Any = 22,
	//    None = 23
	//}

	
	public static class ParserFileErrorMessage
	{
		public static string FileIsNotExist = Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist;//"File : [ {0} ] Is not Exist";
		public static string FileXlsxExpected = Localization.Resources.Log_ParserFileErrorMessage_FileXlsxExpected;//"File : [ {0} ] must have .xlsx Format";
		public static string FileTxtExpected = Localization.Resources.Log_ParserFileErrorMessage_FileTxtExpected; //"File : [ {0} ] must be text File";
		public static string FileIsEmpty = Localization.Resources.Log_ParserFileErrorMessage_FileIsEmpty;//"File : [ {0} ] Is Empty or Is not Exist";
		public static string NoHeaderLine = Localization.Resources.Log_ParserFileErrorMessage_NoHeaderLine;//"There Is no Header Line or not the Marker Header in the First Line of File :  [ {0} ]";
		public static string NoOneDataRow = Localization.Resources.Log_ParserFileErrorMessage_NoOneDataRow;//"There Is no One Data Row in File : [ {0} ]";
		public static string NoMatchNumberSubstrings = Localization.Resources.Log_ParserFileErrorMessage_NoMatchNumberSubstrings;//"Does not Match the Number of Substrings in the Data Row with the Expected :  [ {0} ]";
		public static string NoExpectedLengthString = Localization.Resources.Log_ParserFileErrorMessage_NoExpectedLengthString;//"Length of Input String does not match the Expected Length : [ {0} ]";
		public static string NoExpectedMarker = Localization.Resources.Log_ParserFileErrorMessage_NoExpectedMarker;//"Data Row no Expected Marker : [ {0} ]";
		public static string MakatNotExistInDB = Localization.Resources.Log_ParserFileErrorMessage_MakatNotExistInDB;//"Makat [ {0} ] Exist in Data Row, but Not Exist in Catalog as Makat";
		public static string MakatExistInDB = Localization.Resources.Log_ParserFileErrorMessage_MakatExistInDB;//"Same Makat [ {0} ] Exist in DB";
		public static string BarcodeNotExistInDB = Localization.Resources.Log_ParserFileErrorMessage_BarcodeNotExistInDB;//"Barcode [ {0} ] Exist in Data Row, but Not Exist in Catalog as Barcode";
		public static string BarcodeExistInDB = Localization.Resources.Log_ParserFileErrorMessage_BarcodeExistInDB;//"Same Barcode [ {0} ] Exist in DB";
		public static string BarcodeIsEmpty = Localization.Resources.Log_ParserFileErrorMessage_BarcodeIsEmpty;//"Barcode : [ {0} ] Is Empty";
		public static string MakatIsEmpty = Localization.Resources.Log_ParserFileErrorMessage_MakatIsEmpty;//"Makat : [ {0} ] Is Empty";
		public static string ParentMakatIsEmpty = Localization.Resources.Log_ParserFileErrorMessage_ParentMakatIsEmpty;//"Parent Makat : [ {0} ] Is Empty";
		public static string IturCodeNotExistInDB = Localization.Resources.Log_ParserFileErrorMessage_IturCodeNotExistInDB;//"IturCode [ {0} ] Exist in Data Row, but Not Exist in Itur as Code";
		public static string IturCodeExistInDB = Localization.Resources.Log_ParserFileErrorMessage_IturCodeExistInDB;//"Itur with Same Code [ {0} ] Exist in DB";
		public static string IturCodeIsEmpty = Localization.Resources.Log_ParserFileErrorMessage_IturCodeIsEmpty;//"IturCode : [ {0} ] Is Empty";
		public static string DocumentCodeExistInDB = Localization.Resources.Log_ParserFileErrorMessage_DocumentCodeExistInDB;//"Document with Same Code [ {0} ] Exist in DB";
		public static string SessionCodeExistInDB = Localization.Resources.Log_ParserFileErrorMessage_SessionCodeExistInDB;//"Session with Same Code [ {0} ] Exist in DB";
		public static string LocationCodeNotExistInDB = Localization.Resources.Log_ParserFileErrorMessage_LocationCodeNotExistInDB;//"LocationCode [ {0} ] Exist in Data Row, but Not Exist in Location as Code";
		public static string LocationCodeExistInDB = Localization.Resources.Log_ParserFileErrorMessage_LocationCodeExistInDB;//"Location with Same Code [ {0} ] Exist in DB";
		public static string LocationCodeIsEmpty = Localization.Resources.Log_ParserFileErrorMessage_LocationCodeIsEmpty;//"LocationCode : [ {0} ] Is Empty";
		public static string MakatAndBarcodeNotExistInDB = Localization.Resources.Log_ParserFileErrorMessage_MakatAndBarcodeNotExistInDB;//"Barcode (or Makat) [ {0} ] Exist in Data Row, but Not Exist in Catalog";
		public static string Warning = Localization.Resources.Log_ParserFileErrorMessage_Warning;//"Parser Warning : " ;
		public static string Error = Localization.Resources.Log_ParserFileErrorMessage_Error;//"Parser Error : ";
		public static string BranchCodeExistInDB = Localization.Resources.Log_ParserFileErrorMessage_BranchCodeExistInDB;//"Same BranchCode [ {0} ] Exist in DB";
		public static string TableNameIsEmpty = Localization.Resources.Log_ParserFileErrorMessage_TableNameIsEmpty;
		public static string TableNotExistInDB = Localization.Resources.Log_ParserFileErrorMessage_TableNotExistInDB;
		public static string KeyIsEmpty = Localization.Resources.Log_ParserFileErrorMessage_KeyIsEmpty;//"Key : [ {0} ] Is Empty";
		public static string KeyNotFind = Localization.Resources.Log_ParserFileErrorMessage_KeyNotFind;//"Key : [ {0} ] Not Find";
	}

}

