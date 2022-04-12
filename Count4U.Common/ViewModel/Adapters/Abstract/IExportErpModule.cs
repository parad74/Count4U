using System;
using System.Collections.Generic;
using System.Text;
using Count4U.Common.ViewModel.Adapters.Export;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Common.ViewModel.Adapters.Abstract
{
    public interface IExportErpModule : IAdapterModule
    {      
        Action RaiseCanExport { set; }
        void RunExportBase(ExportErpCommandInfo info);
		void RunExportErpWithoutGUIBase(ExportErpCommandInfo info, CBIState state);
		void RunExportErpClearWithoutGUIBase(ExportErpCommandInfo info, CBIState state);
        void RunClear(ExportClearCommandInfo info);
        string BuildPathToExportErpDataFolder();
		Dictionary<string, string> ParmsDictionary { get; set; }
		void AddParamsInDictionary(string parameters);
		
		void OnNavigatedFrom(NavigationContext navigationContext);
    }
}