using System;

namespace Count4U.Common.Services.Navigation.Data
{
    [Serializable]
    public class ExportLogViewData
    {
        public string Log { get; set; }
        public string Path { get; set; }
		public string InDataPath { get; set; }
		public string FtpPath { get; set; }
		public string AbsolutPath { get; set; }
		public string AdapterType { get; set; } //Import,ExportErp,ExportPDA
		public string AdapterName { get; set; } 
		public string ConfigXML { get; set; }
		public string DataInConfigPath { get; set; } 
		
    }
}