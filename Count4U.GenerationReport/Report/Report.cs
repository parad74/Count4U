using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.GenerationReport
{
	public class Report
	{
		public long ID { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public string DomainContext { get; set; }
		public string TypeDS { get; set; }
		public string Path { get; set; }
		public string FileName { get; set; }
		public string DomainType { get; set; }
		public bool Menu { get; set; }
		public string MenuCaption { get; set; }
		public string Tag { get; set; }
		public bool AllowedContextSelectParm { get; set; }
		public bool Print { get; set; }
		public bool Landscape { get; set; }
		public int NN { get; set; }
		public string MenuCaptionLocalizationCode { get; set; }
		public bool IturAdvancedSearchMenu { get; set; }
		public bool InventProductAdvancedSearchMenu { get; set; }
		public bool InventProductSumAdvancedSearchMenu { get; set; }
		public bool CustomerSearchMenu { get; set; }
		public bool BranchSearchMenu { get; set; }
		public bool InventorSearchMenu { get; set; }
		public bool AuditConfigSearchMenu { get; set; }
		public bool IturSearchMenu { get; set; }
		public bool InventProductSearchMenu { get; set; }
		public bool LocationSearchMenu { get; set; }
		public bool ProductSearchMenu { get; set; }
		public string CodeReport { get; set; }
		public bool SupplierSearchMenu { get; set; }
		public bool SectionSearchMenu { get; set; }
		public bool ItursPopupMenu { get; set; }
		public bool IturPopupMenu { get; set; }
		public bool DocumentHeaderPopupMenu { get; set; }
		public bool ItursListPopupMenu { get; set; }
	}
}
