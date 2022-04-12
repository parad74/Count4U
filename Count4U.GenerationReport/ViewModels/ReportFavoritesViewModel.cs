using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model.Interface.Audit;
using Count4U.Report.ViewModels.Script;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Count4U.Model.Interface;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Common.Extensions;
using Count4U.Model.Interface.Main;
using Count4U.Report.ViewModels.ReportButton;

namespace Count4U.Report.ViewModels
{
    public class ReportFavoritesViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IReportRepository _reportRepository;
        private readonly ICustomerReportRepository _customerReport;
        
        private readonly IEventAggregator _eventAggregator;
        private readonly IServiceLocator _serviceLocator;
        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly UICommandRepository _commandRepository;
        private readonly ReportButtonViewModel _reportButtonViewModel;

        private readonly DelegateCommand<ReportItemViewModel> _editGridCommand;
        private readonly DelegateCommand<ReportItemViewModel> _cancelGridCommand;
        private readonly DelegateCommand<ReportItemViewModel> _commitGridCommand;

        private bool _isMain;
        private bool _isCurrentUser;

        private readonly ObservableCollection<ReportItemViewModel> _items;
        private ReportItemViewModel _selectedItem;

        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _scriptOpenCommand;
        private readonly DelegateCommand _scriptSaveCommand;
        private readonly UICommand _runScriptCommand;
        private readonly UICommand _clearScriptCommand;
        private readonly DelegateCommand _upCommand;
        private readonly DelegateCommand _downCommand;

        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

        private bool _isBusy;

        public ReportFavoritesViewModel(
            IContextCBIRepository contextCbiRepository,
            IReportRepository reportRepository,
            ICustomerReportRepository customerReport  ,
            IEventAggregator eventAggregator,
            IServiceLocator serviceLocator,
             ReportButtonViewModel reportButtonViewModel,
            IGenerateReportRepository generateReportRepository,
            IUserSettingsManager userSettingsManager,
            UICommandRepository commandRepository
            )
            : base(contextCbiRepository)
        {
            this._commandRepository = commandRepository;
            this._customerReport = customerReport;
            this._userSettingsManager = userSettingsManager;
            this._generateReportRepository = generateReportRepository;
            this._eventAggregator = eventAggregator;
            this._reportRepository = reportRepository;
            this._serviceLocator = serviceLocator;
            this._reportButtonViewModel = reportButtonViewModel;

            this._isMain = true;

            this._items = new ObservableCollection<ReportItemViewModel>();

            this._addCommand = _commandRepository.Build(enUICommand.Add, AddCommandExecuted, AddCommandCanExecute);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted, EditCommandCanExecte);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, DeleteCommandExecuted, DeleteCommandCanExecute);
            this._scriptOpenCommand = _commandRepository.Build(enUICommand.OpenScript, ScriptOpenCommandExecuted, ScriptOpenCommandCanExecute);
            this._scriptSaveCommand = _commandRepository.Build(enUICommand.SaveScript, ScriptSaveCommandExecuted, ScriptSaveCommandCanExecute);
            this._upCommand = _commandRepository.Build(enUICommand.Up, UpCommandExecuted, UpCommandCanExecute);
            this._downCommand = _commandRepository.Build(enUICommand.Down, DownCommandExecuted, DownCommandCanExecute);
            this._clearScriptCommand = _commandRepository.Build(enUICommand.ClearTag, ClearScriptCommandExecuted, ClearScriptCommandCanExecute);
            this._runScriptCommand = _commandRepository.Build(enUICommand.Restore, RunScriptCommandExecuted, RunScriptCommandCanExecute);
            _runScriptCommand.Title = Localization.Resources.View_ReportFavorites_btnRestoreLinks;

            _editGridCommand = new DelegateCommand<ReportItemViewModel>(EditGridCommandExecuted);
            _cancelGridCommand = new DelegateCommand<ReportItemViewModel>(CancelGridCommandExecuted);
            _commitGridCommand = new DelegateCommand<ReportItemViewModel>(CommitGridCommandExecuted);

            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._isSaveTags = true;
        }

        public bool _isSaveTags;
        public bool IsSaveTags
        {
            get { return _isSaveTags; }
            set { 
                _isSaveTags = value;
                RaisePropertyChanged(() => IsSaveTags);
                this._clearScriptCommand.RaiseCanExecuteChanged();
                //ClearScriptCommandCanExecute
            }
        }

        public DelegateCommand<ReportItemViewModel> EditGridCommand
        {
            get { return _editGridCommand; }
        }

        public DelegateCommand<ReportItemViewModel> CancelGridCommand
        {
            get { return _cancelGridCommand; }
        }

        public DelegateCommand<ReportItemViewModel> CommitGridCommand
        {
            get { return _commitGridCommand; }
        }

        private bool _isEditing;
        private void CommitGridCommandExecuted(ReportItemViewModel item)
        {
            if (_isEditing)
            {
                SaveInventProductViewModel(item);
                _isEditing = false;
            }
        }
        private void SaveInventProductViewModel(ReportItemViewModel viewModel)
        {
            using (new CursorWait())
            {
                this._reportRepository.Update(_selectedItem.Report, this.GetAllowedReportTemplate());
            }
        }
        private void CancelGridCommandExecuted(ReportItemViewModel item)
        {
            _isEditing = false;
        }

        private void EditGridCommandExecuted(ReportItemViewModel item)
        {
            _isEditing = true;
        }

        public bool IsMain
        {
            get { return _isMain; }
            set
            {
                _isMain = value;
                RaisePropertyChanged(() => IsMain);

                if (_isMain)
                {
                    _isCurrentUser = false;
                    RaisePropertyChanged(() => IsCurrentUser);
                }

                BuildItems();
            }
        }

        public bool IsCurrentUser
        {
            get { return _isCurrentUser; }
            set
            {
                _isCurrentUser = value;
                RaisePropertyChanged(() => IsMain);

                if (_isCurrentUser)
                {
                    _isMain = false;
                    RaisePropertyChanged(() => IsMain);
                }

                BuildItems();
            }
        }

        public ObservableCollection<ReportItemViewModel> Items
        {
            get { return _items; }
        }

        public ReportItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                _editCommand.RaiseCanExecuteChanged();
                _deleteCommand.RaiseCanExecuteChanged();
                _upCommand.RaiseCanExecuteChanged();
                _downCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AddCommand
        {
            get { return _addCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }


        public DelegateCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return _yesNoRequest; }
        }

        public DelegateCommand ScriptOpenCommand
        {
            get { return _scriptOpenCommand; }
        }

        public DelegateCommand ScriptSaveCommand
        {
            get { return _scriptSaveCommand; }
        }

        public DelegateCommand RunScriptCommand
        {
            get { return _runScriptCommand; }
        }

        public DelegateCommand ClearScriptCommand
        {
            get { return _clearScriptCommand; }
        }

        public DelegateCommand UpCommand
        {
            get { return _upCommand; }
        }

        public DelegateCommand DownCommand
        {
            get { return _downCommand; }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);

                Mouse.OverrideCursor = _isBusy ? Cursors.Wait : null;

                _addCommand.RaiseCanExecuteChanged();
                _editCommand.RaiseCanExecuteChanged();
                _deleteCommand.RaiseCanExecuteChanged();
                _scriptOpenCommand.RaiseCanExecuteChanged();
                _scriptSaveCommand.RaiseCanExecuteChanged();
                _runScriptCommand.RaiseCanExecuteChanged();
                _clearScriptCommand.RaiseCanExecuteChanged();
                _upCommand.RaiseCanExecuteChanged();
                _downCommand.RaiseCanExecuteChanged();
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildItems();

            SelectedItem = _items.FirstOrDefault();
        }

        private AllowedReportTemplate GetAllowedReportTemplate()
        {
            return _isMain ? AllowedReportTemplate.Main : AllowedReportTemplate.Audit;
        }

        private void BuildItems()
        {
            try
            {
                string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
                string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
                string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

                List<AllowedReportTemplate> allowedReportTemplates = new List<AllowedReportTemplate>();
                allowedReportTemplates.Add(this.GetAllowedReportTemplate());

                Reports reports = this._reportRepository.GetAllowedReportTemplate(
                    currentCustomerCode, currentBranchCode, currentInventorCode,
                    ViewDomainContextEnum.All, allowedReportTemplates);

                List<ReportItemViewModel> toAdd = new List<ReportItemViewModel>();
                int i = 1;
                //foreach (var report in reports.OrderBy(r => r.NN))
                foreach (var report in reports.OrderBy(r => r.CodeReport))
                {
                    if (report.CodeReport == "-") continue;
                    string path = this._generateReportRepository.BuildReportFullPath(report.Path, report.FileName);
                    bool isInFs = File.Exists(path) || report.FileName == "-";
                    toAdd.Add(new ReportItemViewModel(report) { IsReportExistInFs = isInFs, Index = i });
                    i++;
                }

                Utils.RunOnUI(() =>
                    {
                        _items.Clear();
                        toAdd.ForEach(r => _items.Add(r));
                    });
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildItems", exc);
            }
            //   _reportButtonViewModel._viewDomainContextTagItemDictionary = null;      //add 10 04 2022
            this._userSettingsManager.RefreshedReport = true;
        }

        private bool AddCommandCanExecute()
        {
            return !_isBusy;
        }

        private void AddCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.ReportAddEditView;
            payload.WindowTitle = WindowTitles.AddReportToFavorites;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            UtilsConvert.AddEnumToDictionary(payload.Settings, typeof(AllowedReportTemplate), this.GetAllowedReportTemplate());
            string reportFileNames = String.Empty;
            if (_items.Count > 0)
                reportFileNames = _items.Select(r => String.Format("{0}+{1}", r.Report.Path, r.Report.FileName)).Aggregate((working, next) => working += "^" + next);
            payload.Settings.Add(Common.NavigationSettings.ReportFileNames, reportFileNames);

            payload.Callback = r =>
                                   {
                                       ReportAddedEditedData data = r as ReportAddedEditedData;
                                       if (data != null && data.IsOk)
                                       {
                                           GenerationReport.Report reportAdd = data.Report as GenerationReport.Report;
                                           if (reportAdd != null)
                                           {
                                               BuildItems();
                                               SelectedItem = _items.FirstOrDefault(z => z.Report.FileName == reportAdd.FileName && z.Report.Path == reportAdd.Path);
                                           }
                                       }
                                   };
            OnModalWindowRequest(payload);
        }

        private bool DeleteCommandCanExecute()
        {
            return _selectedItem != null && !_isBusy;
        }

        private void DeleteCommandExecuted()
        {
            if (_selectedItem == null || _selectedItem.Report == null) return;

            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Image = MessageBoxImage.Warning;
            notification.Title = "Count4U";
            notification.Settings = this._userSettingsManager;
            notification.Content = String.Format(Localization.Resources.Msg_Delete_Report, _selectedItem.Report.Path, _selectedItem.Report.FileName);
            this._yesNoRequest.Raise(notification, r =>
            {
                if (!r.IsYes) return;

                _reportRepository.Delete(_selectedItem.Report, this.GetAllowedReportTemplate());
                BuildItems();
            });
        }

        private bool EditCommandCanExecte()
        {
            return _selectedItem != null && !_isBusy && _selectedItem.FileName != "-";
        }

        private void EditCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.WindowTitle = WindowTitles.EditReportToFavorites;
            payload.ViewName = Common.ViewNames.ReportAddEditView;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            UtilsConvert.AddEnumToDictionary(payload.Settings, typeof(AllowedReportTemplate), this.GetAllowedReportTemplate());
            string reportUniqueCode = String.Format("{0}^{1}^{2}", _selectedItem.Report.Code, _selectedItem.Report.FileName, _selectedItem.Report.Path);
            payload.Settings.Add(Common.NavigationSettings.ReportUniqueCode, reportUniqueCode);
            payload.Callback = r =>
            {
                ReportAddedEditedData data = r as ReportAddedEditedData;
                if (data != null && data.IsOk)
                {
                    GenerationReport.Report reportEdit = data.Report as GenerationReport.Report;
                    if (reportEdit != null)
                    {
                        BuildItems();
                        SelectedItem = _items.FirstOrDefault(z => z.Report.FileName == reportEdit.FileName && z.Report.Path == reportEdit.Path);
                    }
                }
            };
            OnModalWindowRequest(payload);
        }

        private bool ScriptOpenCommandCanExecute()
        {
            return !_isBusy;
        }

        private void ScriptOpenCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.ReportScriptView;
            payload.WindowTitle = WindowTitles.ReportScript;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            UtilsConvert.AddEnumToDictionary(payload.Settings, typeof(AllowedReportTemplate), this.GetAllowedReportTemplate());

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

        private bool ScriptSaveCommandCanExecute()
        {
            return !_isBusy;
        }

        private void ScriptSaveCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.ReportScriptSaveView;
            payload.WindowTitle = WindowTitles.ReportScriptSave;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            UtilsConvert.AddEnumToDictionary(payload.Settings, typeof(AllowedReportTemplate), this.GetAllowedReportTemplate());

            OnModalWindowRequest(payload);
        }

        private bool DownCommandCanExecute()
        {
            if (_selectedItem == null)
                return false;

            return _selectedItem.Index != _items.Count && !_isBusy;
        }

        private bool UpCommandCanExecute()
        {
            if (_selectedItem == null)
                return false;

            return _selectedItem.Index > 1 && !_isBusy;
        }

        private void DownCommandExecuted()
        {
            ReSort(_selectedItem.Index + 1, _selectedItem.Index);
        }

        private void UpCommandExecuted()
        {
            ReSort(_selectedItem.Index - 1, _selectedItem.Index);
        }

        private void ReSort(int index1, int index2)
        {
            IsBusy = true;

            Task.Factory.StartNew(() =>
                {
                    string selectedPath = _selectedItem.Report.Path;
                    string selectedName = _selectedItem.Report.FileName;

                    ReportItemViewModel vmSwapped = _items.FirstOrDefault(r => r.Index == index1);

                    if (vmSwapped == null) return;

                    _selectedItem.Index = index1;
                    vmSwapped.Index = index2;

                    foreach (ReportItemViewModel reportItemViewModel in _items)
                    {
                        reportItemViewModel.Report.NN = reportItemViewModel.Index;
                        _reportRepository.Update(reportItemViewModel.Report, this.GetAllowedReportTemplate());
                    }

                    BuildItems();

                    Utils.RunOnUI(() =>
                        {
                            SelectedItem = _items.FirstOrDefault(r => r.Path == selectedPath && r.FileName == selectedName);
                            IsBusy = false;
                        });
				}).LogTaskFactoryExceptions("ReSort");
        }

        private bool RunScriptCommandCanExecute()
        {
            return !_isBusy;
        }

        
         private bool ClearScriptCommandCanExecute()
        {
            if (IsSaveTags == true) return false;
            return !_isBusy;
        }

        private void RunScriptCommandExecuted()
        {
            // refresh from script file
            IDBSettings settings = this._serviceLocator.GetInstance<IDBSettings>();
            string fileScript = "MainReportLinkScript.txt";
            bool isMain = this._isMain;

            if (isMain == false)
            {
                fileScript = "AuditReportLinkScript.txt";
            }
            string reportLinkScriptPath = settings.ReportTemplateRootPath() + @"\" + "ReportLinkScript" + @"\" + fileScript;
            if (File.Exists(reportLinkScriptPath) == true)
            {
                using (new CursorWait())
                {
                    IScriptReportRepository scriptReportRepository = this._serviceLocator.GetInstance<IScriptReportRepository>();
                    bool isClear = true;
                    bool toSetupDB = false;
                    bool isClearTag = !this._isSaveTags;
                    string path = reportLinkScriptPath;
                    Encoding encoding = Encoding.GetEncoding("windows-1255");
                    scriptReportRepository.RunReportScriptFromFile(isMain, isClear, isClearTag, toSetupDB, path, encoding);
                }
                string message = String.Format(Localization.Resources.Msg_ReportLinkFill, reportLinkScriptPath);
                UtilsMisc.ShowMessageBox(message, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager, null);

                //ILog log = this._serviceLocator.GetInstance<ILog>();
                //log.PrintLog();

                BuildItems();
            }
            else
            {
                string message = String.Format(Localization.Resources.Msg_FileNotFound, reportLinkScriptPath);
                UtilsMisc.ShowMessageBox(message, MessageBoxButton.OK, MessageBoxImage.Error, _userSettingsManager, null);
            }

        }

        
         private void ClearScriptCommandExecuted()
        {
            // refresh from script file
            IDBSettings settings = this._serviceLocator.GetInstance<IDBSettings>();
            //bool isMain = this._isMain;

            //if (isMain == false)
            //{
            // }
            _reportRepository.ClearTag();
            _customerReport.DeleteAllNotTag();

            BuildItems();
        }

    }
}