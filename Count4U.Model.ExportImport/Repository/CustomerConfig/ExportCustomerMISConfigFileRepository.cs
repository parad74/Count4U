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
using System.IO;
using System.Threading;

namespace Count4U.Model.Count4U
{
	//"Config.ini"
	public class ExportCustomerMISConfigFileRepository : BaseExportFileRepository, IExportConfigIniRepository
	{
		private static int CountRow;

		public ExportCustomerMISConfigFileRepository(
			IServiceLocator serviceLocator,
			ILog log)
			: base(log, serviceLocator)
		{
		}

		public void WriteToFile(string fromPathFile, string toPathFile,
			WriterEnum writerEnum,
			//ExportProviderEnum exportProviderEnum,
			Encoding encoding, string[] separators,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1057%"Cancel Write to File [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1057, toPathFile));
				return;
			}
			//Localization.Resources.Log_TraceRepository1015%"Export to [{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1015, "PDA File Repository", "ExportCustomerConfigFileRepository"));

			StreamWriter sw = GetStreamWriter(toPathFile, encoding);

 			if (importType.Contains(ImportDomainEnum.ExportCustomerConfig) == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1016%"Start Write to File [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1016, toPathFile));
			
				//ExportFileType fileTypeEnum = parms.GetFileTypeFromParm();
				//int FileTypeInt = ConfigIniFileType.ConvertExportConfigIniFileType2FileTypeInt(fileTypeEnum);

				int QuantityType = parms.GetIntValueFromParm(ImportProviderParmEnum.QType);
				string AddNewItem = parms.GetStringValueFromParm(ImportProviderParmEnum.NewItemBool);
				string ChangeQuantityType = parms.GetStringValueFromParm(ImportProviderParmEnum.ChangeQuantityType);
				string Password = parms.GetStringValueFromParm(ImportProviderParmEnum.Password);
				string CustomerCode = parms.GetStringValueFromParm(ImportProviderParmEnum.CustomerCode);
				string CustomerName = parms.GetStringValueFromParm(ImportProviderParmEnum.CustomerName);
				string BranchCode = parms.GetStringValueFromParm(ImportProviderParmEnum.BranchCode);
				string BranchName = parms.GetStringValueFromParm(ImportProviderParmEnum.BranchName);
				string Count4uCode = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorCode);
				Count4uCode = String.IsNullOrEmpty(Count4uCode) ? "000" : Count4uCode;

				bool InvertWordsConfig = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWordsConfig);
				bool InvertLettersConfig = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLettersConfig);

				BranchName = String.IsNullOrEmpty(BranchName) ? "" : BranchName.ReverseDosHebrew(InvertLettersConfig, InvertWordsConfig);
				CustomerName = String.IsNullOrEmpty(CustomerName) ? "" : CustomerName.ReverseDosHebrew(InvertLettersConfig, InvertWordsConfig);	   
				string LookUpEXE = parms.GetStringValueFromParm(ImportProviderParmEnum.LookUpEXE);
				string HTcalculateLookUp = parms.GetStringValueFromParm(ImportProviderParmEnum.HTcalculateLookUp);
	 			string AddNewLocation = parms.GetStringValueFromParm(ImportProviderParmEnum.AddNewLocation);
				string AddExtraInputValueSelectFromBatchListForm = parms.GetStringValueFromParm(ImportProviderParmEnum.AddExtraInputValueSelectFromBatchListForm);
				string AllowNewValueFromBatchListForm = parms.GetStringValueFromParm(ImportProviderParmEnum.AllowNewValueFromBatchListForm);
				string SearchIfExistInBatchList = parms.GetStringValueFromParm(ImportProviderParmEnum.SearchIfExistInBatchList);
				string AllowMinusQuantity = parms.GetStringValueFromParm(ImportProviderParmEnum.AllowMinusQuantity);
				string FractionCalculate = parms.GetStringValueFromParm(ImportProviderParmEnum.FractionCalculate);
				string PartialQuantity = parms.GetStringValueFromParm(ImportProviderParmEnum.PartialQuantity);
				string Host1 = parms.GetStringValueFromParm(ImportProviderParmEnum.Host1);
				string Host2 = parms.GetStringValueFromParm(ImportProviderParmEnum.Host2);
				int Timeout = parms.GetIntValueFromParm(ImportProviderParmEnum.Timeout);
				int Retry = parms.GetIntValueFromParm(ImportProviderParmEnum.Retry);
				int SameBarcodeInLocation = parms.GetIntValueFromParm(ImportProviderParmEnum.SameBarcodeInLocation);
				string DefaultHost = parms.GetStringValueFromParm(ImportProviderParmEnum.DefaultHost);
				string AllowZeroQuantity = parms.GetStringValueFromParm(ImportProviderParmEnum.AllowZeroQuantity);
				int MaxQuantity = parms.GetIntValueFromParm(ImportProviderParmEnum.MaxQuantity);
				string LastSync = parms.GetStringValueFromParm(ImportProviderParmEnum.LastSync);
				string SearchDef = parms.GetStringValueFromParm(ImportProviderParmEnum.SearchDef);
				string ConfirmNewLocation = parms.GetStringValueFromParm(ImportProviderParmEnum.ConfirmNewLocation);
				string ConfirmNewItem = parms.GetStringValueFromParm(ImportProviderParmEnum.ConfirmNewItem);
				string AutoSendData = parms.GetStringValueFromParm(ImportProviderParmEnum.AutoSendData);

				string AllowQuantityFraction = parms.GetStringValueFromParm(ImportProviderParmEnum.AllowQuantityFraction);
				string AddExtraInputValue = parms.GetStringValueFromParm(ImportProviderParmEnum.AddExtraInputValue);
				string AddExtraInputValueHeaderName = parms.GetStringValueFromParm(ImportProviderParmEnum.AddExtraInputValueHeaderName);


//file PreSettings.ini

//[Settings]
//QuantityType=1
//Password=650
//CustomerCode=001
//CustomerName=Next-Line
//BranchCode=001-001
//BranchName=Please Sync
//Count4uCode=a8a79621-4164-4fc4-91a1-9ed6fcceb64c
//LookUpEXE=C:\MIS\IDnextData
//HTcalculateLookUp=true
//MaxQuantity=999
//AllowZeroQuantity=false
//LastSync=08-09-16  13:07
//SearchDef=normal,market:729,add_leading:0;13
//AddNewLocation=true
//AddNewItem=true
//ConfirmNewLocation=True
//ConfirmNewItem=false
//AutoSendData=0
//				AllowQuantityFraction=True
//AddExtraInputValue=True
//AddExtraInputValueHeaderName=Batch Number|מס אצווה|*Batch Number|#Batch Number


				try
				{
					sw.WriteLine("[Settings]");
					sw.WriteLine(ImportProviderParmName.QuantityType + separators[0] + QuantityType);
					sw.WriteLine(ImportProviderParmName.Password + separators[0] + Password);
					sw.WriteLine(ImportProviderParmName.CustomerCode + separators[0] + CustomerCode);
					sw.WriteLine(ImportProviderParmName.CustomerName + separators[0] + CustomerName);
					sw.WriteLine(ImportProviderParmName.BranchCode + separators[0] + BranchCode);
					sw.WriteLine(ImportProviderParmName.BranchName + separators[0] + BranchName);
					sw.WriteLine(ImportProviderParmName.Count4uCode + separators[0] + Count4uCode);
					sw.WriteLine(ImportProviderParmName.LookUpEXE + separators[0] + LookUpEXE);
					sw.WriteLine(ImportProviderParmName.HTcalculateLookUp + separators[0] + HTcalculateLookUp);
					sw.WriteLine(ImportProviderParmName.MaxQuantity + separators[0] + MaxQuantity);
					sw.WriteLine(ImportProviderParmName.AllowZeroQuantity + separators[0] + AllowZeroQuantity);
					sw.WriteLine(ImportProviderParmName.LastSync + separators[0] + LastSync);
					sw.WriteLine(ImportProviderParmName.SearchDef + separators[0] + SearchDef);
					sw.WriteLine(ImportProviderParmName.AddNewLocation + separators[0] + AddNewLocation);
					sw.WriteLine(ImportProviderParmName.AddNewItem + separators[0] + AddNewItem);
					sw.WriteLine(ImportProviderParmName.ChangeQuantityType + separators[0] + ChangeQuantityType);
					sw.WriteLine(ImportProviderParmName.ConfirmNewLocation + separators[0] + ConfirmNewLocation);
					sw.WriteLine(ImportProviderParmName.ConfirmNewItem + separators[0] + ConfirmNewItem);
					sw.WriteLine(ImportProviderParmName.AutoSendData + separators[0] + AutoSendData);
					sw.WriteLine(ImportProviderParmName.AllowQuantityFraction + separators[0] + AllowQuantityFraction);
					sw.WriteLine(ImportProviderParmName.AddExtraInputValue + separators[0] + AddExtraInputValue);
					sw.WriteLine(ImportProviderParmName.AddExtraInputValueHeaderName + separators[0] + AddExtraInputValueHeaderName);
					sw.WriteLine(ImportProviderParmName.AddExtraInputValueSelectFromBatchListForm + separators[0] + AddExtraInputValueSelectFromBatchListForm);
					sw.WriteLine(ImportProviderParmName.AllowNewValueFromBatchListForm + separators[0] + AllowNewValueFromBatchListForm);
					sw.WriteLine(ImportProviderParmName.SearchIfExistInBatchList + separators[0] + SearchIfExistInBatchList);
					sw.WriteLine(ImportProviderParmName.AllowMinusQuantity + separators[0] + AllowMinusQuantity);
					sw.WriteLine(ImportProviderParmName.FractionCalculate + separators[0] + FractionCalculate);
					sw.WriteLine(ImportProviderParmName.PartialQuantity + separators[0] + PartialQuantity);
					sw.WriteLine(ImportProviderParmName.Host1 + separators[0] + Host1);
					sw.WriteLine(ImportProviderParmName.Host2 + separators[0] + Host2);
					sw.WriteLine(ImportProviderParmName.DefaultHost + separators[0] + DefaultHost);
					sw.WriteLine(ImportProviderParmName.Timeout + separators[0] + Timeout);
					sw.WriteLine(ImportProviderParmName.Retry + separators[0] + Retry);
					sw.WriteLine(ImportProviderParmName.SameBarcodeInLocation + separators[0] + SameBarcodeInLocation);
						
				}
				catch (Exception error)
				{
					_logger.ErrorException("WriteToFile", error);
					this.Log.Add(MessageTypeEnum.Error, error.Message + ":" + error.StackTrace);
				}

				sw.Close();
				//Localization.Resources.Log_TraceRepositoryResult1058%"Write to File [{0}] "
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1058, toPathFile));
			}
		}


	}
}
