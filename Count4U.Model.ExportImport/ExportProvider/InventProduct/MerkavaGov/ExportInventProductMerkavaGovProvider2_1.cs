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
	public class ExportInventProductMerkavaGovProvider2_1 : BaseExportERPProvider, IExportERPProvider
	{
		protected readonly IExportCurrentInventorAdvancedRepository _exportRepository;		

		public ExportInventProductMerkavaGovProvider2_1(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.ExportCurrentInventoryAdvanced);

			this._exportRepository = this._serviceLocator.GetInstance
							<IExportCurrentInventorAdvancedRepository>(ExportRepositoryEnum.ExportCurrentInventorAdvancedERPRepository.ToString());
		}

		public override void WriteToFile(string toPathFile)
		{
			XLWorkbook workbook = new XLWorkbook { RightToLeft = true };	
			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms, this.ProviderEncoding);

				this._exportRepository.WriteToFile(this.FromPathDB, toPathFile, WriterEnum.ExportCurrentInventoryMerkavaGovXslxWriter2_1,
					this.ProviderEncoding, this._separators, this._importTypes, sw, this.Parms);

				sw.Flush();

				bool fileXlsx = this.Parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
				if (fileXlsx == true)
				{
					XLColor[] colors = new XLColor[] { XLColor.YellowGreen, XLColor.Yellow, XLColor.Yellow };

					WriteAllToExcel(ms, toPathFile, this.ProviderEncoding,
								this._countExcludeFirstString, true,
								colors, ClosedXML.Excel.XLReferenceStyle.A1, true, this._separators[0], "SN",
								//"לטעינה סידורי",
								1, workbook);
				}
			}

			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms, this.ProviderEncoding);

				this._exportRepository.WriteToFile(this.FromPathDB, toPathFile, WriterEnum.ExportCurrentInventoryMerkavaGovXslxWriter2_2,
					this.ProviderEncoding, this._separators, this._importTypes, sw, this.Parms);

				sw.Flush();

				bool fileXlsx = this.Parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
				if (fileXlsx == true)
				{
					XLColor[] colors = new XLColor[] { XLColor.YellowGreen, XLColor.Yellow, XLColor.Yellow };

					WriteAllToExcel(ms, toPathFile, this.ProviderEncoding,
								this._countExcludeFirstString, true,
								colors, ClosedXML.Excel.XLReferenceStyle.A1, true, this._separators[0],	"Q",
								//"לטעינה-כמותי",
								2, workbook);
				}
			}

			workbook.SaveAs(toPathFile);
		}


		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Cr };
			this._countExcludeFirstString = 3;
		}

		public void Clear()
		{
			this._exportRepository.DeleteFile(this.FromPathFile);
		}

	}
}
