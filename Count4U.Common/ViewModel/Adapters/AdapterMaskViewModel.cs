using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Common.ViewModel.Adapters
{
    public class AdapterMaskViewModel : CBIContextBaseViewModel
    {
        private readonly IUnityContainer _container;

        private readonly DelegateCommand _saveCommand;
        private readonly DelegateCommand _barcodeMaskSelectCommand;
        private readonly DelegateCommand _makatMaskSelectCommand;

        private IMaskRepository _maskRepo;
        private string _barcodeMask;
        private string _makatMask;

        private string _fileName;
        private string _adapterName;

        private bool _isCustomer;
        private bool _isBranch;
        private bool _isInventor;

        private bool _isCustomerVisible;
        private bool _isBranchVisible;
        private bool _isInventorVisible;
		 private bool _isMakatMaskVisible;

        private string _cbiObjCode;
        private Mask _mask;
        private readonly IEventAggregator _eventAggregator;

        public AdapterMaskViewModel(IContextCBIRepository contextCBIRepository, IUnityContainer container, IEventAggregator eventAggregator)
            : base(contextCBIRepository)
        {
            this._eventAggregator = eventAggregator;
            this._container = container;
            this._saveCommand = new DelegateCommand(SaveCommandExecuted);

            this._barcodeMaskSelectCommand = new DelegateCommand(BarcodeMaskSelectCommandExecuted);
            this._makatMaskSelectCommand = new DelegateCommand(MakatMaskSelectCommandExecuted);
        }       

        public DelegateCommand SaveCommand
        {
            get { return this._saveCommand; }
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

        public bool IsCustomer
        {
            get { return this._isCustomer; }
            set
            {
                this._isCustomer = value;
                RaisePropertyChanged(() => IsCustomer);

                if (this._isCustomer)
                {
                    this._isBranch = false;
                    this._isInventor = false;
                    RaisePropertyChanged(() => IsBranch);
                    RaisePropertyChanged(() => IsInventor);

                    Mask mask = GetMask(base.CurrentCustomer.Code, GetCustomerRepo());
                    LoadForm(mask);
                }
            }
        }

        public bool IsBranch
        {
            get { return this._isBranch; }
            set
            {
                this._isBranch = value;
                RaisePropertyChanged(() => IsBranch);

                if (this._isBranch)
                {
                    this._isCustomer = false;
                    this._isInventor = false;
                    RaisePropertyChanged(() => IsCustomer);
                    RaisePropertyChanged(() => IsInventor);

                    Mask mask = GetMask(base.CurrentBranch.Code, GetBranchRepo());
                    LoadForm(mask);

                }
            }
        }

        public bool IsInventor
        {
            get { return this._isInventor; }
            set
            {
                this._isInventor = value;
                RaisePropertyChanged(() => IsInventor);

                if (this._isInventor)
                {
                    this._isCustomer = false;
                    this._isBranch = false;
                    RaisePropertyChanged(() => IsCustomer);
                    RaisePropertyChanged(() => IsBranch);

                    Mask mask = GetMask(base.CurrentInventor.Code, GetInventorRepo());
                        LoadForm(mask);
                }
            }
        }

        public bool IsCustomerVisible
        {
            get { return this._isCustomerVisible; }
            set
            {
                this._isCustomerVisible = value;
                RaisePropertyChanged(() => IsCustomerVisible);
            }
        }

        public bool IsBranchVisible
        {
            get { return this._isBranchVisible; }
            set
            {
                this._isBranchVisible = value;
                RaisePropertyChanged(() => IsBranchVisible);
            }
        }

        public bool IsInventorVisible
        {
            get { return this._isInventorVisible; }
            set
            {
                this._isInventorVisible = value;
                RaisePropertyChanged(() => IsInventorVisible);
            }
        }

		

        public bool IsMakatMaskVisible
        {
            get { return this._isMakatMaskVisible; }
            set
            {
                this._isMakatMaskVisible = value;
                RaisePropertyChanged(() => IsMakatMaskVisible);
            }
        }


        public bool IsBranchOrInventorVisible
        {
            get { return IsInventorVisible || IsBranchVisible; }
        }

        public Mask Mask
        {
            get { return this._mask; }
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

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.MaskFileName))
            {
                this._fileName = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.MaskFileName).Value;
            }

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AdapterName))
            {
                this._adapterName = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AdapterName).Value;
            }

            this._cbiObjCode = String.Empty;
            switch (base.CBIDbContext)
            {
                case Common.NavigationSettings.CBIDbContextCustomer:
                    this._maskRepo = GetCustomerRepo();
                    this._cbiObjCode = CurrentCustomer.Code;

                    IsCustomerVisible = false;
                    IsBranchVisible = false;
                    IsInventorVisible = false;

                    IsCustomer = true;
                    break;
                case Common.NavigationSettings.CBIDbContextBranch:
                    this._maskRepo = GetBranchRepo();
                    this._cbiObjCode = CurrentBranch.Code;

                    IsCustomerVisible = true;
                    IsBranchVisible = true;
                    IsInventorVisible = false;
  					
					Mask maskB = GetMask(base.CurrentBranch.Code, GetBranchRepo());
					if (maskB != null) { IsBranch = true; }
					else { IsCustomer = true; }
                    break;
                case Common.NavigationSettings.CBIDbContextInventor:
                    this._maskRepo = GetInventorRepo();
                    this._cbiObjCode = CurrentInventor.Code;

                    IsCustomerVisible = true;
                    IsBranchVisible = true;
                    IsInventorVisible = true;
   					
					Mask maskI = GetMask(base.CurrentInventor.Code, GetInventorRepo());
					if (maskI != null)
					{
						IsInventor = true;
					}
					else
					{
						Mask maskB1 = GetMask(base.CurrentBranch.Code, GetBranchRepo());
						if (maskB1 != null) 
						{
							IsBranch = true; 
						}
						else
						{
							IsCustomer = true;
						}
					}
                    break;
            }
        }

        private IMaskRepository GetCustomerRepo()
        {
           return this._container.Resolve<IMaskRepository>("CustomerMaskEFRepository");            
        }

        private IMaskRepository GetBranchRepo()
        {
            return this._container.Resolve<IMaskRepository>("BranchMaskEFRepository");
        }

        private IMaskRepository GetInventorRepo()
        {
            return this._container.Resolve<IMaskRepository>("InventorMaskEFRepository");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        private Mask GetMask(string cbiCode, IMaskRepository repo)
        {
			//var list = repo.GetMasks().Where(r => r.Code == cbiCode).Select(e => e).ToList();
            return repo.GetMasks().FirstOrDefault(r => r.AdapterCode == this._adapterName && r.FileCode == this._fileName && r.Code == cbiCode);
        }

        private void SaveCommandExecuted()
        {
            this._mask = GetMask(this._cbiObjCode, this._maskRepo);
            if (this._mask == null)
            {
                this._mask = new Mask();
                this._mask.Code = this._cbiObjCode;
                this._mask.AdapterCode = this._adapterName;
                this._mask.FileCode = this._fileName;

                SaveForm();
                this._maskRepo.Insert(this._mask, GetDbPath);

                int fileName = Convert.ToInt32(this._fileName);
                for(int i = 1; i < fileName; i++)
                {
                    if (this._maskRepo.GetMasks().Any(r=>r.AdapterCode==_adapterName && r.FileCode == i.ToString() && r.Code == this._cbiObjCode))
                        continue;

                    Mask mask = new Mask();
                    mask.Code = this._cbiObjCode;
                    mask.AdapterCode = this._adapterName;
                    mask.FileCode = i.ToString();
                    mask.MakatMask = String.Empty;
                    mask.BarcodeMask = String.Empty;
                    this._maskRepo.Insert(mask, GetDbPath);
                }
            }
            else
            {
                SaveForm();
                this._maskRepo.Update(this._mask, GetDbPath);
            }

        }

        void SaveForm()
        {
            this._mask.BarcodeMask = BarcodeMask;
            this._mask.MakatMask = MakatMask;
        }

        void LoadForm(Mask mask)
        {
            if (mask != null)
            {
                BarcodeMask = mask.BarcodeMask;
                MakatMask = mask.MakatMask;
            }
            else
            {
                BarcodeMask = String.Empty;
                MakatMask = String.Empty;
            }
        }

        private void MakatMaskSelectCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string> ();
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
            payload.Settings = new Dictionary<string, string> ();
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