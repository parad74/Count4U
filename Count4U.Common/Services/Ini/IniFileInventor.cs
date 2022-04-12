using System;
using System.IO;
using System.Text;
using Count4U.Model;
using Count4U.Model.Interface;
using IniParser;
using NLog;

namespace Count4U.Common.Services.Ini
{
    public class IniFileInventor : IIniFileInventor
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;

        private const string SectionNameGlobal = "Global";
        private const string SectionNameCustomer = "Customer";
        private const string SectionNameBranch = "Branch";

        public IniFileInventor(IDBSettings dbSettings)
        {
            _dbSettings = dbSettings;
        }

        public void Save(IniFileInventorData data)
        {
            try
            {
                string userFolder = BuildParamsFolderPath();

                System.Diagnostics.Debug.Assert(!String.IsNullOrWhiteSpace(userFolder));

                string resultFilePath = Path.Combine(userFolder, "params.ini");
                string resultFilePathAnsi = Path.Combine(userFolder, "params_ansi.ini");

                if (File.Exists(resultFilePath))
                    File.Delete(resultFilePath);

                if (File.Exists(resultFilePathAnsi))
                    File.Delete(resultFilePathAnsi);

                FileIniDataParser parser = new FileIniDataParser();
                parser.KeyValueDelimiter = '=';

                IniData iniData = new IniData();

                iniData.Sections.AddSection(SectionNameCustomer);
                iniData[SectionNameCustomer].AddKey("CustomerCode", data.CustomerCode);
                iniData[SectionNameCustomer].AddKey("CustomerName", data.CustomerName);

                iniData.Sections.AddSection(SectionNameBranch);
                iniData[SectionNameBranch].AddKey("BranchCode", data.BranchCode);
                iniData[SectionNameBranch].AddKey("BranchName", data.BranchName);
                iniData[SectionNameBranch].AddKey("BranchCodeLocal", data.BranchCodeLocal);
                iniData[SectionNameBranch].AddKey("BranchCodeERP", data.BranchCodeERP);

                iniData.Sections.AddSection(SectionNameGlobal);

                iniData[SectionNameGlobal].AddKey("InventorCode", data.InventorCode);
                iniData[SectionNameGlobal].AddKey("SDFPath", data.SDFPath);
                iniData[SectionNameGlobal].AddKey("InDataFolderPath", data.InDataFolderPath);
                iniData[SectionNameGlobal].AddKey("ExportToPDAPath", data.ExportToPDAPath);
                iniData[SectionNameGlobal].AddKey("ExportToERPPath", data.ExportToERPPath);
                iniData[SectionNameGlobal].AddKey("ProgramType", data.ProgramType);

                parser.SaveFile(resultFilePath, iniData, Encoding.Unicode);
                parser.SaveFile(resultFilePathAnsi, iniData, Encoding.Default);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Save", exc);
            }
        }

        public string BuildParamsFolderPath()
        {
            try
            {
                string pdaFolder = _dbSettings.ExportToPdaFolderPath();

                if (Directory.Exists(pdaFolder))
                {
                    string finalPath = Path.Combine(pdaFolder, @"ParamsPDA");
                    Directory.CreateDirectory(finalPath);

                    return finalPath;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildParamsFolderPath", exc);
            }

            return String.Empty;
        }
    }
}