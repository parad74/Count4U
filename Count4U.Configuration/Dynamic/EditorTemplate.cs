using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Configuration.Dynamic
{
    [Serializable]
	public class EditorTemplate
	{
	    public EditorTemplate()
	    {
	        PropertyLinkList = new List<PropertyLink>();
			DetailPropertyLinkList = new List<PropertyLink>();
			DetailPropertyLinkDictionary = new Dictionary<string, PropertyLink>();
	    }

		public string Code { get; set; }	 	//key
		public string DomainType { get; set; }
		public string DetailDomainType { get; set; }
		public string CodeLocalizationEditorLabel { get; set; }
		public string DefaultEditorLabel { get; set; }				
		public List<PropertyLink> PropertyLinkList { get; set; }
		public Dictionary<string, PropertyLink> DetailPropertyLinkDictionary { get; set; }
		public List<PropertyLink> DetailPropertyLinkList { get; set; }
		public string ViewName { get; set; }
        public Type Type { get; set; }
		public Type DetailType { get; set; }
        public string Title { get; set; }
        public int NN { get; set; }
		public int DetailPart { get; set; }
	}
	
}
