using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Planogram.Lib;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Media;

namespace Count4U.Planogram.ViewModel
{
    public class PlanPictureAssignViewModel : CBIContextBaseViewModel, IChildWindowViewModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private readonly IDBSettings _dbSettings;

        private readonly ObservableCollection<PlanPictureItemViewModel> _items;
        private PlanPictureItemViewModel _selected;

        public PlanPictureAssignViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            IEventAggregator eventAggregator,
            IDBSettings dbSettings)
            : base(contextCbiRepository)
        {
            _dbSettings = dbSettings;
            _eventAggregator = eventAggregator;
            _navigationRepository = navigationRepository;

            _okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            _cancelCommand = new DelegateCommand(CancelCommandExecuted);

            _items = new ObservableCollection<PlanPictureItemViewModel>();
        }

        public DelegateCommand OkCommand
        {
            get { return _okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public PlanPictureItemViewModel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RaisePropertyChanged(() => Selected);

                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<PlanPictureItemViewModel> Items
        {
            get { return _items; }
        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            PlanPictureInfo pictureInfo = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, null, true) as PlanPictureInfo;
            if (pictureInfo != null)
            {
                Build(pictureInfo);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void Build(PlanPictureInfo pictureInfo)
        {
            PlanPictureItemViewModel emptyItem = new PlanPictureItemViewModel();
            emptyItem.Path = String.Format("pack://application:,,,/Count4U.Media;component/Icons/{0}.png", "empty");
            emptyItem.FileName = String.Empty;
            _items.Add(emptyItem);

            foreach (string file in Helpers.EnumeratePictures(_dbSettings))
            {
                FileInfo fi = new FileInfo(file);

                PlanPictureItemViewModel item = new PlanPictureItemViewModel();
                item.FileName = fi.Name;
                item.Path = fi.FullName;

                _items.Add(item);
            }         

            if (!String.IsNullOrWhiteSpace(pictureInfo.FileName))
            {
                _selected = _items.FirstOrDefault(r => r.FileName.ToLower() == pictureInfo.FileName.ToLower());
            }
            else
            {
                _selected = emptyItem;
            }
        }

        private void CancelCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return _selected != null;
        }

        private void OkCommandExecuted()
        {
            if (_selected == null) return;

            PlanPictureInfo info = new PlanPictureInfo();
            info.FileName = _selected.FileName;

            this.ResultData = info;
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        public object ResultData { get; set; }

    }
}