using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.SearchFilter;
using Count4U.Configuration.Dynamic;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.Events;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Count4U.Common.Extensions;
using System.Windows.Threading;
using System.IO;
using Count4U.Model.Interface;
using Count4U.Common.ViewModel.Filter.Sorting;

namespace Count4U.Modules.Audit.ViewModels
{
    public class InventProductViewModel : CBIContextBaseViewModel, IRegionMemberLifetime
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static System.Type InventProductType = typeof(Count4U.Model.Count4U.InventProduct);

        private readonly IInventProductRepository _inventProductRepository;
		private readonly IIturRepository _iturRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IRegionManager _regionManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly EditorTemplateComboViewModel _editorComboViewModel;
        private readonly UICommandRepository _uiCommandRepository;
        private readonly ModalWindowLauncher _modalWindowLauncher;
		protected readonly IDBSettings _dbSettings;

        private readonly DelegateCommand<InventProductItemViewModel> _openDetailsCommand;

        private readonly DelegateCommand _cloneCommand;
		private readonly DelegateCommand _copyCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _deleteCommand;
		private readonly DelegateCommand _deleteNoneCatalogCommand;
        private readonly DelegateCommand _resetErrorInventProductSelectedCommand;
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _resetQuantityCommand;

        private readonly DelegateCommand<InventProductItemViewModel> _editGridCommand;
        private readonly DelegateCommand<InventProductItemViewModel> _cancelGridCommand;
        private readonly DelegateCommand<InventProductItemViewModel> _commitGridCommand;

		private readonly DelegateCommand<InventProductPropertyStrItemViewModel> _editDetailGridCommand;
		private readonly DelegateCommand<InventProductPropertyStrItemViewModel> _cancelDetailGridCommand;
		private readonly DelegateCommand<InventProductPropertyStrItemViewModel> _commitDetailGridCommand;

		private readonly UICommandRepository<InventProductItemViewModel> _commandRepositoryInventProduct;
		private readonly DelegateCommand<InventProductItemViewModel> _clearMarkCommand;
		private readonly DelegateCommand<InventProductItemViewModel> _acceptQuantityCommand;
		private readonly DelegateCommand<InventProductItemViewModel> _accept1Command;
		private readonly DelegateCommand<InventProductItemViewModel> _accept2Command;

        private readonly ObservableCollection<InventProductItemViewModel> _items;
        private InventProductItemViewModel _selectedItem;

        private int _itemsTotal;
        private int _pageCurrent;
        private int _pageSize;

        private readonly ReportButtonViewModel _reportButtonViewModel;
        private SearchFilterViewModel _searchFilterViewModel;

		private bool _isDataGridProperyVisibility;
		private readonly DispatcherTimer _timer;

		private InventProductPropertyStrItemViewModel _detailSelectedItem;
		private readonly ObservableCollection<InventProductPropertyStrItemViewModel> _detailItems;
		private Dictionary<string, string> _dictionaryPropertyStr;
		private Dictionary<string, PropertyStr> _dictionaryObjectPropertyStr;
		private IPropertyStrRepository _propertyStrRepository;

        public InventProductViewModel(
            IContextCBIRepository contextCbiRepository,
            IInventProductRepository inventProductRepository,
			 IIturRepository iturRepository,
            IUserSettingsManager userSettingsManager,
            IEventAggregator eventAggregator,
            ReportButtonViewModel reportButtonViewModel,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
             EditorTemplateComboViewModel editorComboViewModel,
            UICommandRepository uiCommandRepository,
			UICommandRepository<InventProductItemViewModel> commandRepositoryInventProduct,
            ModalWindowLauncher modalWindowLauncher,
			IPropertyStrRepository propertyStrRepository ,
			IDBSettings dbSettings
            )
            : base(contextCbiRepository)
        {
            this._modalWindowLauncher = modalWindowLauncher;
			this._uiCommandRepository = uiCommandRepository;
			this._editorComboViewModel = editorComboViewModel;
			this._navigationRepository = navigationRepository;
			this._regionManager = regionManager;
			this._reportButtonViewModel = reportButtonViewModel;
            this._eventAggregator = eventAggregator;
            this._userSettingsManager = userSettingsManager;
            this._inventProductRepository = inventProductRepository;
			this._iturRepository = iturRepository;
			
			this._propertyStrRepository = propertyStrRepository;
			this._dbSettings = dbSettings;

			this._commandRepositoryInventProduct = commandRepositoryInventProduct;

			this._cloneCommand = _uiCommandRepository.Build(enUICommand.Clone, this.CloneCommandExecuted, CloneCommandCanExecute);
			this._copyCommand = _uiCommandRepository.Build(enUICommand.Copy, this.CopyCommandExecuted, this.CopyCommandCanExecute);
            this._openDetailsCommand = new DelegateCommand<InventProductItemViewModel>(OpenDetailsCommandExecuted);
            this._editCommand = uiCommandRepository.Build(enUICommand.Edit, EditCommandExecuted, EditCommandCanExecute);
            this._deleteCommand = uiCommandRepository.Build(enUICommand.Delete, DeleteCommandExecuted, DeleteCommandCanExecute);
			
			this._deleteNoneCatalogCommand = uiCommandRepository.Build(enUICommand.DeleteNoneCatalog, DeleteNoneCatalogCommandExecuted, DeleteNoneCatalogCommandCanExecute);

            this._resetErrorInventProductSelectedCommand = uiCommandRepository.Build(enUICommand.ResetBit,
                ResetErrorInventProductSelectedExecuted,
                ResetErrorInventProductSelectedCommandCanExecute);

            this._items = new ObservableCollection<InventProductItemViewModel>();
			this._detailItems = new ObservableCollection<InventProductPropertyStrItemViewModel>();
																
            _importCommand = _uiCommandRepository.Build(enUICommand.Import, ImportCommandExecuted);

            _editGridCommand = new DelegateCommand<InventProductItemViewModel>(EditGridCommandExecuted);
            _cancelGridCommand = new DelegateCommand<InventProductItemViewModel>(CancelGridCommandExecuted);
            _commitGridCommand = new DelegateCommand<InventProductItemViewModel>(CommitGridCommandExecuted);

			_editDetailGridCommand = new DelegateCommand<InventProductPropertyStrItemViewModel>(EditDetailGridCommandExecuted);
			_cancelDetailGridCommand = new DelegateCommand<InventProductPropertyStrItemViewModel>(CancelDetailGridCommandExecuted);
			_commitDetailGridCommand = new DelegateCommand<InventProductPropertyStrItemViewModel>(CommitDetailGridCommandExecuted);

			
            _resetQuantityCommand = uiCommandRepository.Build(enUICommand.Undo, ResetQuantityCommandExecuted);


			this._acceptQuantityCommand = _commandRepositoryInventProduct.Build(enUICommand.Accept, AcceptQuantityEditCommandExecuted);
			this._clearMarkCommand = _commandRepositoryInventProduct.Build(enUICommand.Clear, ClearMarkCommandExecuted);
			
			this._accept1Command = _commandRepositoryInventProduct.Build(enUICommand.Accept, Accept1CommandExecuted, Accept1CommandCanExecute);
			this._accept2Command = _commandRepositoryInventProduct.Build(enUICommand.Accept, Accept2CommandExecuted);//, Accept2CommandCanExecute);


			this._isDataGridProperyVisibility = false;

			this._timer = new DispatcherTimer();
			this._timer.Interval = TimeSpan.FromMilliseconds(this._userSettingsManager.DelayGet());
			this._timer.Tick += Timer_Tick;
        }

        public DataGrid DataGrid { get; set; }
        public string SearchFilterRegionKey { get; set; }

        public ObservableCollection<InventProductItemViewModel> Items
        {
            get { return _items; }
        }


		public void SelectedItemSet(InventProductItemViewModel item)
		{
			//this.SelectedItem = item;

			//this._deleteCommand.RaiseCanExecuteChanged();
			//this._editCommand.RaiseCanExecuteChanged();
			//RaisePropertyChanged(() => this.IsDataGridProperyVisibility);

			this._timer.Stop();
			this._timer.Start();
		}

		public void SelectedPropertySet(InventProductPropertyStrItemViewModel item)
		{
			this._timer.Stop();
			this._timer.Start();
		}

	

		void Timer_Tick(object sender, EventArgs e)
		{
			if (IsDataGridProperyVisibility == true)
			{
				using (new CursorWait())
				{
					this._timer.Stop();
					this.BuildDetailsItems();
				}
			}
		}

		public ObservableCollection<InventProductPropertyStrItemViewModel> DetailItems
		{
			get { return this._detailItems; }
		}

		public InventProductPropertyStrItemViewModel DetailSelectedItem
		{
			get { return this._detailSelectedItem; }
			set { this._detailSelectedItem = value; }
		}


		private string GetPropertyStrName(DomainObjectTypeEnum domainPropertyStr, string propertyStr1Code)
		{
			string name = "";
			if (string.IsNullOrWhiteSpace(propertyStr1Code) == true) return name;
			if (string.IsNullOrWhiteSpace(domainPropertyStr.ToString()) == true) return name;
			if (this._dictionaryObjectPropertyStr == null) return name;

			string key = domainPropertyStr.ToString() + "|" + propertyStr1Code;
			if (this._dictionaryObjectPropertyStr.ContainsKey(key) == true)
			{
				if (string.IsNullOrWhiteSpace(this._dictionaryObjectPropertyStr[key].Name) == false)
				{
					name = this._dictionaryObjectPropertyStr[key].Name;
				}
			}
			return name;
		}

		private PropertyStr GetPropertyStrObject(DomainObjectTypeEnum domainPropertyStr, string propertyStrCode)
		{
			PropertyStr retPropertyStr = new PropertyStr();
			retPropertyStr.DomainObject = domainPropertyStr.ToString();
			if (string.IsNullOrWhiteSpace(propertyStrCode) == true) return retPropertyStr;
			retPropertyStr.PropertyStrCode = propertyStrCode;
			if (string.IsNullOrWhiteSpace(domainPropertyStr.ToString()) == true) return retPropertyStr;
			if (this._dictionaryObjectPropertyStr == null) return retPropertyStr;

			string key = domainPropertyStr.ToString() + "|" + propertyStrCode;
			if (this._dictionaryObjectPropertyStr.ContainsKey(key) == true)
			{
				if (this._dictionaryObjectPropertyStr[key] != null)
					retPropertyStr = this._dictionaryObjectPropertyStr[key];
			}
			return retPropertyStr;
		}

		

		private string GetPropertyStrTitle(DomainObjectTypeEnum domainPropertyStr, Dictionary<string, PropertyLink> detailPropertyLinkDictionary)
		{
			//Dictionary<string, PropertyLink> detailPropertyLinkDictionary = _editorComboViewModel.EditorTemplateCurrent.DetailPropertyLinkDictionary;
			string title = domainPropertyStr.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(title) == true)
			{
				var detailPropertyLink = detailPropertyLinkDictionary[title];
				title = detailPropertyLink.DefaultEditorLabel;
				if (string.IsNullOrWhiteSpace(detailPropertyLink.CodeLocalizationEditorLabel) == false)
				{
					title = UtilsMisc.LocalizationFromLocalizationKey(detailPropertyLink.CodeLocalizationEditorLabel);
				}
			}
			return title;
		}

		private void BuildDetailsItems()
		{
			this._detailItems.Clear();

			if (this._selectedItem == null)
				return;

			InventProduct master = _selectedItem.InventProduct;
			Dictionary<string, PropertyLink> detailPropertyLinkDictionary = _editorComboViewModel.EditorTemplateCurrent.DetailPropertyLinkDictionary;
		  	string propertyStrCode = "";
			string propertyStrTitle = "";
			string propertyStrName = "";
			PropertyStr propertyStrObject = new PropertyStr();
			InventProductPropertyStrItemViewModel viewModel;
			//================ PropertyStr1 ===========
			string propertyN = DomainObjectTypeEnum.PropertyStr1.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr1;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr1, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr1, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr1, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr1.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr2 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr2.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr2;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr2, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr2, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr2, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr2.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr3 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr3.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr3;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr3, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr3, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr3, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr3.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr4 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr4.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr4;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr4, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr4, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr4, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr4.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr5 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr5.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr5;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr5, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr5, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr5, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr5.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr6 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr6.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr6;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr6, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr6, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr6, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr6.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr7 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr7.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr7;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr7, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr7, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr7, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr7.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr8 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr8.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr8;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr8, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr8, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr8, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr8.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr9 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr9.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr9;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr9, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr9, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr9, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr9.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr10 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr10.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr10;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr10, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr10, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr10, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr10.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr11 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr11.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr11;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr11, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr11, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr11, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr11.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr12 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr12.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr12;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr12, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr12, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr12, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr12.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr13 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr13.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr13;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr13, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr13, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr13, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr13.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr14 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr14.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr14;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr14, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr14, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr14, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr14.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr15 ===========

			propertyN = DomainObjectTypeEnum.PropertyStr15.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr15;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr15, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr15, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr15, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr15.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr16 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr16.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr16;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr16, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr16, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr16, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr16.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr17 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr17.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr17;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr17, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr17, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr17, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr17.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr18 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr18.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr18;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr18, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr18, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr18, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr18.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr19 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr19.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr19;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr19, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr19, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr19, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr19.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr20 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr20.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr20;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr20, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr20, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr20, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr20.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
	
		}

		public bool IsDataGridProperyVisibility
		{
			get 
			{
				return this._isDataGridProperyVisibility; 
			}
			set
			{
				this._isDataGridProperyVisibility = value;
				RaisePropertyChanged(() => this.IsDataGridProperyVisibility);
			}
		}

        public InventProductItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                this._selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                this._editCommand.RaiseCanExecuteChanged();
                this._deleteCommand.RaiseCanExecuteChanged();
                this._cloneCommand.RaiseCanExecuteChanged();

                this._resetErrorInventProductSelectedCommand.RaiseCanExecuteChanged();
            }
        }


		

        public int ItemsTotal
        {
            get { return this._itemsTotal; }
            set
            {
                this._itemsTotal = value;
                this.RaisePropertyChanged(() => ItemsTotal);
            }
        }

        public int PageCurrent
        {
            get { return this._pageCurrent; }
            set
            {
                this._pageCurrent = value;
                this.RaisePropertyChanged(() => PageCurrent);

                using (new CursorWait())
                {
                    Build();
                }
            }
        }

        public int PageSize
        {
            get { return this._pageSize; }
            set
            {
                this._pageSize = value;
                this.RaisePropertyChanged(() => PageSize);
            }
        }

        public DelegateCommand<InventProductItemViewModel> OpenDetailsCommand
        {
            get { return _openDetailsCommand; }
        }

        public DelegateCommand ResetErrorInventProductSelectedCommand
        {
            get { return _resetErrorInventProductSelectedCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }


		public DelegateCommand DeleteNoneCatalogCommand
        {
			get { return _deleteNoneCatalogCommand; }
        }

        public ReportButtonViewModel ReportButtonViewModel
        {
            get { return _reportButtonViewModel; }
        }

        public EditorTemplateComboViewModel EditorComboViewModel
        {
            get { return _editorComboViewModel; }
        }


		public DelegateCommand<InventProductItemViewModel> Accept1Command
		{
			get { return this._accept1Command; }
		}


		public DelegateCommand<InventProductItemViewModel> AcceptQuantityCommand
		{
			get { return this._acceptQuantityCommand; }
		}



		public DelegateCommand<InventProductItemViewModel> ClearMarkCommand
		{
			get { return this._clearMarkCommand; }
		}

		//private bool Accept1CommandCanExecute(InventProductItemViewModel viewModel)
		//{
		//	if (viewModel == null) return false;
		//	InventProduct inventProduct = viewModel.InventProduct;
		//	if (inventProduct.IPValueInt1 == inventProduct.IPValueInt2)
		//	{
		//		return false;
		//	}
		//	return true;
		//}

		//private bool Accept2CommandCanExecute(InventProductItemViewModel viewModel)
		//{
		//	if (viewModel == null) return false;
		//	InventProduct inventProduct = viewModel.InventProduct;
		//	if (inventProduct.IPValueInt1 == inventProduct.IPValueInt2)
		//	{
		//		return false;
		//	}
		//	return true;
		//}

		public DelegateCommand<InventProductItemViewModel> Accept2Command
		{
			get { return this._accept2Command; }
		}

		//IPValueInt1 - In1
		//IPValueInt2 - In2
		//IPValueInt3 - Difference
		//IPValueInt4 - Result

		//IPValueStr1 - File1
		//IPValueStr2 - File2
		//IPValueStr3 - Mark
		private void AcceptQuantityEditCommandExecuted(InventProductItemViewModel viewModel)
		{
			InventProduct inventProduct = viewModel.InventProduct;
			if (inventProduct.IPValueInt1 == inventProduct.IPValueInt2)
			{
				inventProduct.IPValueStr3 = "";
			}
			else
			{
				//inventProduct.IPValueInt4 = inventProduct.IPValueInt1;
				//inventProduct.QuantityEdit = inventProduct.IPValueInt1;
				inventProduct.QuantityOriginal = inventProduct.QuantityEdit;
				inventProduct.IPValueStr3 = "AcceptQE";
				this._inventProductRepository.Update(inventProduct, GetDbPath);
				viewModel.InventProductSet(inventProduct);
			}

		}


		private void ClearMarkCommandExecuted(InventProductItemViewModel viewModel)
		{
			InventProduct inventProduct = viewModel.InventProduct;

			inventProduct.QuantityOriginal = 0.0;
			inventProduct.QuantityEdit = 0.0;
			inventProduct.IPValueStr3 = "";
			this._inventProductRepository.Update(inventProduct, GetDbPath);
			viewModel.InventProductSet(inventProduct);
		

		}

		//IPValueInt1 - In1
		//IPValueInt2 - In2
		//IPValueInt3 - Difference
		 //IPValueInt4 - Result

		//IPValueStr1 - File1
		//IPValueStr2 - File2
		//IPValueStr3 - Mark
		private void Accept1CommandExecuted(InventProductItemViewModel viewModel)
		{
			InventProduct inventProduct = viewModel.InventProduct;
			if (inventProduct.IPValueInt1 == inventProduct.IPValueInt2)
			{

				inventProduct.IPValueStr3 = "";
			}
			else
			{
				inventProduct.IPValueInt4 = inventProduct.IPValueInt1;			  //Result
				inventProduct.QuantityEdit = inventProduct.IPValueInt1;
				inventProduct.QuantityOriginal = inventProduct.IPValueInt1;
				inventProduct.IPValueStr3 = "Accept1";
				this._inventProductRepository.Update(inventProduct, GetDbPath);
				viewModel.InventProductSet(inventProduct);
			}

		}

		private void Accept2CommandExecuted(InventProductItemViewModel viewModel)
		{
			InventProduct inventProduct = viewModel.InventProduct;
			if (inventProduct.IPValueInt1 == inventProduct.IPValueInt2)
			{

				inventProduct.IPValueStr3 = "";
			}
			else
			{
				inventProduct.IPValueInt4 = inventProduct.IPValueInt2;				     //Result
				inventProduct.QuantityEdit = inventProduct.IPValueInt2;
				inventProduct.QuantityOriginal = inventProduct.IPValueInt2;
				inventProduct.IPValueStr3 = "Accept2";
				this._inventProductRepository.Update(inventProduct, GetDbPath);
				viewModel.InventProductSet(inventProduct);
			}

		}


		private bool Accept1CommandCanExecute(InventProductItemViewModel viewModel)
		{
			return true;
		}

        public DelegateCommand CloneCommand
        {
            get { return _cloneCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			string templateCode = "";
			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AdapterName))
			{
				templateCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AdapterName).Value;
			}

			InitEditorTemplateList(templateCode);
            InitReportButton(navigationContext);
            InitSearchFilter(navigationContext, templateCode);
			//this._dictionaryPropertyStr = this._propertyStrRepository.GetNamePropertyStrDictionaryByDomainAndCodeAsKey(base.GetDbPath);
			this._dictionaryObjectPropertyStr = this._propertyStrRepository.GetPropertyStrDictionaryByDomainAndCodeAsKey(base.GetDbPath);

            this._eventAggregator.GetEvent<InventProductAddedEvent>().Subscribe(InventProductAdded);
            this._eventAggregator.GetEvent<InventProductEditedEvent>().Subscribe(InventoProductEdited);

            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionInventProductsGet();

			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (navigationContext.NavigationService.Journal.CurrentEntry.Uri.OriginalString.StartsWith(Common.ViewNames.InventorDashboardFullView))
                return false;
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._reportButtonViewModel.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<InventProductAddedEvent>().Unsubscribe(this.InventProductAdded);
            this._eventAggregator.GetEvent<InventProductEditedEvent>().Unsubscribe(this.InventoProductEdited);

            ClearInventProduct();
        }

        private void InitSearchFilter(NavigationContext navigationContext, string templateCode)
        {
            _searchFilterViewModel = Utils.GetViewModelFromRegion<SearchFilterViewModel>(Common.RegionNames.InventProductSearchFilter + SearchFilterRegionKey, this._regionManager);


			_searchFilterViewModel.FilterAction = Build;

			_searchFilterViewModel.PopupExtSearch.NavigationData = new InventProductFilterData();
            _searchFilterViewModel.PopupExtSearch.Region = Common.RegionNames.PopupSearchInventProduct;
            _searchFilterViewModel.PopupExtSearch.ViewModel = this;
            _searchFilterViewModel.PopupExtSearch.Init();

            _searchFilterViewModel.PopupExtFilter.Region = Common.RegionNames.PopupFilterInventProduct;
            _searchFilterViewModel.PopupExtFilter.ViewModel = this;
            _searchFilterViewModel.PopupExtFilter.View = Common.ViewNames.FilterView;
            _searchFilterViewModel.PopupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, _searchFilterViewModel.Filter, Common.NavigationObjects.Filter);
		
			_searchFilterViewModel.PopupExtFilter.Init();

			if (templateCode == "Compare")
			{
				InventProductFilterData filterData = new InventProductFilterData();
				this._userSettingsManager.InventProductPropertyFilterSelectedNumberItemSet("IPValueInt3");

				filterData.PropertyNumberName = "IPValueInt3";
				filterData.PropertyNumber = "0";
				filterData.PropertyNumberIsAbsolute = true;
				filterData.PropertyNumberEquality = Common.Constants.ComboValues.Equality.Greater;
				filterData.SortField = "ERPIturCode";
				filterData.SortDirection = enSortDirection.ASC;
				_searchFilterViewModel.Filter = filterData;
			}
			else
			{
				_searchFilterViewModel.Filter = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, true) as InventProductFilterData;
				if (_searchFilterViewModel.Filter == null)
					_searchFilterViewModel.Filter = new InventProductFilterData();
			}

		
		}

        private void InitEditorTemplateList(string templateCode)
        {
            _editorComboViewModel.DataGrid = this.DataGrid;
            _editorComboViewModel.State = base.State;
            _editorComboViewModel.ViewName = Configuration.Constants.DynamicView.InventProductView_InventProduct;
            _editorComboViewModel.RebuildAction = Build;
			_editorComboViewModel.Init(templateCode);
			
        }

        private void InitReportButton(NavigationContext navigationContext)
        {
            this._reportButtonViewModel.OnNavigatedTo(navigationContext);

            this._reportButtonViewModel.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = SelectParamsBuild();
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.PDA);

        }

        private SelectParams SelectParamsBuild()
        {
            SelectParams result = new SelectParams()
            {
                IsEnablePaging = true,
                CountOfRecordsOnPage = this._pageSize,
                CurrentPage = this._pageCurrent,
            };

            InventProductFilterData inventProductFilter = _searchFilterViewModel.Filter as InventProductFilterData;
            if (inventProductFilter != null)
            {
                inventProductFilter.ApplyToSelectParams(result);
            }

            return result;
        }

        private void Build()
        {
			//string currentCustomerCode = "000";
			//if (base.State.CurrentCustomer != null) currentCustomerCode = base.State.CurrentCustomer.Code;

			//string importFolderPath = _dbSettings.ImportFolderPath();
			//string path = Path.Combine(importFolderPath, "Customer", currentCustomerCode);

			//string importPath = base.ContextCBIRepository.GetImportFolderPath(domainObject);

			string currentInventorCode = "000";

			string importPDAFolder = this._dbSettings.ImportFolderPath();//inData в папке каждого инвентора
			string path =  Path.Combine(importPDAFolder, "Inventor", currentInventorCode, "inData", "photo");
			if (base.State.CurrentInventor != null)
			{
				currentInventorCode = base.State.CurrentInventor.Code;
				//string targetPhotoFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData\photo";
				path = Path.Combine(importPDAFolder, "Inventor", currentInventorCode, "inData", "photo");
			}
			else if (base.State.CurrentBranch != null)
			{
				string currentBranchCode = base.State.CurrentBranch.Code;
				//string targetPhotoFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData\photo";
				path = Path.Combine(importPDAFolder, "Branch", currentBranchCode, "inData", "photo");
			}
			else if (base.State.CurrentCustomer != null)
			{
				string currentCustomerCode = base.State.CurrentCustomer.Code;
				//string targetPhotoFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData\photo";
				path = Path.Combine(importPDAFolder, "Customer", currentCustomerCode, "inData", "photo");
			}

			
			string sourseFolder = Path.GetFullPath(path);

            Utils.RunOnUI(ClearInventProduct);
			string photoPropertyName = this._userSettingsManager.InventProductPropertyPhotoSelectedItemGet();

			bool mark = this._userSettingsManager.ShowMarkGet();
			string markPropertyName = this._userSettingsManager.InventProductPropertyMarkSelectedItemGet();
			bool propertyIsEmpty = this._userSettingsManager.PropertyIsEmptyGet();
			var markColor = this._userSettingsManager.InventProductMarkColorGet();
			var markColorString = ColorParser.ColorToString(markColor);
			Iturs iturs = this._iturRepository.GetIturs(base.GetDbPath);
			//Dictionary<string, Itur> iturDictionary = iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);

			if (_editorComboViewModel.EditorTemplateCurrent.DetailPart == 1)
			{
				IsDataGridProperyVisibility = true;
			}
			else
			{
				IsDataGridProperyVisibility = false;
			}
			RaisePropertyChanged(() => this.IsDataGridProperyVisibility);

            SelectParams sp = null;
            try
            {
                sp = SelectParamsBuild();

                InventProducts inventProducts = this._inventProductRepository.GetInventProducts(sp, base.GetDbPath);

                Utils.RunOnUI(() =>
                                  {
                                      this._items.Clear();
                                      ItemsTotal = (int)inventProducts.TotalCount;

                                      foreach (InventProduct inventProduct in inventProducts)
                                      {
										  //Itur itur = iturDictionary[inventProduct.IturCode];
										  //if (itur != null) inventProduct.ERPIturCode = itur.ERPIturCode;
										  InventProductItemViewModel viewModel = new InventProductItemViewModel(inventProduct, _editorComboViewModel,
								  photoPropertyName, sourseFolder, mark, markPropertyName, propertyIsEmpty, markColorString);
	
										  //InventProductItemViewModel viewModel = new InventProductItemViewModel(inventProduct, _editorComboViewModel, 
										  //	photoPropertyName, sourseFolder);
                                          this._items.Add(viewModel);
                                      }

                                      _editorComboViewModel.DynamicRepository.FillObjectListWithDynamicProperties(this._items, o => ((InventProductItemViewModel)o).InventProduct);

                                      if ((inventProducts.TotalCount > 0) && (inventProducts.Count == 0)) //do not show empty space - move on previous page           
                                      {
                                          this.PageCurrent = this._pageCurrent - 1;
                                      }
                                  });
				if (inventProducts.Count > 0)
				{
					InventProductItemViewModel selectedItem = this.Items.FirstOrDefault();
					this.SelectedItem = selectedItem;
				}
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
                _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                if (sp != null)
                    _logger.Error("SelectParams: {0}", sp.ToString());
                throw;
            }
        }

        private void ClearInventProduct()
        {
            this._items.Clear();
        }

        private void OpenDetailsCommandExecuted(InventProductItemViewModel item)
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            SelectParams selectParams = new SelectParams();
            selectParams.IsEnablePaging = false;
            selectParams.FilterParams.Add("IturCode", new FilterParam() { Operator = FilterOperator.Contains, Value = item.InventProduct.IturCode });

            InventProductDetailsData navigationData = new InventProductDetailsData();
            navigationData.IturSelectParams = selectParams;
            navigationData.IturCode = item.InventProduct.IturCode;
            navigationData.DocumentCode = item.InventProduct.DocumentCode;
            navigationData.InventProductId = item.InventProduct.ID;
            //navigationData.SearchItem = this._searchItem;
            //navigationData.SearchExpression = this._searchExpression;

            UtilsConvert.AddObjectToQuery(query, this._navigationRepository, navigationData, NavigationObjects.InventProductDetails);

            UtilsNavigate.InventProductDetailsOpen(this._regionManager, query);
        }

        private void InventProductAdded(InventProduct inventProduct)
        {
            RefreshStatus();
            this.SelectedItem = this._items.FirstOrDefault(r => r.InventProduct.ID == inventProduct.ID);
        }

        private void InventoProductEdited(InventProduct inventProduct)
        {
            RefreshStatus();
            this.SelectedItem = this._items.FirstOrDefault(r => r.InventProduct.ID == inventProduct.ID);
        }

        private bool ResetErrorInventProductSelectedCommandCanExecute()
        {
            return this._selectedItem != null;
        }

        private void ResetErrorInventProductSelectedExecuted()
        {
            if (this._selectedItem == null) return;

            InventProduct iv = _selectedItem.InventProduct;
            iv.StatusInventProductBit = 0;
            this._inventProductRepository.Update(iv, GetDbPath);
            _selectedItem.InventProductSet(iv);

            RefreshStatus();
        }

        private void DeleteInventProduct(InventProduct inventProduct)
        {
            string message = String.Format(Localization.Resources.Msg_Delete_Invent_Product, inventProduct.ProductName, Environment.NewLine, inventProduct.Makat);

            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this._inventProductRepository.Delete(inventProduct, base.GetDbPath);
                RefreshStatus();
            }
        }

		private void DeleteInventProductWithoutCatalog()
		{
			string message = String.Format(Localization.Resources.Msg_Delete_Invent_Product_NotExistInCatalog);

			MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				this._inventProductRepository.DeleteAllNotExistInCatalog( base.GetDbPath);
				RefreshStatus();
			}

		}

        private void EditInventProduct(InventProduct inventProduct)
        {
            this._eventAggregator.GetEvent<InventProductEditEvent>().Publish(
               new InventProductEditEventPayLoad()
               {
                   InventProduct = inventProduct,
                   Context = base.Context,
                   DbContext = base.CBIDbContext,
               });
        }

        private void RefreshStatus()
        {
            using (new CursorWait())
            {
                InventProductItemViewModel selectedIp = this._selectedItem;
                Build();
                if (selectedIp != null)
                    SelectedItem = this._items.FirstOrDefault(r => r.InventProduct.ID == selectedIp.InventProduct.ID);
            }
        }

        private bool EditCommandCanExecute()
        {
            return this._selectedItem != null;
        }

        private void EditCommandExecuted()
        {
            EditInventProduct(this._selectedItem.InventProduct);
        }

        private bool DeleteCommandCanExecute()
        {
            return this._selectedItem != null;
        }


		private bool DeleteNoneCatalogCommandCanExecute()
        {
			return true;
        }

        private void DeleteCommandExecuted()
        {
            DeleteInventProduct(this._selectedItem.InventProduct);
        }


		private void DeleteNoneCatalogCommandExecuted()
        {
			DeleteInventProductWithoutCatalog();
        }
		

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.PDA);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);

            SelectParams sp = SelectParamsBuild();

            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);

            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        public bool KeepAlive { get { return false; } }

        public DelegateCommand ImportCommand
        {
            get { return _importCommand; }
        }

        public DelegateCommand<InventProductItemViewModel> EditGridCommand
        {
            get { return _editGridCommand; }
        }

        public DelegateCommand<InventProductItemViewModel> CancelGridCommand
        {
            get { return _cancelGridCommand; }
        }

        public DelegateCommand<InventProductItemViewModel> CommitGridCommand
        {
            get { return _commitGridCommand; }
        }


		public DelegateCommand<InventProductPropertyStrItemViewModel> EditDetailGridCommand
		{
			get { return _editDetailGridCommand; }
		}

		public DelegateCommand<InventProductPropertyStrItemViewModel> CancelDetailGridCommand
		{
			get { return _cancelDetailGridCommand; }
		}

		public DelegateCommand<InventProductPropertyStrItemViewModel> CommitDetailGridCommand
		{
			get { return _commitDetailGridCommand; }
		}

        public DelegateCommand ResetQuantityCommand
        {
            get { return _resetQuantityCommand; }
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.ImportFromPdaOpen(this._regionManager, uriQuery);
        }

        private bool _isEditing;
		private bool _isDetailEditing;
		

        private void CommitGridCommandExecuted(InventProductItemViewModel item)
        {
            if (_isEditing)
            {
                SaveInventProductViewModel(item);

                item.UpdateUIAfterDbSave();
                _isEditing = false;
            }
        }

        private void CancelGridCommandExecuted(InventProductItemViewModel item)
        {
            _isEditing = false;
        }

        private void EditGridCommandExecuted(InventProductItemViewModel item)
        {
            _isEditing = true;
        }
		private void SaveInventProductViewModel(InventProductItemViewModel viewModel)
		{
			using (new CursorWait())
			{
				this._inventProductRepository.Update(viewModel.InventProduct, base.GetDbPath);
			}
		}

//	
		private void CommitDetailGridCommandExecuted(InventProductPropertyStrItemViewModel item)
        {
			if (_isDetailEditing)
            {
				item.PropertyStrName = "";
				DomainObjectTypeEnum domain = DomainObjectTypeEnum.Unknown;
				bool ret = Enum.TryParse<DomainObjectTypeEnum>(item.DomainObject, out domain);
				if (ret == true)
				{
					item.PropertyStrName = this.GetPropertyStrName(domain, item.PropertyStrCode);
				}
				SavePropertyStrViewModel(item);

				item.UpdateUIAfterDbSave();
				_isDetailEditing = false;
            }
        }

		private void CancelDetailGridCommandExecuted(InventProductPropertyStrItemViewModel item)
        {
			_isDetailEditing = false;
        }

		private void EditDetailGridCommandExecuted(InventProductPropertyStrItemViewModel item)
        {
			_isDetailEditing = true;
        }

  
		private void SavePropertyStrViewModel(InventProductPropertyStrItemViewModel viewModel)
		{
			using (new CursorWait())
			{
				viewModel.InventProductUpdate();
				this._inventProductRepository.Update(viewModel.InventProduct, base.GetDbPath);
			}
		}

        private void ResetQuantityCommandExecuted()
        {
            using (new CursorWait())
            {
                InventProduct ip = _selectedItem.InventProduct;
                ip.QuantityEdit = ip.QuantityOriginal;
                _inventProductRepository.Update(ip, base.GetDbPath);

                _selectedItem.QuantityEdit = ip.QuantityEdit;
            }
        }

		private bool CopyCommandCanExecute()
		{
			return _selectedItem != null;
		}

		public DelegateCommand CopyCommand
		{
			get { return this._copyCommand; }
		}

		private void CopyCommandExecuted()
		{
			InventProduct ip = this._selectedItem.InventProduct;
			if (ip == null) return;
			string copy = "Makat = " + ip.Makat + ", Barcode = " + ip.Barcode + ", SerialNumber = " +  ip.SerialNumber + ", IturCode = " + ip.IturCode + ", ERPIturCode = " + ip.ERPIturCode;
			Clipboard.SetText(copy);
		}

        private bool CloneCommandCanExecute()
        {
            return _selectedItem != null;
        }


        private void CloneCommandExecuted()
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            query.Add(Common.NavigationSettings.InventProductId, _selectedItem.InventProduct.ID.ToString());
            Utils.AddContextToDictionary(query, base.Context);
            Utils.AddDbContextToDictionary(query, base.CBIDbContext);

            object result = _modalWindowLauncher.StartModalWindow(
                Common.ViewNames.InventProductCloneView, 
                WindowTitles.InventProductClone, 
                330, 220, 
                ResizeMode.NoResize, query);

            if (result != null)
            {
                this.Build();
            }
        }
    }
}