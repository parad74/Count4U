using System;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.Events.Misc;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.DomainObject
{
    public class BranchDashboardPartViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IBranchRepository _branchRepository;
        private readonly UICommandRepository _commandRepository;
        private readonly IRegionManager _regionManager;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _moreCommand;

        private readonly DelegateCommand _propertiesCommand;
        private readonly DelegateCommand _packCommand;
		private readonly DelegateCommand _profileCommand;

        private string _name;
        private string _codeLocal;
        private string _codeERP;
        private string _phone;
        private string _fax;
        private string _mail;

        public BranchDashboardPartViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IBranchRepository branchRepository,
            IRegionManager regionManager,
            UICommandRepository commandRepository)
            : base(contextCbiRepository)
        {
            _commandRepository = commandRepository;
            _regionManager = regionManager;
            _branchRepository = branchRepository;
            _eventAggregator = eventAggregator;
            _editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted);
            _moreCommand = _commandRepository.Build(enUICommand.More, MoreCommandExecuted);

            _propertiesCommand = _commandRepository.Build(enUICommand.Properties, PropertiesCommandExecuted);
            _packCommand = _commandRepository.Build(enUICommand.Pack, PackCommandExecuted);
			//_profileCommand = _commandRepository.Build(enUICommand.Profile, ProfileCommandExecuted);
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string CodeLocal
        {
            get { return _codeLocal; }
            set
            {
                _codeLocal = value;
                RaisePropertyChanged(() => CodeLocal);
            }
        }

        public string CodeERP
        {
            get { return _codeERP; }
            set
            {
                _codeERP = value;
                RaisePropertyChanged(() => CodeERP);
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                RaisePropertyChanged(() => Phone);
            }
        }

        public string Fax
        {
            get { return _fax; }
            set
            {
                _fax = value;
                RaisePropertyChanged(() => Fax);
            }
        }

        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                RaisePropertyChanged(() => Mail);
            }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public DelegateCommand PropertiesCommand
        {
            get { return _propertiesCommand; }
        }

        public DelegateCommand PackCommand
        {
            get { return _packCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<BranchEditedEvent>().Subscribe(BranchEdited);

            Build();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<BranchEditedEvent>().Unsubscribe(BranchEdited);
        }

        private void Build()
        {
            Branch branch = GetBranch();

            if (branch == null) return;

            Name = branch.Name;
            CodeLocal = branch.BranchCodeLocal;
            CodeERP = branch.BranchCodeERP;
            Phone = branch.Phone;
            Fax = branch.Fax;
            Mail = branch.Mail;
        }

        private void EditCommandExecuted()
        {
            this._eventAggregator.GetEvent<BranchEditEvent>().Publish(new BranchEditEventPayload() { Branch = GetBranch(), Context = CBIContext.Main });
        }

        private void BranchEdited(Branch branch)
        {
            Build();
        }

        private Branch GetBranch()
        {
            return _branchRepository.GetBranchByCode(base.CurrentBranch.Code);
        }

        private void MoreCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetMainAuditConfig());
            UtilsNavigate.BranchChooseOpen(this.Context, this._regionManager, query);
        }

        private void PackCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextBranch);
            Utils.AddAuditConfigToQuery(uriQuery, base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
            UtilsNavigate.PackOpen(this._regionManager, uriQuery);
        }

        private void PropertiesCommandExecuted()
        {
            ObjectPropertiesViewEventPayload payload = new ObjectPropertiesViewEventPayload();
            payload.Context = CBIContext.Main;
            payload.DbContext = Common.NavigationSettings.CBIDbContextBranch;

            this._eventAggregator.GetEvent<ObjectPropertiesViewEvent>().Publish(payload);
        }
    }
}