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
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model
{
	public class ExportInventProductAS400AprilERPFileProvider : BaseExportERPProvider, IExportERPProvider
	{
		protected readonly IExportInventProductSimpleRepository _exportRepository;
		protected readonly IIturRepository _iturRepository;

		public ExportInventProductAS400AprilERPFileProvider(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.ExportSumInventProduct);
			this._importTypes.Add(ImportDomainEnum.ExportInventProductGroupHeaderByItur);
			//this._importTypes.Add(ImportDomainEnum.ExportInventProductByIturCode);

			this._exportRepository = this._serviceLocator.GetInstance
				<IExportInventProductSimpleRepository>(ExportRepositoryEnum.ExportInventProductSimpleERPFileRepository.ToString());
			this._iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
		}

		public override void WriteToFile(string toPathFile)
		{
			//this._exportRepository.DeleteFile(toPathFile);
			//string folder = Path.GetDirectoryName(toPathFile);
			//if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
			//StreamWriter sw = new StreamWriter(toPathFile, false, this.ProviderEncoding);
			//List<string> iturCodeList =  this._iturRepository.GetIturCodeList(this.FromPathDB);
			//foreach (string iturCode in iturCodeList)
			//{

			//	this.Parms[ImportProviderParmEnum.IturCode] = iturCode;
			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms,  this.ProviderEncoding);

				this._exportRepository.WriteToFile(this.FromPathDB, toPathFile,
					WriterEnum.ExportInventProductAS400AprilERPFileWriter,
				this.ProviderEncoding, this._separators, this._importTypes, sw, this.Parms);

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
			this._separators = new string[] { SeparatorField.Empty };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._exportRepository.DeleteFile(this.FromPathFile);
		}

	
	}
}
