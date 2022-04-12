using System;
using System.Text;
using System.Windows.Shapes;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Common.ViewModel.Adapters.Export
{
    public abstract class ExportModuleBaseViewModel : ModuleBaseViewModel
    {
        private Action _raiseCanExport;
		private Encoding _encoding;

        protected ExportModuleBaseViewModel(IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator)        
            : base(contextCbiRepository, logImport, serviceLocator)
        {
            
        }

		protected abstract void EncondingUpdated();


		public override Encoding Encoding
		{
			get
			{
				return _encoding;
			}
			set
			{
				_encoding = value;

				RaisePropertyChanged(() => Encoding);

				this.EncondingUpdated();
			}
		}

        public Action RaiseCanExport
        {
            set { this._raiseCanExport = value; }
            protected get { return this._raiseCanExport; }
        }

        protected override string GetModulesFolderPath()
        {
            return FileSystem.ExportModulesFolderPath();
        }

		protected string GetPathToIniFile(string iniFileName)
		{
			return System.IO.Path.Combine(this.GetModulesFolderPath(), iniFileName);
		}
    }

}