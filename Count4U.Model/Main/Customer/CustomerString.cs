using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Model.Main
{
    public class CustomerString
    {
		//public long ID { get; set; }
		public string Address { get; set; }
        public string Code { get; set; }
        public string ContactPerson { get; set; }
        public string Description { get; set; }
        public string Fax { get; set; }
		//public System.Byte[] Logo { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
		public string DBPath { get; set; }
		//public string ImportCatalogProviderCode { get; set; }
		//public string ImportIturProviderCode { get; set; }
		//public string ImportLocationProviderCode { get; set; }
		//public string ImportPDAProviderCode { get; set; }
		//public string ImportCatalogAdapterParms { get; set; }
		//public string ImportIturAdapterParms { get; set; }
		//public string ImportLocationAdapterParms { get; set; }
		//public string ImportPDAAdapterParms { get; set; }
		//public string LogoPath { get; set; }
		//public string ExportCatalogAdapterCode { get; set; }
		//public string ExportIturAdapterCode { get; set; }



		public CustomerString()
		{
				Address = "";
				ContactPerson = "";
				Description = "";
				Fax = "";
				Mail = "";
				Name = "";
				Phone = "";
		}


	}
}
