using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using System.Text;
using Count4U.Common.Services.Ini;
using Count4U.Model.Count4U.Validate;
using Count4U.Common.ViewModel.Adapters.Import;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Constants;

namespace Count4U.ExportHT630Adapter
{
	public class ExportPdaHt630AdapterViewModel : ExportPdaModuleBaseViewModel	   //все для динамической смены\выбора  ExportPda Adapter
    {
        public ExportPdaHt630AdapterViewModel(
            IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager)
        {
			
        }

        protected override void InitDefault()
        {
			base.InvertPrefix = true;
			base.InvertWords = true;
			base.CutAfterInvert = false;
			base.Encoding = System.Text.Encoding.GetEncoding(1255);
        }

        protected override void InitFromIniFile()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
        }

		protected override ExportCommandInfo InitFromConfig(ExportCommandInfo info, CBIState state)
		{
			if (state == null) return info;
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
						//XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);
						info = this.FillInfoFromConfig(configXDoc, info);
						info.EncodingCodePage = 1255; //потому что 
						if (this.State != null)
						{
							if (this.CurrentCustomer != null)
							{
								info.Customer = this.CurrentCustomer;
							}
						}
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
			return info;
		}

        public override bool CanExport()
        {
            return true;
        }

		//Заполняем из конфига ExportCommandInfo
		private ExportCommandInfo FillInfoFromConfig(XDocument configXDocument, ExportCommandInfo info)
		{
			//	Dictionary<string, OperationXElement> GetDictenryFromInitXDocumentConfig(object viewModel, XDocument configXDocument)
			try
			{
				Dictionary<string, OperationXElement> dictionaryFromInitXDocument = XDocumentConfigRepository.GetDictionaryFromInitXDocumentConfig(configXDocument);

				info.WithoutProductName = false;
				info.MakatWithoutMask = false;
				info.CheckBaudratePDA = true;
				info.BarcodeWithoutMask = false;
				info.WithoutProductName = dictionaryFromInitXDocument.SetValueFromConfig("WithoutProductName", info.WithoutProductName);
				info.MakatWithoutMask = dictionaryFromInitXDocument.SetValueFromConfig("MakatWithoutMask", info.MakatWithoutMask);
				info.CheckBaudratePDA = dictionaryFromInitXDocument.SetValueFromConfig("CheckBaudratePDA", info.CheckBaudratePDA);
				info.BarcodeWithoutMask = dictionaryFromInitXDocument.SetValueFromConfig("BarcodeWithoutMask", info.BarcodeWithoutMask);

				info.IsInvertWords = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertWords", info.IsInvertWords);		  //true
				info.IsInvertLetters = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertWords", info.IsInvertLetters);			//true

	//	<PROPERTY returntype="Int32" name="Hash" value="0" />
	//<PROPERTY returntype="Int32" name="FileType" value="2" />
	//<PROPERTY returntype="Int32" name="QType" value="0" />
	//<PROPERTY returntype="Int32" name="UseAlphaKey" value="1" />
	//<PROPERTY returntype="Int32" name="ClientId" value="0" />
	//<PROPERTY returntype="Int32" name="NewItem" value="0" />

   				info.Hash = dictionaryFromInitXDocument.SetValueFromConfig("Hash", info.Hash);
				info.FileType = dictionaryFromInitXDocument.SetValueFromConfig("FileType", info.FileType);
				info.QType = dictionaryFromInitXDocument.SetValueFromConfig("QType", info.QType);
				info.UseAlphaKey = dictionaryFromInitXDocument.SetValueFromConfig("UseAlphaKey", info.UseAlphaKey);
				info.ClientId = dictionaryFromInitXDocument.SetValueFromConfig("ClientId", info.ClientId);
				info.NewItem = dictionaryFromInitXDocument.SetValueFromConfig("NewItem", info.NewItem);

			//<PROPERTY returntype="Boolean" name="IturNameType" value="True" />
			//<PROPERTY returntype="String" name="IturNamePrefix" value="אתר" />
			//	<PROPERTY returntype="Int32" name="MaxLen" value="0" />
			//<PROPERTY returntype="Boolean" name="InvertPrefix" value="False" />
			//<PROPERTY returntype="Boolean" name="IsAddBinarySearch" value="False" />

				info.Password = dictionaryFromInitXDocument.SetValueFromConfig("Password", info.Password);
				info.IturTypeByName = dictionaryFromInitXDocument.SetValueFromConfig("IturTypeByName", info.IturTypeByName);

				bool isInvertPrefix = true;
				isInvertPrefix = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertPrefix", isInvertPrefix);
				info.IturInvertPrefix = isInvertPrefix;

				info.IturNamePrefix = dictionaryFromInitXDocument.SetValueFromConfig("IturNamePrefix", info.IturNamePrefix);
				string max = "16";
				max  = dictionaryFromInitXDocument.SetValueFromConfig("MaxCharacters", max);
				int intmax = 16;
				if (Int32.TryParse(max, out intmax) == true)
				info.MaxLen = intmax;

				info.IsAddBinarySearch = dictionaryFromInitXDocument.SetValueFromConfig("IsAddBinarySearch", info.IsAddBinarySearch);
				info.EncodingCodePage = 1255;
	//	<PROPERTY returntype="String" name="PdaTypeKey" value="HT630" />
	//<PROPERTY returntype="String" name="MaintenanceTypeKey" value="Regular" />
	//<PROPERTY returntype="String" name="ProgramTypeKey" value="Fashion" />
//	"PDAType\HT630"
//"MaintenanceType\Regular"
//"ProgramType\Fashion"

			info.PDAType = @"PDAType\" + dictionaryFromInitXDocument.SetValueFromConfig("PdaTypeKey", "");
			info.MaintenanceType = @"MaintenanceType\" + dictionaryFromInitXDocument.SetValueFromConfig("MaintenanceTypeKey", "");
			info.ProgramType = @"ProgramType\" + dictionaryFromInitXDocument.SetValueFromConfig("ProgramTypeKey", "");
			}
			catch { }
			return info;
		}



        protected override void RunExportInner(ExportCommandInfo info)
        {
            base.WithoutProductName = info.WithoutProductName ? "1" : "";
            base.MakatWithoutMask = info.MakatWithoutMask ? "1" : "";
			base.CheckBaudratePDA = info.CheckBaudratePDA ? "1" : "";
            base.BarcodeWithoutMask = info.BarcodeWithoutMask ? "1" : "";
			base.InvertWords = info.IsInvertWords;
			base.InvertLetters = info.IsInvertLetters;
     
            base.Hash = info.Hash;
            base.FileType = info.FileType;
            base.QType = info.QType;
            base.UseAlphaKey = info.UseAlphaKey;
            base.ClientId = info.ClientId;
            base.NewItem = info.NewItem;
			base.Password = "";
            base.CustomerCode = info.Customer.Code;
            base.CustomerName = info.Customer.Name;
            base.IturNameType = info.IturTypeByName;
            base.IturNamePrefix = info.IturNamePrefix;
            base.MaxLen = info.MaxLen;
            base.InvertPrefix = info.IturInvertPrefix;
			base.IsAddBinarySearch = info.IsAddBinarySearch;
			if (info.EncodingCodePage == 0) info.EncodingCodePage = 1255;
			base.Encoding = System.Text.Encoding.GetEncoding(info.EncodingCodePage);
		    base.PDAType = info.PDAType;
            base.MaintenanceType = info.MaintenanceType;
            base.ProgramType = info.ProgramType;
			
            this.Export();
        }

		//public override void RunClear(ExportClearCommandInfo info)
		//{
		//	this.Clear();
		//	if (info.Callback != null) Utils.RunOnUI(info.Callback);
		//}

		public override void RunClear()
		{
			this.Clear();
		}

		public override void ClearFolders(CBIState state) { }


        public void Export()
        {
			if (this.State != null)
			{
				if (this.CurrentCustomer != null)
				{
					base.CustomerCode = this.CurrentCustomer.Code;
					base.CustomerName = this.CurrentCustomer.Name;
				}
			}
			

            string processFolder = base.GetExportToPDAFolderPath(false) + @"\" + "Process";
            string objectCodeFolder = base.GetExportToPDAFolderPath(true);
            base.ClearFolder(processFolder);
            base.ClearFolder(objectCodeFolder);
		     string lookupFileInProcessFolder = String.Empty;
            if (base.FileType != 0)	  //No LookupFile
            {
            

                IExportProvider provider =
                    base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportCatalogPdaHt630FileProvider.ToString());
				//if (base.CurrentInventor.ImportCatalogAdapterCode == Common.Constants.ImportAdapterName.ImportCatalogMade4NetAdapter)
				//{
				//	provider =
				//	base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportCatalogPdaHt630FileProvider.ToString());
				//}
                provider.Parms.Clear();
                provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
                provider.Parms[ImportProviderParmEnum.FileType] = base.FileType;
                provider.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
                provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
				provider.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
				provider.ProviderEncoding = base.Encoding;
                provider.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
				provider.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	provider.Parms[ImportProviderParmEnum.FileXlsx] = "1";//test
                //provider.ProviderEncoding = Encoding.GetEncoding(862);

                provider.FromPathDB = base.GetDbPath;
                lookupFileInProcessFolder = processFolder + @"\" + @"Lookup.txt";
                provider.ToPathFile = lookupFileInProcessFolder;
				provider.Export();

				string codePathFile = objectCodeFolder + @"\" + @"Lookup.txt";
				base.CopyFile(lookupFileInProcessFolder, codePathFile);
            }

            IExportProvider provider1 =
                base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportIturPdaHt630FileProvider.ToString());
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.Parms[ImportProviderParmEnum.IturNameType] = base.IturNameType ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.IturNamePrefix] = base.IturNamePrefix;
			provider1.Parms[ImportProviderParmEnum.InvertPrefix] = base.InvertPrefix ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.IsAddBinarySearch] = base.IsAddBinarySearch ? "1" : String.Empty;
			
			provider1.ProviderEncoding = base.Encoding;
			//provider1.ProviderEncoding = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty; 
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty; 
			provider1.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;

            //provider1.Parms.Add(ImportProviderParmEnum.WithoutProductName, base.WithoutProductName);
            //provider1.Parms.Add(ImportProviderParmEnum.BarcodeWithoutMask, base.BarcodeWithoutMask);
            //provider1.Parms.Add(ImportProviderParmEnum.MakatWithoutMask, base.MakatWithoutMask);

            provider1.FromPathDB = base.GetDbPath;
            string pathFile1 = processFolder + @"\" + @"Loc.txt";
            provider1.ToPathFile = pathFile1;
            provider1.Export();

            string codePathFile1 = objectCodeFolder + @"\" + @"Loc.txt";
            base.CopyFile(pathFile1, codePathFile1);

            IExportProvider provider2 =
                base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportCustomerConfigToFileProvider.ToString());
            provider2.Parms.Clear();
            provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider2.Parms[ImportProviderParmEnum.Hash] = base.Hash;
            provider2.Parms[ImportProviderParmEnum.FileType] = base.FileType;
            provider2.Parms[ImportProviderParmEnum.QType] = base.QType;
            provider2.Parms[ImportProviderParmEnum.UseAlphaKey] = base.UseAlphaKey;
            provider2.Parms[ImportProviderParmEnum.ClientId] = base.ClientId;
            provider2.Parms[ImportProviderParmEnum.NewItem] = base.NewItem;
			//provider2.Parms[ImportProviderParmEnum.NewItemBool] = base.NewItemBool;
			provider2.Parms[ImportProviderParmEnum.Password] = base.Password;
            provider2.Parms[ImportProviderParmEnum.IturNameType] = base.IturNameType ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.IsAddBinarySearch] = base.IsAddBinarySearch ? "1" : String.Empty;
            provider2.Parms[ImportProviderParmEnum.IturNamePrefix] = base.IturNamePrefix;
			provider2.Parms[ImportProviderParmEnum.InvertPrefix] = base.InvertPrefix ? "1" : String.Empty;
            provider2.Parms[ImportProviderParmEnum.CustomerCode] = base.CustomerCode;
            provider2.Parms[ImportProviderParmEnum.CustomerName] = base.CustomerName;
			string branchCodeLocal = base.CurrentBranch == null ? "000" : base.CurrentBranch.BranchCodeLocal;
			string branchName = base.CurrentBranch == null ? base.CurrentCustomer.Name : base.CurrentBranch.Name;
			provider2.Parms[ImportProviderParmEnum.StoreNumber] = branchCodeLocal;
			provider2.Parms[ImportProviderParmEnum.StoreName] = branchName;
            provider2.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//provider2.ProviderEncoding = base.Encoding;

            string pathFile2 = processFolder + @"\" + @"Config.ini";
            provider2.ToPathFile = pathFile2;
            provider2.Export();

            string codePathFile2 = objectCodeFolder + @"\" + @"Config.ini";
            base.CopyFile(pathFile2, codePathFile2);

            IIniFileInventor iniFileInventor =
                base.ServiceLocator.GetInstance<IIniFileInventor>();
            string paramsFolderPath = iniFileInventor.BuildParamsFolderPath();
            string codePathFile5 = paramsFolderPath + @"\" + @"Config.ini";
            base.CopyFile(pathFile2, codePathFile5);

            IExportProvider provider3 =
                base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportUserIniToFileProvider.ToString());
            provider3.Parms.Clear();
            provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);

            IDBSettings settings = base.ServiceLocator.GetInstance<IDBSettings>();
            string fromPathFile = settings.ExecutablePath().Trim('\\') + @"\" + @"op.txt";
            provider3.FromPathDB = Path.GetFullPath(fromPathFile);
            string toPathFile = processFolder + @"\" + @"op.txt";
            provider3.ToPathFile = toPathFile;
            provider3.Export();

            string codePathFile3 = objectCodeFolder + @"\" + @"op.txt";
            base.CopyFile(toPathFile, codePathFile3);

			BuildLookupFile(processFolder, objectCodeFolder, "Lookup.txt", "lookup.BLT");
			if (base.IsAddBinarySearch == true)
			{
				BuildLookupFile(processFolder, objectCodeFolder, "Loc.txt", "Loc.BLT");
			}

			CopyProgramTypeFiles(processFolder, objectCodeFolder);	//копирование всех файлов

			SaveFileLog(ExportPdaAdapterName.ExportHT630Adapter);
        }

        public void Clear()
        {
            base.LogImport.Clear();
            string processFolder = base.GetExportToPDAFolderPath(false) + @"\" + "Process";
            string objectCodeFolder = base.GetExportToPDAFolderPath(true);

            IExportProvider provider =
                base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportCatalogPdaHt630FileProvider.ToString());
            provider.ToPathFile = processFolder + @"\" + @"Lookup.txt";
            provider.Clear();
            provider.ToPathFile = objectCodeFolder + @"\" + @"Lookup.txt";
            provider.Clear();

            provider.ToPathFile = processFolder + @"\" + @"Loc.txt";
            provider.Clear();
            provider.ToPathFile = objectCodeFolder + @"\" + @"Loc.txt";
            provider.Clear();

            provider.ToPathFile = processFolder + @"\" + @"Config.ini";
            provider.Clear();
            provider.ToPathFile = objectCodeFolder + @"\" + @"Config.ini";
            provider.Clear();

            provider.ToPathFile = processFolder + @"\" + @"op.txt";
            provider.Clear();
            provider.ToPathFile = objectCodeFolder + @"\" + @"op.txt";
            provider.Clear();

            provider.ToPathFile = processFolder + @"\" + @"lookup.BLT";
            provider.Clear();
            provider.ToPathFile = objectCodeFolder + @"\" + @"lookup.BLT";
            provider.Clear();

			provider.ToPathFile = processFolder + @"\" + @"Loc.BLT";
			provider.Clear();
			provider.ToPathFile = objectCodeFolder + @"\" + @"Loc.BLT";
			provider.Clear();

            UpdateLogFromILog();
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="processFolder">"C:\Count4U\trunk\Count4U\Count4U.Model\ExportToPDA\Process"</param>
		/// <param name="objFolder">"C:\Count4U\trunk\Count4U\Count4U.Model\ExportToPDA\d74cec0f-7405-4521-9920-174d5c95bf5f"</param>
		/// <param name="lookupFileInProcessFolder">"C:\Count4U\trunk\Count4U\Count4U.Model\ExportToPDA\Process\Lookup.txt"</param>
		private void BuildLookupFile(string processFolder, string objFolder, string lookupFile, string bltFileName = "lookup.BLT")
        {
			string lookupFileInProcessFolder = processFolder + @"\" + lookupFile; //@"Lookup.txt";    lookupFile

            if (String.IsNullOrEmpty(processFolder) || String.IsNullOrEmpty(objFolder) || String.IsNullOrEmpty(lookupFileInProcessFolder))
                return;

            if (!Directory.Exists(processFolder) || !Directory.Exists(objFolder) || !File.Exists(lookupFileInProcessFolder))
                return;

            string exePath = Assembly.GetEntryAssembly().Location;
            string exeFolder = new FileInfo(exePath).Directory.FullName.Trim('\\') + @"\"; ;

            string processFolderPath = processFolder.Trim('\\') + @"\";

			//C:\Count4U\trunk\Count4U\Count4U\bin\Debug\BConvert.exe
            string convertExe = String.Format("{0}BConvert.exe", exeFolder);
			//input:"C:\Count4U\trunk\Count4U\Count4U.Model\ExportToPDA\Process\Lookup.txt" /output:"C:\Count4U\trunk\Count4U\Count4U.Model\ExportToPDA\Process\lookup.BLT" /fieldno:1 /fieldsep:, /error:"C:\Count4U\trunk\Count4U\Count4U.Model\ExportToPDA\Process\ERR.LOG"
			//string paramLine = String.Format(@"/input:""{0}"" /output:""{1}lookup.BLT"" /fieldno:1 /fieldsep:, /error:""{1}ERR.LOG""",
			//	lookupFile, processFolderPath);

			//if (bltFileName != "lookup.BLT")
			//{
			string paramLine = String.Format(@"/input:""{0}"" /output:""{1}{2}"" /fieldno:1 /fieldsep:, /error:""{1}{2}_ERR.LOG""",
			   lookupFileInProcessFolder, processFolderPath, bltFileName);
			//}

            Process pr = System.Diagnostics.Process.Start(convertExe, paramLine);
            pr.WaitForExit();

			string resultSourceFile = Path.Combine(processFolder, bltFileName);
			string resultDestFile = Path.Combine(objFolder, bltFileName);

            if (File.Exists(resultSourceFile))
            {
                if (File.Exists(resultDestFile))
                    File.Delete(resultDestFile);

                File.Copy(resultSourceFile, resultDestFile);
            }
        }

        private void CopyProgramTypeFiles(string processFolder, string objFolder)
        {
            try
            {
                string sourceDir = base.GetExportToPDAFolderPath(false);
                sourceDir = Path.Combine(sourceDir, Common.Constants.ExportPdaAdapterName.ExportHT630Adapter);
                if (!Directory.Exists(sourceDir))
                {
                    Directory.CreateDirectory(sourceDir);
                }

                string[] folders = new string[] { base.PDAType, base.MaintenanceType, base.ProgramType };

                foreach (string folder in folders)
                {
                    if (String.IsNullOrWhiteSpace(folder)) continue;

                    string finalDir = Path.Combine(sourceDir, folder.Trim('"'));

                    if (!Directory.Exists(finalDir))
                    {
                        Directory.CreateDirectory(finalDir);
                    }

                    foreach (string file in Directory.EnumerateFiles(finalDir))
                    {
                        FileInfo fi = new FileInfo(file);

                        string[] targetFolders = new string[] { processFolder, objFolder };

                        foreach (string targetFolder in targetFolders)
                        {
                            string targetFile = Path.Combine(targetFolder, fi.Name);

                            if (File.Exists(targetFile))
                            {
                                File.Delete(targetFile);
                            }

                            File.Copy(fi.FullName, targetFile);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("CopyProgramTypeFiles", exc);
            }
        }
    }
}