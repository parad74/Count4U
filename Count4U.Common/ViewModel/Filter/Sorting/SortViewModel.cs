using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Count4U.Translation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.Filter.Sorting
{
    public class SortViewModel : NotificationObject
    {
        private readonly IPropertyTranslation _propertyTranslation;
        private readonly UserSettingsManager _userSettingsManager;

        private readonly DelegateCommand _sortDirectionCommand;

        private readonly ObservableCollection<SortItem> _items;
        private SortItem _selectedItem;

        private bool _isDesc;
        private bool _isAbsolute;
        private bool _isAbsoluteEnabled;


        public SortViewModel(
            UserSettingsManager userSettingsManager,
            IPropertyTranslation propertyTranslation)
        {
            _userSettingsManager = userSettingsManager;
            _propertyTranslation = propertyTranslation;
            _items = new ObservableCollection<SortItem>();

            _sortDirectionCommand = new DelegateCommand(SortDirectionCommandExecuted);

            _isDesc = false;
        }

        public ObservableCollection<SortItem> Items
        {
            get { return _items; }
        }

        public bool IsDesc
        {
            get { return _isDesc; }
            set
            {
                _isDesc = value;
                RaisePropertyChanged(() => IsDesc);
            }
        }

        public DelegateCommand SortDirectionCommand
        {
            get { return _sortDirectionCommand; }
        }

        public SortItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                IsAbsoluteEnabled = IsSelectedItemNumeric();
            }
        }

        public bool IsAbsolute
        {
            get { return _isAbsolute; }
            set
            {
                _isAbsolute = value;
                RaisePropertyChanged(() => IsAbsolute);
            }
        }

        public bool IsAbsoluteEnabled
        {
            get { return _isAbsoluteEnabled; }
            set
            {
                _isAbsoluteEnabled = value;
                RaisePropertyChanged(() => IsAbsoluteEnabled);

                if (_isAbsoluteEnabled == false)
                {
                    IsAbsolute = false;
                }
            }
        }

        public string AscImage
        {
            get
            {
                if (_userSettingsManager.LanguageGet() == enLanguage.Hebrew)
                    return @"/Count4U.Media;component/Icons/ascHe.png";

                return @"/Count4U.Media;component/Icons/asc.png";
            }
        }

        public string DescImage
        {
            get
            {
                if (_userSettingsManager.LanguageGet() == enLanguage.Hebrew)
                    return @"/Count4U.Media;component/Icons/descHe.png";

                return @"/Count4U.Media;component/Icons/desc.png";
            }
        }

        public void Add(List<PropertyInfo> propeties)
        {
            _items.Clear();

            List<SortItem> unordered = new List<SortItem>();

            foreach (PropertyInfo propertyInfo in propeties)
            {
                SortItem item = new SortItem(propertyInfo);
                item.Name = _propertyTranslation.GetTranslation(propertyInfo);
                unordered.Add(item);
            }

            foreach (SortItem sortItem in unordered.OrderBy(r=>r.Name))
            {
                _items.Add(sortItem);
            }
        }

        private void SortDirectionCommandExecuted()
        {
            IsDesc = !_isDesc;
        }

        private string GetSortField()
        {
            if (_selectedItem == null)
                return null;

            return _selectedItem.PropertyInfo.Name;
        }

        private void SetSortField(string propertyName)
        {
            SelectedItem = _items.FirstOrDefault(r => r.PropertyInfo.Name == propertyName);
        }

        private enSortDirection GetSortDirection()
        {
            return _isDesc ? enSortDirection.DESC : enSortDirection.ASC;
        }

        private void SetSortDirection(enSortDirection sortDirection)
        {
            IsDesc = sortDirection == enSortDirection.DESC;
        }

        private bool GetIsAbsolute()
        {
            return _isAbsolute;
        }

        private void SetIsAbsolute(bool isAbsolute)
        {
            IsAbsolute = isAbsolute;
        }

        public void Reset()
        {
            SelectedItem = null;
            IsDesc = false;
            IsAbsolute = false;
        }

        public void ApplyToFilterData(CommonFilterData filter)
        {
            filter.SortDirection = GetSortDirection();
            filter.SortField = GetSortField();
            filter.SortIsAbsolute = GetIsAbsolute();
        }

        public void InitFromFilterData(CommonFilterData filter)
        {
            SetSortDirection(filter.SortDirection);
            SetSortField(filter.SortField);
            SetIsAbsolute(filter.SortIsAbsolute);
        }

        private bool IsSelectedItemNumeric()
        {
            if (_selectedItem == null)
                return false;

            PropertyInfo pi = _selectedItem.PropertyInfo;

            if (pi.PropertyType == typeof(int) ||
                pi.PropertyType == typeof(double) ||
                pi.PropertyType == typeof(int?) ||
                pi.PropertyType == typeof(double?))
                return true;

            return false;
        }
    }
}