using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.ServiceLocation;
using Type = System.Type;

namespace Count4U.Common.Interfaces.Adapters
{
	//Типа Send to Office
	public interface IResultModuleInfo : IBaseImportExportModuleInfo
    {
		string ZipPath { get; set; }
       // string Name { get; set; }
      //  string Title { get; set; }
      //  Type UserControlType { get; set; }
      //  bool IsDefault { get; set; }
      //  ImportDomainEnum ImportDomainEnum { get; set; }
      //  string Description { get; set; }
    }

	public class ResultModuleInfo : ComplexModuleBaseViewModel, IResultModuleInfo
    {
		public string ZipPath { get; set; }
		public string ConfigPathWithFile { get; set; }
		public string ConfigPath{ get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }

		public ResultModuleInfo() : base() { }
		//public ResultModuleInfo(IContextCBIRepository contextCbiRepository,
		//	ILog logImport,
		//	IServiceLocator serviceLocator,
		//	IIniFileParser iniFileParser,
		//	IUserSettingsManager userSettingsManager,
		//	IDBSettings dbSettings,
		//	IContextCBIRepository contextCBIRepository)
		//	: base(contextCbiRepository,
		//	 logImport,
		//	 serviceLocator,
		//	 iniFileParser,
		//	 userSettingsManager,
		//	 dbSettings,
		//	 contextCBIRepository)
		//{
		
		//}

		//public override string GetObjectWorkingFolderPath(IContextCBIRepository contextCbiRepository, IDBSettings dbSettings, object currentDomainObject, string subFolder = "")
		//{
		//	if (currentDomainObject == null) return "";
		//	return ConfigPath;
		//}

	
	}
    
}