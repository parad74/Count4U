using System;
using System.IO;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.ServiceLocation;
using Type = System.Type;

namespace Count4U.Common.Interfaces.Adapters
{
	public interface IImportModuleInfo : IBaseImportExportModuleInfo
    {
       // string Name { get; set; }
      //  string Title { get; set; }
        Type UserControlType { get; set; }
      //  bool IsDefault { get; set; }
        ImportDomainEnum ImportDomainEnum { get; set; }
      //  string Description { get; set; }
    }

	public class ImportModuleInfo : ComplexModuleBaseViewModel, IImportModuleInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public Type UserControlType { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }

        public ImportDomainEnum ImportDomainEnum { get; set; }

		public ImportModuleInfo() : base(){ } 

		//public ImportModuleInfo(IContextCBIRepository contextCbiRepository,
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
		//	string path = "";
		//	if (currentDomainObject == null) return path;
		//	path = contextCbiRepository.GetImportFolderPath(currentDomainObject);
		//	if (string.IsNullOrWhiteSpace(subFolder) == true) return path;

		//	string pathWithSubFolder = path.TrimEnd('\\') + @"\" + subFolder.Trim('\\');
		//	if (Directory.Exists(pathWithSubFolder) == false)
		//	{
		//		try
		//		{
		//			Directory.CreateDirectory(pathWithSubFolder);
		//		}
		//		catch (Exception exp)
		//		{
		//			pathWithSubFolder = "";
		//		}
		//	}
		//	return pathWithSubFolder;
		//}
	 
    }
    
}