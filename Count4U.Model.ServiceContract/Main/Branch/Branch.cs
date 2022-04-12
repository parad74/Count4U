using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Count4U.Model.Audit;

namespace Count4U.Model.Main
{
	public class Branch
	{
		public long ID { get; set; }
		public string Address { get; set; }
		public string Code { get; set; }
		public string ContactPerson { get; set; }		//DONE this._branch.ContactPerson; занято на Selected Adapter import  Family
		public string Description { get; set; }
		public string Fax { get; set; }
		public string LogoFile { get; set; }
		public string Mail { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string CustomerCode { get; set; }
		public System.Byte[] Logo { get; set; }
		public string DBPath { get; set; }
 		public string ImportCatalogProviderCode { get; set; }
 		public string ImportIturProviderCode { get; set; }
 		public string ImportLocationProviderCode { get; set; }
  		public string ImportPDAProviderCode { get; set; }
		public string ImportCatalogAdapterParms { get; set; } // used ImportPath=C:\temp;ExportPath=C:\temp set in Form.EditBranch.Parameter BranchFormViewModel.Params
		public string ImportIturAdapterParms { get; set; }
		public string ImportLocationAdapterParms { get; set; }
		public string ImportPDAAdapterParms { get; set; }
		public string BranchCodeLocal { get; set; }
		public string BranchCodeERP { get; set; }
		public string ExportCatalogAdapterCode { get; set; }
		public string ExportIturAdapterCode { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime ModifyDate { get; set; }						 //NOT DONE The Last update Catalog
		public string ReportName { get; set; }
		public string ReportContext { get; set; }  //DONE				  ComplexAdapterCode
		public string ReportDS { get; set; }
		public string ReportPath { get; set; }
		public string ImportSectionAdapterCode { get; set; }
		public string ExportSectionAdapterCode { get; set; }
		public string UpdateCatalogAdapterCode { get; set; }
		public string ExportERPAdapterCode { get; set; }
		public string ImportSupplierAdapterCode { get; set; }
		public string Restore { get; set; }
		public bool RestoreBit { get; set; }
		public string PriceCode { get; set; }
		public bool Print { get; set; }			
		public string PDAType { get; set; }
		public string MaintenanceType { get; set; }
		public string ProgramType { get; set; }

		//=========		11/02/2019
		public string ComplexStaticPath1 { get; set; }
		public string ComplexStaticPath2 { get; set; }
		public string ComplexStaticPath3 { get; set; }

		public string Host { get; set; }
		public string Port { get; set; }

		public string ImportCatalogPath { get; set; }		  
		public string ImportFromPdaPath { get; set; }
		public string ExportErpPath { get; set; }			 
		public string ExportPdaPath { get; set; }
		public string SendToOfficePath { get; set; }	   


		public DateTime LastUpdatedCatalog { get; set; }	  //NOT DONE
		public string Tag { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		public string Tag3 { get; set; }

		public string ComplexAdapterCode { get; set; }
		public string ComplexAdapterParametr { get; set; }
		public string ComplexAdapterParametr1 { get; set; }
		public string ComplexAdapterParametr2 { get; set; }
		public string ComplexAdapterParametr3 { get; set; }
		public string ComplexAdapterParametrERPCode { get; set; }
		public string ComplexAdapterParametrInventorDateTime { get; set; }


		public string ImportFamilyAdapterCode { get; set; }
		public string MaxCharacters { get; set; }					
		public string MakatOrMakatOriginal { get; set; }		

		public string StatusAuditConfig { get; set; } // что -то за идея была, что проверять в каком статусе Branch InProcess or Not

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Branch)) return false;
			return Equals((Branch)obj);
		}

		public bool Equals(Branch other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Code, this.Code) && Equals(other.CustomerCode, this.CustomerCode));
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);
		}

		public Branch(AuditConfig auditConfig)
		{
			Code = auditConfig.BranchCode;
			CustomerCode = auditConfig.CustomerCode;
			DBPath = auditConfig.BranchCode;
			Name = auditConfig.BranchCode;
			CreateDate = DateTime.Now;
			ModifyDate = DateTime.Now;
			LastUpdatedCatalog = DateTime.Now;
			RestoreBit = true;
			Restore = "Restore form Inventor Code: " + auditConfig.InventorCode;
			Description = "Restore form Inventor Code: " + auditConfig.InventorCode;
			Address = "";
			ContactPerson = "";
			Phone = "";
			Mail = "";
			Fax = "";
		}

		public Branch(Inventor inventor)
		{
			Code = inventor.BranchCode;
			CustomerCode = inventor.CustomerCode;
			DBPath = inventor.BranchCode;
			Name = inventor.BranchCode;
			CreateDate = DateTime.Now;
			ModifyDate = DateTime.Now;
			LastUpdatedCatalog = DateTime.Now;
			RestoreBit = true;
			Restore = "Restore form Inventor Code: " + inventor.Code;
			Description = "Restore form Inventor Code: " + inventor.Code;
			Address = "";
			ContactPerson = "";
			Phone = "";
			Mail = "";
			Fax = "";
		}

		public Branch()
		{
			Code = "";
			CustomerCode = "";
			DBPath = "";
			Name = "";
			CreateDate = DateTime.Now;
			ModifyDate = DateTime.Now;
			LastUpdatedCatalog = DateTime.Now;
			Description = "";
			Address = "";
			ContactPerson = "";
			Phone = "";
			Mail = "";
			Fax = "";
			LogoFile = "";
			ImportCatalogProviderCode = "";
			ImportIturProviderCode = "";
			ImportLocationProviderCode = "";
			ImportPDAProviderCode = "";
			ImportCatalogAdapterParms = "";
			ImportIturAdapterParms = "";
			ImportLocationAdapterParms = "";
			ImportPDAAdapterParms = "";
			BranchCodeLocal = "";
			BranchCodeERP = "";
			ExportCatalogAdapterCode = "";
			ExportIturAdapterCode = "";
			ReportName = "";
			ReportContext = "";
			ReportDS = "";
			ReportPath = "";
			ImportSectionAdapterCode = "";
			ExportSectionAdapterCode = "";
			UpdateCatalogAdapterCode = "";
			ExportERPAdapterCode = "";
			ImportSupplierAdapterCode = "";
			Restore = "";
			PriceCode = "";
			PDAType = "";
			MaintenanceType = "";
			ProgramType = "";
		}
		
	}
}
