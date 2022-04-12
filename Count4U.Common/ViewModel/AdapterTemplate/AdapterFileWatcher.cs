using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Threading;
using Count4U.Common.ViewModel.Adapters;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.AdapterTemplate
{
    public class AdapterFileWatcher
    {
        private readonly ModuleBaseViewModel _viewModel;
        private readonly PropertyInfo _info;
        private readonly DispatcherTimer _timer;
        private bool _isFile;

        private bool? _isExistsState;

        public AdapterFileWatcher(ModuleBaseViewModel viewModel, PropertyInfo info, bool isFile = true)
        {
            _isFile = isFile;
            _info = info;
            _viewModel = viewModel;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        }

        public bool IsFile
        {
            get { return _isFile; }
            set { _isFile = value; }
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _info.Name)
            {
                _isExistsState = IsPathExists();
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            bool isPathExists = IsPathExists();

            bool isUpdate = false;

            if (_isExistsState == null) //first run
                isUpdate = true;
            else
            {
                isUpdate = _isExistsState != isPathExists;
            }

            if (isUpdate)
            {
                _info.SetValue(_viewModel, GetPath(), null);
            }
        }

        private bool IsPathExists()
        {
            string path = GetPath();

            bool result = false;
            if (!String.IsNullOrWhiteSpace(path))
            {
                if (_isFile)
                {
                    result = File.Exists(path);
                }
                else
                {
                    result = Directory.Exists(path);
                }
            }

            return result;
        }

        private string GetPath()
        {
            object o = _info.GetValue(_viewModel, null);
            if (o is String)
            {
                return (String)o;
            }

            return String.Empty;
        }

        public void Clear()
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _timer.Stop();
        }
    }
}