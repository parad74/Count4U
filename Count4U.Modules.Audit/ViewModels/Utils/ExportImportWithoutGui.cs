using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Count4U.Common.Enums;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Main;
using Count4U.Modules.Audit.ViewModels.Export;
using Count4U.Modules.Audit.ViewModels.Import;
using Count4U.Modules.ContextCBI.Xml.Config;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Modules.Audit
{
	public static class  ExportImportWithoutGui
	{

		public static void GenerationAndSaveExportErpConfig(Customer customer, IServiceLocator serviceLocator,
			IExportErpModuleInfo exportErpModuleInfo,
			DomainObjectType fromDomainObjectType, HowUse pathHowUse,
			CBIState state, bool resave) //base.CurrentCustomer this.SelectedExportErp
		{
			if (exportErpModuleInfo == null) return;	  //from customer
			ExportErpModuleBaseViewModel exportErpModuleBaseViewModel = null;
			string adapterName = exportErpModuleInfo.Name;
			try
			{
				exportErpModuleBaseViewModel = serviceLocator.GetInstance<ExportErpModuleBaseViewModel>(adapterName);
			}
			catch { }

			if (exportErpModuleBaseViewModel == null)
			{
				return;
			}

			//IExportErpModule viewModel = exportErpModuleBaseViewModel as IExportErpModule;
			ExportErpWithModulesViewModel exportErpWithModulesViewModel = serviceLocator.GetInstance<ExportErpWithModulesViewModel>();
			exportErpWithModulesViewModel.SaveConfigByDefaultForCustomer(customer, exportErpModuleInfo, exportErpModuleBaseViewModel,
				fromDomainObjectType, pathHowUse, resave);
			return;
		}

		public static void GenerationAndSaveExportPdaConfig(Customer customer, IServiceLocator serviceLocator,
				IExportPdaModuleInfo exportPdaModuleInfo,
			DomainObjectType fromDomainObjectType, HowUse pathHowUse,
			CBIState state, bool resave) //base.CurrentCustomer this.SelectedExportErp
		{
			if (exportPdaModuleInfo == null) return;	  //from customer
			ExportPdaModuleBaseViewModel exportPdaModuleBaseViewModel = null;
			string adapterName = exportPdaModuleInfo.Name;
			try
			{
				exportPdaModuleBaseViewModel = serviceLocator.GetInstance<ExportPdaModuleBaseViewModel>(adapterName);
			}
			catch { }

			if (exportPdaModuleBaseViewModel == null)
			{
				return;
			}

			//IExportErpModule viewModel = exportErpModuleBaseViewModel as IExportErpModule;
			ExportPdaWithModulesViewModel exportPdaWithModulesViewModel = serviceLocator.GetInstance<ExportPdaWithModulesViewModel>();
			exportPdaWithModulesViewModel.SaveConfigByDefaultForCustomer(customer, exportPdaModuleInfo, exportPdaModuleBaseViewModel,
				fromDomainObjectType, pathHowUse, resave);
			return;
		}

		// ================= ImportPDA
		public static void GenerationAndSaveImportConfig(Customer customer, IServiceLocator serviceLocator,
		ImportModuleInfo importPdaModuleInfo,
			DomainObjectType fromDomainObjectType, HowUse pathHowUse,
			CBIState state, bool resave) //base.CurrentCustomer this.SelectedExportErp
		{
			if (importPdaModuleInfo == null) return;	  //from customer
			ImportModuleBaseViewModel importModuleBaseViewModel = null;
			string adapterName = importPdaModuleInfo.Name;
			try
			{
				importModuleBaseViewModel = serviceLocator.GetInstance<ImportModuleBaseViewModel>(adapterName);
			}
			catch { }

			if (importModuleBaseViewModel == null)
			{
				return;
			}

			//IExportErpModule viewModel = exportErpModuleBaseViewModel as IExportErpModule;
			ImportFromPdaViewModel importFromPdaViewModel = serviceLocator.GetInstance<ImportFromPdaViewModel>();
			ImportWithModulesBaseViewModel importWithModulesBaseViewModel = importFromPdaViewModel as ImportWithModulesBaseViewModel;
			//SaveConfigByDefaultForCustomer(Customer customer,
			//	ImportModuleInfo selectedAdapter,
			//	ImportModuleBaseViewModel importModuleBaseViewModel, bool resave = false)

			importWithModulesBaseViewModel.SaveConfigByDefaultForCustomer(customer, importPdaModuleInfo, importModuleBaseViewModel,
				fromDomainObjectType, pathHowUse, state, resave);
			return;
		}


		//public static XDocument UpdatePath(XDocument doc)
		//{
		//	if (doc == null) return new XDocument();
		//	doc = ViewModelConfigRepository.RemoveAllElement(doc, "FROMPATH");
		//	return doc;
		//}

		//protected XDocument UpdateRelativePath(XDocument doc, string adapterType, string fromDomainObjectType, HowUse pathHowUse,
		//	string absolutePath = @"c:\temp",  bool _isDefault = false)
		//{
		//	XElement rootPath = new XElement("FROMPATH");
		//	rootPath.Add(new XAttribute("domainobject", fromDomainObjectType));


		//	if (adapterType == "ImportWithModulesBaseView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.import.ToString()));
		//	}
		//	else if (adapterType == "ExportPdaWithModulesView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.exportpda.ToString()));
		//	}
		//	else if (adapterType == "ExportErpWithModulesView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.exporterp.ToString()));
		//	}



		//	if (pathHowUse == HowUse.relative)
		//	{
		//		rootPath.Add(new XAttribute("howuse", HowUse.relative.ToString()));
		//		rootPath.Add(new XAttribute("from", From.indata.ToString()));
		//		rootPath.Add(new XAttribute("value", ""));
		//	}
		//	else if (pathHowUse == HowUse.asis)
		//	{
		//		rootPath.Add(new XAttribute("howuse", HowUse.asis.ToString()));
		//		rootPath.Add(new XAttribute("from", From.absolute.ToString()));
		//		rootPath.Add(new XAttribute("value", absolutePath));
		//	}
		//	else if (pathHowUse == HowUse.ftp)
		//	{
		//		rootPath.Add(new XAttribute("from", From.ftp.ToString()));
		//	}



		//	string isdefault = "0";
		//	if (_isDefault == true) isdefault = "1";
		//	rootPath.Add(new XAttribute("isdefault", isdefault));

		//	doc.Root.AddFirst(rootPath);
		//	return doc;
		//}
	}


}
