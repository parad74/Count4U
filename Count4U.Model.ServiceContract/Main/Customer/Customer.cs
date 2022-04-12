using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Count4U.Model.Audit;
using Count4U.Model.ServiceContract;

namespace Count4U.Model.Main
{
	public class Customer : ICustomer
	{
		public long ID { get; set; }
		public string Address { get; set; }
		public string Code { get; set; }
		public string ContactPerson { get; set; }	//DONE TO ImportFamilyAdapterCode //this._customer.ContactPerson; занято на Selected Adapter import  Family
		public string Description { get; set; }
		public string Fax { get; set; }
		public System.Byte[] Logo { get; set; }
		public string Mail { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string DBPath { get; set; }
		public string ImportCatalogProviderCode { get; set; }
		public string ImportIturProviderCode { get; set; }
		public string ImportLocationProviderCode { get; set; }
		public string ImportPDAProviderCode { get; set; }
		public string ImportCatalogAdapterParms { get; set; } //двльше используем used ImportPath=C:\temp;ExportPath=C:\temp in CustomerFormViewModel.Params - set in Form.Editcustomer.Parameter
		public string ImportIturAdapterParms { get; set; } //DONE TO MaxCharacters //used in ExportPda adapter to store value of MaxCharacters For Name (File Type: Product Code & Name)
		public string ImportLocationAdapterParms { get; set; } //DONE TO used in ExportErp adapter to store value of MakatValue/MakatOriginalValue
		public string ImportPDAAdapterParms { get; set; }
		public string LogoPath { get; set; }
		public string ExportCatalogAdapterCode { get; set; }   //ExportToPDA
		public string ExportIturAdapterCode { get; set; }		//DONE TO ImportCatalogPath //used 	Static Path 1 for Import (first of all for complex adapter)
		public bool Print { get; set; }
		public string ReportPath { get; set; }			//DONE TO //used SendToOfficePath//	Static Path 3 for ExportERP (first of all for complex adapter)
		public DateTime CreateDate { get; set; }
		public DateTime ModifyDate { get; set; }	   	 //NOT DONE The Last update Catalog
		public string ReportName { get; set; }
		public string ReportContext { get; set; }		 //DONE		   ComplexAdapterCode
		public string ReportDS { get; set; }
		public string ImportSectionAdapterCode { get; set; }
		public string ExportSectionAdapterCode { get; set; }
		public string UpdateCatalogAdapterCode { get; set; }
		public string ExportERPAdapterCode { get; set; }
		public string ImportSupplierAdapterCode { get; set; }
		public string Restore { get; set; }
		public bool RestoreBit { get; set; }
		public string ImportBranchAdapterCode { get; set; }
		public string ExportBranchAdapterCode { get; set; }
		public string PriceCode { get; set; }
		public string PDAType { get; set; }
		public string MaintenanceType { get; set; }
		public string ProgramType { get; set; }
		public string MaskCode { get; set; }			//DONE 		//used 	Static Path 2 for ExportERP (first of all for complex adapter)

		//=========		11/02/2019
		public string ComplexStaticPath1 { get; set; }
		public string ComplexStaticPath2 { get; set; }
		public string ComplexStaticPath3 { get; set; }

		public string Host { get; set; }
		public string Port { get; set; }

		public string ImportCatalogPath { get; set; }		  //DONE	  from ExportIturAdapterCode
		public string ImportFromPdaPath { get; set; }
		public string ExportErpPath { get; set; }			  //DONE	   MaskCode
		public string ExportPdaPath { get; set; }
		public string SendToOfficePath { get; set; }	   //DONE	   ReportPath
		
		public DateTime LastUpdatedCatalog { get; set; }	  //NOT DONE
		public string Tag { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		public string Tag3 { get; set; }	   //public string CompareMakatOrBarcode { get; set; }	

		public string ComplexAdapterCode { get; set; }
		public string ComplexAdapterParametr { get; set; }
		public string ComplexAdapterParametr1 { get; set; }
		public string ComplexAdapterParametr2 { get; set; }
		public string ComplexAdapterParametr3 { get; set; }
		public string ComplexAdapterParametrERPCode { get; set; }
		public string ComplexAdapterParametrInventorDateTime { get; set; }
		public string StatusAuditConfig { get; set; } // что -то за идея была, что проверять в каком статусе Customer InProcess or Not


		public string ImportFamilyAdapterCode { get; set; }
		public string MaxCharacters { get; set; }					   //DONE
		public string MakatOrMakatOriginal { get; set; }		  //DONE
		// Need to add
		//CreateProfileAutomatically - сейчас использую	  ComplexAdapterParametr3
	
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Customer)) return false;
			return Equals((Customer)obj);
		}

		public bool Equals(Customer other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Code, this.Code);
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);
		}

		public Customer()
		{
			Code = "";
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
		}

		public Customer(AuditConfig auditConfig)
		{
			Code = auditConfig.CustomerCode;
			DBPath = auditConfig.CustomerCode;
			Name = auditConfig.CustomerCode;
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

		public Customer(Inventor inventor)
		{
			Code = inventor.CustomerCode;
			DBPath = inventor.CustomerCode;
			Name = inventor.CustomerCode;
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

		public Customer(Branch branch)
		{
			Code = branch.CustomerCode;
			DBPath = branch.CustomerCode;
			Name = branch.CustomerCode;
			CreateDate = DateTime.Now;
			ModifyDate = DateTime.Now;
			LastUpdatedCatalog = DateTime.Now;
			RestoreBit = true;
			Restore = "Restore form Customer Code: " + branch.CustomerCode;
			Description = "Restore form Customer Code: " + branch.CustomerCode;
			Address = "";
			ContactPerson = "";
			Phone = "";
			Mail = "";
			Fax = "";
		}
	}
}
