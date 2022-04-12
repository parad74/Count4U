using System.Collections.Generic;
using Count4U.Model;

namespace Count4U.GenerationReport.Settings
{
    public class SettingsRepository : ISettingsRepository
    {
        public string CurrentLanguage { get; set; }
        public string LogPath { get; set; }
		public string ProcessCode { get; set; }
		public Dictionary<string, string> StartupArgumentDictionary { get; set; }
        public IturAnalyzesRepositoryEnum ReportRepositoryGet { get; set; } 
    }
}