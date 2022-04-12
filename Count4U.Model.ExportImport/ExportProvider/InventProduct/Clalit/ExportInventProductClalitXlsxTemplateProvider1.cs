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
	public class ExportInventProductClalitXlsxTemplateProvider1 : BaseExportERPProvider, IExportERPProvider
	{
		protected readonly IExportCurrentInventorAdvancedRepository _exportRepository;			  //TODO
																 

		public ExportInventProductClalitXlsxTemplateProvider1(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.ExportCurrentInventoryAdvanced);	 //TODO
				
			this._exportRepository = this._serviceLocator.GetInstance				  //TODO   ExportERPCurrentInventorExtendedRepository
				<IExportCurrentInventorAdvancedRepository>(ExportRepositoryEnum.ExportCurrentInventorAdvancedERPRepository.ToString());
		}

		public override void WriteToFile(string toPathFile)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms,  this.ProviderEncoding);

				//				   SaveToAnaliticDB														   //IExportCurrentInventoryWriter
				this._exportRepository.WriteToFile(this.FromPathDB, toPathFile, WriterEnum.ExportCurrentInventoryClaitXslxWriter1,
			this.ProviderEncoding, this._separators, this._importTypes, sw, this.Parms);

				sw.Flush();

			//	bool fileXlsxTemplate = this.Parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsxTemplate);		 //TODO
				bool fileXlsx = this.Parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
				//if (fileXlsxTemplate == true)				   //TODO
				//{
				//	WriteAllToExcelTemplate(ms, toPathFile, this.ProviderEncoding, this._countExcludeFirstString);			  //TODO
				//}
				//else

			//	protected void WriteAllToExcel(MemoryStream ms, string toPathFile, Encoding encoding, int headerCountLine = 0, 
			//bool headerBold = true,  XLColor headerBackgroung = null, XLReferenceStyle referenceStyle = XLReferenceStyle.R1C1)

				if (fileXlsx == true)
				{
					WriteAllToExcel(ms, toPathFile, this.ProviderEncoding, 
						this._countExcludeFirstString, true,
						null, ClosedXML.Excel.XLReferenceStyle.A1, true, this._separators[0]);
				}
			}
		}


		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Cr };
			this._countExcludeFirstString = 2;
		}

		public void Clear()
		{
			this._exportRepository.DeleteFile(this.FromPathFile);
		}

	}
}
