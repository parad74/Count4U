using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;

namespace Count4U.Model.Count4U
{
	public class SectionDBParser : ISectionParser
	{
		private readonly ISectionRepository _sectionRepository;
		private readonly ILog _log;
		private Dictionary<string, Section> _sectionDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;

		public SectionDBParser(ISectionRepository sectionRepository,
			ILog log)
		{
			if (sectionRepository == null) throw new ArgumentNullException("sectionRepository");

			this._sectionRepository = sectionRepository;
			this._log = log;
			this._sectionDictionary = new Dictionary<string, Section>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Section> SectionDictionary
		{
			get { return this._sectionDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		public Dictionary<string, Section> GetSections(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Section> sectionFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._sectionDictionary.Clear();
			this._errorBitList.Clear();

			this._sectionDictionary = this._sectionRepository.GetSectionDictionary(fromPathFile, true);
			return this._sectionDictionary;
			
		}
	}
}
