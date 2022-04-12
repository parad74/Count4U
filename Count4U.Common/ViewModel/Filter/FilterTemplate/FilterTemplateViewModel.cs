using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Filter.Data;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Common.Constants;
using NLog;

namespace Count4U.Common.ViewModel.Filter.FilterTemplate
{
    public class FilterTemplateViewModel : NavigationAwareViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly IFilterTemplateRepository _filterTemplateRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly INavigationRepository _navigationRepository;

        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _updateCommand;
        private readonly DelegateCommand _renameCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly ObservableCollection<FilterTemplateItemViewModel> _items;
        private FilterTemplateItemViewModel _selectedItem;
        private string _context;

        public FilterTemplateViewModel(
            ModalWindowLauncher modalWindowLauncher,
            IFilterTemplateRepository filterTemplateRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _filterTemplateRepository = filterTemplateRepository;
            _modalWindowLauncher = modalWindowLauncher;

            _addCommand = new DelegateCommand(AddCommandExecuted);
            _updateCommand = new DelegateCommand(UpdateCommandExecuted, UpdateCommandCanExecute);
            _renameCommand = new DelegateCommand(RenameCommandExecuted, RenameCommandCanExecute);
            _deleteCommand = new DelegateCommand(DeleteCommandExecuted, DeleteCommandCanExecute);
            _items = new ObservableCollection<FilterTemplateItemViewModel>();
        }

        public FrameworkElement View { get; set; }

        public DelegateCommand AddCommand
        {
            get { return _addCommand; }
        }

        public ObservableCollection<FilterTemplateItemViewModel> Items
        {
            get { return _items; }
        }

        public FilterTemplateItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                if (_selectedItem != null)
                {
                    using (new CursorWait())
                    {
                        ISearchFieldViewModel searchFieldViewModel = GetFieldViewModel();
                        IFilterData data = _filterTemplateRepository.GetData(_selectedItem.FileInfo, searchFieldViewModel.BuildFilterData().GetType()) as IFilterData;
                        if (data != null)
                            searchFieldViewModel.ApplyFilterData(data);
                    }
                }

                _updateCommand.RaiseCanExecuteChanged();
                _renameCommand.RaiseCanExecuteChanged();
                _deleteCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand UpdateCommand
        {
            get { return _updateCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public DelegateCommand RenameCommand
        {
            get { return _renameCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }


        private void AddCommandExecuted()
        {
            var settings = new Dictionary<string, string>();

            FilterTemplateAddEditDTO dto = new FilterTemplateAddEditDTO();
            dto.Context = _context;
            dto.DisplayName = String.Empty;
            dto.FileName = String.Empty;

            UtilsConvert.AddObjectToDictionary(settings, _navigationRepository, dto);

            object result = this._modalWindowLauncher.StartModalWindow(
                Common.ViewNames.FilterTemplateAddEditView,
                  WindowTitles.FilterTemplateAdd,
                  250, 170,
                  ResizeMode.NoResize, settings,
                  Window.GetWindow(View),
                  minWidth: 230, minHeight: 120);

            FilterTemplateAddEditDTO resultDto = result as FilterTemplateAddEditDTO;
            if (resultDto != null)
            {
                try
                {
                    ISearchFieldViewModel searchFieldViewModel = GetFieldViewModel();
                    IFilterData data = searchFieldViewModel.BuildFilterData();
                    data.DisplayName = resultDto.DisplayName;

                    FileInfo fileInfo = _filterTemplateRepository.Add(resultDto.FileName, data, _context);
                    FilterTemplateItemViewModel viewModel = new FilterTemplateItemViewModel(fileInfo, GetFilterDataType);

                    _items.Add(viewModel);

                    _selectedItem = viewModel;
                    RaisePropertyChanged(() => SelectedItem);
                    _updateCommand.RaiseCanExecuteChanged();
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("AddCommandExecuted", exc);
                }
            }
        }

        public void Build(string context)
        {
            _context = context;

            _items.Clear();

            foreach (FileInfo fileInfo in _filterTemplateRepository.GetFiles(_context))
            {
                FilterTemplateItemViewModel item = new FilterTemplateItemViewModel(fileInfo, GetFilterDataType);

                _items.Add(item);
            }
        }

        private bool UpdateCommandCanExecute()
        {
            return _selectedItem != null;
        }

        private void UpdateCommandExecuted()
        {
            if (_selectedItem == null) return;

            using (new CursorWait())
            {
                try
                {
                    ISearchFieldViewModel searchFieldViewModel = GetFieldViewModel();
                    IFilterData data = searchFieldViewModel.BuildFilterData();

                    _filterTemplateRepository.Update(_selectedItem.FileInfo, data);
                }
                catch (Exception e)
                {
                    _logger.ErrorException("UpdateCommandExecuted", e);
                }
            }
        }

        private ISearchFieldViewModel GetFieldViewModel()
        {
            return Utils.GetViewModelFromRegion<ISearchFieldViewModel>(Common.RegionNames.SearchFieldGround, _regionManager);
        }

        private bool RenameCommandCanExecute()
        {
            return _selectedItem != null;
        }

        private void RenameCommandExecuted()
        {
            var settings = new Dictionary<string, string>();

            FilterTemplateAddEditDTO dto = new FilterTemplateAddEditDTO();
            dto.Context = _context;
            dto.DisplayName = _selectedItem.DisplayName;
            dto.FileName = Path.GetFileNameWithoutExtension(_selectedItem.FileInfo.FullName);

            UtilsConvert.AddObjectToDictionary(settings, _navigationRepository, dto);

            object result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.FilterTemplateAddEditView,
                  WindowTitles.FilterTemplateEdit,
                  250, 170,
                  ResizeMode.NoResize,
                  settings,
                  Window.GetWindow(View),
                  minWidth: 230, minHeight: 120);

            FilterTemplateAddEditDTO resultDto = result as FilterTemplateAddEditDTO;
            if (resultDto != null)
            {
                try
                {
                    ISearchFieldViewModel searchFieldViewModel = GetFieldViewModel();
                    IFilterData data = searchFieldViewModel.BuildFilterData();
                    data.DisplayName = resultDto.DisplayName;

                    FileInfo fileInfo = _filterTemplateRepository.Rename(_selectedItem.FileInfo, resultDto.FileName, data, _context);
                    _selectedItem.Update(fileInfo);
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("RenameCommandExecuted", exc);
                }
            }
        }

        private bool DeleteCommandCanExecute()
        {
            return _selectedItem != null;
        }

        private void DeleteCommandExecuted()
        {
            string message = String.Format(Localization.Resources.ViewModel_FilterTemplate_msgAreYouSureDelete, _selectedItem.Name);

            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager, Window.GetWindow(View));
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    _filterTemplateRepository.Delete(_selectedItem.FileInfo);
                    _items.Remove(_selectedItem);
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("DeleteCommandExecuted", exc);
                }
            }
        }

        private Type GetFilterDataType()
        {
            ISearchFieldViewModel searchFieldViewModel = GetFieldViewModel();
            return searchFieldViewModel.BuildFilterData().GetType();
        }
    }
}