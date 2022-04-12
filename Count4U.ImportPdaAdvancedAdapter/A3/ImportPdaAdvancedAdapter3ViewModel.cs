using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Count4U.GenerationReport;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel;

namespace Count4U.ImportPdaAdvancedAdapter
{
    public class ImportPdaAdvancedAdapter3ViewModel : ImportPdaAdvancedAdapterCommonViewModel
    {
        public ImportPdaAdvancedAdapter3ViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager,
            IGenerateReportRepository generateReportRepository,
            IUnityContainer unityContainer) :
            base(serviceLocator, 
            contextCBIRepository, 
            iturRepository, 
            eventAggregator, 
            regionManager, 
            logImport, 
            iniFileParser,
			temporaryInventoryRepository, 
            userSettingsManager, 
            generateReportRepository, 
            unityContainer)
        {
			base.ParmsDictionary.Clear();
        }

        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
            //init GUI
            this._fileName = FileSystem.inData;
            this.PathFilter = "*.txt|*.txt|All files (*.*)|*.*";
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;
            StepTotal = 1;
			this.InitInventProductAdvancedField();
		}

		private void InitInventProductAdvancedField()
		{
			this.InventProductAdvancedFieldIdx = new InventProductAdvancedFieldIndex
			{
				IPValueStr1 = 6,
				IPValueStr2 = 11,
				IPValueStr3 = 12,
				IPValueStr4 = 13,
				IPValueStr5 = 14,
				IPValueStr6 = 15,
				IPValueStr7 = 16,
				IPValueStr8 = 17,
				IPValueStr9 = 19,
				IPValueStr10 = 20,
				IPValueInt1 = 7,
				IPValueInt2 = 8,
				IPValueInt3 = 9,
				IPValueInt4 = 10,
				IPValueBit1 = 18,
				IPValueBit2 = 21

			};
		}


        public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			Dictionary<ImportProviderParmEnum, string> iniData1 = base.GetIniData("ImportPdaAdvancedAdapter3");
			this.InventProductAdvancedFieldIdx = iniData1.SetValue(this.InventProductAdvancedFieldIdx);

            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);

            try
            {
                if (base._isDirectory)
                {
                    if (!Directory.Exists(this.Path))
                        Directory.CreateDirectory(this.Path);
                }
            }
            catch (Exception exc)
            {
                WriteErrorExceptionToAppLog("Create inData directory", exc);
            }
        }   
    }
}