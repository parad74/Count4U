using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;
using System.IO;

namespace Count4U.Model
{
	public class ExportInventProductNimrodAvivERPFileProvider : BaseExportERPProvider, IExportERPProvider
	{
		protected readonly IExportInventProductSimpleRepository _exportRepository;

		public ExportInventProductNimrodAvivERPFileProvider(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.ExportSumInventProduct);
			this._importTypes.Add(ImportDomainEnum.ProductNameContains);
			this._importTypes.Add(ImportDomainEnum.IturCodeContains);
			this._importTypes.Add(ImportDomainEnum.ChangeMakat2Barcode);
			this._exportRepository = this._serviceLocator.GetInstance
				<IExportInventProductSimpleRepository>(ExportRepositoryEnum.ExportInventProductSimpleERPFileRepository.ToString());
		}

		public override void WriteToFile(string toPathFile)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms,  this.ProviderEncoding);
	
				this._exportRepository.WriteToFile(this.FromPathDB, toPathFile, WriterEnum.ExportInventProductNimrodAvivERPFileWriter,
				this.ProviderEncoding, this._separators, this._importTypes, sw, this.Parms, base.Refill);

				sw.Flush();

				bool fileXlsx = this.Parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
				if (fileXlsx == false)
				{
					File.WriteAllText(toPathFile, this.ProviderEncoding.GetString(ms.ToArray()), this.ProviderEncoding);
				}
				else
				{
					WriteAllToExcel(ms, toPathFile, this.ProviderEncoding, this._countExcludeFirstString);
				}
			}
		}


		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.I };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._exportRepository.DeleteFile(this.FromPathFile);
		}

	}
}
