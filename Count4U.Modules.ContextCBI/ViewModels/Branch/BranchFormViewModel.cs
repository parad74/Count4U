using System;
using System.Collections.Generic;
using System.ComponentModel;
using Count4U.Common.Helpers;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System.Linq;
using Count4U.Model.Audit;
using Count4U.Model;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BranchFormViewModel : NotificationObject, IDataErrorInfo
    {
        private readonly IContextCBIRepository _contextCBIRepository;
        private readonly IBranchRepository _branchRepository;

        private Branch _branch;
        private readonly DelegateCommand _copyCommand;
        private readonly DelegateCommand _clearCommand;
        private readonly DelegateCommand _codeGenerateCommand;

        private string _name;
        private string _description;
		private string _params;
        private string _contactPerson;
        private string _mail;
        private string _address1;
        private string _address2;
        private string _phone;
        private string _fax;
        private string _code;
        private string _codeLocal;
        private string _codeErp;
		
        private bool _isReadOnly;
        private bool _isNewMode;
        private bool _isCodeReadOnly;
        private CBIContext _context;
        private Branches _branches;

        public BranchFormViewModel(IContextCBIRepository contextCBIRepository, IBranchRepository branchRepository)
        {
            this._branchRepository = branchRepository;
            this._contextCBIRepository = contextCBIRepository;
            this._copyCommand = new DelegateCommand(this.CopyCommandExecuted, CopyCommandCanExecute);
            this._clearCommand = new DelegateCommand(this.ClearCommandExecuted, ClearCommandCanExecute);
            this._codeGenerateCommand = new DelegateCommand(CodeGenerateCommandExecuted);
        }

        private bool ClearCommandCanExecute()
        {
            return !this._isReadOnly;
        }

        private bool CopyCommandCanExecute()
        {
			AuditConfig config = this._contextCBIRepository.GetCurrentCBIConfig(this._context);
            Customer current = this._contextCBIRepository.GetCurrentCustomer(config);
            return current != null && !this._isReadOnly;
        }

        public void BranchSet(Branch branch, bool isReadOnly, bool isNewMode, CBIContext context)
        {
            this._context = context;
            this._isReadOnly = isReadOnly;
            this._isNewMode = isNewMode;
            this._branch = branch;

            this.IsCodeReadOnly = (this._isReadOnly == true) || (this.IsNewMode == false);

            AssignNewCodeAccordingToCustomer();            

            this.Name = this._branch.Name;
            this.Description = this._branch.Description;
			this.Params = this._branch.ImportCatalogAdapterParms;
			this.ContactPerson = "";//this._branch.ContactPerson; занято на Selected Adapter import  Family
            this.Mail = this._branch.Mail;

            Address1 = String.Empty;
            Address2 = String.Empty;
            if (String.IsNullOrEmpty(this._branch.Address) == false)
            {
                List<string> split = Helpers.AddressParseIntoMultiple(this._branch.Address);

                if (split.Count > 0)
                    this.Address1 = split[0];
                if (split.Count > 1)
                    this.Address2 = split[1];
            }

            this.Phone = this._branch.Phone;
            this.Fax = this._branch.Fax;            
            this.CodeLocal = this._branch.BranchCodeLocal;
            this.CodeErp = this._branch.BranchCodeERP;

            this._copyCommand.RaiseCanExecuteChanged();
        }       

        #region public properties

        public DelegateCommand CopyCommand
        {
            get { return this._copyCommand; }
        }

        public DelegateCommand ClearCommand
        {
            get { return this._clearCommand; }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        public string Description
        {
            get { return this._description; }
            set
            {
                this._description = value;
                this.RaisePropertyChanged(() => this.Description);
            }
        }

		public string Params
		{
			get { return this._params; }
			set
			{
				this._params = value;
				this.RaisePropertyChanged(() => this.Params);
			}
		}

        public string ContactPerson
        {
            get { return this._contactPerson; }
            set
            {
                this._contactPerson = value;
                this.RaisePropertyChanged(() => this.ContactPerson);
            }
        }

        public string Mail
        {
            get { return this._mail; }
            set
            {
                this._mail = value;
                this.RaisePropertyChanged(() => this.Mail);
            }
        }

        public string Address1
        {
            get { return this._address1; }
            set
            {
                this._address1 = value;
                this.RaisePropertyChanged(() => this.Address1);
            }
        }

        public string Address2
        {
            get { return this._address2; }
            set
            {
                this._address2 = value;
                this.RaisePropertyChanged(() => this.Address2);
            }
        }

        public string Phone
        {
            get { return this._phone; }
            set
            {
                this._phone = value;
                this.RaisePropertyChanged(() => this.Phone);
            }
        }

        public string Fax
        {
            get { return this._fax; }
            set
            {
                this._fax = value;
                this.RaisePropertyChanged(() => this.Fax);
            }
        }

        public string Code
        {
            get { return this._code; }
            set
            {
                this._code = value;
                this.RaisePropertyChanged(() => this.Code);
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

        public bool IsCodeReadOnly
        {
            get { return this._isCodeReadOnly; }
            set
            {
                this._isCodeReadOnly = value;
                this.RaisePropertyChanged(() => this.IsCodeReadOnly);
            }
        }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Code":
                        if (this._isNewMode == false)
                            return String.Empty;

                     string validation = BranchValidate.CodeValidate(_code);
                        if (!String.IsNullOrEmpty(validation))
                            return validation;

                        if (this.IsCodeUnique(_code) == false)
                            return Count4U.Model.ValidateMessage.Branch.CodeUnique;

                        if (UtilsConvert.HaveDosHebrewCharInWord(_code))
                        {
                            return Localization.Resources.Bit_HebrewCharsAreNotAllowed;
                        }
                        break;
                    case "Mail":
                        return BranchValidate.MailValidate(this._mail);                               
                    case "Phone":
                        return BranchValidate.PhoneValidate(this._phone);    
                    case "Fax":
                        return BranchValidate.FaxValidate(this._fax);                                   
                    case "Name":
                        return BranchValidate.NameValidate(this._name);
                    case "ContactPerson":
                        return BranchValidate.ContactPersonValidate(this._contactPerson);       
                    case "CodeLocal":
                        return BranchValidate.CodeLocalValidate(this._codeLocal);
                    case "CodeErp":
                        return BranchValidate.CodeErpValidate(this._codeErp);
                }
                return null;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        public bool IsNewMode
        {
            get { return this._isNewMode; }
        }

        public DelegateCommand CodeGenerateCommand
        {
            get { return this._codeGenerateCommand; }
        }

        public string CodeLocal
        {
            get { return this._codeLocal; }
            set
            {
                this._codeLocal = value;
                RaisePropertyChanged(() => CodeLocal);
            }
        }

        public string CodeErp
        {
            get { return this._codeErp; }
            set
            {
                this._codeErp = value;
                RaisePropertyChanged(() => CodeErp);
            }
        }

        #endregion

        public void ApplyChanges()
        {
            this._branch.Name = this.Name;
            this._branch.Description = this.Description;
			this._branch.ImportCatalogAdapterParms = this.Params;
			this._branch.ContactPerson = this._branch.ContactPerson; //занято на Selected Adapter import  Family
            this._branch.Mail = this.Mail;
            this._branch.Address = Helpers.AddressParseIntoSingle(
                new List<string> { this.Address1, this.Address2 }, 500);

            this._branch.Phone = String.IsNullOrEmpty(this.Phone) ? String.Empty : this.Phone.Trim(new[] { '+' });
            this._branch.Fax = String.IsNullOrEmpty(this.Fax) ? String.Empty : this.Fax.Trim(new[] { '+' });
            this._branch.BranchCodeLocal = CodeLocal;
            this._branch.BranchCodeERP = CodeErp;

            this._branch.Code = this.Code;

            if (this.IsNewMode)
            {
                this._branch.DBPath = this._branch.Code;
            }
        }

        private void ClearCommandExecuted()
        {
            this.ContactPerson = String.Empty;
            this.Mail = String.Empty;
            this.Address1 = String.Empty;
            this.Address2 = String.Empty;
            this.Phone = String.Empty;
            this.Fax = String.Empty;
        }

        public void InvalidateCopyCommand()
        {
            this._copyCommand.RaiseCanExecuteChanged();
        }

        private void CopyCommandExecuted()
        {
			AuditConfig config = this._contextCBIRepository.GetCurrentCBIConfig(this._context);
            Customer current = this._contextCBIRepository.GetCurrentCustomer(config);

            if (current != null)
            {
				this.ContactPerson = "";//this._customer.ContactPerson; занято на Selected Adapter import  Family
                this.Mail = current.Mail;

                if (String.IsNullOrEmpty(current.Address) == false)
                {
                    List<string> split = Helpers.AddressParseIntoMultiple(current.Address);

                    if (split.Count > 0)
                        this.Address1 = split[0];
                    if (split.Count > 1)
                        this.Address2 = split[1];
                }

                this.Phone = current.Phone;
                this.Fax = current.Fax;
            }
        }

        public bool IsFormValid()
        {
            if (this._isNewMode && !IsCodeUnique(this._code))
                return false;

            if (String.IsNullOrWhiteSpace(_name))
                return false;

            return IsEmailValid() &&
                   IsPhoneValid(Phone) &&
                   IsFaxValid(Fax) &&
                   BranchValidate.NameValidate(_name) == String.Empty &&
                   BranchValidate.CodeLocalValidate(_codeLocal) == String.Empty &&
                   BranchValidate.CodeErpValidate(_codeErp) == String.Empty;
        }

        private bool IsCodeUnique(string code)
        {
            if (!this._isNewMode)
                return true;

            if (String.IsNullOrWhiteSpace(code))
                return false;

            if (_branches == null)
            {
                _branches = this._branchRepository.GetBranches(new SelectParams());
            }

            return !_branches.Any(r => r.Code == this._code);
        }

        private bool IsEmailValid()
        {
            if (String.IsNullOrEmpty(Mail) == true) return true;
            return BranchValidate.MailValidate(_mail) == String.Empty;
        }   

        private bool IsPhoneValid(string phone)
        {
            if (String.IsNullOrEmpty(phone))
                return true;

            return BranchValidate.PhoneValidate(phone) == String.Empty;
        }

        private bool IsFaxValid(string fax)
        {
            if (String.IsNullOrEmpty(fax))
                return true;

            return BranchValidate.FaxValidate(fax) == String.Empty;
        }

        private void CodeGenerateCommandExecuted()
        {
            this._code = CodeNewGenerate();

            RaisePropertyChanged(() => Code);
        }

        public void AssignNewCodeAccordingToCustomer()
        {
            if (this._isNewMode)
            {
                this._branch.Code = CodeNewGenerate();
            }

            Code = this._branch.Code;
        }

        private string CodeNewGenerate()
        {
            string result;
            do
            {
				AuditConfig config = this._contextCBIRepository.GetCurrentCBIConfig(this._context);
                Customer current = this._contextCBIRepository.GetCurrentCustomer(config);

                string customerCode = current == null ? String.Empty : current.Code;
                string code = String.Format("{0}-{1}", customerCode, Utils.CodeNewGenerate(4));

                result = code;
            } while (!IsCodeUnique(result));

            return result;
        }
    }
}
