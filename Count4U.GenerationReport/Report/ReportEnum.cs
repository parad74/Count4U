using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.GenerationReport
{
	//public enum ViewDomainContextEnum
	//{
	//    Location = 0,
	//    Catalog = 1,
	//    ItursItur = 2,
	//    //ItursIturAddIn = 3,
	//    ItursIturDoc = 4,
	//    ItursIturDocAddInSimple = 5,
	//    //ItursIturDocAddIn = 5,
	//    ItursIturDocPDA = 6,
	//    //ItursIturDocPDAAddIn = 7,
	//    Doc = 8,
	//    //DocAddIn = 9,
	//    DocPDA = 10,
	//    //DocPDAAddIn = 11,
	//    PDA = 12,
	//    //PDAAddIn = 13,
	//    All = 14,
	//    Iturs = 15,
	//    //ItursAddIn = 16,
	//    //ItursIturDocAddInSimple = 17
	//    Customer = 18,
	//    Branch = 19,
	//    Inventor = 20,
	//    AuditConfig = 21,
	//    IturAdvancedSearch = 22,
	//    InventProductAdvancedSearch = 23,
	//    InventProductSumAdvancedSearch = 24
	//}

	public enum ReportTemplateDomainEnum
	{
		Any = 0,
		None = 1
		//Location = 0,
		//Catalog = 1,
		//Itur = 2,
		//Doc = 3,
		//PDA = 4,
	}

	public enum AllowedReportTemplate
	{
		Main = 0,
		Audit = 1,
		All = 2,
		Context = 3,
		Menu = 4, 
		PrintInventProduct = 5
	}

	public static class DomainContextPath
	{
		public static string Location = @"Location";
		public static string Section = @"Section";
		public static string Supplier = @"Supplier";
		public static string Device = @"Device";
		public static string DeviceWorker = @"DeviceWorker";

		public static string Family = @"Family";
		public static string Shelf = @"Shelf";

		public static string Catalog = @"Catalog";

		public static string Itur = @"Itur";
		public static string IturAddIn = @"Itur\AddIn";

		public static string Iturs = @"Iturs";
		public static string ItursAddIn = @"Iturs\AddIn";
		public static string ItursAddInExcludeLocation = @"Iturs\AddIn\ExcludeLocation";
		public static string ItursItur = @"Iturs\Itur";
		public static string ItursIturAddIn = @"Iturs\Itur\AddIn";
		public static string ItursIturDoc = @"Iturs\Itur\Doc";
		public static string ItursIturDocAddIn = @"Iturs\Itur\Doc\AddIn";
		public static string ItursIturDocAddInSimple = @"Iturs\Itur\Doc\AddIn\Simple";
		public static string ItursAddInSum = @"Iturs\AddIn\Sum";
		public static string ItursAddInWorkerSession = @"Iturs\AddIn\WorkerSession";

		public static string ItursIturDocPDA = @"Iturs\Itur\Doc\PDA";
		public static string ItursIturDocPDAAddIn = @"Iturs\Itur\Doc\PDA\AddIn";


		public static string Doc = @"Doc";
		public static string DocAddIn = @"Doc\AddIn";
		public static string DocPDA = @"Doc\PDA";
		public static string DocPDAAddIn = @"Doc\PDA\AddIn";
		
		public static string PDA = @"PDA";
		public static string PDAAddIn = @"PDA\AddIn";

		public static string Context = @"Context";
		public static string ContextIturList = @"ContextIturList";
		
		public static string All = @"";

		public static string Customer = @"Customer";
		public static string Branch = @"Branch";
		public static string Inventor = @"Inventor";
		public static string AuditConfig = @"AuditConfig";

	
	}

	public static class DomainContextDataSet
	{
		public static string Location = @"LocationDS";
		public static string Section = @"SectionDS";
		public static string Supplier = @"SupplierDS";
		public static string Device = @"DeviceDS";
		public static string DeviceWorker = @"DeviceWorkerDS";
		public static string Worker = @"WorkerDS";
		public static string Family = @"FamilyDS";
		public static string Shelf = @"ShelfDS";

		public static string Catalog = @"CatalogDS";		   //Catalog\Catalog1.rdlc

		public static string Itur = @"IturDS";
		//public static string IturAddIn = @"IturAddInDS";
																			  
		//IturAnalazer
		public static string IturAddInSum = @"IturAddInSumDS";	   //IturAnalazer	- IturAnalyzeTypeEnum.SimpleSum
		public static string IturAddInWorkerSession = @"IturAddInWorkerSessionDS";	   //IturAnalazer	- IturAnalyzeTypeEnum.WorkerSession
		//Iturs\AddIn\Sum\Iturs_Corporative_Report_BranchXXX.rdlc
		//Iturs\AddIn\Sum\Iturs_Detailed_Report_BranchXXX.rdlc	 
		//Iturs\AddIn\Sum\Iturs_Summary_Report_BranchXXX.rdlc
		//Iturs\AddIn\Sum\Iturs_Summary_Report_BranchXXX_With_Departments-Sections.rdlc
		//Iturs\AddIn\Sum\Iturs_Summary_Report_BranchXXX_With_Locations.rdlc

		//IturAnalazer
		public static string IturDocAddInSimple = @"IturDocPDASimpleDS";	//IturAnalazer	- IturAnalyzeTypeEnum.Simple
		//Iturs\Itur\Doc\AddIn\Simple\Document_Report_XXX_YYY_ZZZ.rdlc

		//IturAnalazer									
		public static string IturDocPDA = @"IturDocPDADS";	//IturAnalazer	- IturAnalyzeTypeEnum.Full			
		//Iturs\Itur\Document_Report_XXX_YYY.rdlc
		//Iturs\Summary_Report_Iturs_BranchXXX.rdlc
		//Iturs\Summary_Report_Locations_BranchXXX.rdlc
		//Iturs\Summary_Documents_Report__BranchXXX.rdlc
		// Iturs\Itur\Doc\PDA\Detailed.rdlc					  

		public static string IturDoc = @"IturDocDS";
		public static string IturDocAddIn = @"IturDocAddInDS";

		public static string IturDocPDAAddIn = @"IturDocPDAAddInDS";

		public static string Doc = @"DocDS";	//Doc\DocumentStatus.rdlc
		public static string DocAddIn = @"DocAddInDS";
		public static string DocPDA = @"DocPDADS";
		public static string DocPDAAddIn = @"DocPDAAddInDS";

		public static string PDA = @"PDADS";
		public static string PDAAddIn = @"PDAAddInDS";
		public static string ContextReport = @"ContextReportDS";
		public static string ContextReportIturList = @"ContextReportIturListDS";

		public static string Customer = @"CustomerDS";
		public static string Branch = @"BranchDS";
		public static string Inventor = @"InventorDS";
		public static string AuditConfig = @"AuditConfigDS";
	   
	}

	// for 	 FilterData
	// см namespace Count4U.Common.ViewModel.Filter.Data
	//
	public static class FilterDataSet
	{
		public static string Location = @"LocationFilterDS";		 //LocationFilterData
		public static string Section = @"SectionFilterDS";		 //SectionFilterData
		public static string Supplier = @"SupplierFilterDS";	     //SupplierFilterData
		public static string Shelf = @"ShelfFilterDS";
		public static string Family = @"FamilyFilterDS";
		

		public static string Catalog = @"ProductFilterDS";		 //ProductFilterData  

		public static string Itur = @"IturFilterDS";				      //IturFilterData  
		public static string IturAdvanced = @"IturAdvancedFilterDS";	//??
		//public static string IturAddIn = @"IturAddInDS";

		//IturAnalazer
		public static string IturAddInSum = @"IASimpleSumFilterDS";	   //IturAnalazer	- IturAnalyzeTypeEnum.SimpleSum
		//Iturs\AddIn\Sum\Iturs_Corporative_Report_BranchXXX.rdlc
		//Iturs\AddIn\Sum\Iturs_Detailed_Report_BranchXXX.rdlc	 
		//Iturs\AddIn\Sum\Iturs_Summary_Report_BranchXXX.rdlc
		//Iturs\AddIn\Sum\Iturs_Summary_Report_BranchXXX_With_Departments-Sections.rdlc
		//Iturs\AddIn\Sum\Iturs_Summary_Report_BranchXXX_With_Locations.rdlc

		//IturAnalazer
		public static string IturDocAddInSimple = @"IASimpleFilterDS";	//IturAnalazer	- IturAnalyzeTypeEnum.Simple
		//Iturs\Itur\Doc\AddIn\Simple\Document_Report_XXX_YYY_ZZZ.rdlc

		//IturAnalazer
		public static string IturDocPDA = @"IAFullFilterDS";	//IturAnalazer	- IturAnalyzeTypeEnum.Full			
		//Iturs\Itur\Document_Report_XXX_YYY.rdlc
		//Iturs\Summary_Report_Iturs_BranchXXX.rdlc
		//Iturs\Summary_Report_Locations_BranchXXX.rdlc
		//Iturs\Summary_Documents_Report__BranchXXX.rdlc
		// Iturs\Itur\Doc\PDA\Detailed.rdlc					  

		//public static string IturDoc = @"IturDocDS";
		//public static string IturDocAddIn = @"IturDocAddInDS";

		//public static string IturDocPDAAddIn = @"IturDocPDAAddInDS";

		//public static string Doc = @"DocDS";	//Doc\DocumentStatus.rdlc
		//public static string DocAddIn = @"DocAddInDS";
		//public static string DocPDA = @"DocPDADS";
		//public static string DocPDAAddIn = @"DocPDAAddInDS";

		public static string PDA = @"InventProductFilterDS";			    //InventProductFilterData
		//public static string PDAAddIn = @"PDAAddInDS";
		//public static string ContextReport = @"ContextReportDS";

		public static string Customer = @"CustomerFilterDS";			   //CustomerFilterData
		public static string Branch = @"BranchFilterDS";					  //BranchFilterData
		public static string Inventor = @"InventorFilterDS";				 //InventorFilterData
		//public static string AuditConfig = @"AuditConfigDS";

	}

	public static class ReportFileFormat
	{
		public static string Pdf = @"PDF";
		public static string Word = @"Word";
		public static string Excel = @"Excel";
		public static string EXCELOPENXML = @"EXCELOPENXML";
		public static string Excel2007 = @"Excel2007";
		
	}


}

