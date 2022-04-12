using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.ViewModel;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels.Zip
{
    public abstract class ZipItemViewModel : NotificationObject
    {
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly ObservableCollection<ZipItemViewModel> _children;
        protected readonly PropertyChangedEventHandler _propertyChangedEventHandler;

        protected string _header;
        protected object _image;
        protected bool _isChecked;
        protected bool _isExpanded;

        public ZipItemViewModel(PropertyChangedEventHandler propertyChangedEventHandler)
        {
            this._propertyChangedEventHandler = propertyChangedEventHandler;

            this._children = new ObservableCollection<ZipItemViewModel>();
        }

        public ObservableCollection<ZipItemViewModel> Children
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

        public virtual object Image { get; set; }
        

        public bool IsChecked
        {
            get { return this._isChecked; }
            set
            {
                this._isChecked = value;
                RaisePropertyChanged(() => IsChecked);

                foreach (ZipItemViewModel child in this._children)
                {
                    child.IsChecked = this._isChecked;
                }

                if (this._propertyChangedEventHandler != null)
                    this._propertyChangedEventHandler(this, new PropertyChangedEventArgs("IsChecked"));
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


        public IEnumerable<ZipItemViewModel> GetChildren()
        {
            return this._children;
        }        
    }
}