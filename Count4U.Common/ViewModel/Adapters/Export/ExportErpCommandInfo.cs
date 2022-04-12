using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Extensions;

namespace Count4U.Common.ViewModel.Adapters.Export
{
    public class ExportErpCommandInfo
    {
        public ExportErpCommandInfo()
        {
            IturCodeList = new List<string>();
            LocationCodeList = new List<string>();
			AdapterName = "";
			ConfigXDocument = null;
			ConfigXDocumentPath = "";
			FromConfigXDoc = ConfigXDocFromEnum.InitWithoutConfig;
        }

		public ExportErpCommandInfo(ExportErpCommandInfo info)
		{
			IturCodeList = new List<string>();
			LocationCodeList = new List<string>();
			if (info.IturCodeList != null) IturCodeList = info.IturCodeList;
			if (info.LocationCodeList != null) LocationCodeList = info.LocationCodeList;
			AdapterName = info.AdapterName;
			ConfigXDocument = info.ConfigXDocument;
			ConfigXDocumentPath = info.ConfigXDocumentPath;
			FromConfigXDoc = info.FromConfigXDoc;
		}

		public XDocument ConfigXDocument { get; set; }
		[NotInludeAttribute]
		public string ConfigXDocumentPath { get; set; }
		public ConfigXDocFromEnum FromConfigXDoc { get; set; }
		[NotInludeAttribute]
		public string AdapterName { get; set; }

		[NotInludeAttribute]
        public Action Callback { get; set; }
		[NotInludeAttribute]
        public bool IsSaveFileLog { get; set; }
		[NotInludeAttribute]
        public CancellationToken CancellationToken { get; set; }

        public bool IsFull { get; set; }
		public bool IsExcludeNotExistingInCatalog { get; set; }
        public bool IsFilterByIturs { get; set; }
		[NotInludeAttribute]
        public bool IsFilterByLocations { get; set; }
		[NotInludeAttribute]
        public List<string> IturCodeList { get; set; }
		[NotInludeAttribute]
        public List<string> LocationCodeList { get; set; }
    }
}