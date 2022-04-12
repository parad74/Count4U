using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Count4U.ImportPdaAdvancedAdapter
{
    public class ImportPdaAdvancedAdapter5ViewModel : ImportPdaAdvancedAdapterCommonViewModel
    {
        public ImportPdaAdvancedAdapter5ViewModel(
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
				IPValueStr2 = 7,
				IPValueStr3 = 8,
				IPValueStr4 = 9,
				IPValueStr5 = 10,
				IPValueStr6 = 11,
				IPValueStr7 = 12,
				IPValueStr8 = 13,
				IPValueStr9 = 14,
				IPValueStr10 = 15,
				IPValueFloat1 = 16,
				IPValueFloat2 = 17,
				IPValueFloat3 = 18,
				IPValueFloat4 = 19,
				IPValueFloat5 = 20,
				IPValueInt1 = 21,
				IPValueInt2 = 22,
				IPValueInt3 = 23,
				IPValueInt4 = 24,
				IPValueInt5 = 25,
				IPValueBit1 = 26,
				IPValueBit2 = 27,
				IPValueBit3 = 28,
				IPValueBit4 = 29,
				IPValueBit5 = 30
			};
		}

        public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			Dictionary<ImportProviderParmEnum, string> iniData1 = base.GetIniData("ImportPdaAdvancedAdapter5");
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