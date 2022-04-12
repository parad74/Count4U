using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Planogram.Lib;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Planogram.ViewModel
{
    public class PlanPropertiesViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private DrawingCanvas _drawingCanvas;

        private string _selectedX;
        private string _selectedY;
        private string _selectedWidth;
        private string _selectedHeight;
        private string _selectedAngle;
        private string _selectedName;
        private List<DrawingObject> _selected;

        public PlanPropertiesViewModel(
            IContextCBIRepository contextCbiRepository)
            : base(contextCbiRepository)
        {

        }

        public string SelectedX
        {
            get { return _selectedX; }
            set
            {
                _selectedX = value;
                RaisePropertyChanged(() => SelectedX);

                double x;
                if (Double.TryParse(_selectedX, out x))
                {
                    _drawingCanvas.SetSelectedX(Helpers.MetersToPixes(x));
                }
            }
        }

        public string SelectedY
        {
            get { return _selectedY; }
            set
            {
                _selectedY = value;
                RaisePropertyChanged(() => SelectedY);

                double y;
                if (Double.TryParse(_selectedY, out y))
                {
                    _drawingCanvas.SetSelectedY(Helpers.MetersToPixes(y));
                }
            }
        }

        public string SelectedWidth
        {
            get { return _selectedWidth; }
            set
            {
                _selectedWidth = value;
                RaisePropertyChanged(() => SelectedWidth);

                double width;
                if (Double.TryParse(_selectedWidth, out width))
                {
                    _drawingCanvas.SetSelectedWidth(Helpers.MetersToPixes(width));
                }
            }
        }

        public string SelectedHeight
        {
            get { return _selectedHeight; }
            set
            {
                _selectedHeight = value;
                RaisePropertyChanged(() => SelectedHeight);

                double height;
                if (Double.TryParse(_selectedHeight, out height))
                {
                    _drawingCanvas.SetSelectedHeight(Helpers.MetersToPixes(height));
                }
            }
        }

        public string SelectedAngle
        {
            get { return _selectedAngle; }
            set
            {
                _selectedAngle = value;
                RaisePropertyChanged(() => SelectedAngle);

                double angle;
                if (Double.TryParse(_selectedAngle, out angle))
                {
                    _drawingCanvas.SetSelectedAngle(angle);
                }
            }
        }

        public string SelectedName
        {
            get { return _selectedName; }
            set
            {
                _selectedName = value;
                RaisePropertyChanged(() => SelectedName);

                _drawingCanvas.SetSelectedName(_selectedName);
            }
        }

        public bool CanEditDimensions
        {
            get
            {

                if (_selected.Count == 0)
                    return false;

                if (_selected.All(r => r.Inner.IsLocked))
                    return false;

                return true;
            }
        }

        public bool CanEditName
        {
            get
            {
                if (_selected.Count != 1)
                    return false;

                if (_selected.Single().Inner.IsLocked)
                    return false;

                if (_selected.Single().InnerLocation != null)                
                    return false;

                if (_selected.Single().InnerPicture != null)
                    return false;

                return true;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _selected = new List<DrawingObject>();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _drawingCanvas.SelectedObjectsChanged -= DrawingCanvas_SelectedObjectsChanged;
            _drawingCanvas.CanvasObjectRenamed -= DrawingCanvas_CanvasObjectRenamed;
            _drawingCanvas.DrawingChanged -= DrawingCanvas_DrawingChanged;
            _drawingCanvas.CanvasObjectLockChanged -= DrawingCanvas_CanvasObjectLockChanged;
        }

        public void Init(DrawingCanvas drawingCanvas)
        {
            _drawingCanvas = drawingCanvas;

            _drawingCanvas.SelectedObjectsChanged += DrawingCanvas_SelectedObjectsChanged;
            _drawingCanvas.CanvasObjectRenamed += DrawingCanvas_CanvasObjectRenamed;
            _drawingCanvas.DrawingChanged += DrawingCanvas_DrawingChanged;
            _drawingCanvas.CanvasObjectLockChanged += DrawingCanvas_CanvasObjectLockChanged;
        }

        void DrawingCanvas_DrawingChanged(object sender, EventArgs e)
        {
            BuildSelectedInfo();
        }

        void DrawingCanvas_CanvasObjectLockChanged(object sender, DrawingObject drawingObject)
        {
            RaisePropertyChanged(() => CanEditDimensions);
            RaisePropertyChanged(() => CanEditName);
        }

        void DrawingCanvas_CanvasObjectRenamed(object sender, DrawingObject drawingObject)
        {
            List<DrawingObject> list = _drawingCanvas.Objects.Where(r => r.IsSelected).ToList();
            if (list.Count != 1) return;

            DrawingObject selected = list.FirstOrDefault();

            if (selected == null) return;

            if (selected.Inner.Code == drawingObject.Inner.Code)
            {
                _selectedName = drawingObject.Inner.PlanName;
                RaisePropertyChanged(() => SelectedName);
            }
        }

        void DrawingCanvas_SelectedObjectsChanged(object sender, EventArgs e)
        {
            BuildSelectedInfo();
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "SelectedX":
                        if (!UtilsConvert.IsOkAsDouble(_selectedX))
                            return Localization.Resources.ViewModel_PlanCanvas_msgValueIsIncorrect;
                        break;
                    case "SelectedY":
                        if (!UtilsConvert.IsOkAsDouble(_selectedX))
                            return Localization.Resources.ViewModel_PlanCanvas_msgValueIsIncorrect;
                        break;
                    case "SelectedWidth":
                        if (!UtilsConvert.IsOkAsDouble(_selectedX))
                            return Localization.Resources.ViewModel_PlanCanvas_msgValueIsIncorrect;
                        break;
                    case "SelectedHeight":
                        if (!UtilsConvert.IsOkAsDouble(_selectedX))
                            return Localization.Resources.ViewModel_PlanCanvas_msgValueIsIncorrect;
                        break;
                    case "SelectedAngle":
                        if (!UtilsConvert.IsOkAsDouble(_selectedX))
                            return Localization.Resources.ViewModel_PlanCanvas_msgValueIsIncorrect;
                        break;
                }

                return String.Empty;
            }
        }

        public string Error { get; private set; }

        public void BuildSelectedInfo()
        {
            _selected = _drawingCanvas.Objects.Where(r => r.IsSelected).ToList();
            DrawingObject selectedItem = _selected.FirstOrDefault();

            if (selectedItem == null || _selected.Count != 1)
            {
                this._selectedX = null;
                this._selectedY = null;
                this._selectedWidth = null;
                this._selectedHeight = null;
                this._selectedAngle = null;
                this._selectedName = String.Empty;
            }
            else
            {

                this._selectedX = Round(Helpers.PixelsToMeters(selectedItem.GetLeft())).ToString();
                this._selectedY = Round(Helpers.PixelsToMeters(selectedItem.GetTop())).ToString();
                this._selectedWidth = Round(Helpers.PixelsToMeters(selectedItem.GetWidth())).ToString();
                this._selectedHeight = Round(Helpers.PixelsToMeters(selectedItem.GetHeight())).ToString();
                this._selectedAngle = selectedItem.GetAngle().ToString();
                this._selectedName = selectedItem.Inner.PlanName;
            }

            RaisePropertyChanged(() => SelectedX);
            RaisePropertyChanged(() => SelectedY);
            RaisePropertyChanged(() => SelectedWidth);
            RaisePropertyChanged(() => SelectedHeight);
            RaisePropertyChanged(() => SelectedAngle);
            RaisePropertyChanged(() => SelectedName);

            RaisePropertyChanged(() => CanEditDimensions);
            RaisePropertyChanged(() => CanEditName);
        }

        private double Round(double d, int n = 2)
        {
            return Math.Round(d, n);
        }
    }
}