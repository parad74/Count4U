using System;
using System.IO;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Common.Interfaces.Adapters
{
	public interface IExportErpModuleInfo : IBaseImportExportModuleInfo
    {
        //string Name { get; set; }
      //  string Title { get; set; }
        Type UserControlType { get; set; }
        //bool IsDefault { get; set; }
        //string Description { get; set; }
    }

	public class ExportErpModuleInfo : ComplexModuleBaseViewModel, IExportErpModuleInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public Type UserControlType { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }

		public ExportErpModuleInfo() : base() { } 

	  //public ExportErpModuleInfo(IContextCBIRepository contextCbiRepository,
	  //	  ILog logImport,
	  //	  IServiceLocator serviceLocator,
	  //	  IIniFileParser iniFileParser,
	  //	  IUserSettingsManager userSettingsManager,
	  //	  IDBSettings dbSettings,
	  //	  IContextCBIRepository contextCBIRepository)
	  //	  : base(contextCbiRepository,
	  //	   logImport,
	  //	   serviceLocator,
	  //	   iniFileParser,
	  //	   userSettingsManager,
	  //	   dbSettings,
	  //	   contextCBIRepository)
	  //  {
		
	  //  }

	//	public override string GetObjectWorkingFolderPath(IContextCBIRepository contextCbiRepository, IDBSettings dbSettings, object currentDomainObject, string subFolder = "")
	//{
	//	string path = "";
	//	if (currentDomainObject == null) return path;
	//	path = contextCbiRepository.GetImportFolderPath(currentDomainObject);
	//	//path = GetExportErpFolderPath(dbSettings, currentDomainObject);
	//	 if (string.IsNullOrWhiteSpace(subFolder) == true) return path;

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

	public string GetExportErpFolderPath(IDBSettings dbSettings, object currentDomainObject)
		{
			//if (string.IsNullOrWhiteSpace(code) == true)
			//	return String.Empty;

			string currentObjectType = "";
			string code = "";
			if (currentDomainObject is Customer)
			{
				Customer customer = currentDomainObject as Customer;
				currentObjectType = "Customer";
				code = customer.Code;
			}
			else if (currentDomainObject is Branch)
			{
				Branch branch = currentDomainObject as Branch;
				currentObjectType = "Branch";
				code = branch.Code;
			}
			else if (currentDomainObject is Inventor)
			{
				Inventor inventor = currentDomainObject as Inventor;
				currentObjectType = "Inventor";
				code = inventor.Code;
			}

			return UtilsPath.ExportErpFolder(dbSettings, currentObjectType, code);
		}






	}
	
}
