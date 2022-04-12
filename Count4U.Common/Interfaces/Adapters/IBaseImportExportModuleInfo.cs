using Count4U.Model;
using Type = System.Type;

namespace Count4U.Common.Interfaces.Adapters
{
	public interface IBaseImportExportModuleInfo
    {
        string Name { get; set; }
        string Title { get; set; }
		//Type UserControlType { get; set; }
		bool IsDefault { get; set; }
		//ImportDomainEnum ImportDomainEnum { get; set; }
		string Description { get; set; }
	
    }

  
    
}