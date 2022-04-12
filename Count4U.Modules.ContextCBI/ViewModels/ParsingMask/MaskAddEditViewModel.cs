using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.ParsingMask;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Count4U.Model.Interface;
using Count4U.Model;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask
{
    public class MaskAddEditViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnityContainer _container;

        private IMaskRepository _maskRepo;

        private bool _isNew;

        private Mask _mask;
        private string _barcodeMask;
        private string _makatMask;

        private readonly DelegateCommand _barcodeMaskSelectCommand;
        private readonly DelegateCommand _makatMaskSelectCommand;
        private IImportModuleInfo _selectedAdapter;
        private ObservableCollection<IImportModuleInfo> _adapters;
        private readonly IImportAdapterRepository _importAdapterRepository;

        public MaskAddEditViewModel(IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IUnityContainer container,
            IImportAdapterRepository importAdapterRepository)
            : base(contextCbiRepository)
        {
            this._importAdapterRepository = importAdapterRepository;
            this._container = container;
            this._eventAggregator = eventAggregator;
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);

            this._barcodeMaskSelectCommand = new DelegateCommand(BarcodeMaskSelectCommandExecuted);
            this._makatMaskSelectCommand = new DelegateCommand(MakatMaskSelectCommandExecuted);          
        }
        
        public string Error
        {
            get { return string.Empty; }
        }

        public string BarcodeMask
        {
            get { return this._barcodeMask; }
            set
            {
                this._barcodeMask = value;
                RaisePropertyChanged(() => BarcodeMask);
            }
        }

        public string MakatMask
        {
            get { return this._makatMask; }
            set
            {
                this._makatMask = value;
                RaisePropertyChanged(() => MakatMask);
            }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public ObservableCollection<IImportModuleInfo> Adapters
        {
            get { return this._adapters; }
        }

        public IImportModuleInfo SelectedAdapter
        {
            get { return this._selectedAdapter; }
            set
            {
                this._selectedAdapter = value;
                RaisePropertyChanged(() => SelectedAdapter);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsNew
        {
            get { return this._isNew; }
        }

        public DelegateCommand BarcodeMaskSelectCommand
        {
            get { return this._barcodeMaskSelectCommand; }
        }

        public DelegateCommand MakatMaskSelectCommand
        {
            get { return this._makatMaskSelectCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
            string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
            string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

            var adapters = Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportCatalog, 
                currentCustomerCode, currentBranchCode, currentInventorCode);

            this._adapters = new ObservableCollection<IImportModuleInfo>(adapters);           

            switch (base.CBIDbContext)
            {
                case Common.NavigationSettings.CBIDbContextCustomer:
                    this._maskRepo = this._container.Resolve<IMaskRepository>("CustomerMaskEFRepository");
                    break;
                case Common.NavigationSettings.CBIDbContextBranch:
                    this._maskRepo = this._container.Resolve<IMaskRepository>("BranchMaskEFRepository");
                    break;
                case Common.NavigationSettings.CBIDbContextInventor:
                    this._maskRepo = this._container.Resolve<IMaskRepository>("InventorMaskEFRepository");
                    break;
                default:
                    throw new InvalidOperationException("unreachable");
            }

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.MaskId))
            {
                string id = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.MaskId).Value;
                this._mask = this._maskRepo.GetMasks().FirstOrDefault(r => r.ID.ToString() == id);

                if (this._mask == null) return;

                this._isNew = false;
                BarcodeMask = this._mask.BarcodeMask;
                MakatMask = this._mask.MakatMask;
                this._selectedAdapter = this._adapters.FirstOrDefault(r => r.Name == this._mask.AdapterCode);
            }
            else
            {
                this._isNew = true;
                this._mask = new Mask();

                switch (base.CBIDbContext)
                {
                    case Common.NavigationSettings.CBIDbContextCustomer:
                        this._mask.Code = base.CurrentCustomer.Code;
                        this._selectedAdapter = this._adapters.FirstOrDefault(r => base.CurrentCustomer == null ? r.IsDefault : r.Name == base.CurrentCustomer.ImportCatalogProviderCode);
                        break;
                    case Common.NavigationSettings.CBIDbContextBranch:
                        this._mask.Code = base.CurrentBranch.Code;
                        this._selectedAdapter = this._adapters.FirstOrDefault(r => base.CurrentBranch == null ? r.IsDefault : r.Name == base.CurrentBranch.ImportCatalogProviderCode);
                        break;
                    case Common.NavigationSettings.CBIDbContextInventor:
                        this._mask.Code = base.CurrentInventor.Code;
                        this._selectedAdapter = this._adapters.FirstOrDefault(r => base.CurrentInventor == null ? r.IsDefault : r.Name == base.CurrentInventor.ImportCatalogAdapterCode);
                        break;
                }
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        #region Implementation of IDataErrorInfo

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "BarcodeMask":
                        if (!String.IsNullOrEmpty(this._barcodeMask) && MaskTemplateRepository.ToMaskRecord(this._barcodeMask) == null)
                            return Localization.Resources.ViewModel_MaskAddEdit_NotValidMask;
                        break;
                    case "MakatMask":
                        if (!String.IsNullOrEmpty(this._makatMask) && MaskTemplateRepository.ToMaskRecord(this._makatMask) == null)
                            return Localization.Resources.ViewModel_MaskAddEdit_NotValidMask;
                        break;
                }

                return String.Empty;
            }

        }

        #endregion

        private bool OkCommandCanExecute()
        {
            return this._selectedAdapter != null;
        }

        private void OkCommandExecuted()
        {
            this._mask.BarcodeMask = BarcodeMask;
            this._mask.MakatMask = MakatMask;           

            if (IsNew)
            {
                this._mask.AdapterCode = this._selectedAdapter.Name;
                var masks = this._maskRepo.GetMasks().Where(r => r.AdapterCode == this._selectedAdapter.Name && !String.IsNullOrEmpty(r.FileCode)).ToList();
                int max = masks.Any() ? masks.Max(r => Int32.Parse(r.FileCode)) : 0;
                this._mask.FileCode = (max + 1).ToString();
                this._maskRepo.Insert(this._mask, GetDbPath);
            }
            else
            {
                this._maskRepo.Update(this._mask, GetDbPath);
            }

            this._eventAggregator.GetEvent<MaskAddedEditedEvent>().Publish(this._mask);
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void MakatMaskSelectCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            payload.Callback = r =>
            {
                MaskSelectedData data = r as MaskSelectedData;
                if (data != null)
                    MakatMask = data.Value;
            };
            OnModalWindowRequest(payload);
        }

        private void BarcodeMaskSelectCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            payload.Callback = r =>
            {
                MaskSelectedData data = r as MaskSelectedData;
                if (data != null)
                    BarcodeMask = data.Value;
            };
            OnModalWindowRequest(payload);
        }    
    }
}