using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Count4U.Model;
using System.Xml.Linq;
using Microsoft.Practices.ServiceLocation;
using ErikEJ.SqlCe;
using System.ComponentModel;

namespace Count4U.Model
{
	public abstract class BaseProvider
	{
		private readonly ILog _log;
		public readonly IServiceLocator _serviceLocator;
		protected List<ImportDomainEnum> _importTypes;
		protected string _pathDB;
		protected string _pathFile;
		protected string _locationCode;
		protected string _iturCode;
		protected MaskPackage _parserMaskPackage;
		protected Dictionary<ImportProviderParmEnum, object> _parms;
		//private List<IImportProvider> _importProviderList;
		private Encoding _encoding;
		private bool _fastImport;

		private List<string[]> _columnMappings ;

		protected string[] _separators;
		protected int _countExcludeFirstString;

		public BaseProvider(ILog log,
			IServiceLocator serviceLocator)
		{
			if (log == null) throw new ArgumentNullException("log");
			this._log = log;
			this._serviceLocator = serviceLocator;
			this._parms = new Dictionary<ImportProviderParmEnum, object>();
			this._importTypes = new List<ImportDomainEnum>();
			//this._importProviderList = new List<IImportProvider>();
			this._parserMaskPackage = null;
			this.InitDefault();
			//this._columnMappings = new List<string[]>();
		}


		//protected List<IImportProvider> ImportProviderList
		//{
		//    get { return this._importProviderList; }
		//    set { this._importProviderList = value; }
		//}

		public ILog Log
		{
			get { return this._log; }
		}

		public Encoding ProviderEncoding
		{
			get
			{
				if (this._encoding != null)
				{
					return this._encoding;
				}
				else
				{
					return Encoding.GetEncoding("windows-1255");
				}
			}
			set
			{
				if (value != null)
				{
					this._encoding = value;
				}
				else
				{
					this._encoding = Encoding.GetEncoding("windows-1255");
				}
			}
		}


		public List<string[]> ColumnMappings
		{
			get
			{
		
					return this._columnMappings;
			}
			set
			{
				if (value != null)
				{
					this._columnMappings = value;
				}
			}
		}

		public void SetColumnMappings<T>()
		{
			var props = TypeDescriptor.GetProperties(typeof(T))
				.Cast<PropertyDescriptor>()
				.ToArray();
			this._columnMappings = new List<string[]>();
			///ColumnMappings.Clear();
			foreach (var propertyInfo in props)
			{
				if (propertyInfo.Name != "ID")
				{
					this._columnMappings.Add(new string[] { propertyInfo.Name, propertyInfo.Name });
				}
			}
		}

		//public List<string> GetLog()
		//{
		//    return this.Log.Message();
		//}

		//public string PrintLog()
		//{
		//    string log = "";
		//    foreach (var message in this.Log.Message())
		//    {
		//        log = log + message + Environment.NewLine;
		//    }
		//    return log;
		//}

		//public void ClearLog()
		//{
		//    this.Log.Clear();
		//}

		public void FillInfoLog(string fromPathFile, string typeName, List<ImportDomainEnum> importTypes)
		{
			//Localization.Resources.Log_TraceProvider1101%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceProvider, String.Format(Localization.Resources.Log_TraceProvider1101, "Provider", typeName));
			//Localization.Resources.Log_TraceProvider1102%"From  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceProvider, String.Format(Localization.Resources.Log_TraceProvider1102, fromPathFile));
			foreach (ImportDomainEnum importType in importTypes)
			{
				//Localization.Resources.Log_TraceProvider1103%"Import Option - [{0}]"
				this.Log.Add(MessageTypeEnum.TraceProvider, String.Format(Localization.Resources.Log_TraceProvider1103, importType.ToString()));
			}
		}

		public void FillInfoLog(string typeName, List<ImportDomainEnum> importTypes)
		{
			//Localization.Resources.Log_TraceProvider1101%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceProvider, String.Format(Localization.Resources.Log_TraceProvider1101, "Provider", typeName));
			foreach (ImportDomainEnum importType in importTypes)
			{
				//Localization.Resources.Log_TraceProvider1103%"Import Option - [{0}]"
				this.Log.Add(MessageTypeEnum.TraceProvider, String.Format(Localization.Resources.Log_TraceProvider1103, importType.ToString()));
			}
		}

	

		public bool IsEmptyPath(string pathFile)
		{
			if (string.IsNullOrWhiteSpace(pathFile) == true)
			{
				//Localization.Resources.Log_Error1005% "File Path  Is Empty"
				this.Log.Add(MessageTypeEnum.Error, Localization.Resources.Log_Error1005);
				return true;
			}
			//if (System.IO.File.Exists(pathFile) == false)
			//{
			//    this.Log.Add("Import File Path [ " + pathFile + " ] Is Not Exist");
			//    return true;
			//}
			return false;
		}

		protected abstract void InitDefault();
		//public abstract void Export();
		//protected abstract void InitFromUserParms();
		//protected abstract void InitConfig(XDocument parms = null);
		
		public Dictionary<ImportProviderParmEnum, object> Parms
		{
			get { return this._parms; }
			set { this._parms = value; }
		}

		public bool FastImport
		{
			get { return _fastImport; }
			set { _fastImport = value; }
		}

		public string ToPathDB
		{
			get { return this._pathDB; }
			set { this._pathDB = value; }
		}

		public string FromPathFile
		{
			get { return this._pathFile; }
			set { this._pathFile = value; }
		}

		//public string LocationCode
		//{
		//    get { return this._locationCode; }
		//    set { this._locationCode = value; }
		//}
		//public string IturCode
		//{
		//    get { return this._iturCode; }
		//    set { this._iturCode = value; }
		//}

		public string FromPathDB
		{
			get { return this._pathDB; }
			set { this._pathDB = value; }
		}

		public string ToPathFile
		{
			get { return this._pathFile; }
			set { this._pathFile = value; }
		}

		public XDocument GetXDocumentConfig()
		{
			throw new NotImplementedException();
		}

		public MaskPackage ParserMaskPackage
		{
			get { return this._parserMaskPackage; }
			set { this._parserMaskPackage = value; }
		}

	}

	public enum DomainObjectTypeEnum
	{
		Unknown,
		Employee,
		BuildingConfig,
		PropertyDecorator,
		PropertyExportErpDecorator1,
		PropertyExportErpDecorator2	,
		PropertyExportErpDecorator3,
		PropertyExportErpDecorator4,
		PropertyStr1,
		PropertyStr2,
		PropertyStr3,
		PropertyStr4,
		PropertyStr5,
		PropertyStr6,
		PropertyStr7,
		PropertyStr8,
		PropertyStr9,
		PropertyStr10,
		PropertyStr11,
		PropertyStr12,
		PropertyStr13,
		PropertyStr14,
		PropertyStr15,
		PropertyStr16,
		PropertyStr17,
		PropertyStr18,
		PropertyStr19,
		PropertyStr20,
		PropertyStr1_20,
		Profile

	}
}
