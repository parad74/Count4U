using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.Interfaces;
using IniParser;
using NLog;

namespace Count4U.Common.Services.Ini
{
    public class IniFileParser : IIniFileParser
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const string SectionName = "Global";

        public IniFileData Get(string filePath, string sectionName)
        {
            if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            try
            {
                FileIniDataParser parser = new FileIniDataParser();
                var data = parser.LoadFile(filePath);
                
                IniFileData result = new IniFileData();
                string section = String.IsNullOrWhiteSpace(sectionName)? SectionName : sectionName;
                result.SectionName = section;

                if (data.Sections.ContainsSection(section))
                {
                    foreach (var kvp in data.Sections[section])
                    {
                        result.Data.Add(kvp.KeyName, kvp.Value);
                    }

                    return result;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Get", exc);
            }

            return null;
        }

        public List<IniFileData> Get(string filePath)
        {
            if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

             List<IniFileData> result = new List<IniFileData>();

            try
            {
                FileIniDataParser parser = new FileIniDataParser();
                IniData data = parser.LoadFile(filePath);

                foreach (SectionData sectionData in data.Sections)
                {
                    IniFileData iniSection = new IniFileData();
                    iniSection.SectionName = sectionData.SectionName;
                    iniSection.Data = new Dictionary<string, string>();
                    foreach (KeyData keyData in sectionData.Keys)
                    {
                        iniSection.Data[keyData.KeyName] = keyData.Value.Trim();
                    }

                    result.Add(iniSection);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Get", exc);
            }

            return result;
        }
    }
}