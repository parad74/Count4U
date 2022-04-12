using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using NLog;

namespace Count4U.Common.ViewModel
{
    public class CBIContextBaseViewModel : NavigationAwareViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public readonly IContextCBIRepository _contextCBIRepository;

        private CBIState _state;

        public CBIContextBaseViewModel(IContextCBIRepository contextCBIRepository)
        {
            this._contextCBIRepository = contextCBIRepository;
        }

        public CBIContext Context
        {
            get
            {
                if (_state == null)
                {
                    _logger.Warn("State is not initialized!");
                }
                return State.Context;
            }          
        }

        public IContextCBIRepository ContextCBIRepository
        {
            get { return this._contextCBIRepository; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            CBIContext? context = Utils.CBIContextFromNavigationParameters(navigationContext);
            if (context == null)
            {
                throw new InvalidOperationException();
            }

            string cbiDbContext = Utils.CBIDbContextFromNavigationParameters(navigationContext);

            _state = new CBIState(_contextCBIRepository, context.Value, cbiDbContext);
        }


		public bool IsConfigFileImportExists(string adapterName)
		{
			//if (info == null) return false;
			//string adapterName = info.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == true) return false;
			if (adapterName == Common.Constants.UpdateCatalogAdapterName.UpdateCatalogEmptyAdapter) return false;
		
			// ?? Пока договорились что конфиг всегда берется из Customer
			string adapterConfigFileName = @"\" + adapterName + ".config";
			string configPath = this._contextCBIRepository.GetConfigFolderPath(this.CurrentCustomer) + adapterConfigFileName;
			if (File.Exists(configPath) == true)
			{
				return true;
			}
			return false;
		}

        protected AuditConfig GetCreateAuditConfig()
        {
			if (_state == null) return null;
            return _state.GetCreateAuditConfig();
        }

        protected AuditConfig GetMainAuditConfig()
        {
			if (_state == null) return null;
            return _state.GetMainAuditConfig();
        }

        protected AuditConfig GetHistoryAuditConfig()
        {
			if (_state == null) return null;
            return _state.GetHistoryAuditConfig();
        }

        protected AuditConfig GetAuditConfigByCurrentContext()
        {
			if (_state == null) return null;
            return _state.GetAuditConfigByCurrentContext();
        }

		[NotInludeAttribute]
        public string GetDbPath
        {
            get 
			{
				if (_state == null) return "";
				return State.GetDbPath; 
			}

        }

        public Customer CurrentCustomer
        {
            get {
				if (_state == null) return null;
				return _state.CurrentCustomer; 
			}
        }

        public Branch CurrentBranch
        {
			get
			{
				if (_state == null) return null;
				return _state.CurrentBranch; 
			}
        }

		[NotInludeAttribute]
		public string CurrentCustomerNameAndCode
		{
			get {
				if (_state == null) return "";
				return UtilsConvert.CustomerFancyName(_state.CurrentCustomer); 
			}
		}

		[NotInludeAttribute]
		public string CurrentBranchNameAndCode
		{
			get {
				if (_state == null) return "";
				return UtilsConvert.BranchFancyName(_state.CurrentBranch); 
			}
		}

        public Inventor CurrentInventor
        {
			get
			{
				if (_state == null) return null;
				return _state.CurrentInventor; 
			}
        }

		[NotInludeAttribute]
        public string CBIDbContext
        {
            get
			{
				if (_state == null) return "";
				return this.State.DbContext;
			}
        }

        public CBIState State
        {
            get { return _state; }
			set { _state = value; }
        }

        public object GetCurrentDomainObject()
        {
			if (_state == null) return null;
            return State.GetCurrentDomainObject();
        }

		public void SetModifyDateTimeCurrentDomainObject(DateTime updateDateTime)
        {
			if (_state == null) return;
			State.SetModifyDateTimeCurrentDomainObject(updateDateTime);
        }

		public DateTime GetModifyDateTimeCurrentDomainObject()
		{
			return State.GetModifyDateTimeCurrentDomainObject();
		}

		public DateTime GeLastUpdateDateTimeCurrentDomainObject()
		{
			return State.GeLastUpdateDateTimeCurrentDomainObject();
		}

        public void ResetCurrentCustomer()
        {
			if (_state == null) return;
            State.ResetCurrentCustomer();
        }

        public void ResetCurrentBranch()
        {
			if (_state == null) return;
            State.ResetCurrentBranch();
        }

        public void ResetCurrentInventor()
        {
			if (_state == null) return;
            State.ResetCurrentInventor();
        }

	
	}
}