using System;
using System.Text;
using Count4U.Common.ViewModel.Adapters.Export;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Common.ViewModel.Adapters.Abstract
{
    public interface IExportPdaModule : IAdapterModule
    {       
        bool CanExport();
        void RunClear(ExportClearCommandInfo info);
		void RunClear();
		void ClearFolders(CBIState state);
        void RunExportPdaBase(ExportCommandInfo info);
		void RunExportPdaWithoutGUIBase(ExportCommandInfo info, CBIState state);
		void RunExportPdaClearWithoutGUIBase(ExportCommandInfo info, CBIState state);
        string GetExportToPDAFolderPath(bool withCurrentDomainObject = true);
        Action RaiseCanExport { set; }
        void OnNavigatedFrom(NavigationContext navigationContext);
    }
}