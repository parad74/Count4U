using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels.Pack
{
    public abstract class PackBaseItemViewModel : NotificationObject
    {
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly ObservableCollection<PackBaseItemViewModel> _children;

        protected string _header;
        protected string _image;
        protected bool _isChecked;
        protected bool _isExpanded;

        private bool _isCheckboxVisible;

        protected readonly PackBaseItemViewModel _parent;

        protected PackBaseItemViewModel(PackBaseItemViewModel parent)
        {
            _parent = parent;

            this._children = new ObservableCollection<PackBaseItemViewModel>();
            _isCheckboxVisible = true;

        }

        public ObservableCollection<PackBaseItemViewModel> Children
        {
            get { return this._children; }
        }

        public string Header
        {
            get { return this._header; }
            set
            {
                this._header = value;
                RaisePropertyChanged(() => Header);
            }
        }

        public string Image
        {
            get
            {
                return String.Format("/Count4U.Media;component/Icons/{0}.png", this._image);
            }
            set
            {
                this._image = value;
                RaisePropertyChanged(() => Image);
            }
        }


        public virtual bool IsChecked
        {
            get { return this._isChecked; }
            set
            {
                this._isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        public bool IsExpanded
        {
            get { return this._isExpanded; }
            set
            {
                this._isExpanded = value;
                RaisePropertyChanged(() => IsExpanded);
            }
        }

        public PackBaseItemViewModel Parent
        {
            get { return _parent; }
        }

        public bool IsCheckboxVisible
        {
            get { return _isCheckboxVisible; }
            set
            {
                _isCheckboxVisible = value;
                RaisePropertyChanged(() => IsCheckboxVisible);
            }
        }
    }
}