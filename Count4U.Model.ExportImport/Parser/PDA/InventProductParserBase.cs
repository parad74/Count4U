using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Globalization;
using Count4U.Model.Count4U.Validate;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class InventProductParserBase
	{
		protected IFileParser _fileParser;
		protected readonly IServiceLocator _serviceLocator;
		protected readonly ILog _log;
		protected Dictionary<string, DocumentHeader> _documentHeaderDictionary;
		protected IDocumentHeaderRepository _documentHeaderRepository;
		protected Dictionary<string, Itur> _iturDictionary;
		protected List<BitAndRecord> _errorBitList;
		protected DateTimeFormatInfo _dtfi;

		public InventProductParserBase(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString());

			this._log = log;
			this._documentHeaderRepository = documentHeaderRepository;
			this._documentHeaderDictionary = new Dictionary<string, DocumentHeader>();
			this._iturDictionary = new Dictionary<string, Itur>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, DocumentHeader> DocumentHeaderDictionary
		{
			get { return this._documentHeaderDictionary; }
		}

		public Dictionary<string, Itur> IturDictionary
		{
			get { return this._iturDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

	

	}
}


	


	

