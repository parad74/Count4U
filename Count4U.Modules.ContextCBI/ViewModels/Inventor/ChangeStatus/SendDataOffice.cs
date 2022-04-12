using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Transfer;
using Microsoft.ReportingServices.Interfaces;
using NLog;
using Count4U.Model.SelectionParams;
using System.Threading.Tasks;
using Count4U.Common.Extensions;
using Count4U.Model.Interface.Count4U;
using System.Threading;

namespace Count4U.Modules.ContextCBI.ViewModels
{
	public class SendDataOffice
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;
        private readonly IZip _zip;
        private readonly IReportSaveProvider _reportSaveProvider;
        private readonly IReportRepository _reportRepository;
		private readonly IIturAnalyzesRepository _iturAnalyzesRepository;
		private readonly IReportPrintProvider _reportPrintProvider;
        private readonly ModalWindowLauncher _windowLauncher;
        private readonly IContextCBIRepository _contextCbiRepository;

		private bool _isBusy = false;

        public SendDataOffice(
            IDBSettings dbSettings,
            IZip zip,
            IReportSaveProvider reportSaveProvider,
            IReportPrintProvider reportPrintProvider,
            IReportRepository reportRepository,
            ModalWindowLauncher windowLauncher,
            IContextCBIRepository contextCbiRepository,
			IIturAnalyzesRepository iturAnalyzesRepository)
        {
            _contextCbiRepository = contextCbiRepository;
            _windowLauncher = windowLauncher;
            _reportPrintProvider = reportPrintProvider;
            _reportRepository = reportRepository;
            _reportSaveProvider = reportSaveProvider;
            _zip = zip;
            _dbSettings = dbSettings;
			_iturAnalyzesRepository = iturAnalyzesRepository;
			this._isBusy = false;
        }

        public void BuildZip(
            CBIState cbiState,
            Action<string> updateStatus,
			List<ReportInfo> reportInfoList,
            bool includeInventorSdf,
            bool includePack,
            bool includeEndOfInventoryFiles,
            bool isRunExportErp,
            string resultPathZipPath)
        {
			if (this._isBusy == true) return;
			//cancellationTokenSource = new CancellationTokenSource();

			//if (cancellationTokenSource.IsCancellationRequested)
			//{
			//	return;
			//}
				
			this._isBusy = true;
            const string zipRoot = "/";
			reportInfoList = reportInfoList.Where(x => x != null).ToList();

			foreach (ReportInfo reportInfo in reportInfoList)
			{
				reportInfo.BuildReportArgs(cbiState); 
			}

			//if (cancellationTokenSource.IsCancellationRequested)
			//{
			//	return;
			//}

            try
            {
                List<ZipRelativePath> filesToZip = new List<ZipRelativePath>();
			
                //print reports
                this.BuildAndPrintReports(cbiState, updateStatus, reportInfoList);

				//if (cancellationTokenSource.IsCancellationRequested)
				//{
				//	return;
				//}

                //build reports files
				List<string> reportFiles = this.BuildAndSaveReports(cbiState, updateStatus, reportInfoList, "BuildZip");
				//if (cancellationTokenSource.IsCancellationRequested)
				//{
				//	return;
				//}


				if (reportFiles != null)
				{
					foreach (string reportPath in reportFiles)
					{
						filesToZip.Add(new ZipRelativePath() { Path = reportPath, RelativePath = zipRoot });
					}
				}

				//if (cancellationTokenSource.IsCancellationRequested)
				//{
				//	return;
				//}

                //include .sdf
                if (includeInventorSdf)
                {
                    updateStatus(Localization.Resources.View_SendDataOffice_BuildingInventorSdf);

                    string inventorSdfFullPath = BuildInventorSdfPath(cbiState);
                    if (File.Exists(inventorSdfFullPath))
                    {
                        filesToZip.Add(new ZipRelativePath() { Path = inventorSdfFullPath, RelativePath = zipRoot });
                    }
                }

				//if (cancellationTokenSource.IsCancellationRequested)
				//{
				//	return;
				//}

                //include pack
                if (includePack)
                {
                    updateStatus(Localization.Resources.View_SendDataOffice_BuildingPackZipFile);

                    string pack = BuildPackFile(cbiState);
                    filesToZip.Add(new ZipRelativePath() { Path = pack, RelativePath = zipRoot });
                }

				//if (cancellationTokenSource.IsCancellationRequested)
				//{
				//	return;
				//}

                if (includeEndOfInventoryFiles)
                {
                    updateStatus(Localization.Resources.View_SendDataOffice_BuildingEndInventor);

                    if (isRunExportErp)
                    {
						RunExport(cbiState);	    // TODO       
                    }

					//if (cancellationTokenSource.IsCancellationRequested)
					//{
					//	return;
					//}

                    List<string> exportFiles = BuildExportFilesPath(cbiState);
                    if (exportFiles != null)
                    {
                        foreach (string exportFile in exportFiles)
                        {
                            filesToZip.Add(new ZipRelativePath() { Path = exportFile, RelativePath = zipRoot });
                        }
                    }
                }

                //building zip
                updateStatus(Localization.Resources.View_SendDataOffice_BuildingZip);

				//if (cancellationTokenSource.IsCancellationRequested)
				//{
				//	return;
				//}


                if (File.Exists(resultPathZipPath))
                {
                    _logger.Info(String.Format("Removed previous zip: {0}", resultPathZipPath));
                    File.Delete(resultPathZipPath);
                }
				//if (cancellationTokenSource.IsCancellationRequested)
				//{
				//	return;
				//}

                _zip.CreateZip(filesToZip, resultPathZipPath);
            }

            catch (Exception exc)
            {
				this._isBusy = false;
                _logger.ErrorException("BuildZip", exc);
            }
			this._isBusy = false;
        }

		//build reports files
		//List<string> reportFiles = this.BuildAndSaveReports(cbiState, updateStatus, reportInfoList);

        public List<string> BuildAndSaveReports(CBIState cbiState, Action<string> updateStatus, 
			IEnumerable<ReportInfo> reportInfs, string fromContext = "")
        {
			_logger.Info("SendToOffice:: BuildAndSaveReports start :: fromContext " + fromContext);
            List<string> result = new List<string>();

            string tempPath = Path.GetTempPath();
			string tempDir = Path.Combine(tempPath, "Count4U");

			if (Directory.Exists(tempDir) == false) { try { Directory.CreateDirectory(tempDir); } catch (Exception ex) { _logger.ErrorException("BuildReportsPath : " + ex.Message, ex); return result; } }

			List<ReportInfo> reportInfoList = reportInfs.Where(x => x!=null && x.IncludeInZip == true && x.GenerateArgs != null).ToList();

			//foreach (ReportInfo reportInfo in reportInfoList)
			//{
			//	reportInfo.BuildReportArgs(cbiState); //дублируется заполнение аргументов
			//}

			Dictionary<string, string> domainContextDataSetDictionary = this._reportRepository.GetDomainContextDataSetDictionary();

			//Группируем отчеты по dataSet type
			foreach (KeyValuePair<string, string> keyValuePair in domainContextDataSetDictionary)
			{
				bool refillReportDS = true;
				string dataSetKey = keyValuePair.Key.ToLower().Trim();

				List<ReportInfo> reportDataSetInfoList = reportInfoList.Where(x => x.GenerateArgs.Report.Path.ToLower().Trim() == dataSetKey).ToList();

				Task<string>[] dataSetReportTasks = new Task<string>[reportDataSetInfoList.Count()];
				//int i = -1;
				if (reportDataSetInfoList.Count() > 0) _logger.Info("SendToOffice::Save DataSetReport : " + dataSetKey + "[" + reportDataSetInfoList.Count()  + "]  start");

				//foreach (ReportInfo info in reportDataSetInfoList)
				for (int i = 0; i < reportDataSetInfoList.Count(); i++)
				{
					//i++;
					//if (info.IncludeInZip == false) continue;
					//if (info.GenerateArgs == null) continue;
					//if (info.GenerateArgs.Report.Path.ToLower().Trim() == keyValuePair.Key.ToLower().Trim())
					//{
						ReportInfo info = reportDataSetInfoList[i];
						info.GenerateArgs.RefillReportDS = refillReportDS;
						info.GenerateArgs.SaveReportToSendOffice = true;
						if (fromContext == "TagSelect")
						{
							info.GenerateArgs.Param1 = info.param1 as string;
						}
						//if (fromContext == "LocationCodeSelect")
						//{
						//	info.GenerateArgs.Param1 = info.param1 as string;
						//}
						//if (refillReportDS == true) // первый отчет с одинаковым DS
						//{

						//if (i == 0 || info.ReportCode == "[Rep-IS1-62n]" || info.ReportCode == "[Rep-IS1-82p]" || info.ReportCode == "[Rep-IS1-53p]")
						//{
						//	if (info.ReportCode == "[Rep-IS1-62n]" || info.ReportCode == "[Rep-IS1-82p]" || info.ReportCode == "[Rep-IS1-53p]")
						//	{
						//		info.GenerateArgs.RefillReportDS = true;
						//	}

						 // Сделано для того чтобы заполнять источник каждый раз, каждый отчет
						if (i == 0 || info.RefillAlways == true || cbiState.CurrentCustomer.Code == "901")
						{
							
								if (info.RefillAlways == true || cbiState.CurrentCustomer.Code == "901")
								{
									info.GenerateArgs.RefillReportDS = true;
								}
							
							dataSetReportTasks[i] = new Task<string>(() => SaveReportAction(updateStatus, tempDir, info, info.GenerateArgs));
							//Save Report
							dataSetReportTasks[i].RunSynchronously();
							_logger.Info("SendToOffice::dataSetReportTasks[" + i + "].RunSynchronously() : " + info.ReportCode);
							//string generatedReportPath = SaveReportAction(updateStatus, tempDir, info, info.GenerateArgs);
							AddToResultListReportPath(result, dataSetReportTasks[i].Result);
							dataSetReportTasks[i].Wait();  //wait first report
							_logger.Info("SendToOffice::Save DataSetReport Wait [" + i + "] : " + dataSetKey);
							refillReportDS = false;
							//if (dataSetReportTasks[i].Exception != null)
							//{
							//	foreach (var exp in dataSetReportTasks[i].Exception.Flatten().InnerExceptions)
							//	{
							//		_logger.Warn("BuildReportsPath: " + info.GenerateArgs.Report.Path, exp.Message);
							//	}
							//}
						}
						else // второй и т.д. отчет с одинаковым DS
						{
							// асинхронный запуск
							dataSetReportTasks[i] = new Task<string>(() => SaveReportAction(updateStatus, tempDir, info, info.GenerateArgs));
							//Save Report
							dataSetReportTasks[i].Start();
							_logger.Info("SendToOffice::dataSetReportTasks[" + i + "].Start() : " + info.ReportCode);
							// можно заменить здесь на синхронный запуск
							//dataSetReportTasks[i].Wait();  
						}

						//int countReportParallel = reportInfo.Count() - 1;
						//Task<string>[] tasks = new Task<string>[countReportParallel];
						//foreach (ReportInfo report in reportInfo)
						//{
						//	string generatedReportPath1 = SaveReportAction(updateStatus, tempDir, report, report.GenerateArgs);
						//	tasks[i] = Task.Factory.StartNew(() => SimpleCalculator(10001 + i * 2, N, 8));
						//	AddToResultListReportPath(result, generatedReportPath1);
						//}
						//Task.WaitAll(tasks);
					//}
				}//reportInfo

				//Task<string>[] tasksRun = dataSetReportTasks.Where(t => t != null).ToArray();
				if (dataSetReportTasks.Count() > 1)
				{
					Task.WaitAll(dataSetReportTasks);
					_logger.Info("SendToOffice::Save DataSetReport WaitAll : " + dataSetKey + "["+ dataSetReportTasks.Count().ToString() + "]");
				}

				for (int k = 0; k < dataSetReportTasks.Count(); k++)
				{
					if (k != 0) AddToResultListReportPath(result, dataSetReportTasks[k].Result);
					if (dataSetReportTasks[k].Exception != null)
					{
						foreach (var exp in dataSetReportTasks[k].Exception.Flatten().InnerExceptions)
						{
							_logger.Warn("BuildReportsPath: ", exp.Message);
						}
					}
				}
			}//domainContextDataSetDictionary
			
			//==================
			//Обрабтать и собрать список отчетов 1. Одного контекста
			// Из одного контекста пускать в цикле 
			// До цикла заполнять DS
			// В цикле не перезаполнять
			//string domainContextDataSet = this._reportRepository.GetDomainContextDataSetDictionary()[args.Report.Path];
			//==================


			//foreach (ReportInfo info in reportInfo)
			//{
			//	if (!info.IncludeInZip) continue;

			//	string reportString = String.Format(Localization.Resources.View_SendDataOffice_GeneratingReport,
			//		info.ReportCode.Trim(new[] { '[', ']' }),
			//		info.Format.Trim(new[] { '[', ']' }));

			//	updateStatus(reportString);

			//	GenerateReportArgs generateArgs = BuildReportArgs(cbiState, info);
			//	//generateArgs.RefillReportDS = true;
			//	//generateArgs.SaveReportToSendOffice = true;

			//	if (generateArgs == null) continue;


			//	string resultReportPath = Path.Combine(tempDir, generateArgs.Report.FileName);

			//	if (File.Exists(resultReportPath) == true)
			//	{
			//		File.Delete(resultReportPath);
			//	}


			//	string generatedReportPath = _reportSaveProvider.Save(generateArgs, resultReportPath, info.Format);

			//	if (result.Contains(generatedReportPath))
			//	{
			//		_logger.Warn("Result already contains: {0}", generatedReportPath);
			//	}
			//	else
			//	{
			//		result.Add(generatedReportPath);
			//	}

			//}

            return result;
        }

		private static void AddToResultListReportPath(List<string> result, string generatedReportPath)
		{
			if (string.IsNullOrWhiteSpace(generatedReportPath) == true) return;
			if (result.Contains(generatedReportPath))
			{
				_logger.Warn("Result already contains: {0}", generatedReportPath);
			}
			else
			{
				result.Add(generatedReportPath);
			}
		}

		private string SaveReportAction(Action<string> updateStatus, string tempDir, ReportInfo info, GenerateReportArgs generateArgs)
		{
			string reportString = String.Format(Localization.Resources.View_SendDataOffice_GeneratingReport,
							  info.ReportCode.Trim(new[] { '[', ']' }),
							  info.Format.Trim(new[] { '[', ']' }));

			updateStatus("Start : " + reportString);

			string resultReportPath = Path.Combine(tempDir, generateArgs.Report.FileName);

			if (File.Exists(resultReportPath) == true)
			{
				File.Delete(resultReportPath);
			}

			string generatedReportPath = _reportSaveProvider.Save(generateArgs, resultReportPath, info.Format, info);
			updateStatus("Finish : " + reportString);
			return generatedReportPath;
		}

		public void BuildAndPrintReports(CBIState cbiState, Action<string> updateStatus, IEnumerable<ReportInfo> reportInfs)
        {
            List<string> reportCodePrinted = new List<string>();

			List<ReportInfo> reportInfoList = reportInfs.Where(x => x != null && x.Print == true && x.GenerateArgs != null).ToList();

			foreach (ReportInfo reportInfo in reportInfoList)
            {
                //if (reportInfo.Print == false) continue;
				if (string.IsNullOrWhiteSpace(reportInfo.ReportCode) == true) continue;
                if (reportCodePrinted.Contains(reportInfo.ReportCode) == true) continue;

                updateStatus(String.Format(Localization.Resources.View_SendDataOffice_PrintingReport, reportInfo.ReportCode));

				//reportInfo.BuildReportArgs(cbiState);  //дублируется заполнение аргументов

				if ( reportInfo.GenerateArgs == null) continue;

				this._reportPrintProvider.Print(reportInfo.GenerateArgs);

                reportCodePrinted.Add(reportInfo.ReportCode);
            }
        }

        

        public string BuildInventorSdfPath(CBIState cbiState)
        {
            string inventorRelative = cbiState.ContextCBIRepository.GetDBPath(cbiState.CurrentInventor);
            string inventorSdfFullPath = this._dbSettings.BuildCount4UDBFilePath(inventorRelative);

            FileInfo fi = new FileInfo(inventorSdfFullPath);
            inventorSdfFullPath = fi.FullName;

            return inventorSdfFullPath;
        }

        public List<string> BuildExportFilesPath(CBIState cbiState)
        {
            List<string> result = new List<string>();
			Dictionary<string, string> parmsDictionary = new Dictionary<string,string>();
			if (cbiState.CurrentCustomer != null)
			{
				this.AddParamsInDictionary(parmsDictionary, cbiState.CurrentCustomer.ImportCatalogAdapterParms);
			}

			if (cbiState.CurrentBranch != null)
			{
				this.AddParamsInDictionary(parmsDictionary, cbiState.CurrentBranch.ImportCatalogAdapterParms);
			}


			string exportErpFolder = UtilsPath.ExportErpFolder(_dbSettings, "Inventor", cbiState.CurrentInventor.Code);

			if (parmsDictionary.ContainsKey("ExportPath") == true)
			{
				string exportPath = parmsDictionary["ExportPath"];
				if (string.IsNullOrWhiteSpace(exportPath) == false)
				{
					exportErpFolder = exportPath.Trim('\\');
				}
			}

            if (!Directory.Exists(exportErpFolder))
                return null;

            foreach (string file in Directory.GetFiles(exportErpFolder))
            {
                FileInfo fi = new FileInfo(file);
				string fileName = fi.Name.ToLower();
              //  if (fi.Extension.ToLower() != ".log.txt")
				if (fileName.Contains(".log.txt") == false)
                {
                    result.Add(file);
                }
            }

            return result;
        }


		private void AddParamsInDictionary(Dictionary<string, string> parmsDictionary, string parameters)
		{
			if (string.IsNullOrWhiteSpace(parameters) == true) return;
			string[] parmArray = parameters.Split(';');   //ImportPath=C\:temp;ExportPath=C\:temp;
			foreach (var parm in parmArray)
			{
				string[] keyVal = parm.Split('=');
				if (keyVal.Length > 1)
				{
					parmsDictionary[keyVal[0]] = keyVal[1];
				}
			}
		}

	    public string BuildPackFile(CBIState cbiState)
        {
            string result = String.Empty;

            try
            {
                string relativeInventorPath = _contextCbiRepository.BuildRelativeDbPath(cbiState.CurrentInventor);
                relativeInventorPath = Path.Combine(_dbSettings.FolderApp_Data, relativeInventorPath);


                string fullInventorPath = BuildInventorSdfPath(cbiState);

                ZipRelativePath inventorToPack = new ZipRelativePath()
                {
                    Path = fullInventorPath,
                    RelativePath = relativeInventorPath
                };

                var archive = new List<ZipRelativePath>() { inventorToPack };

                List<ZipRelativePath> importFiles = BuildImportFilesPath(cbiState);
                foreach (ZipRelativePath zipRelativePath in importFiles)
                {
                    archive.Add(zipRelativePath);
                }

                string tempFileName = Path.Combine(Path.GetTempPath(), _zip.BuildFileName("Pack_"));

                _zip.CreateZip(archive, tempFileName);

                result = tempFileName;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildPackFile", exc);
            }

            return result;
        }

        private List<ZipRelativePath> BuildImportFilesPath(CBIState cbiState)
        {
            List<ZipRelativePath> result = new List<ZipRelativePath>();

            try
            {
                string importFolderPath = _dbSettings.ImportFolderPath();

                string finalPath = Path.Combine(importFolderPath, "Inventor", cbiState.CurrentInventor.Code);

                foreach (string file in Directory.EnumerateFiles(finalPath, "*.*", SearchOption.AllDirectories))
                {
                    FileInfo fi = new FileInfo(file);
                    string withoutFileName = Path.GetDirectoryName(fi.FullName);
                    string relative = withoutFileName.Substring(finalPath.Length).Trim(new[] { '/', '\\' });
                    relative = Path.Combine("ImportData", relative);
                    result.Add(new ZipRelativePath() { Path = file, RelativePath = relative });
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildImportFilesPath", exc);
            }

            return result;
        }

        public void RunExport(CBIState cbiState)
        {
            Utils.RunOnUI(() =>
            {
                Dictionary<string, string> query = new Dictionary<string, string>();
                Utils.AddContextToDictionary(query, cbiState.Context);
                Utils.AddDbContextToDictionary(query, cbiState.DbContext);
                query.Add(Common.NavigationSettings.OpenAsModalWindow, String.Empty);
                query.Add(Common.NavigationSettings.AutoStartExportErp, String.Empty);
                query.Add(Common.NavigationSettings.AutoCloseExportErpWindow, String.Empty);


                _windowLauncher.StartModalWindow(
                    Common.ViewNames.ExportErpWithModulesView,
                    String.Empty,
                    settings: query,
                    hiddenWindow: true);
            });

        }
    }
}