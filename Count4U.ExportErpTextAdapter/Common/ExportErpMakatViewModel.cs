using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTextAdapter
{
    public abstract class ExportErpMakatViewModel : ExportErpModuleBaseViewModel
    {
		protected bool _makat;
        protected bool _makatOriginal;
		private Dictionary<string, string> _parmsDictionary;

        public ExportErpMakatViewModel(
            IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
            IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {
            //UseWithout Mask!
            this._makatOriginal = true;
	
        }

	
        public bool Makat
        {
            get { return this._makat; }
            set
            {
                this._makat = value;
                RaisePropertyChanged(() => this.Makat);

                //if (this._makat == true)
                //{
                this._makatOriginal = (!this._makat);
                RaisePropertyChanged(() => this.MakatOriginal);
                //}
            }
        }

	

        public bool MakatOriginal
        {
            get { return this._makatOriginal; }
            set
            {
                this._makatOriginal = value;
                RaisePropertyChanged(() => this.MakatOriginal);

                //if (this._makatOriginal == true)
                //{
                this._makat = (!this._makatOriginal);
                RaisePropertyChanged(() => this.Makat);
                //}
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (base.CurrentInventor != null)
            {
                Inventor inventor = base.CurrentInventor;
                if (inventor.ImportLocationParms == Common.Constants.Misc.MakatValue)
                {
                    this.Makat = true;
                }
                else if (inventor.ImportLocationParms == Common.Constants.Misc.MakatOriginalValue)
                {
                    this.MakatOriginal = true;
                }
                else
                {
                    this.Makat = true;
                }
            }
        }

		protected override void InitFromConfig(ExportErpCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = this.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						//string exportPath = XDocumentConfigRepository.GetExportPath(this, configXDoc);
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
		}

    }
}