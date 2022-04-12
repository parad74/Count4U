using System;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Extensions;
using Count4U.Model.Main;

namespace Count4U.Common.ViewModel.Adapters.Export
{
    public class ExportCommandInfo
    {
		public XDocument ConfigXDocument { get; set; }
		[NotInludeAttribute]
		public string ConfigXDocumentPath { get; set; }
		public ConfigXDocFromEnum FromConfigXDoc { get; set; }
		[NotInludeAttribute]
		public string AdapterName { get; set; }

        public Action Callback { get; set; }

        public bool WithoutProductName { get; set; }
        public bool BarcodeWithoutMask { get; set; }
		public bool MakatWithoutMask { get; set; }
		public bool CheckBaudratePDA { get; set; }
		

        public int Hash { get; set; }
        public int FileType { get; set; }
        public int QType { get; set; }
        public int UseAlphaKey { get; set; }
        public int ClientId { get; set; }
        public int NewItem { get; set; }
		public string NewItemBool { get; set; }
		public string ChangeQuantityType { get; set; }
		
		public string Password { get; set; }

        public int MaxLen { get; set; }
		public bool IturTypeByName { get; set; }
		public string IturNamePrefix { get; set; }
        public bool IturInvertPrefix { get; set; }
		public bool IsAddBinarySearch { get; set; }
		public bool IsInvertWords { get; set; }
		public bool IsInvertLetters { get; set; }
		public int EncodingCodePage { get; set; }
		public bool IsCutAfterInvert { get; set; }
		public string SearchDef { get; set; }
		
			  

        public Customer Customer { get; set; }

        public bool IsSaveFileLog { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public string PDAType { get; set; }
        public string MaintenanceType { get; set; }
        public string ProgramType { get; set; }

		public string HTcalculateLookUp { get; set; }
		public string LookUpEXE { get; set; }

		public string AddNewLocation { get; set; }
		public int MaxQuantity { get; set; }
		public string AllowZeroQuantity { get; set; }
		public string LastSync { get; set; }

		public string AddExtraInputValueSelectFromBatchListForm { get; set; }
		public string AllowNewValueFromBatchListForm { get; set; }
		public string SearchIfExistInBatchList { get; set; }
		public string AllowMinusQuantity { get; set; }
		public string FractionCalculate { get; set; }
		public string PartialQuantity { get; set; }
		public string Host1 { get; set; }
		public string Host2 { get; set; }
		public int Timeout { get; set; }
		public int Retry { get; set; } 
		public int SameBarcodeInLocation { get; set; }
		public string DefaultHost { get; set; }


		public bool IsInvertWordsConfig { get; set; }
		public bool IsInvertLettersConfig { get; set; }

		public bool IncludeCurrentInventor { get; set; }
		public bool IncludePreviousInventor { get; set; }
		public bool IncludeProfile { get; set; }
		
		[NotInludeAttribute]
		public bool CreateZipFile { get; set; }

		public string ConfirmNewLocation { get; set; }
        public string ConfirmNewItem { get; set; }
        public string AutoSendData { get; set; }

		public string AllowQuantityFraction { get; set; }
        public string AddExtraInputValue { get; set; }
        public string AddExtraInputValueHeaderName { get; set; }

		public  ExportCommandInfo()
		{
			AdapterName = "";
			ConfigXDocument = null;
			ConfigXDocumentPath = "";
			FromConfigXDoc = ConfigXDocFromEnum.InitWithoutConfig;

			NewItemBool = "";
			ChangeQuantityType = "";
			Password = "";
			IturNamePrefix = "";
			SearchDef = "";
 			PDAType = "";
			MaintenanceType = "";
			ProgramType = "";
   			HTcalculateLookUp = "";
			LookUpEXE = "";
  			AddNewLocation = "";
			AllowZeroQuantity = "";
			LastSync = "";
			ConfirmNewLocation = "";
			ConfirmNewItem = "";
			AutoSendData = "";
			AllowQuantityFraction = "";
			AddExtraInputValue = "";
			AddExtraInputValueHeaderName = "";
			AddExtraInputValueSelectFromBatchListForm  = "";
			AllowNewValueFromBatchListForm  = "";
			SearchIfExistInBatchList = "";
			AllowMinusQuantity = "";
			FractionCalculate = "";
			PartialQuantity = "";
			Host1 = "";
			Host2 = "";
			Timeout = 0;
			Retry = 0;
			SameBarcodeInLocation = 0;
			DefaultHost = "";

  		}
	}
}