using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Configuration.Dynamic
{
	[Serializable]
	public class PropertyLink
	{
		public string PropertyNameInDomainType { get; set; }
		public string PropertyCode { get; set; }
		public string CodeLocalizationEditorLabel { get; set; }
		public string DefaultEditorLabel { get; set; }
		public int NN { get; set; }

		public PropertyLink()
		{
			PropertyNameInDomainType = "";
			PropertyCode = "";
			CodeLocalizationEditorLabel = "";
			DefaultEditorLabel = "";
			NN = 0;
		}
	}
}
