using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Count4U.Common.Constants;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Modules.ContextCBI.Events.ParsingMask;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.Linq;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask
{
    public class MaskListViewModel : CBIContextBaseViewModel
    {
        private readonly IUnityContainer _container;
        private IMaskRepository _maskRepo;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _scriptOpenCommand;
        private readonly DelegateCommand _scriptSaveCommand;
        private readonly UICommandRepository _uiCommandRepository;

        private readonly ObservableCollection<MaskItemViewModel> _items;
        private MaskItemViewModel _selectedItem;

        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        private readonly IImportAdapterRepository _importAdapterRepository;


        public MaskListViewModel(IContextCBIRepository contextCBIRepository,
            IUnityContainer container,
            IEventAggregator eventAggregator,
            IImportAdapterRepository importAdapterRepository,
            IUserSettingsManager userSettingsManager,
            UICommandRepository uiCommandRepository)
            : base(contextCBIRepository)
        {
            _uiCommandRepository = uiCommandRepository;
            _userSettingsManager = userSettingsManager;
            this._importAdapterRepository = importAdapterRepository;
            this._eventAggregator = eventAggregator;
            this._container = container;
            this._items = new ObservableCollection<MaskItemViewModel>();

            this._deleteCommand =_uiCommandRepository.Build(enUICommand.Delete,  DeleteCommandExecuted, DeleteCommandCanExecute);
            this._editCommand = _uiCommandRepository.Build(enUICommand.Edit, EditCommandExecuted, EditCommandCanExecute);
            this._addCommand = _uiCommandRepository.Build(enUICommand.Add, AddCommandExecuted);
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._scriptOpenCommand = _uiCommandRepository.Build(enUICommand.OpenScript, ScriptOpenCommandExecuted);
            this._scriptSaveCommand = _uiCommandRepository.Build(enUICommand.SaveScript, ScriptSaveCommandExecuted);
        }

        public ObservableCollection<MaskItemViewModel> Items
        {
            get { return this._items; }
        }

        public MaskItemViewModel SelectedItem
        {
            get { return this._selectedItem; }
            set
            {
                this._selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                this._editCommand.RaiseCanExecuteChanged();
                this._deleteCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return this._editCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return this._deleteCommand; }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return this._yesNoRequest; }
        }

        public DelegateCommand ScriptOpenCommand
        {
            get { return _scriptOpenCommand; }
        }

        public DelegateCommand ScriptSaveCommand
        {
            get { return _scriptSaveCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<MaskAddedEditedEvent>().Subscribe(MaskAddedEdited);

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

            BuildItems();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<MaskAddedEditedEvent>().Unsubscribe(MaskAddedEdited);
        }

        private void BuildItems()
        {
            this._items.Clear();

            string cbiObjCode = String.Empty;
            switch (base.CBIDbContext)
            {
                case Common.NavigationSettings.CBIDbContextCustomer:
                    cbiObjCode = CurrentCustomer.Code;
                    break;
                case Common.NavigationSettings.CBIDbContextBranch:
                    cbiObjCode = CurrentBranch.Code;
                    break;
                case Common.NavigationSettings.CBIDbContextInventor:
                    cbiObjCode = CurrentInventor.Code;
                    break;
            }

            if (String.IsNullOrEmpty(cbiObjCode)) return;

            var adapters = GetAdapters();

            foreach (Mask mask in this._maskRepo.GetMasks().Where(r => r.Code == cbiObjCode
                && adapters.Any(z => z.Name == r.AdapterCode)))
            {
                MaskItemViewModel viewModel = new MaskItemViewModel(mask);
                this._items.Add(viewModel);
            }
        }

        private List<IImportModuleInfo> GetAdapters()
        {
            string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
            string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
            string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

            return Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportCatalog,
                currentCustomerCode, currentBranchCode, currentInventorCode);
        }

        private void AddCommandExecuted()
        {
            MaskAddEditEventPayload payload = new MaskAddEditEventPayload();
            payload.Mask = null;
            payload.CBIDbContext = base.CBIDbContext;
            payload.CDBContext = base.Context;

            this._eventAggregator.GetEvent<MaskAddEditEvent>().Publish(payload);
        }

        private bool EditCommandCanExecute()
        {
            return this._selectedItem != null;
        }

        private void EditCommandExecuted()
        {
            MaskAddEditEventPayload payload = new MaskAddEditEventPayload();
            payload.Mask = this._selectedItem.Mask;
            payload.CBIDbContext = base.CBIDbContext;
            payload.CDBContext = base.Context;

            this._eventAggregator.GetEvent<MaskAddEditEvent>().Publish(payload);
        }

        private bool DeleteCommandCanExecute()
        {
            return this._selectedItem != null;
        }

        private void DeleteCommandExecuted()
        {
            Mask mask = this._selectedItem.Mask;

            var adapter = GetAdapters().FirstOrDefault(r => r.ImportDomainEnum == ImportDomainEnum.ImportCatalog && r.Name == mask.AdapterCode);
            string adapterName = adapter == null ? mask.AdapterCode : adapter.Title;

            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Image = MessageBoxImage.Warning;
            notification.Settings = this._userSettingsManager;
            notification.Content = String.Format(Localization.Resources.Msg_Delete_Parsing_Mask, adapterName);
            this._yesNoRequest.Raise(notification, r =>
                                                  {
                                                      if (!r.IsYes) return;

                                                      this._maskRepo.Delete(mask.Code, mask.AdapterCode, GetDbPath);
                                                      BuildItems();
                                                  });
        }

        private void MaskAddedEdited(Mask mask)
        {
            BuildItems();
        }

        private void ScriptSaveCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.MaskScriptSaveView;
            payload.WindowTitle = WindowTitles.ScriptSave;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);

            OnModalWindowRequest(payload);
        }

        private void ScriptOpenCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.MaskScriptOpenView;
            payload.WindowTitle = WindowTitles.ScriptOpen;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);

            payload.Callback = r =>
            {
                ScriptOpenData data = r as ScriptOpenData;
                if (data == null) return;
                if (data.IsOk)
                {
                    this.BuildItems();
                }
            };

            OnModalWindowRequest(payload);
        }
    }
}