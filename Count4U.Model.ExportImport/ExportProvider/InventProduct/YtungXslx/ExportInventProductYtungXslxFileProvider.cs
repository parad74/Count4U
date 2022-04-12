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
using ClosedXML.Excel;

namespace Count4U.Model
{
	public class ExportInventProductYtungXslxFileProvider : BaseExportERPProvider, IExportERPProvider
	{
		protected readonly IExportInventProductSimpleRepository _exportRepository;

		public ExportInventProductYtungXslxFileProvider(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.ExportInventProduct);
			this._importTypes.Add(ImportDomainEnum.SectionCodeContains);
			this._importTypes.Add(ImportDomainEnum.SupplierCodeContains);
			this._importTypes.Add(ImportDomainEnum.CountInParentPackAndPricesFromCatalog);

			//this._importTypes.Add(ImportDomainEnum.FullDataFromContains);
			//this._importTypes.Add(ImportDomainEnum.NameAndBalanceOriginalERPAndCountInParentPackAndPriceContains);
			this._exportRepository = this._serviceLocator.GetInstance
				<IExportInventProductSimpleRepository>(ExportRepositoryEnum.ExportInventProductSimpleERPFileRepository.ToString());
		}

		public override void WriteToFile(string toPathFile)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms,  this.ProviderEncoding);

				this._exportRepository.WriteToFile(this.FromPathDB, toPathFile, WriterEnum.ExportInventProductYtungXslxFileWriter,
			this.ProviderEncoding, this._separators, this._importTypes, sw, this.Parms);

			sw.Flush();

				bool fileXlsx = this.Parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
				if (fileXlsx == false)
				{
					File.WriteAllText(toPathFile, this.ProviderEncoding.GetString(ms.ToArray()), this.ProviderEncoding);
				}
				else
				{
					//XLColor[] colors = new XLColor[] { XLColor.YellowGreen };
						WriteAllToExcel(ms, toPathFile, this.ProviderEncoding, this._countExcludeFirstString, false, null, XLReferenceStyle.R1C1, false, "^");

			//		WriteAllToExcel(ms, string toPathFile, Encoding encoding, int headerCountLine = 0, 
			//bool headerBold = true,  XLColor[] headerBackgroungs = null,
			//XLReferenceStyle referenceStyle = XLReferenceStyle.R1C1, bool rightToLeft = false, string separator = ",",
				}
			}
		}


		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Cr };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._exportRepository.DeleteFile(this.FromPathFile);
		}

		
	}
}
