using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;

namespace Count4U.Model.Main
{
	public class BranchString
	{
		//public string Address { get; set; }
		public string Code { get; set; }
		//public string ContactPerson { get; set; }
		//public string Description { get; set; }
		//public string Fax { get; set; }
		//public string LogoFile { get; set; }
		//public string Mail { get; set; }
		public string Name { get; set; }
		//public string Phone { get; set; }
		public string CustomerCode { get; set; }
		public string DBPath { get; set; }
		//public string ImportCatalogProviderCode { get; set; }
		//public string ImportIturProviderCode { get; set; }
		//public string ImportLocationProviderCode { get; set; }
		//public string ImportPDAProviderCode { get; set; }
		//public string ImportCatalogAdapterParms { get; set; }
		//public string ImportIturAdapterParms { get; set; }
		//public string ImportLocationAdapterParms { get; set; }
		//public string ImportPDAAdapterParms { get; set; }
		public string BranchCodeLocal { get; set; }
		public string BranchCodeERP { get; set; }
		//public string ExportCatalogAdapterCode { get; set; }
		//public string ExportIturAdapterCode { get; set; }
		//public string ReportName { get; set; }
		//public string ReportContext { get; set; }
		//public string ReportDS { get; set; }
		//public string ReportPath { get; set; }
		//public string ImportSectionAdapterCode { get; set; }
		//public string ExportSectionAdapterCode { get; set; }
		//public string UpdateCatalogAdapterCode { get; set; }
		//public string Restore { get; set; }

		public BranchString()
		{
			Code = "";
			Name = "";
			CustomerCode = "";
			DBPath = "";
			BranchCodeLocal = "";
			BranchCodeERP = "";
		}
	}
}
