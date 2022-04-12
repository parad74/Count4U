using System;
using System.ComponentModel;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Interfaces;
using Count4U.Model.Count4U.Validate;
using Count4U.Planogram.Lib;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Planogram.ViewModel
{
    public class PlanSizeChangeViewModel : NotificationObject, INavigationAware, IDataErrorInfo, IChildWindowViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private string _width;
        private string _height;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        public PlanSizeChangeViewModel(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            _cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public string Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged(() => Width);

                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public string Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged(() => Height);

                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand OkCommand
        {
            get { return _okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            int widthPixels = (int)Math.Ceiling(Double.Parse(navigationContext.Parameters[Common.NavigationSettings.PlanogramWidth]));
            int heightPixels = (int)Math.Ceiling(Double.Parse(navigationContext.Parameters[Common.NavigationSettings.PlanogramHeight]));

            _width = ((int)(Helpers.PixelsToMeters(widthPixels))).ToString();
            _height = ((int)(Helpers.PixelsToMeters(heightPixels))).ToString();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Width":

                        return IsWidthOk();

                    case "Height":

                        return IsHeightOk();
                }

                return String.Empty;
            }
        }

        private string IsWidthOk()
        {
            if (String.IsNullOrWhiteSpace(_width))
                return Localization.Resources.Bit_Validate_Empty;

            if (!CommonValidate.IsOkAsInt(_width))
            {
                return Localization.Resources.Bit_Validate_Format;
            }

            int width = Int32.Parse(_width);
            width = (int)Helpers.MetersToPixes(width);

            if (width < Helpers.MinCanvasWidth)
                return String.Format(Localization.Resources.ViewModel_PlanSizeChange_minWidth, Math.Floor(Helpers.PixelsToMeters(Helpers.MinCanvasWidth)));
            if (width > Helpers.MaxCanvasWidth)
                return String.Format(Localization.Resources.ViewModel_PlanSizeChange_maxWidth, Math.Floor(Helpers.PixelsToMeters(Helpers.MinCanvasWidth)));

            return String.Empty;
        }

        private string IsHeightOk()
        {
            if (String.IsNullOrWhiteSpace(_height))
                return Localization.Resources.Bit_Validate_Empty;

            if (!CommonValidate.IsOkAsInt(_height))
            {
                return Localization.Resources.Bit_Validate_Format;
            }

            int height = Int32.Parse(_height);
            height = (int)(Helpers.MetersToPixes(height));

            if (height < Helpers.MinCanvasHeight)
                return String.Format(Localization.Resources.ViewModel_PlanSizeChange_minHeight, Math.Floor(Helpers.PixelsToMeters(Helpers.MinCanvasHeight)));
            if (height > Helpers.MaxCanvasHeight)
                return String.Format(Localization.Resources.ViewModel_PlanSizeChange_maxHeight, Math.Floor(Helpers.PixelsToMeters(Helpers.MaxCanvasHeight)));

            return String.Empty;
        }

        public string Error { get; private set; }
        public object ResultData { get; set; }

        private void CancelCommandExecuted()
        {
            ResultData = null;
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return String.IsNullOrEmpty(IsWidthOk()) && String.IsNullOrEmpty(IsHeightOk());
        }

        private void OkCommandExecuted()
        {
            ResultData = new Size(Double.Parse(_width), Double.Parse(_height));
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }
    }
}