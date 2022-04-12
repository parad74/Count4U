using System;
using System.ComponentModel;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.Web;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class InventorFormViewModel : NotificationObject, IDataErrorInfo
    {
        private readonly IContextCBIRepository _contextCBIRepository;
		private readonly FtpFolderProFile _ftpFolderProfile;
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly IInventorRepository _inventorRepository;

        private Inventor _inventor;
        private bool _isReadOnly;
        private bool _isNewMode;
		private bool _askProfile;
        private string _code;
        private bool _isCodeReadOnly;
        private string _dbFilePath;
        public CBIContext _context;
        private string _createDate;
        private string _inventorDateString;
        private DateTime _inventorDate;

		public InventorFormViewModel(IContextCBIRepository contextCBIRepository, 
			IInventorRepository inventorRepository,
			IUserSettingsManager userSettingsManager,
			FtpFolderProFile ftpFolderProfile)
        {
			this._userSettingsManager = userSettingsManager;
			this._inventorRepository = inventorRepository;
			this._contextCBIRepository = contextCBIRepository;
			this._ftpFolderProfile = ftpFolderProfile;
        }

		//customer can be null
        public void InventorSet(Customer customer, Inventor inventor, bool isReadOnly, bool isNewMode, CBIContext context)
        {
            this._context = context;
            this._isNewMode = isNewMode;
            this.IsReadOnly = isReadOnly;
            this._inventor = inventor;

            this.IsCodeReadOnly = ((this._isReadOnly == true)
                || (this._isNewMode == false));

            this.Description = this._inventor.Description;
            this.Tag3 = this._inventor.Tag3;
            

            this._createDate = UtilsConvert.DateToStringLong(this._inventor.CreateDate);
		    // this._completeDate = UtilsConvert.DateToStringLong(this._inventor.CompleteDate);


            this._inventorDateString = UtilsConvert.DateToStringLong(this._inventor.InventorDate);
            this._inventorDate = this._inventor.InventorDate;

            this.Code = this._inventor.Code;
			this.AskProfile = false;
			if (customer != null)
			{
				if (string.IsNullOrWhiteSpace(customer.ComplexAdapterParametr3) == false) 
				{
					if (customer.ComplexAdapterParametr3 == "1") this.AskProfile = true;
				}
			}
		}

		public void UpdateAskProfileFromCustomer(Customer customer)
		{
			this.AskProfile = false;

			if (customer != null)
			{
				if (string.IsNullOrWhiteSpace(customer.ComplexAdapterParametr3) == false)
				{
					if (customer.ComplexAdapterParametr3 == "1") this.AskProfile =true;
				}
			}
			this.RaisePropertyChanged(() => this.AskProfile);
		}

        public string Description
        {
            get { return this._inventor.Description; }
            set
            {
                this._inventor.Description = value;
                this.RaisePropertyChanged(() => this.Description);
            }
        }

        public string Tag3           //Manager Name
        {
            get { return this._inventor.Tag3; }
            set
            {
                this._inventor.Tag3 = value;
                this.RaisePropertyChanged(() => this.Tag3);
            }
        }

        public bool IsReadOnly
        {
            get { return this._isReadOnly; }
            set
            {
                this._isReadOnly = value;
                this.RaisePropertyChanged(() => this.IsReadOnly);
            }
        }

        public string Code
        {
            get { return this._code; }
            set
            {
                this._code = value;
                this.RaisePropertyChanged(() => this.Code);

                CalculateDbPath();
            }
        }

        private void CalculateDbPath()
        {
            this._inventor.InventorDate = this.InventorDate;
            string path = this._contextCBIRepository.CreateNewDBPath(this._inventor, this._code);
            this.DbFilePath = path;
        }

        public bool IsCodeReadOnly
        {
            get { return this._isCodeReadOnly; }
            set
            {
                this._isCodeReadOnly = value;
                this.RaisePropertyChanged(() => this.IsCodeReadOnly);
            }
        }

        public bool IsEdit
        {
            get { return !this._isNewMode; }
        }

        public bool IsNewMode
        {
            get { return this._isNewMode; }
        }

		public bool AskProfile
		{
			get { return this._askProfile; }
			set
			{
				this._askProfile = value;
				this.RaisePropertyChanged(() => this.AskProfile);
			}
		}

        public string DbFilePath
        {
            get { return this._dbFilePath; }
            set
            {
                this._dbFilePath = value;
                this.RaisePropertyChanged(() => this.DbFilePath);
            }
        }             

        public string CreateDate
        {
            get { return _createDate; }
            set
            {
                _createDate = value;
                RaisePropertyChanged(() => CreateDate);
            }
        }

        public string InventorDateString
        {
            get { return _inventorDateString; }
            set
            {
                _inventorDateString = value;
                RaisePropertyChanged(() => InventorDateString);
            }
        }

        public AuditConfig ApplyChanges()
        {
            this._inventor.Description = this.Description;
            this._inventor.Tag3 = this.Tag3;//Manager Name
            this._inventor.Code = this.Code;
            this._inventor.InventorDate = this.InventorDate;

			AuditConfig auditConfig = this._contextCBIRepository.GetCurrentCBIConfig(this._context);

			if (this._isNewMode)
			{
				this._inventor.DBPath = this.DbFilePath;
				//if (this.AskProfile == true)
				//{

				//	Customer currentCustomer = this._contextCBIRepository.GetCustomerByCode(auditConfig.CustomerCode);
				//	Branch currentBranch = this._contextCBIRepository.GetBranchByCode(auditConfig.BranchCode);
				//	Inventor newInventor = this._inventor;
				//	newInventor.CustomerCode = auditConfig.CustomerCode;
				//	newInventor.BranchCode = auditConfig.BranchCode;
				//			if (currentCustomer != null)
				//	{
				//		string rootFonderOnFtp = this._inventorRepository.Connection.RootFolderFtp(); //mINV 
				//                    FtpCommandResult ftpCommandResult = new FtpCommandResult();
				//                    this._ftpFolderProfile.InventorProfileCreate(currentCustomer, currentBranch, newInventor, rootFonderOnFtp, ref ftpCommandResult);
				//	}


				//}
				 }



			auditConfig.InventorCode = this._inventor.Code;
            auditConfig.InventorName = this._inventor.Name;
            auditConfig.DBPath = this._inventor.DBPath;
            auditConfig.InventorDate = this._inventor.InventorDate;
            auditConfig.CreateDate = this._inventor.CreateDate;
           // this._contextCBIRepository.SaveCurrentCBIConfig(this._context, auditConfig);                  //elfkbkf 14.12.2021
            return auditConfig;
        }

        public FtpCommandResult CheckFtpAfterApplyChanges(AuditConfig auditConfig)
        {
            // bool ret = true;
            FtpCommandResult ftpCommandResult = new FtpCommandResult();
            ftpCommandResult.Successful = SuccessfulEnum.Successful;
            if (this.IsNewMode)
            {
                //this._inventor.DBPath = this.DbFilePath;

                //if (this.AskProfile == true)
                //{
                //    string rootFonderOnFtp = this._customerRepository.Connection.RootFolderFtp(); //mINV
                //    this._ftpFolderProfile.CustomerProfileCreate(this._customer, rootFonderOnFtp, ref ftpCommandResult);
                //}

                if (this.AskProfile == true)
                {

                    Customer currentCustomer = this._contextCBIRepository.GetCustomerByCode(auditConfig.CustomerCode);
                    Branch currentBranch = this._contextCBIRepository.GetBranchByCode(auditConfig.BranchCode);
                    Inventor newInventor = this._inventor;
                    newInventor.CustomerCode = auditConfig.CustomerCode;
                    newInventor.BranchCode = auditConfig.BranchCode;
                    if (currentCustomer != null)
                    {
                        string rootFonderOnFtp = this._inventorRepository.Connection.RootFolderFtp(); //mINV 
                        this._ftpFolderProfile.InventorProfileCreate(currentCustomer, currentBranch, newInventor, rootFonderOnFtp, ref ftpCommandResult);
                    }
                }
            }
            return ftpCommandResult;
        }
        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Code":
                        if (String.IsNullOrWhiteSpace(_code))
                            return Localization.Resources.ViewModel_InventorForm_InventorCodeEmpty;
                        if (this.IsCodeUnique() == false)
                            return Localization.Resources.ViewModel_InventorForm_InventorExist;
                        break;
                }
                return null;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        public DateTime InventorDate
        {
            get { return _inventorDate; }
            set
            {
                _inventorDate = value;
                RaisePropertyChanged(()=>InventorDate);

                CalculateDbPath();
            }
        }

        public bool IsCodeUnique()
        {
            if (String.IsNullOrWhiteSpace(this._code))
                return false;

            if (!_isNewMode) 
                return true;

            if (this._contextCBIRepository.IsExistsInventorCode(this._code)) 
                return false;

            return true;
        }
       
    }

}