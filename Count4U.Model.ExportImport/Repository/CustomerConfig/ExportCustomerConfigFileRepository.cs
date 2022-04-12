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
	public class ExportCustomerConfigFileRepository : BaseExportFileRepository, IExportConfigIniRepository
	{
		private static int CountRow;

		public ExportCustomerConfigFileRepository(
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
				int Hash = parms.GetIntValueFromParm(ImportProviderParmEnum.Hash);
				ExportFileType fileTypeEnum = parms.GetFileTypeFromParm();

				int FileTypeInt = ConfigIniFileType.ConvertExportConfigIniFileType2FileTypeInt(fileTypeEnum);
				int QType = parms.GetIntValueFromParm(ImportProviderParmEnum.QType);
				int UseAlphaKey = parms.GetIntValueFromParm(ImportProviderParmEnum.UseAlphaKey);
				int ClientId = parms.GetIntValueFromParm(ImportProviderParmEnum.ClientId);
				int NewItem = parms.GetIntValueFromParm(ImportProviderParmEnum.NewItem);
				string NewItemBool = parms.GetStringValueFromParm(ImportProviderParmEnum.NewItemBool);
				string ChangeQuantityType = parms.GetStringValueFromParm(ImportProviderParmEnum.ChangeQuantityType);
				string Password = parms.GetStringValueFromParm(ImportProviderParmEnum.Password);
				string StoreNumber = parms.GetStringValueFromParm(ImportProviderParmEnum.StoreNumber);
				string StoreName = parms.GetStringValueFromParm(ImportProviderParmEnum.StoreName);
				StoreName = String.IsNullOrEmpty(StoreName) ? "" : StoreName.ReverseDosHebrew(true, true);	   //!!(true)
//	StoreNumber=002
//StoreName=емйб арб
//Hash=0
//FileType=2
//QType=0
//UseAlphaKey=1
//ClientId=0
//NewItem=0
				try
				{
					sw.WriteLine(ImportProviderParmName.StoreNumber + separators[0] + StoreNumber);
					sw.WriteLine(ImportProviderParmName.StoreName + separators[0] + StoreName);
					sw.WriteLine(ImportProviderParmName.Hash + separators[0] + Hash);
					sw.WriteLine(ImportProviderParmName.FileType + separators[0] + FileTypeInt);
					sw.WriteLine(ImportProviderParmName.QType + separators[0] + QType);
					sw.WriteLine(ImportProviderParmName.UseAlphaKey + separators[0] + UseAlphaKey);
					sw.WriteLine(ImportProviderParmName.ClientId + separators[0] + ClientId);
					sw.WriteLine(ImportProviderParmName.NewItem + separators[0] + NewItem);
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
