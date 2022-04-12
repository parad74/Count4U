using System;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Planogram.ViewModel
{
    public class PlanTreeItemViewModel : NotificationObject
    {
        private readonly ObservableCollection<PlanTreeItemViewModel> _children;

        private readonly PlanTreeItemViewModel _parent;

        private string _text;
        private string _code;
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isChecked;
        private bool _isCheckedVisible;

        private bool _lockVisible;
        private bool _isLocked;
        private readonly DelegateCommand _lockCommand;

        private bool _canRename;
        private string _renameText;
        private bool _canNotRenameAtAll;

        private readonly DelegateCommand _checkCommand;        

        public PlanTreeItemViewModel(PlanTreeItemViewModel parent)
        {
            _parent = parent;
            _children = new ObservableCollection<PlanTreeItemViewModel>();

            _isExpanded = false;
            _isSelected = false;
            _isChecked = false;
            _lockVisible = false;
            _isLocked = false;
            _lockVisible = false;

            _lockCommand = new DelegateCommand(LockCommandExecuted);

            _checkCommand = new DelegateCommand(CheckCommandExecuted);
        }       

        public event EventHandler IsCheckedChanged = delegate { };
        public event EventHandler IsLockedChanged = delegate { };
        public event EventHandler TextChanged = delegate { };

        protected virtual void OnTextChanged()
        {
            EventHandler handler = TextChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnIsLockedChanged()
        {
            EventHandler handler = IsLockedChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnIsCheckedChanged()
        {
            EventHandler handler = IsCheckedChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public ObservableCollection<PlanTreeItemViewModel> Children
        {
            get { return _children; }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(() => IsExpanded);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    RaisePropertyChanged(() => IsChecked);

                    OnIsCheckedChanged();
                }
            }
        }

        public bool IsCheckedVisible
        {
            get { return _isCheckedVisible; }
            set
            {
                _isCheckedVisible = value;
                RaisePropertyChanged(() => IsCheckedVisible);
            }
        }

        public bool LockVisible
        {
            get { return _lockVisible; }
            set
            {
                _lockVisible = value;
                RaisePropertyChanged(() => LockVisible);
            }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                _isLocked = value;
                RaisePropertyChanged(() => IsLocked);
            }
        }

        public DelegateCommand LockCommand
        {
            get { return _lockCommand; }
        }

        public bool CanRename
        {
            get { return _canRename; }
            set
            {
                _canRename = value;
                RaisePropertyChanged(() => CanRename);
            }
        }

        public string RenameText
        {
            get { return _renameText; }
            set
            {
                _renameText = value;
                RaisePropertyChanged(() => RenameText);

                this._text = _renameText;
                RaisePropertyChanged(() => Text);

                OnTextChanged();
            }
        }

        public bool CanNotRenameAtAll
        {
            get { return _canNotRenameAtAll; }
            set { _canNotRenameAtAll = value; }
        }

        public DelegateCommand CheckCommand
        {
            get { return _checkCommand; }
        }

        public PlanTreeItemViewModel Parent
        {
            get { return _parent; }
        }


        public void SetIsCheckedSilent(bool value)
        {
            _isChecked = value;
            RaisePropertyChanged(() => IsChecked);
        }

        private void LockCommandExecuted()
        {
            _isLocked = !_isLocked;

            RaisePropertyChanged(() => IsLocked);

            OnIsLockedChanged();

            if (_canNotRenameAtAll == false)
            {
                this.CanRename = _isLocked == false;
            }
        }

        public void SetRenameTextSilent(string renameText)
        {
            _renameText = renameText;
            RaisePropertyChanged(()=>RenameText);
        }

        public void SetIsLockedSilent(bool value)
        {
            _isLocked = value;
            RaisePropertyChanged(() => IsLocked);

            if (_canNotRenameAtAll == false)
            {
                this.CanRename = _isLocked == false;
            }
        }

        private void CheckCommandExecuted()
        {
            this.IsChecked = ! this._isChecked;
        }
    }
}