using System.Collections.Generic;

namespace Count4U.Common.Interfaces.Adapters
{
    public interface IImportPdaAdapter
    {
        void RunPrintReport(string documentCode);
        List<string> NewDocumentCodeList { get; }
		List<string> NewSessionCodeList { get; }    
    }
}