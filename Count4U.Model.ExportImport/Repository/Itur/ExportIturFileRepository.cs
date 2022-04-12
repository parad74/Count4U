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
using System.Windows;


namespace Count4U.Model.Count4U
{
	public class ExportIturFileRepository : BaseExportFileRepository, IExportIturRepository
	{
		private IIturParser _iturParser;
		private IExportIturStreamWriter _exportIturStreamWriter;

		public ExportIturFileRepository(
			IServiceLocator serviceLocator,
			ILog log
			)
			: base(log, serviceLocator)
		{
		}

		public void WriteToFile(string fromPathDB, string toPathFile,
			IturParserEnum iturParserEnum,
			WriterEnum iturWriter,
			Encoding encoding, string[] separators,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			System.Threading.CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == System.Threading.CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1057%"Cancel Write to File [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1057, toPathFile));
				return;
			}

			this._exportIturStreamWriter = this._serviceLocator.GetInstance<IExportIturStreamWriter>(iturWriter.ToString());
			if (this._exportIturStreamWriter == null) throw new ArgumentNullException(iturWriter.ToString() + " is null");

			this._iturParser =
				this._serviceLocator.GetInstance<IIturParser>(iturParserEnum.ToString());
			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			if (this._iturParser == null)
			{
				//Localization.Resources.Log_Error1007%"In  IturParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1007, iturParserEnum));
				return;
			}

		
			//Localization.Resources.Log_TraceRepository1015%"Export to [{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1015, "Itur", "ExportIturFileRepository"));
			//Localization.Resources.Log_TraceRepository1040%"[{0}]  is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "IturParser", iturParserEnum.ToString()));

		
			base.DeleteFile(toPathFile);

			string folder = Path.GetDirectoryName(toPathFile);
			if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);

			string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}
			string iturNamePrefix = "ITUR";
			bool invertWords = true;
			bool invertLetters = true;
			bool invertPrefix = false;
			bool isAddBinarySearch = false;
			
			bool iturNameOrERPIturCode = true;

			iturNamePrefix = parms.GetStringValueFromParm(ImportProviderParmEnum.IturNamePrefix);
			//Name or ERP IturCode
			iturNameOrERPIturCode = parms.GetBoolValueFromParm(ImportProviderParmEnum.IturNameType);	 //Name or ERP IturCode
			invertWords = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);	 //invertWords
			invertLetters = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);	 //invertWords
			invertPrefix = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertPrefix);	 //invertPrefix
			isAddBinarySearch = parms.GetBoolValueFromParm(ImportProviderParmEnum.IsAddBinarySearch);	 //IsAddBinarySearch

			//button1.Dispatcher.Invoke(new Action(() => { button1.Content = "123"; }));
			 
			//Application.Current.Dispatcher.Invoke(new Action(() => { iturNamePrefix = Localization.Resources.Domain_Itur_NAME; }));
			iturNamePrefix = iturNamePrefix.ReverseDosHebrew(invertPrefix, false);

			//Localization.Resources.Log_TraceRepositoryResult1016%"Start Write to File [{0}] "
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1016, toPathFile));
			StreamWriter sw = new StreamWriter(toPathFile, false, encoding);
			try
			{
				if (importType.Contains(ImportDomainEnum.ExportItur) == true)
				{
					int k = 0;
					countAction(k);
					//Dictionary<string, Itur>  iturDictionaryFromDB = iturRepository.GetIturDictionary(fromPathDB, true);
					foreach (KeyValuePair<string, Itur> keyValuePair in
						this._iturParser.GetIturs(fromPathDB, encoding, separators, 0, null))//iturDictionaryFromDB))		
					{
						if (cancellationToken.IsCancellationRequested == true)
						{
							break;
						}
						k++;
						if (k % 100 == 0)
						{
							countAction(k);
						}
						Itur itur = keyValuePair.Value;

						this._exportIturStreamWriter.AddRow(sw, itur, separator, iturNameOrERPIturCode, iturNamePrefix, invertLetters, invertWords);

						//string iturName = itur.Name;
						////"ITUR " & Itur Prefix & "-" & Itur code 
						//if (string.IsNullOrWhiteSpace(iturName) == true) iturName = 
						//    "ITUR " + itur.NumberPrefix + "-" + itur.NumberSufix;
						//string[] newRows = new string[] { itur.IturCode, iturName };
						//string newRow = string.Join(separator, newRows);
						//sw.WriteLine(newRow.Trim(','));
					}
				}
			}
			catch (Exception error)
			{
				_logger.ErrorException("WriteToFile", error);
				this.Log.Add(MessageTypeEnum.Error, error.Message + ":" + error.StackTrace);
			}
  			sw.Close();
			//Localization.Resources.Log_TraceRepositoryResult1058%"Write to File [{0}]"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1058, toPathFile));
		}
	}
}
