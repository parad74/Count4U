using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Count4U.Common.ViewModel.Adapters
{
    public abstract class ModuleBaseViewModel : CBIContextBaseViewModel
    {
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ILog _logImport;
		private readonly IServiceLocator _serviceLocator;
        private readonly IIniFileParser _iniFileParser;
        private readonly IUserSettingsManager _userSettingsManager;

        private Action<string> _updateLog = delegate { };
        private Action<bool> _setIsBusy = delegate { };
        private Action<long> _updateProgress = delegate { };
        private Action<string> _updateBusyText = delegate { };
        private Action<bool> _setIsCancelOk = delegate { };        
        private CancellationToken _cancellationToken;
        private bool _isSaveFileLog;        

        protected ModuleBaseViewModel(
            IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator)
            : base(contextCbiRepository)
        {
            this._logImport = logImport;
            this._serviceLocator = serviceLocator;

            this._iniFileParser = serviceLocator.GetInstance<IIniFileParser>();
            this._userSettingsManager = serviceLocator.GetInstance<IUserSettingsManager>();
        }

        public abstract Encoding Encoding { get; set; }

        public Action<string> UpdateLog
        {
            set { this._updateLog = value; }
            protected get { return this._updateLog; }
        }

        public Action<bool> SetIsBusy
        {
            set { this._setIsBusy = value; }
            protected get { return _setIsBusy; }
        }

	

        public Action<long> UpdateProgress
        {
            set { this._updateProgress = value; }
            protected get { return _updateProgress; }
        }

        protected CancellationToken CancellationToken
        {
            set { this._cancellationToken = value; }
            get { return this._cancellationToken; }
        }

        public Action<string> UpdateBusyText
        {
            set { _updateBusyText = value; }
            protected get { return _updateBusyText; }
        }

        protected bool IsSaveFileLog
        {
            get { return _isSaveFileLog; }
            set { this._isSaveFileLog = value; }
        }

        protected ILog LogImport
        {
            get { return _logImport; }
        }

        protected IServiceLocator ServiceLocator
        {
            get { return _serviceLocator; }
        }

        public Action<bool> SetIsCancelOk
        {
            protected get { return _setIsCancelOk; }
            set { _setIsCancelOk = value; }
        }       

        protected void WriteErrorExceptionToAppLog(string error, Exception exc)
        {
            _logger.ErrorException(error, exc);
        }

        protected abstract string GetModulesFolderPath();

        protected void InitEncodingFromIniData(Dictionary<ImportProviderParmEnum, string> iniData)
        {
            if (iniData.ContainsKey(ImportProviderParmEnum.Encoding))
            {
                string value = iniData[ImportProviderParmEnum.Encoding];
                if (!String.IsNullOrWhiteSpace(value))
                {
                    this.Encoding = UtilsConvert.StringToEncoding(value);
                }

                if (this.Encoding == null)
                {
                    int codepage = this._userSettingsManager.ImportEncodingGet();
                    try
                    {
                        this.Encoding = Encoding.GetEncoding(codepage);
                    }
                    catch (Exception exc)
                    {
                        _logger.ErrorException("Parsing usersettings.xml import encoding", exc);
                    }
                }

                if (this.Encoding == null)
                {
                    Encoding = Encoding.GetEncoding(1255);
                }
            }
        }

		protected Dictionary<ImportProviderParmEnum, string> ParseAdapterIniFile(string sectionName, string iniFilePath)
		{
			Dictionary<ImportProviderParmEnum, string> result = new Dictionary<ImportProviderParmEnum, string>();
			try
			{
				foreach (ImportProviderParmEnum en in Enum.GetValues(typeof(ImportProviderParmEnum)))
				{
					result.Add(en, String.Empty);
				}

				string iniPath = String.Empty;
				if (String.IsNullOrWhiteSpace(iniFilePath))
				{
					string name = this.GetType().Assembly.GetName().Name;
					string iniName = string.Format("{0}{1}", name, ".ini");

					iniPath = System.IO.Path.Combine(this.GetModulesFolderPath(), iniName);
				}
				else
				{
					iniPath = iniFilePath;
				}

				var iniData = _iniFileParser.Get(iniPath, sectionName);

				if (iniData != null)
				{
					foreach (var kvp in iniData.Data)
					{
						string key = kvp.Key.Trim();
						string value = kvp.Value.Trim();

						ImportProviderParmEnum en;

						if (Enum.TryParse(key, true, out en))
						{
							if (result.ContainsKey(en))
								result[en] = value;
						}
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("ParseAdapterIniFile", exc);
			}
			return result;
		}
    }
}