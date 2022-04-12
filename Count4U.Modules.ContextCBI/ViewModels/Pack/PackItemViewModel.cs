using System;
using System.Linq;
using Count4U.Model.Audit;
using Count4U.Model.Main;

namespace Count4U.Modules.ContextCBI.ViewModels.Pack
{
    public class PackItemViewModel : PackBaseItemViewModel
    {
        private bool _isVisible;

        private bool _isSelected;

        private bool _importFolderIsVisible;
        private bool _importFolderIsChecked;
        private bool _importFolderInDataIsVisible;
		private bool _importFolderProfileIsVisible;
		private bool _importFolderConfigIsVisible;
        private bool _importFolderInDataIsChecked;
		private bool _importFolderConfigIsChecked;
		private bool _importFolderProfileIsChecked;
		

        public PackItemViewModel(PackBaseItemViewModel parent)
            : base(parent)
        {
            _isVisible = true;
        }

        public object DomainObject { get; set; }
        public int Level { get; set; }
        public Func<bool> AutoCheckCatalogFiles { get; set; }
        public Func<bool> AutoCheckInDataFiles { get; set; }
		public Func<bool> AutoCheckConfigFiles { get; set; }
		public Func<bool> AutoCheckProfileFiles { get; set; }
		

        public override bool IsChecked
        {
            get { return this._isChecked; }
            set
            {
                this._isChecked = value;
                RaisePropertyChanged(() => IsChecked);

                if (_isChecked)
                {
                    if (DomainObject is Customer || DomainObject is Branch || DomainObject is Inventor)
                    {
                        if (AutoCheckCatalogFiles != null)
                        {
                            ImportFolderIsChecked = AutoCheckCatalogFiles();
                        }
                        if (AutoCheckInDataFiles != null)
                        {
                            ImportFolderInDataIsChecked = AutoCheckInDataFiles();
                        }
						if (AutoCheckConfigFiles != null)
                        {
							ImportFolderConfigIsChecked = AutoCheckConfigFiles();
                        }
						if (AutoCheckProfileFiles != null)
                        {
							ImportFolderProfileIsChecked = AutoCheckProfileFiles();
                        }
						
						
                    }
                }
                else
                {
//                    foreach (PackBaseItemViewModel child in _children)
//                    {
//                        child.IsChecked = false;
//                    }
                }
            }
        }

        public bool StopPropagateVisibilityToParent { get; set; }
        public bool StopPropagateVisibilityToChild { get; set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged(() => IsVisible);

                }


                if (_isVisible)
                {
                    PackItemViewModel parent = base._parent as PackItemViewModel;
                    if (parent != null && StopPropagateVisibilityToParent == false) //if parent is not null, and we are not going down tree
                    {
                        parent.StopPropagateVisibilityToChild = true;
                        parent.IsVisible = true;
                        parent.StopPropagateVisibilityToChild = false;
                    }

                    if (parent != null && StopPropagateVisibilityToChild == false) //if this is not root node, make visible all child
                    {
                        foreach (PackItemViewModel child in _children.Cast<PackItemViewModel>())
                        {
                            child.StopPropagateVisibilityToParent = true;
                            child.IsVisible = true;
                            child.StopPropagateVisibilityToParent = false;
                        }
                    }
                }
            }
        }

        public bool ImportFolderIsVisible
        {
            get { return _importFolderIsVisible; }
            set
            {
                _importFolderIsVisible = value;
                RaisePropertyChanged(() => ImportFolderIsVisible);
            }
        }

        public bool ImportFolderIsChecked
        {
            get { return _importFolderIsChecked; }
            set
            {
                _importFolderIsChecked = value;
                RaisePropertyChanged(() => ImportFolderIsChecked);
            }
        }

        public bool ImportFolderInDataIsVisible
        {
            get { return _importFolderInDataIsVisible; }
            set
            {
                _importFolderInDataIsVisible = value;
                RaisePropertyChanged(() => ImportFolderInDataIsVisible);
            }
        }


		public bool ImportFolderProfileIsVisible
        {
			get { return _importFolderProfileIsVisible; }
            set
            {
				_importFolderProfileIsVisible = value;
				RaisePropertyChanged(() => ImportFolderProfileIsVisible);
            }
        }

		public bool ImportFolderConfigIsVisible
		{
			get { return _importFolderConfigIsVisible; }
			set
			{
				_importFolderConfigIsVisible = value;
				RaisePropertyChanged(() => ImportFolderConfigIsVisible);
			}
		}

        public bool ImportFolderInDataIsChecked
        {
            get { return _importFolderInDataIsChecked; }
            set
            {
                _importFolderInDataIsChecked = value;
                RaisePropertyChanged(() => ImportFolderInDataIsChecked);
            }
        }


		public bool ImportFolderConfigIsChecked
        {
			get { return _importFolderConfigIsChecked; }
            set
            {
				_importFolderConfigIsChecked = value;
                RaisePropertyChanged(() => ImportFolderConfigIsChecked);
            }
        }

		public bool ImportFolderProfileIsChecked
		{
			get { return _importFolderProfileIsChecked; }
			set
			{
				_importFolderProfileIsChecked = value;
				RaisePropertyChanged(() => ImportFolderProfileIsChecked);
			}
		}

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(()=>IsSelected);
            }
        }
    }
}