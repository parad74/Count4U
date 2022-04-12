using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Common;

namespace Count4U.Model.Count4Mobile
{
	public class TemplateInventoryNativSdf2SqliteParser : ITemplateInventorySQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, TemplateInventory> _templateInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		public IImportTemplateInventoryRepository _importTemplateInventoryRepository;
		public ITemplateInventoryRepository _templateInventoryRepository;

		public TemplateInventoryNativSdf2SqliteParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._templateInventoryDictionary = new Dictionary<string, TemplateInventory>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, TemplateInventory> TemplateInventoryDictionary
		{
			get { return this._templateInventoryDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TemplateInventory> GetTemplateInventorys(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> templateInventoryFromDBDictionary,
			//Dictionary<string, string> catalogFromDBDictionary,
			//Dictionary<string, string> locationFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{

			//this._importTemplateInventoryRepository = this._serviceLocator.GetInstance<IImportTemplateInventoryRepository>();
			_templateInventoryRepository = this._serviceLocator.GetInstance<ITemplateInventoryRepository>();
			foreach (TemplateInventory templateInventory in this._templateInventoryRepository.GetTemplateInventorys(fromPathFile))
			{
				if (templateInventory == null) continue;
				templateInventory.Uid = templateInventory.Id.ToString();
				yield return templateInventory;
			}
		}

	
	}
}

