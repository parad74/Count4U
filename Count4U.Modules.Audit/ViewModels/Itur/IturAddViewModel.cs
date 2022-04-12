using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using System.Threading;
using Count4U.Model.Common;

namespace Count4U.Modules.Audit.ViewModels
{
    public class IturAddViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private readonly IEventAggregator _eventAggregator;
		private readonly IServiceLocator _serviceLocator;
        private readonly ILocationRepository _locationRepository;
        private readonly IStatusIturRepository _statusIturRepository;
        private readonly IIturRepository _iturRepository;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private string _textValue;
        private string _numberPrefix;
        private string _erpCode;
        
        private bool _disabled;        

        public IturAddViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
			IServiceLocator serviceLocator,
            ILocationRepository locationRepository,
            IStatusIturRepository statusIturRepository,
            IIturRepository iturRepository
            )
            : base(contextCBIRepository)
        {
            _iturRepository = iturRepository;
            this._statusIturRepository = statusIturRepository;
            this._locationRepository = locationRepository;
            this._eventAggregator = eventAggregator;
			this._serviceLocator = serviceLocator;
		   
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
        }

        public Locations Locations { get; set; }
        public Location SelectedLocation { get; set; }
//        public List<StatusItur> Statuses { get; set; }
//        public StatusItur SelectedStatus { get; set; }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public string TextValue
        {
            get { return this._textValue; }
            set
            {
                this._textValue = value;
				this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.TotalIturs);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public string TotalIturs
        {
            get
            {
				List<int> res = CommaDashStringParser.Parse(this._textValue);
                return res == null ? "0" : res.Count.ToString();
            }
        }

        public string NumberPrefix
        {
            get { return this._numberPrefix; }
            set
            {
                this._numberPrefix = value;
				this.RaisePropertyChanged(() => this.NumberPrefix);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }


        public bool Disabled
        {
            get { return this._disabled; }
            set
            {
                this._disabled = value;
                this.RaisePropertyChanged(() => this.Disabled);
            }
        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this.Locations = this._locationRepository.GetLocations(base.GetDbPath);
            this.SelectedLocation = this.Locations.FirstOrDefault();

//            this.Statuses = this._statusIturRepository.CodeStatusIturDictionary.Data.ToList();
//            this.SelectedStatus = this.Statuses.FirstOrDefault();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
					case "TextValue":
						{
							if (this.IsTextValid() == false)
							{
								return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
							}
						}
					break;
					case "NumberPrefix":
						{
							int bit = this._numberPrefix.PrefixValidate();
							if (bit != 0)
							{
								return IturValidate.Bit2PrefixErrorMessage(bit);
							}
						}
						//if (IsPrefixValid() == false)
						//{
						//    return "Prefix is not empty numeric string 4 characters maximum";
						//}

						break;
                }
                return null;
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string ERPCode
        {
            get { return _erpCode; }
            set { _erpCode = value; }
        }

        private bool IsTextValid()
        {
            return CommaDashStringParser.IsValid(this._textValue);
        }

		//private bool IsPrefixValid()
		//{
		//    int ret = this._numberPrefix.PrefixValidate();
		//    if (ret == 0)
		//    {
		//        return true;
		//    }
		//    else
		//    {
		//        return false;
		//    }

		//    //if (String.IsNullOrEmpty(this._numberPrefix))
		//    //    return true;
		//    //return this._numberPrefix.Length <= 4 &&
		//    //       Regex.IsMatch(this._numberPrefix, @"^[0-9]+$");
		//}

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
            if (OkCommandCanExecuted())
            {
                this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

                using (new CursorWait())
                {
					Dictionary<string, Itur> iturFromDBDictionary = this._iturRepository.GetIturDictionary(base.GetDbPath, true);
					Dictionary<string, Itur> iturToDBDictionary = new Dictionary<string, Itur>();

                    List<int> itursNumbers = CommaDashStringParser.Parse(this._textValue);
                    List<int> itursThatExist = new List<int>();
                    string[] iturCodes = this._iturRepository.GetIturCodes(base.GetDbPath);

                    foreach (int number in itursNumbers)
                    {
                        Itur itur = new Itur();
                        
                        itur.CreateDate = DateTime.Now;
                        itur.ModifyDate = DateTime.Now;
                        //itur.Location = SelectedLocation == null ? null : this.SelectedLocation.Name;
                        itur.LocationCode = SelectedLocation == null ? Common.Constants.Misc.UnknownLocation : this.SelectedLocation.Code;
                        itur.Name = String.Empty;
                        itur.Number = number;
                        itur.NumberSufix = UtilsItur.SuffixFromNumber(itur.Number);
                        itur.NumberPrefix = UtilsItur.PrefixFromString(_numberPrefix);
                        itur.IturCode = UtilsItur.CodeFromPrefixAndSuffix(itur.NumberPrefix, itur.NumberSufix);
						itur.StatusIturBit = (int)CodeIturStatusEnum.NotApprove;
						itur.StatusIturGroupBit = (int)IturStatusGroupEnum.Empty;
						itur.StatusDocHeaderBit = (int)ConvertDataErrorCodeEnum.NoError;
						if (this._erpCode == null)
						{
							this._erpCode = "";
							itur.ERPIturCode = "";
						}
						else
						{
							itur.ERPIturCode = this._erpCode.CutLength(249);
						}

                        if (iturCodes.Contains(itur.IturCode))
                        {
                            itursThatExist.Add(itur.Number);
                            continue;
                        }

                        if (itur.Disabled == true)
                        {
                            itur.StatusIturBit = itur.StatusIturBit.Or((int)CodeIturStatusEnum.DisableItur);
							itur.StatusIturGroupBit = (int)IturStatusGroupEnum.DisableAndNoOneDoc;
                        }
                        else
                        {
                            if (itur.StatusIturBit > (int)CodeIturStatusEnum.DisableItur)
                            {
                                itur.StatusIturBit = itur.StatusIturBit - (int)CodeIturStatusEnum.DisableItur;
                            }
                        }

//                        itur.StatusIturCode = this.SelectedStatus.Code;
//                        itur.StatusIturBit = this.SelectedStatus.Bit;
                        itur.Disabled = this._disabled;
                        
                        //itursToInsert.Add(itur);
						if (iturFromDBDictionary.ContainsKey(itur.IturCode) == false)
						{
							iturToDBDictionary[itur.IturCode] = itur;
						}
                    }

                    //this._iturRepository.Insert(itursToInsert, base.GetDbPath);
                   // this._iturRepository.RefillApproveStatusBit(itursToInsert, base.GetDbPath);
			
					IImportIturRepository iturADORepository = this._serviceLocator.GetInstance<IImportIturRepository>();
					iturADORepository.FromDictionaryToDB(base.GetDbPath, iturToDBDictionary, CancellationToken.None, null);
	
                    this._eventAggregator.GetEvent<ItursAddedEvent>().Publish(new ItursAddedEventPayload()
                    {
                        ItursThatExist = itursThatExist,
                    });
                }
            }            
        }

        private bool OkCommandCanExecuted()
        {
            return 
				((String.IsNullOrEmpty(this._textValue) == false)
				&& (String.IsNullOrEmpty(this._numberPrefix) == false)
				&& (this.IsTextValid() == true)
//				&& (this.SelectedLocation != null) 
				//&& (this.IsPrefixValid() == true)
				&& (this._numberPrefix.PrefixValidate() == 0)
				);
        }
    }
}