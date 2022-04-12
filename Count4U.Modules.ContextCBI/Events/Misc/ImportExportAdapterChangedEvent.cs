using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events.Misc
{
    public class ImportExportAdapterChangedEvent : CompositePresentationEvent<ImportExportAdapterChangedEventPayload>
    {
         
    }

    public class ImportExportAdapterChangedEventPayload
    {
        public IImportModuleInfo ImportModule { get; set; }
		public IImportModuleInfo UpdateModule { get; set; }
        public IExportErpModuleInfo ExportErpModule { get; set; }
		public IExportPdaModuleInfo ExportPdaModule { get; set; }
		public ResultModuleInfo ResultModule { get; set; }
		
    }
}