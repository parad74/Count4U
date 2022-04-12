using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Report.ViewModels.Script;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using NLog;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4U;
using System.Text;
using System.IO;
using System.Windows;
using Count4U.Common.Extensions;
using Count4U.Model.Count4U;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters
{
    public class AdapterLinkViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IImportAdapterRepository _importAdapterRepository;
        private readonly IUnityContainer _container;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly DelegateCommand _scriptOpenCommand;
        private readonly DelegateCommand _scriptSaveCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _restoreLinksCommand;
        protected readonly IServiceLocator _serviceLocator;
        private readonly UICommandRepository _commandRepository;

        private readonly ObservableCollection<AdapterLinkItemViewModel> _items;
        private AdapterLinkItemViewModel _selectedItem;

        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

        public AdapterLinkViewModel(
            IContextCBIRepository contextCbiRepository,
            IImportAdapterRepository importAdapterRepository,
            IUnityContainer container,
            IUserSettingsManager userSettingsManager,
            IServiceLocator serviceLocator,
            UICommandRepository commandRepository)
            : base(contextCbiRepository)
        {
            this._commandRepository = commandRepository;
            this._userSettingsManager = userSettingsManager;
            this._container = container;
            this._importAdapterRepository = importAdapterRepository;
            this._scriptOpenCommand = _commandRepository.Build(enUICommand.OpenScript, ScriptOpenCommandExecuted);
            this._scriptSaveCommand = _commandRepository.Build(enUICommand.SaveScript, ScriptSaveCommandExecuted);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, DeleteCommandExecuted, DeleteCommandCanExcute);
            this._restoreLinksCommand = _commandRepository.Build(enUICommand.Restore, RestoreLinksCommandExecuted);
            this._serviceLocator = serviceLocator;
            this._items = new ObservableCollection<AdapterLinkItemViewModel>();

            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
        }

        public DelegateCommand ScriptOpenCommand
        {
            get { return _scriptOpenCommand; }
        }

        public DelegateCommand ScriptSaveCommand
        {
            get { return _scriptSaveCommand; }
        }

        public ObservableCollection<AdapterLinkItemViewModel> Items
        {
            get { return _items; }
        }

        public AdapterLinkItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                this._deleteCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return _yesNoRequest; }
        }

        public DelegateCommand RestoreLinksCommand
        {
            get { return _restoreLinksCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			Task.Factory.StartNew(BuildItems).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        private void BuildItems()
        {
            try
            {
                ImportAdapters adapters = this._importAdapterRepository.GetImportAdapters(); //db

                List<IImportModuleInfo> containerAdapters = _container.ResolveAll<IImportModuleInfo>().ToList(); //unity
                List<IExportPdaModuleInfo> containerAdaptersPda = _container.ResolveAll<IExportPdaModuleInfo>().ToList(); //unity
                List<IExportErpModuleInfo> containerAdaptersErp = _container.ResolveAll<IExportErpModuleInfo>().ToList(); //unity

                List<AdapterLinkItemViewModel> toAdd = new List<AdapterLinkItemViewModel>();
                foreach (ImportAdapter adapter in adapters.OrderBy(r => r.DomainType).ThenBy(r => r.AdapterCode))
                {
                    AdapterLinkItemViewModel viewModel = new AdapterLinkItemViewModel(adapter);

                    string name = String.Empty;
                    string description = String.Empty;
                    bool isInFs = false;

                    if (adapter.DomainType == ImportDomainEnum.ExportCatalogPDA.ToString())
                    {
                        IExportPdaModuleInfo containerAdapter = containerAdaptersPda.FirstOrDefault(r => r.Name.Trim() == adapter.AdapterCode.Trim());
                        if (containerAdapter != null)
                        {
                            name = containerAdapter.Title;
                            description = containerAdapter.Description;
                            isInFs = true;
                        }
                    }
                    else if (adapter.DomainType == ImportDomainEnum.ExportCatalogERP.ToString())
                    {
                        IExportErpModuleInfo containerAdapter = containerAdaptersErp.FirstOrDefault(r => r.Name.Trim() == adapter.AdapterCode.Trim());
                        if (containerAdapter != null)
                        {
                            name = containerAdapter.Title;
                            description = containerAdapter.Description;
                            isInFs = true;
                        }
                    }
                    else
                    {
                        IImportModuleInfo containerAdapter = containerAdapters.FirstOrDefault(r => r.Name.Trim() == adapter.AdapterCode.Trim());
                        if (containerAdapter != null)
                        {
                            name = containerAdapter.Title;
                            description = containerAdapter.Description;
                            isInFs = true;
                        }
                    }

                    viewModel.Name = name;
                    viewModel.IsAdapterExistInFs = isInFs;

                    viewModel.AdapterCode = adapter.AdapterCode;
                    viewModel.DomainType = adapter.DomainType;
                    viewModel.Description = String.IsNullOrWhiteSpace(adapter.Description) ? description : adapter.Description;
                    viewModel.Code = adapter.Code;

                    toAdd.Add(viewModel);
                }

                Count4U.Common.Helpers.Utils.RunOnUI(() =>
                                                         {
                                                             this._items.Clear();
                                                             toAdd.ForEach(r => this._items.Add(r));
                                                         });
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildItems", exc);
            }
        }

        private void ScriptSaveCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.AdapterLinkScriptSaveView;
            payload.WindowTitle = WindowTitles.ScriptSave;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);

            OnModalWindowRequest(payload);
        }

        private void ScriptOpenCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.AdapterLinkScriptOpenView;
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

        private bool DeleteCommandCanExcute()
        {
            return this._selectedItem != null;
        }

        private void DeleteCommandExecuted()
        {
            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Title = String.Empty;
            notification.Settings = this._userSettingsManager;
            notification.Content = String.Format(Localization.Resources.Msg_Delete_Adapter_Link);
            this.YesNoRequest.Raise(notification, r =>
            {
                if (r.IsYes == true && this._selectedItem != null)
                {
                    this._importAdapterRepository.Delete(this._selectedItem.ImportAdapter.ID);
                    this.BuildItems();
                }
            });
        }

        private void RestoreLinksCommandExecuted()
        {
            IDBSettings dbSetting = this._serviceLocator.GetInstance<IDBSettings>();
            string adapterLinkTxtPath = dbSetting.AdapterLinkTxtPath();

            if (File.Exists(adapterLinkTxtPath) == true)
            {
                try
                {
                    using (new CursorWait())
                    {
                        IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
                        string importAdapterTable = @"[ImportAdapter]";
                        string sql = InsertSqlFromScriptToTable(adapterLinkTxtPath, importAdapterTable);
                        alterADOProvider.ImportToMainDB(sql, importAdapterTable, true);

                        string message = String.Format(Localization.Resources.Msg_ReportLinkFill, adapterLinkTxtPath);
                        UtilsMisc.ShowMessageBox(message, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager, null);
                        BuildItems();
                    }
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("RunScript", exc);
                }
            }
            else
            {
                string message = String.Format(Localization.Resources.Msg_FileNotFound, adapterLinkTxtPath);
                UtilsMisc.ShowMessageBox(message, MessageBoxButton.OK, MessageBoxImage.Error, _userSettingsManager, null);
            }
        }


        public string InsertSqlFromScriptToTable(string adapterLinkTxtPath, string importAdapterTable, List<string> exludeCode = null)
        {
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString());
            string sql = "";
            foreach (String record in fileParser.GetRecords(adapterLinkTxtPath,
            Encoding.GetEncoding("windows-1255"), 0))
            {
                if (ExcludeRecordeCodeExistsInDB(record, exludeCode) == true) continue;
                if (record.ToUpper().Contains("DROP") == false
                    && record.ToUpper().Contains("DELETE") == false
                    //&& record.ToUpper().Contains("UPDATE") == false
                    && record.ToUpper().Contains("ALTER") == false
                    && record.ToUpper().Contains("CREATE") == false
                    && record.ToUpper().Contains("SELECT") == false)
                {
                    if (record.ToUpper().Contains("INSERT") == true)
                    {
                        if (record.Contains(importAdapterTable) == true)
                        {
                            sql = sql + record + Environment.NewLine;
                        }

                    }
                }
            }  //foreach record
            return sql;
        }

        private bool ExcludeRecordeCodeExistsInDB(string record, List<string> exludeCode)
        {
            bool ret = false;
            if (exludeCode != null)
            {
                foreach (string code in exludeCode)
                {
                    if (record.ToUpper().Contains(code.ToUpper()))
                    {
                        return true;
                    }
                }
            }
            return ret;
        }
    }
}