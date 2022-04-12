using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using System.Xml.Linq;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IExportProvider 
	{
		void Export();
		void Clear();
		Dictionary<ImportProviderParmEnum, object> Parms { get; set; }
		string FromPathDB { get; set; }
		string ToPathFile { get; set; }
		Encoding ProviderEncoding { get; set; }
		//MaskPackage ParserMaskPackage { get; set; }
	}
}
