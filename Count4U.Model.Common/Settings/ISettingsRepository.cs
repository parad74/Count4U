using System.Collections.Generic;
using Count4U.Model;

namespace Count4U.GenerationReport.Settings
{
    public interface ISettingsRepository
    {
        string CurrentLanguage { get; set; }
        string LogPath { get; set; }
		string ProcessCode { get; set; }
		Dictionary<string, string> StartupArgumentDictionary { get; set; }
        IturAnalyzesRepositoryEnum ReportRepositoryGet { get; set; } 
    }
}