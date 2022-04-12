using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Count4U.Model.Interface;
using Ionic.Zip;
using Ionic.Zlib;
using NLog;

namespace Count4U.Model.Transfer
{
    public class Zip : IZip
    {
        const string App_DataName = "App_Data";
        const string ImportDataName = "ImportData";
        const string ImportModulesName = "ImportModules";
        const string ExportAdaptersName = "ExportModules";
        const string ExportDataName = "ExportData";

        private static readonly char[] Separator = new char[] { '\\' };
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;
        private readonly Dictionary<ZipRootFolder, ZipRootFolderInfo> _folderInfo;

        public Zip(IDBSettings dbSettings)
        {
            this._dbSettings = dbSettings;

            this._folderInfo = new Dictionary<ZipRootFolder, ZipRootFolderInfo>();

            this._folderInfo.Add(ZipRootFolder.Db, new ZipRootFolderInfo
            {
                PathOnDisk = this._dbSettings.BuildCount4UDBFolderPath(),
                NameInArchive = AppData,
            });

            this._folderInfo.Add(ZipRootFolder.Import, new ZipRootFolderInfo()
            {
                PathOnDisk = this._dbSettings.ImportFolderPath(),
                NameInArchive = ImportData,
            });

            this._folderInfo.Add(ZipRootFolder.Adapters, new ZipRootFolderInfo
             {
                 PathOnDisk = FileSystem.ImportModulesFolderPath(),
                 NameInArchive = ImportModules,
             });

            this._folderInfo.Add(ZipRootFolder.Reports, new ZipRootFolderInfo
            {
                PathOnDisk = this._dbSettings.ReportTemplatePath(),
                NameInArchive = new DirectoryInfo(this._dbSettings.ReportTemplatePath()).Name,
            });

            this._folderInfo.Add(ZipRootFolder.ExportAdapters, new ZipRootFolderInfo
            {
                PathOnDisk = FileSystem.ExportModulesFolderPath(),
                NameInArchive = ExportAdapters,
            });

            this._folderInfo.Add(ZipRootFolder.ExportData, new ZipRootFolderInfo
            {
                PathOnDisk = this._dbSettings.ExportErpFolderPath(),
                NameInArchive = ExportData,
            });
        }

        public string AppData { get { return App_DataName; } }
        public string ImportData { get { return ImportDataName; } }
        public string ImportModules { get { return ImportModulesName; } }
        public string ReportTemplate
        {
            get
            {
                return new DirectoryInfo(this._dbSettings.ReportTemplatePath()).Name;
            }
        }
        public string ExportAdapters { get { return ExportAdaptersName; } }
        public string ExportData { get { return ExportDataName; } }

        public List<string> ValidExtensionsForDbFolder
        {
            get { return new List<string>() { ".sdf", ".png", ".jpg", ".jpeg", ".gif" }; }
        }

        public ZipRootFolderInfo GetRootFolder(ZipRootFolder rootFolder)
        {
            if (this._folderInfo.ContainsKey(rootFolder))
                return this._folderInfo[rootFolder];

            return null;
        }

        public string BuildFileName(String prefix = null)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            string outFileName = String.Format("{0}{1}.zip", prefix, date);
            return outFileName;
        }

        public string Pack(IList<ZipPackInfo> packages, CancellationToken cancellationToken, Action<double, string, string> progress, String outPackageFolder = null)
        {
            //check for correct input
            if (packages.All(r => r.EmptyFolders.Count == 0 && r.Files.Count == 0))
            {
                _logger.Error("Zip pack error - input is empty");
                return String.Empty;
            }

            string outFolder;
            if (String.IsNullOrEmpty(outPackageFolder))
                outFolder = FileSystem.UserCount4UFolder().Trim(Separator);
            else
                outFolder = outPackageFolder;

            string outFileName = BuildFileName();
            string result = Path.Combine(outFolder, outFileName);
            _logger.Info("Zip pack out file: {0}", result);
            try
            {
                using (ZipFile bundle = new ZipFile())
                {
					bundle.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    bundle.BufferSize = 1048576;
                    bundle.CodecBufferSize = 1048576;
                    bundle.ParallelDeflateThreshold = -1; 
#if DEBUG
                    bundle.CompressionLevel = CompressionLevel.None;
#else
                                  bundle.CompressionLevel = CompressionLevel.Level2;
#endif

                    foreach (ZipPackInfo package in packages) //enumerate db/import packages
                    {
                        if (package.EmptyFolders.Count == 0 && package.Files.Count == 0) //skip if empty
                            continue;

                        CreateZip(package, bundle, cancellationToken);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return String.Empty;
                        }

                    }

                    ZipEntry curEntry = null;
                    int total = bundle.Entries.Count;
                    int processed = 0;
                    bundle.SaveProgress += (s, e) =>
                                           {
                                               if (e.CurrentEntry == null)
                                                   return;

                                               if (curEntry != e.CurrentEntry)
                                               {
                                                   processed++;
                                                   curEntry = e.CurrentEntry;
                                               }
                                               double v = e.BytesTransferred * 100 / (double)e.TotalBytesToTransfer;

                                               progress(v, String.Format("{0}/{1}", processed, total), curEntry.FileName);

                                               if (cancellationToken.IsCancellationRequested)
                                                   e.Cancel = true;
                                           };
                    bundle.Save(result);
                    _logger.Info("Zip pack completed");
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Pack", exc);

                throw;
            }

            return cancellationToken.IsCancellationRequested ? String.Empty : result;
        }

        void CreateZip(ZipPackInfo zipInfo, ZipFile zipFile, CancellationToken cancellationToken)
        {
            try
            {
                DirectoryInfo baseFolderInfo = new DirectoryInfo(this._folderInfo[zipInfo.RootFolder].PathOnDisk).Parent;
                if (baseFolderInfo == null)
                {
                    _logger.Error("CreateZip baseFolderInfo == null");
                    return;
                }
                string baseFolder = baseFolderInfo.FullName.Trim(Separator);
                string subPath;

                foreach (DirectoryInfo di in zipInfo.EmptyFolders) //empty folders
                {
                    if (di == null) continue;

                    string dirPath = di.FullName.Trim(Separator);
                    subPath = dirPath.Substring(baseFolder.Length, dirPath.Length - baseFolder.Length);

                    if (!String.IsNullOrEmpty(subPath))
                        subPath = subPath.Trim(Separator);

                    zipFile.AddDirectoryByName(subPath);

                    if (cancellationToken.IsCancellationRequested)
                        return;
                }

                foreach (FileInfo fi in zipInfo.Files) //actual files
                {
                    if (fi == null || fi.Directory == null) continue;

                    string filePath = fi.Directory.FullName;
                    subPath = filePath.Substring(baseFolder.Length, filePath.Length - baseFolder.Length);

                    if (!String.IsNullOrEmpty(subPath))
                        subPath = subPath.Trim(Separator);

                    zipFile.AddFile(fi.FullName, subPath);

                    if (cancellationToken.IsCancellationRequested)
                        return;
                }

            }
            catch (Exception exc)
            {
                _logger.ErrorException("CreateZip", exc);
                throw;
            }
        }

        public bool IsZipInCorrectFormat(string path)
        {
            using (ZipFile zip = new ZipFile(path))
            {
                var result = this._folderInfo.Any(r => zip.Any(z => z.FileName.StartsWith(r.Value.NameInArchive)));

                //                var result = this._folderInfo.Any(r => zip.Any(z =>
                //                    {
                //                        if (Directory.Exists(r.Value.PathOnDisk))
                //                        {
                //                            DirectoryInfo di = new DirectoryInfo(r.Value.PathOnDisk);
                //                            if (z.FileName.ToLower().StartsWith(di.Name.ToLower()))
                //                                return true;
                //                        }
                //                        return false;
                //                    }));
                return result;
            }
        }

        public void Unpack(ZipUnpackInfo info, CancellationToken cancellationToken, Action<double, string, string> progress)
        {
            try
            {
                if (info.IsClearImportData)
                {
                    ClearImportData();
                }
                int totalEntries = info.Items.SelectMany(r => r.Entries).Count();
                int processed = 0;
                ZipEntry entry = null;
                using (ZipFile zip = new ZipFile(info.ZipFilePath))
                {
					zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    zip.ExtractProgress += (s, e) =>
                                           {
                                               if (e.CurrentEntry == null)
                                                   return;

                                               if (entry != e.CurrentEntry)
                                               {
                                                   processed++;
                                                   entry = e.CurrentEntry;
                                               }
                                               double v = e.BytesTransferred * 100 / (double)e.TotalBytesToTransfer;

                                               string fileName = entry.FileName;
                                               progress(v, String.Format("{0}/{1}", processed, totalEntries), fileName);

                                               if (cancellationToken.IsCancellationRequested)
                                                   e.Cancel = true;
                                           };

                    foreach (ZipEntry zipEntry in zip)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        ExtractExistingFileAction isOverwrite = info.IsOverwrite ? ExtractExistingFileAction.OverwriteSilently : ExtractExistingFileAction.DoNotOverwrite;

                        ZipUnpackInfoItem item = info.Items.FirstOrDefault(r => r.Entries.Any(z => z.FileName == zipEntry.FileName));
                        if (item != null)
                        {
                            string path = this._folderInfo[item.RootFolder].PathOnDisk;
                            DirectoryInfo di = new DirectoryInfo(path).Parent;
                            if (di == null) continue;

                            //check for .tmp file
                            if (!zipEntry.IsDirectory)
                            {
                                string tmpFileName = String.Format("{0}.tmp", zipEntry.FileName);
                                string tmpFilePath = Path.Combine(di.FullName, tmpFileName);
                                if (File.Exists(tmpFilePath))
                                {
                                    _logger.Info("Unpack - .tmp file found: {0}", tmpFilePath);
                                    File.Delete(tmpFilePath);
                                }
                            }

                            zipEntry.Extract(di.FullName, isOverwrite);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                _logger.ErrorException("Unpack", exc);
                throw;
            }
        }

        private void ClearImportData()
        {
            string importFolderPath = this._dbSettings.ImportFolderPath();
            if (Directory.Exists(importFolderPath))
            {
                DirectoryInfo di = new DirectoryInfo(importFolderPath);
                Directory.Delete(di.FullName, true);
            }
        }

        public void CreateZip(List<ZipRelativePath> files, string outZipPath)
        {
            if (files == null || String.IsNullOrWhiteSpace(outZipPath))
                return;

            if (File.Exists(outZipPath))
            {
                File.Delete(outZipPath);
            }

            using (ZipFile bundle = new ZipFile())
            {
				bundle.UseZip64WhenSaving = Zip64Option.AsNecessary;
                bundle.ParallelDeflateThreshold = -1;

                foreach (ZipRelativePath file in files)
                {                    
                    bundle.AddFile(file.Path, file.RelativePath);
                }

                bundle.Save(outZipPath);
            }
        }

		 /// <summary>
		 /// 
		 /// </summary>
		 /// <param name="filesPathList"> Полные пути до файлов с именем файла</param>
		 /// <param name="outZipPath">путь до zip - файл, с именем файла </param>
		 /// <param name="directory"></param>
		public void DoZipFile(List<string> filesPathList, string outZipPath/*, string directory*/)
		{
			if (filesPathList == null || String.IsNullOrWhiteSpace(outZipPath))
				return;

			using (ZipFile zip = new ZipFile()) // Создаем объект для работы с архивом
			{
				//zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression; // Задаем максимальную степень сжатия 
				zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
				zip.ParallelDeflateThreshold = -1;
				//zip.AddDirectory(directory); // Кладем в архив папку вместе с содежимым
				foreach (string filesPath in filesPathList)
				{
					zip.AddFile(filesPath, "");//zip.AddFile(@"c:\Temp\Import.csv"); // Кладем в архив одиночный файл
				}			
				zip.Save(outZipPath);//zip.Save(@"C:\Temp\PackedProject.zip"); // Создаем архив     
			}
		}

		public void ReadDb3FromZipFile(string zipFilePath, string outputDirectory)
		{
			if (File.Exists(zipFilePath) == true)
			{
				if (System.IO.Path.GetExtension(zipFilePath) == ".zip")
				{
					try
					{
						ZipFile zip = ZipFile.Read(zipFilePath);
						zip.ExtractSelectedEntries("name = *.db3", "", outputDirectory, ExtractExistingFileAction.OverwriteSilently);
					}
					catch (Exception exc)
					{
						_logger.ErrorException("ReadDb3FromZipFile : " + zipFilePath, exc);
					}
				}
			}
		}
		
    }
}