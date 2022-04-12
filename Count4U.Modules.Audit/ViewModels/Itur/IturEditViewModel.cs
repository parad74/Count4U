using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Common;

namespace Count4U.Modules.Audit.ViewModels
{
    public class IturEditViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IStatusIturRepository _statusIturRepository;
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private string _code;
        private string _number;
        private string _numberPrefix;
        private string _numberSuffix;
        private Itur _itur;
        private bool _isDisabled;
        private string _erpCode;
        private readonly IIturRepository _iturRepository;


        public IturEditViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IStatusIturRepository statusIturRepository,
            IIturRepository iturRepository
            )
            : base(contextCBIRepository)
        {
            this._iturRepository = iturRepository;
            this._statusIturRepository = statusIturRepository;
            this._eventAggregator = eventAggregator;

            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
        }

        public List<StatusItur> Statuses { get; set; }
        public StatusItur SelectedStatus { get; set; }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this.Statuses = this._statusIturRepository.CodeStatusIturDictionary.Values.ToList();
            this.SelectedStatus = this.Statuses.FirstOrDefault();

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.IturCode))
            {
                string iturCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.IturCode).Value;
                this._itur = this._iturRepository.GetIturByCode(iturCode, base.GetDbPath);

				if (this._itur != null)
				{
					this._code = this._itur.IturCode;
					this._number = this._itur.Number.ToString();
					this._numberPrefix = this._itur.NumberPrefix;
					this._numberSuffix = this._itur.NumberSufix;
					this._isDisabled = this._itur.Disabled ?? false;
					this._erpCode = this._itur.ERPIturCode;
				}
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Number":
						if (String.IsNullOrEmpty(this._number) == true)
						{
							return Count4U.Model.ValidateMessage.IsEmpty.Number;  // return "Number can not be empty";
						}
                        if (UtilsConvert.IsOkAsInt(this._number) == false)
						{
							return Count4U.Model.ValidateMessage.InvalidFormat.General;
						}
                        break;
					case "NumberPreffix":
						{
							int bit = this._numberPrefix.PrefixValidate();
							if (bit != 0)
							{
								return IturValidate.Bit2PrefixErrorMessage(bit);
							}
							//if (!this.IsPrefixValid())
							//    return "Prefix is not empty numeric string 4 characters maximum";
							break;
						}
                }
                return String.Empty;
            }
        }

        public string Error
        {
            get { return string.Empty; }
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

        public string Number
        {
            get { return this._number; }
            set
            {
                this._number = value;
				this.RaisePropertyChanged(() => this.Number);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public string NumberPreffix
        {
            get { return this._numberPrefix; }
            set
            {
                this._numberPrefix = value;
				this.RaisePropertyChanged(() => this.NumberPreffix);
            }
        }

        public string NumberSuffix
        {
            get { return this._numberSuffix; }
            set
            {
                this._numberSuffix = value;
				this.RaisePropertyChanged(() => this.NumberSuffix);
            }
        }

        public bool IsDisabled
        {
            get { return this._isDisabled; }
            set
            {
                this._isDisabled = value;
                this.RaisePropertyChanged(() => this.IsDisabled);
            }
        }

        public string ERPCode
        {
            get { return _erpCode; }
            set { _erpCode = value; }
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
            if (OkCommandCanExecuted() == true)
            {
                this._itur.Number = Convert.ToInt32(this._number);
				this._itur.NumberPrefix = String.IsNullOrEmpty(this._numberPrefix) ? String.Empty : String.Format("{0:0000}", Int32.Parse(this._numberPrefix));    
                this._itur.StatusIturBit = this.SelectedStatus.Bit;
                this._itur.ModifyDate = DateTime.Now;
                this._itur.Disabled = this._isDisabled;
				if (this._erpCode != null)
				{
					this._itur.ERPIturCode = this._erpCode.CutLength(249);
				}
				else
				{
					this._itur.ERPIturCode = "";
				}

                this._iturRepository.Update(this._itur, base.GetDbPath);

                this._eventAggregator.GetEvent<IturEditedEvent>().Publish(this._itur);
                this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
            }
        }

        private bool OkCommandCanExecuted()
        {
			return 
				((String.IsNullOrEmpty(this._number) == false)
                && (UtilsConvert.IsOkAsInt(this._number) == true)
				//&& this.IsPrefixValid() == true)
				&& (this._numberPrefix.PrefixValidate() == 0)
				);
        }

		//private bool IsPrefixValid()
		//{
		//    if (String.IsNullOrEmpty(this._numberPrefix))
		//        return true;
		//    return this._numberPrefix.Length <= 4 &&
		//           Regex.IsMatch(this._numberPrefix, @"^[0-9]+$");
		//}
    }
}