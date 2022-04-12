using System;

namespace Count4U.Model.Audit
{
    public  class Inventor
	{
		public long ID { get; set; }
        public string Code { get; set; }
	    public string CustomerCode { get; set; }
        public string BranchCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime InventorDate { get; set; }
	    public string ImportCatalogAdapterCode { get; set; }
		public string ImportIturAdapterCode { get; set; }
		public string ImportLocationAdapterCode { get; set; }
		public string ImportCatalogParms { get; set; }
		public string ImportIturParms { get; set; }				  //Done   используется   ImportFamilyAdapterCode
		public string ImportLocationParms { get; set; }
    	public string Status    { get; set; }
		public string StatusCode { get; set; }
		public string DBPath { get; set; }
		public string ImportSectionAdapterCode { get; set; }
		public string UpdateCatalogAdapterCode { get; set; }
		public string ImportPDAProviderCode { get; set; }
		public string Restore  { get; set; }
		public bool RestoreBit  { get; set; }
		public DateTime CompleteDate { get; set; }					 //NOT DONE The Last update Catalog
		public string Manager { get; set; }								   //NOT DONE Используется в Set адаптерах - проверить и заменить на дополнительное поле
		public string ExportERPAdapterCode { get; set; }
		public string ImportSupplierAdapterCode { get; set; }
		public string PriceCode { get; set; }

		public string ReportName { get; set; }
		public string ReportContext { get; set; }
		public string ReportDS { get; set; }
		public string ReportPath { get; set; }
		public bool Print { get; set; }

		public string PDAType { get; set; }					//DONE TO ExportCatalogAdapterCode 	//Sorry используем как Export PDA Adapter Code
		public string MaintenanceType { get; set; }
		public string ProgramType { get; set; }

	
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
		public string Tag3 { get; set; }    //Manager Name

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
 
		public string ExportSectionAdapterCode { get; set; }
		public string ExportCatalogAdapterCode { get; set; } //DONE использовали 	 PDAType
		public string ParentCode { get; set; }		//    - Если есть родительский инвентор, для того чтобы дерево построить.	Manager - пока использовать это поле буду
		public string SourceCode { get; set; }			   	// SourceCode - если наследуется клонированием из другого Inventor
		public string RootCode { get; set; }			 // RootCode	 - усли наслкдуется от кого-то то самый корень
		public string ProcessCode { get; set; }		   	//ProcessCode - это для android 
	
		

		public Inventor Clone(string newInventorCode, string parentInventorCode)
		{
			string newDBPath =this.DBPath.Replace(this.Code, newInventorCode);
			return new Inventor()
			{
				DBPath = newDBPath,
				Code = newInventorCode,

				CustomerCode = this.CustomerCode,
				BranchCode = this.BranchCode,
				Name = this.Name,
				Description = this.Description,
				CreateDate = this.CreateDate,
				LastUpdatedCatalog	= DateTime.Now,
				InventorDate = this.InventorDate,
				ImportCatalogAdapterCode = this.ImportCatalogAdapterCode,
				ImportIturAdapterCode = this.ImportIturAdapterCode,
				ImportLocationAdapterCode = this.ImportLocationAdapterCode,
				ImportCatalogParms = this.ImportCatalogParms,
				ImportIturParms = this.ImportIturParms,
				ImportLocationParms = this.ImportLocationParms,
				Status = this.Status,
				StatusCode = this.StatusCode,

				ImportSectionAdapterCode = this.ImportSectionAdapterCode,
				UpdateCatalogAdapterCode = this.UpdateCatalogAdapterCode,
				ImportPDAProviderCode = this.ImportPDAProviderCode,
				Restore = this.Restore,
				RestoreBit = this.RestoreBit,
				CompleteDate = this.CompleteDate,					 //The Last update Catalog
				Manager = parentInventorCode,
				ExportERPAdapterCode = this.ExportERPAdapterCode,
				ImportSupplierAdapterCode = this.ImportSupplierAdapterCode,
				PriceCode = this.PriceCode,
				ReportName = this.ReportName,
				ReportContext = this.ReportContext,
				ReportDS = this.ReportDS,
				ReportPath = this.ReportPath,
				Print = this.Print,
				PDAType = this.PDAType,
				MaintenanceType = this.MaintenanceType,
				ProgramType = this.ProgramType,
				ComplexStaticPath1 = this.ComplexStaticPath1,
				ComplexStaticPath2 = this.ComplexStaticPath2,
				ComplexStaticPath3 = this.ComplexStaticPath3,
				Host = this.Host,
				Port = this.Port,
				ImportCatalogPath = this.ImportCatalogPath,
				ImportFromPdaPath = this.ImportFromPdaPath,
				ExportErpPath = this.ExportErpPath,
				ExportPdaPath = this.ExportPdaPath,
				SendToOfficePath = this.SendToOfficePath,
				Tag = this.Tag,
				Tag1 = this.Tag1,
				Tag2 = this.Tag2,
				Tag3 = this.Tag3,
				ComplexAdapterCode = this.ComplexAdapterCode,
				ComplexAdapterParametr = this.ComplexAdapterParametr,
				ComplexAdapterParametr1 = this.ComplexAdapterParametr1,
				ComplexAdapterParametr2 = this.ComplexAdapterParametr2,
				ComplexAdapterParametr3 = this.ComplexAdapterParametr3,
				ComplexAdapterParametrERPCode = this.ComplexAdapterParametrERPCode,
				ComplexAdapterParametrInventorDateTime = this.ComplexAdapterParametrInventorDateTime,
				StatusAuditConfig = this.StatusAuditConfig,
				ImportFamilyAdapterCode = this.ImportFamilyAdapterCode,
				MaxCharacters = this.MaxCharacters,
				MakatOrMakatOriginal = this.MakatOrMakatOriginal,
				ExportSectionAdapterCode = this.ExportSectionAdapterCode,
				ExportCatalogAdapterCode = this.ExportCatalogAdapterCode,
				ParentCode = this.ParentCode,
				SourceCode = this.SourceCode,
				RootCode = this.RootCode,
				ProcessCode = this.ProcessCode

			};
		}

	

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Inventor)) return false;
            return Equals((Inventor)obj);
        }

        public bool Equals(Inventor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Code, this.Code);	//TODO: ? CustomerCode, BtranchCode	  
		}

        public override int GetHashCode()
        {
            return (Code != null ? Code.GetHashCode() : 0);
        }


	}
}
