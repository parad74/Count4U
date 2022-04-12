using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.Web;
using Count4U.Model;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ImportCatalogKitAdapter.ProfileXml
{
	public class ImportCatalogProfileXmlAdapterViewModel : TemplateAdapterOneFileViewModel
	{
		public string _fileName {get; set;}
		private string _branchErpCode = String.Empty;
		private bool _updateProfileFromFtp;
		private bool _isInventorComboVisible;
		private readonly IInventorRepository _inventorRepository;
		private readonly FtpFolderProFile _ftpFolderProfile;
		private readonly IPropertyStrRepository _propertyStrRepository;
		
		private string _uidmInv;
		private string _uidCount4U;
	
		public ImportCatalogProfileXmlAdapterViewModel(IServiceLocator serviceLocator,
		  IContextCBIRepository contextCBIRepository,
		  IEventAggregator eventAggregator,
		  IRegionManager regionManager,
		  ILog logImport,
		  IIniFileParser iniFileParser,
		  ITemporaryInventoryRepository temporaryInventoryRepository,
		  IUserSettingsManager userSettingsManager,
			IPropertyStrRepository propertyStrRepository,
			IInventorRepository inventorRepository, 
		   FtpFolderProFile ftpFolderProfile) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
		{
			base.ParmsDictionary.Clear();
			this._ftpFolderProfile = ftpFolderProfile;
			this._inventorRepository = inventorRepository;
			this._propertyStrRepository = propertyStrRepository;

		
		}

	

		public bool UpdateProfileFromFtp
		{
			get { return _updateProfileFromFtp; }
			set
			{
				_updateProfileFromFtp = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => UpdateProfileFromFtp);
			}
		}


		public string UidmInv
		{
			get { return _uidmInv; }
			set
			{
				_uidmInv = value;
				RaisePropertyChanged(() => UidmInv);
			}
		}

		public string UidCount4U
		{
			get { return _uidCount4U; }
			set
			{
				_uidCount4U = value;
				RaisePropertyChanged(() => UidCount4U);
			}
		}


		public bool IsInventorComboVisible
		{
			get { return _isInventorComboVisible; }
			set
			{
				_isInventorComboVisible = value;
				RaisePropertyChanged(() => IsInventorComboVisible);
			}
		}



		private int GetStep()
		{
			int step = 1;
			if (this.UpdateProfileFromFtp == true) step++;
			return step;
		}
	 
		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);


		}

		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
						base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						if (System.IO.Path.GetExtension(base.Path) == ".xlsx") XlsxFormat = true; else XlsxFormat = false;
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

		protected override void RunImport(ImportCommandInfo info)
		{
			this.Import();
		}

		protected override void RunClear()
		{
			this.Clear();
		}

		#region Implementation of IImportAdapter

		public override void InitDefault(CBIState state = null)
		{
			if (state != null) base.State = state;
			base.ParmsDictionary.Clear();
			if (base.CurrentCustomer != null)
			{
				base.AddParamsInDictionary(base.CurrentCustomer.ImportCatalogAdapterParms);
			}

			if (base.CurrentBranch != null)
			{
				this._branchErpCode = base.CurrentBranch.BranchCodeERP;
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

			//init GUI
			this.PathFilter = "*.xml|*.xml|All files (*.*)|*.*";
			this._fileName = "profile.xml";
			base.IsInvertLetters = false;
			base.IsInvertWords = false;
			base.Encoding = System.Text.Encoding.GetEncoding(1255);

			base.StepTotal = this.GetStep();
			this.UpdateProfileFromFtp = false;
			this.IsInventorComboVisible = false;

			if (base.CurrentInventor != null)
			{
				this.UpdateProfileFromFtp = false;
				this.IsInventorComboVisible = true;
			}

			//for test убрать после
			Dictionary<string, string> keyPropertyDictionary =
				_propertyStrRepository.GetDictionaryProfileProperty(DomainObjectTypeEnum.Profile.ToString(), "UIDKey", base.GetDbPath);

			UidCount4U = _propertyStrRepository.GetUIDKey_Count4U(base.GetDbPath);
			RaisePropertyChanged(() => UidCount4U);
			UidmInv = _propertyStrRepository.GetUIDKey_mINV(base.GetDbPath);
			RaisePropertyChanged(() => UidmInv);
		}

		public override void InitFromIni()
		{
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogMerkavaXslxAdapter.ini"));
			//init GUI
			this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}

			this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);


		}

		public override void Import()
		{
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

			string branchErpCode = String.Empty;
			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

			StepCurrent = 0;

			if (this.UpdateProfileFromFtp == true)
			{
				if (base.CurrentInventor != null)
				{
					StepCurrent++;
					string rootFonderOnFtp = this._inventorRepository.Connection.RootFolderFtp(); //mINV 
					FtpCommandResult ftpCommandResult = new FtpCommandResult();
					Utils.RunOnUI(() => 
						this._ftpFolderProfile.InventorProfileCreate(base.CurrentCustomer, base.CurrentBranch, base.CurrentInventor, rootFonderOnFtp, ref ftpCommandResult)  );

					string fromFtpFonder = rootFonderOnFtp.Trim('\\')  + @"\" + 
						this._contextCBIRepository.BuildLongCodesPath(base.CurrentInventor).Trim('\\') + @"\Profile";
					string toLocalFolder = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\'));
					string fileName =  "profile.xml";
					string	messageCreateFolder	= "";
					 Utils.RunOnUI(() =>
						this._ftpFolderProfile.CopyFileFromFtp (fromFtpFonder,  toLocalFolder, fileName, ref messageCreateFolder));
					 base.LogImport.Add(MessageTypeEnum.Trace, messageCreateFolder);
				}
			}
		
	  		{																					 //PropertyStrProfileNativXslx2SdfParser
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportProfileXml2SdfProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this.Path;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				StepCurrent++;
				provider3.Clear();
				provider3.Import();
			}

			Dictionary<string, string> keyPropertyDictionary =
	_propertyStrRepository.GetDictionaryProfileProperty(DomainObjectTypeEnum.Profile.ToString(), "UIDKey", base.GetDbPath);

			UidCount4U = _propertyStrRepository.GetUIDKey_Count4U(base.GetDbPath);
			RaisePropertyChanged(() => UidCount4U);
			UidmInv = _propertyStrRepository.GetUIDKey_mINV(base.GetDbPath);
			RaisePropertyChanged(() => UidmInv);

			TemporaryInventory temporaryInventory = base.GetTemporaryInventoryWithImportModuleInfo
	(Common.Constants.ImportAdapterName.ImportCatalogNativPlusXslxAdapter, "IMPORT UIDKEY FROM PROFILE.XML", this._fileName, updateDateTime);
			this.TemporaryInventoryRepository.Insert(temporaryInventory, base.GetDbPath);   

			FileLogInfo fileLogInfo = new FileLogInfo();
			fileLogInfo.File = this.Path;
			base.SaveFileLog(fileLogInfo);
		}

		public override void Clear()
		{
			base.LogImport.Clear();
			UpdateLogFromILog();
		}

		#endregion
	}
}