using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.ParsingMask;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask
{
    public class MaskSelectViewModel : CBIContextBaseViewModel, IChildWindowViewModel
    {
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private readonly IEventAggregator _eventAggregator;

        private string _inputString;

        private readonly ObservableCollection<MaskSelectItemViewModel> _items;

        public MaskSelectViewModel(IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            this._eventAggregator = eventAggregator;

            this._okCommand = new DelegateCommand(OkCommandExecuted);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._items = new ObservableCollection<MaskSelectItemViewModel>();
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public ObservableCollection<MaskSelectItemViewModel> Items
        {
            get { return this._items; }
        }

        public string InputString
        {
            get { return this._inputString; }
            set
            {
                this._inputString = value;
                RaisePropertyChanged(() => InputString);

                UpdateItemsResult();
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            var masks = new List<string>();

            masks.Add("111{R}");
            masks.Add("S{R}");
            masks.Add("22{L}");
            masks.Add("L{L}");
            masks.Add("5{N}");
            masks.Add("13{N}");
			masks.Add("-5{N}");
			masks.Add("-13{N}");
            masks.Add("0000000{F}");
			masks.Add("0000000000000{F}");
			masks.Add("7290000000000{F}");
			masks.Add("0000000000000{S}");
			masks.Add("7290000000000{S}");
//            masks.Add("{E}");

            foreach (string mask in masks)
            {
                MaskSelectItemViewModel item = new MaskSelectItemViewModel();
                item.MaskTemplate = mask;
                item.Result = String.Empty;
                item.IsEdit = false;

                this._items.Add(item);
            }

            MaskSelectItemViewModel editItem = new MaskSelectItemViewModel();
            editItem.MaskEditTemplate = String.Empty;
            editItem.IsEdit = true;
            this._items.Add(editItem);

            MaskSelectItemViewModel first = this._items.FirstOrDefault();
            if (first != null)
                first.IsChecked = true;

            foreach (MaskSelectItemViewModel item in this._items)
            {
                item.PropertyChanged += SelectMaskItem_PropertyChanged;
            }
        }

        void SelectMaskItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                MaskSelectItemViewModel item = sender as MaskSelectItemViewModel;
                if (item != null && item.IsChecked)
                {
                    foreach (MaskSelectItemViewModel maskItem in this._items)
                    {
                        if (item == maskItem) continue;

                        maskItem.IsChecked = false;
                    }
                }
            }

            if (e.PropertyName == "MaskEditTemplate")
            {
                MaskSelectItemViewModel item = sender as MaskSelectItemViewModel;
                if (item != null && item.IsEdit)
                    item.Result = MaskSelectUtils.FormatInputString(this._inputString, item.MaskEditTemplate);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            foreach (MaskSelectItemViewModel item in this._items)
            {
                item.PropertyChanged -= SelectMaskItem_PropertyChanged;
            }
        }

        private void OkCommandExecuted()
        {
            MaskSelectItemViewModel selected = this._items.FirstOrDefault(r => r.IsChecked);
            if (selected != null)
            {
                ResultData = new MaskSelectedData() {Value = selected.IsEdit ? selected.MaskEditTemplate : selected.MaskTemplate};
            }

            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void UpdateItemsResult()
        {
            foreach (MaskSelectItemViewModel item in this._items)
            {
                if (item.IsEdit)
                    item.Result = MaskSelectUtils.FormatInputString(this._inputString, item.MaskEditTemplate);
                else
                    item.Result = MaskSelectUtils.FormatInputString(this._inputString, item.MaskTemplate);
            }
        }

        #region Implementation of IChildWindowViewModel

        public object ResultData { get; set; }

        #endregion
    }
}