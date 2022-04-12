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
	public class ExportUserIniFileRepository : BaseExportFileRepository, IExportConfigIniRepository
	{
		private static int CountRow;

		public ExportUserIniFileRepository(
			IServiceLocator serviceLocator,
			ILog log	)
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
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1015, "PDA File Repository", "ExportUserIniFileRepository"));
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString());
		
			StreamWriter sw = base.GetStreamWriter(toPathFile, encoding);

			//Localization.Resources.Log_TraceRepositoryResult1016%"Start Write to File [{0}] "
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1016, toPathFile));

			if (importType.Contains(ImportDomainEnum.ExportUserIni) == true)
			{
			
				try
				{
					foreach (String rec in fileParser.GetRecords(fromPathFile, encoding, 0))
					{
						if (string.IsNullOrWhiteSpace(rec) == true) { continue; }
						sw.WriteLine(rec);
					}
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
