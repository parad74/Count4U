using System;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;

namespace Count4U.Common.ViewModel
{
    public class CBIState
    {
        private readonly IContextCBIRepository _contextCBIRepository;
        private readonly CBIContext _context;
        private readonly string _dbContext;

        private string _dbPath;
        private object _currentObject;
        private Customer _currentCustomer;
        private Branch _currentBranch;
        private Inventor _currentInventor;

        public CBIState(IContextCBIRepository contextCBIRepository,
            CBIContext context,
            string dbContext)
        {
            _dbContext = dbContext;
            _context = context;
            _contextCBIRepository = contextCBIRepository;
        }

        public IContextCBIRepository ContextCBIRepository
        {
            get { return _contextCBIRepository; }
        }

        public CBIContext Context
        {
            get { return _context; }
        }

        public string DbContext
        {
            get { return _dbContext; }
        }

        public AuditConfig GetCreateAuditConfig()
        {
            return this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
        }

        public AuditConfig GetMainAuditConfig()
        {
            return this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
        }

        public AuditConfig GetHistoryAuditConfig()
        {
            return this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
        }

        public AuditConfig GetAuditConfigByCurrentContext()
        {
            return this._contextCBIRepository.GetCurrentCBIConfig(this.Context);
        }

        public string GetDbPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._dbPath) && !String.IsNullOrEmpty(this._dbContext))
                {
                    switch (this._dbContext)
                    {
                        case NavigationSettings.CBIDbContextCustomer:
                            this._currentObject = CurrentCustomer;
                            break;
                        case NavigationSettings.CBIDbContextBranch:
                            this._currentObject = CurrentBranch;
                            break;
                        case NavigationSettings.CBIDbContextInventor:
                            this._currentObject = CurrentInventor;
                            break;
                    }
                    if (this._currentObject != null)
                        this._dbPath = this._contextCBIRepository.GetDBPath(this._currentObject);
                }
                return this._dbPath;
            }
        }

        public Customer CurrentCustomer
        {
            get
            {
                if (this._currentCustomer == null)
                {
                    switch (this.Context)
                    {
                        case CBIContext.CreateInventor:
                            this._currentCustomer = this._contextCBIRepository.GetCurrentCustomer(this.GetCreateAuditConfig());
                            break;
                        case CBIContext.History:
                            this._currentCustomer = this._contextCBIRepository.GetCurrentCustomer(this.GetHistoryAuditConfig());
                            break;
                        case CBIContext.Main:
                            this._currentCustomer = this._contextCBIRepository.GetCurrentCustomer(this.GetMainAuditConfig());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return this._currentCustomer;
            }
        }

        public Branch CurrentBranch
        {
            get
            {
                if (this._currentBranch == null)
                {
                    switch (this.Context)
                    {
                        case CBIContext.CreateInventor:
                            this._currentBranch = this._contextCBIRepository.GetCurrentBranch(this.GetCreateAuditConfig());
                            break;
                        case CBIContext.History:
                            this._currentBranch = this._contextCBIRepository.GetCurrentBranch(this.GetHistoryAuditConfig());
                            break;
                        case CBIContext.Main:
                            this._currentBranch = this._contextCBIRepository.GetCurrentBranch(this.GetMainAuditConfig());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return this._currentBranch;
            }
        }

        public Inventor CurrentInventor
        {
            get
            {
                if (this._currentInventor == null)
                    switch (this.Context)
                    {
                        case CBIContext.CreateInventor:
                            this._currentInventor = this._contextCBIRepository.GetCurrentInventor(this.GetCreateAuditConfig());
                            break;
                        case CBIContext.History:
                            this._currentInventor = this._contextCBIRepository.GetCurrentInventor(this.GetHistoryAuditConfig());
                            break;
                        case CBIContext.Main:
                            this._currentInventor = this._contextCBIRepository.GetCurrentInventor(this.GetMainAuditConfig());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                return this._currentInventor;
            }
        }

        public object GetCurrentDomainObject()
        {
            if (!String.IsNullOrEmpty(this._dbContext))
            {
                switch (this._dbContext)
                {
                    case NavigationSettings.CBIDbContextCustomer:
                        return CurrentCustomer;
                    case NavigationSettings.CBIDbContextBranch:
                        return CurrentBranch;
                    case NavigationSettings.CBIDbContextInventor:
                        return CurrentInventor;
                }
            }

            return null;
        }

		public void SetModifyDateTimeCurrentDomainObject(DateTime updateDateTime)
		{
			if (!String.IsNullOrEmpty(this._dbContext))
			{
				switch (this._dbContext)
				{
					case NavigationSettings.CBIDbContextCustomer:
						{
							CurrentCustomer.ModifyDate = updateDateTime;
							CurrentCustomer.LastUpdatedCatalog = updateDateTime;
							this._contextCBIRepository.Update(CurrentCustomer);
						}
						break;
					case NavigationSettings.CBIDbContextBranch:
						{
							CurrentBranch.ModifyDate = updateDateTime;
							CurrentBranch.LastUpdatedCatalog = updateDateTime;
							
							this._contextCBIRepository.Update(CurrentBranch);
						}
						break;
					case NavigationSettings.CBIDbContextInventor:
						{
							CurrentInventor.CompleteDate = updateDateTime;
							CurrentInventor.LastUpdatedCatalog = updateDateTime;
							this._contextCBIRepository.Update(CurrentInventor);
						}
						break;
				}
			}
		}


		public DateTime GeLastUpdateDateTimeCurrentDomainObject()
		{
			if (!String.IsNullOrEmpty(this._dbContext))
			{
				switch (this._dbContext)
				{
					case NavigationSettings.CBIDbContextCustomer:
						{
							return CurrentCustomer.LastUpdatedCatalog;
						}
					case NavigationSettings.CBIDbContextBranch:
						{
							return CurrentBranch.LastUpdatedCatalog;
						}
					case NavigationSettings.CBIDbContextInventor:
						{
							return CurrentInventor.LastUpdatedCatalog;
						}
				}

			}
			return DateTime.Now;
		}

		public DateTime GetModifyDateTimeCurrentDomainObject()
		{
			if (!String.IsNullOrEmpty(this._dbContext))
			{
				switch (this._dbContext)
				{
					case NavigationSettings.CBIDbContextCustomer:
						{
							return CurrentCustomer.ModifyDate;
						}
					case NavigationSettings.CBIDbContextBranch:
						{
							return CurrentBranch.ModifyDate;
						}
					case NavigationSettings.CBIDbContextInventor:
						{
							return CurrentInventor.CompleteDate;
						}
				}

			}
			return DateTime.Now;
		}

        public void ResetCurrentCustomer()
        {
            _currentCustomer = null;
        }

        public void ResetCurrentBranch()
        {
            _currentBranch = null;
        }

        public void ResetCurrentInventor()
        {
            _currentInventor = null;
        }
    }
}