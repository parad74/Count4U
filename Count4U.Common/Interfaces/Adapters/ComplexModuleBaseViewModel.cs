using System;
using System.Text;
using System.Windows.Shapes;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Count4U.Common.Interfaces.Adapters
{
    public abstract class ComplexModuleBaseViewModel
    {
		//private Action _raiseCanExport;
		//private Encoding _encoding;

		//protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		//public readonly IContextCBIRepository _contextCBIRepository;
		//private readonly ILog _logImport;
		//private readonly IServiceLocator _serviceLocator;
		//private readonly IIniFileParser _iniFileParser;
		//private readonly IUserSettingsManager _userSettingsManager;
		//public readonly IDBSettings _dbSettings;

		protected ComplexModuleBaseViewModel( )  {}
			//IContextCBIRepository contextCbiRepository,
			//ILog logImport,
			//IServiceLocator serviceLocator,
			//IIniFileParser iniFileParser,
			//IUserSettingsManager userSettingsManager,
			//IDBSettings dbSettings,
			//IContextCBIRepository contextCBIRepository)        
		//{
		//	this._logImport = logImport;
		//	this._serviceLocator = serviceLocator;
		//	this._iniFileParser = iniFileParser;
		//	this._userSettingsManager = userSettingsManager;
		//	this._dbSettings = dbSettings;
		//	this._contextCBIRepository = contextCBIRepository;
		//}

		//protected IContextCBIRepository ContextCBIRepository
		//{
		//	get { return this._contextCBIRepository; }
		//}

		//public abstract string GetObjectWorkingFolderPath(IContextCBIRepository contextCbiRepository, IDBSettings dbSettings, object currentDomainObject, string subFolder = "");
		//TODO
		//private abstract string GetRelativeWorkingFolderPath(object currentDomainObject);

		 
		//protected abstract void EncondingUpdated();

		//public override Encoding Encoding
		//{
		//	get
		//	{
		//		return _encoding;
		//	}
		//	set
		//	{
		//		_encoding = value;

		//		RaisePropertyChanged(() => Encoding);

		//		this.EncondingUpdated();
		//	}
		//}

		//public Action RaiseCanExport
		//{
		//	set { this._raiseCanExport = value; }
		//	protected get { return this._raiseCanExport; }
		//}

 
    }

}