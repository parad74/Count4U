using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Count4U.Common.Constants;
using Count4U.Common.Converters;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.View.DragDrop;
using Count4U.Common.View.DragDrop.Utilities;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Planogram.Lib;
using Count4U.Planogram.Lib.Enums;
using Count4U.Planogram.Lib.Infrastructure;
using Count4U.Planogram.View.PlanObjects;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using Microsoft.Practices.Unity;
using NLog;
using Count4U.Common.Extensions;

namespace Count4U.Planogram.ViewModel
{
    public class PlanCanvasViewModel : CBIContextBaseViewModel, IDataErrorInfo, IDragSource
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IUnitPlanRepository _unitPlanRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IIturRepository _iturRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IIturAnalyzesRepository _iturAnalyzesRepository;
        private readonly IUnitPlanValueRepository _unitPlanValueRepository;
        private readonly UICommandRepository _commandRepository;
        private readonly IUnityContainer _container;
        private readonly IDBSettings _dbSettings;

        private readonly DelegateCommand _saveCommand;
        private readonly DelegateCommand _cancelCommand;

        private readonly DelegateCommand _cloneCommand;
        private readonly DelegateCommand _copyCommand;
        private readonly DelegateCommand _pasteCommand;
        private readonly DelegateCommand _deleteCommand;

        private readonly DelegateCommand _iturAddCommand;
        private readonly DelegateCommand _iturDeleteCommand;
        private readonly DelegateCommand _iturClearCommand;
        private readonly DelegateCommand _infoCommand;

        private readonly DelegateCommand _locationAssignCommand;
        private readonly DelegateCommand _textAssignCommand;
        private readonly DelegateCommand _pictureAssignCommand;

        private readonly DelegateCommand _alignLeftCommand;
        private readonly DelegateCommand _alignRightCommand;
        private readonly DelegateCommand _alignTopCommand;
        private readonly DelegateCommand _alignBottomCommand;
        private readonly DelegateCommand _alignSameWidthCommand;
        private readonly DelegateCommand _alignSameHeightCommand;

        private readonly DelegateCommand _bringForwardommand;
        private readonly DelegateCommand _sendBackwardCommand;

        private DrawingCanvas _drawingCanvas;

        private readonly ObservableCollection<CanvasToolItemViewModel> _tools;
        private readonly ObservableCollection<PlanPictureToolItemViewModel> _pictures;

        private readonly ObservableCollection<ScaleItemViewModel> _scaleItems;
        private ScaleItemViewModel _scaleSelectedItem;
        private string _scaleText;

        private readonly DelegateCommand _sizeChangeCommand;
        private string _sizeText;

        private bool _generatedByCode = false;

        private Popup _popup;

        private UnitPlans _unitPlansCache;
        private Iturs _itursCache;
        private Locations _locationsCache;

        //itur code as key
        private readonly Dictionary<string, IturPlanItem> _iturPlan;

        private readonly StringToBrushConverter stringToBrushConverter;

        private List<IturAnalyzesSimple> _iturAnalyzesSimple;

        private readonly List<DrawingInfo> _copyInfo;

        private List<DrawingObject> _selected;

        private bool? _isLocked;
        private bool _isLockedEnabled;

        private bool _isGeneratedByCode;

        public PlanCanvasViewModel(
            IContextCBIRepository contextCbiRepository,
            ModalWindowLauncher modalWindowLauncher,
            IUserSettingsManager userSettingsManager,
            IUnitPlanRepository unitPlanRepository,
            IUnitPlanValueRepository unitPlanValueRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IIturRepository iturRepository,
            INavigationRepository navigationRepository,
            ILocationRepository locationRepository,
            IIturAnalyzesRepository iturAnalyzesRepository,
            UICommandRepository _commandRepository,
            IUnityContainer container,
            IDBSettings dbSettings
            )
            : base(contextCbiRepository)
        {
            _dbSettings = dbSettings;
            _container = container;
            this._commandRepository = _commandRepository;
            _unitPlanValueRepository = unitPlanValueRepository;
            _iturAnalyzesRepository = iturAnalyzesRepository;
            _locationRepository = locationRepository;
            _navigationRepository = navigationRepository;
            _iturRepository = iturRepository;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _unitPlanRepository = unitPlanRepository;
            _userSettingsManager = userSettingsManager;
            _modalWindowLauncher = modalWindowLauncher;
            _tools = new ObservableCollection<CanvasToolItemViewModel>();
            _scaleItems = new ObservableCollection<ScaleItemViewModel>()
                {
                    new ScaleItemViewModel() {Text = "800%", Value = 800},
                    new ScaleItemViewModel() {Text = "400%", Value = 400},
                    new ScaleItemViewModel() {Text = "200%", Value = 200},
                    new ScaleItemViewModel() {Text = "150%", Value = 150},
                    new ScaleItemViewModel() {Text = "100%", Value = 100},
                    new ScaleItemViewModel() {Text = "66.67%", Value = 66.67},
                    new ScaleItemViewModel() {Text = "50%", Value = 50},
                    new ScaleItemViewModel() {Text = "33.33%", Value = 33.33},
                    new ScaleItemViewModel() {Text = "25%", Value = 25},
                    new ScaleItemViewModel() {Text = "12.5%", Value = 12.5},
                };
            _pictures = new ObservableCollection<PlanPictureToolItemViewModel>();

            _scaleSelectedItem = _scaleItems.FirstOrDefault(r => r.Value == 100);
            _scaleText = _scaleSelectedItem == null ? String.Empty : _scaleSelectedItem.Text;
            _sizeChangeCommand = new DelegateCommand(SizeChangeCommandExecuted);

            _iturAddCommand = new DelegateCommand(IturAddCommandExecuted, IturAddCommandCanExecute);
            _iturDeleteCommand = new DelegateCommand(IturDeleteCommandExecuted, IturDeleteCommandCanExecute);
            _infoCommand = new DelegateCommand(InfoCommandExecuted, InfoCommandCanExecute);

            _saveCommand = new DelegateCommand(SaveCommandExecuted, SaveCommandCanExecute);
            _cancelCommand = new DelegateCommand(CancelCommandExecuted, CancelCommandCanExecute);

            _locationAssignCommand = new DelegateCommand(LocationAssignCommandExecuted, LocationAssignCommandCanExecute);
            _textAssignCommand = new DelegateCommand(TextAssignCommandExecuted, TextAssignCommandCanExecute);
            _pictureAssignCommand = new DelegateCommand(PictureAssignCommandExecuted, PictureAssignCommandCanExecute);

            _deleteCommand = new DelegateCommand(DeleteCommandExecuted, DeleteCommandCanExecute);
            _cloneCommand = new DelegateCommand(CloneCommandExecuted, CloneCommandCanExecute);
            _copyCommand = new DelegateCommand(CopyCommandExecuted, CopyCommandCanExecute);
            _pasteCommand = new DelegateCommand(PasteCommandExecuted, PasteCommandCanExecute);

            _alignLeftCommand = _commandRepository.Build(enUICommand.AlignLeft, AlignLeftCommandExecuted, AlignLeftCommandCanExecute);
            _alignRightCommand = _commandRepository.Build(enUICommand.AlignRight, AlignRightCommandExecuted, AlignRightCommandCanExecute);
            _alignTopCommand = _commandRepository.Build(enUICommand.AlignTop, AlignTopCommandExecuted, AlignTopCommandCanExecute);
            _alignBottomCommand = _commandRepository.Build(enUICommand.AlignBottom, AlignBottomCommandExecuted, AlignBottomCommandCanExecute);
            _alignSameWidthCommand = _commandRepository.Build(enUICommand.AlignSameWidth, AlignSameWidthCommandExecuted, AlignSameWidthCommandCanExecute);
            _alignSameHeightCommand = _commandRepository.Build(enUICommand.AlignSameHeight, AlignSameHeightCommandExecuted, AlignSameHeightCommandCanExecute);

            _bringForwardommand = _commandRepository.Build(enUICommand.BringForward, BringForwardCommandExecuted, BringForwardCommandCanExecute);
            _sendBackwardCommand = _commandRepository.Build(enUICommand.SendBackward, SendBackwardCommandExecuted, SendBackwardCommandCanExecute);

            _iturClearCommand = new DelegateCommand(IturClearCommandExecuted, IturClearCommandCanExecute);

            _iturPlan = new Dictionary<string, IturPlanItem>();

            stringToBrushConverter = new StringToBrushConverter();

            _copyInfo = new List<DrawingInfo>();
        }

        public FrameworkElement View { get; set; }

        public ObservableCollection<CanvasToolItemViewModel> Tools
        {
            get { return _tools; }
        }

        public ObservableCollection<PlanPictureToolItemViewModel> Pictures
        {
            get { return _pictures; }
        }

        public ObservableCollection<ScaleItemViewModel> ScaleItems
        {
            get { return _scaleItems; }
        }

        public ScaleItemViewModel ScaleSelectedItem
        {
            get { return _scaleSelectedItem; }
            set
            {
                _scaleSelectedItem = value;
                RaisePropertyChanged(() => ScaleSelectedItem);

                if (_scaleSelectedItem != null)
                {
                    _drawingCanvas.Scrolling.SetZoom(_scaleSelectedItem.Value, false);
                }
            }
        }

        public string ScaleText
        {
            get { return _scaleText; }
            set
            {
                _scaleText = value;
                RaisePropertyChanged(() => ScaleText);
            }
        }

        public DelegateCommand SizeChangeCommand
        {
            get { return _sizeChangeCommand; }
        }

        public string SizeText
        {
            get { return _sizeText; }
            set
            {
                _sizeText = value;
                RaisePropertyChanged(() => SizeText);
            }
        }

        public DelegateCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public DelegateCommand IturAddCommand
        {
            get { return _iturAddCommand; }
        }

        public DelegateCommand IturDeleteCommand
        {
            get { return _iturDeleteCommand; }
        }

        public DelegateCommand SaveCommand
        {
            get { return _saveCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        private UnitPlans DbUnitPlans
        {
            get
            {
                if (_unitPlansCache == null)
                {
                    LoadDbUnitPlans();
                }
                return _unitPlansCache;
            }
        }

        private Iturs DbIturs
        {
            get
            {
                if (_itursCache == null)
                {
                    LoadDbIturs();
                }
                return _itursCache;
            }
        }

        private Locations DbLocations
        {
            get
            {
                if (_locationsCache == null)
                {
                    LoadDbLocations();
                }
                return _locationsCache;
            }
        }

        public bool DrawingCanvasIsDirty
        {
            get { return _drawingCanvas.IsDirty; }
        }

        private void LoadDbUnitPlans()
        {
            _unitPlansCache = _unitPlanRepository.GetUnitPlans(base.GetDbPath);
        }

        private void LoadDbIturs()
        {
            _itursCache = _iturRepository.GetIturs(base.GetDbPath);
        }

        private void LoadDbLocations()
        {
            _locationsCache = _locationRepository.GetLocations(base.GetDbPath);
        }

        public DelegateCommand InfoCommand
        {
            get { return _infoCommand; }
        }

        public DelegateCommand CloneCommand
        {
            get { return _cloneCommand; }
        }

        public DelegateCommand CopyCommand
        {
            get { return _copyCommand; }
        }

        public DelegateCommand PasteCommand
        {
            get { return _pasteCommand; }
        }

        public DelegateCommand IturClearCommand
        {
            get { return _iturClearCommand; }
        }

        public bool NoOneObjectSelected
        {
            get { return _selected.Count == 0; }
        }

        public bool NotSingleObjectSelected
        {
            get { return _selected.Count != 1; }
        }

        public bool SingleObjectSelected
        {
            get { return _selected.Count == 1; }
        }

        public bool OneOrMoreObjectsSelected
        {
            get { return _selected.Count >= 1; }
        }

        public DelegateCommand AlignLeftCommand
        {
            get { return _alignLeftCommand; }
        }

        public DelegateCommand AlignRightCommand
        {
            get { return _alignRightCommand; }
        }

        public DelegateCommand AlignTopCommand
        {
            get { return _alignTopCommand; }
        }

        public DelegateCommand AlignBottomCommand
        {
            get { return _alignBottomCommand; }
        }

        public DelegateCommand AlignSameWidthCommand
        {
            get { return _alignSameWidthCommand; }
        }

        public DelegateCommand AlignSameHeightCommand
        {
            get { return _alignSameHeightCommand; }
        }

        public bool? IsLocked
        {
            get { return _isLocked; }
            set
            {
                _isLocked = value;
                RaisePropertyChanged(() => IsLocked);

                if (_isLocked.HasValue)
                {
                    _isGeneratedByCode = true;
                    foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.IsSelected))
                    {
                        _drawingCanvas.SetIsLocked(drawingObject, _isLocked.Value);
                    }
                    _isGeneratedByCode = false;
                }
            }
        }

        public bool IsLockedEnabled
        {
            get { return _isLockedEnabled; }
            set
            {
                _isLockedEnabled = value;
                RaisePropertyChanged(() => IsLockedEnabled);
            }
        }

        public DelegateCommand BringForwardCommand
        {
            get { return _bringForwardommand; }
        }

        public DelegateCommand SendBackwardCommand
        {
            get { return _sendBackwardCommand; }
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

                return true;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<PopupWindowCloseEvent>().Subscribe(PopupWindowClose);
            _eventAggregator.GetEvent<ApplicationDeactivatedEvent>().Subscribe(PopupWindowClose);

            Utils.SetCursor(true);

            _selected = new List<DrawingObject>();

            Task.Factory.StartNew(() =>
                {
                    LoadDbUnitPlans();
                    LoadDbIturs();
                    LoadDbLocations();

                    Utils.RunOnUI(() =>
                        {
                            BuildTools();
                            Build();

                            PlanTreeViewModel treeViewModel = Utils.GetViewModelFromRegion<PlanTreeViewModel>(Common.RegionNames.PlanogramTree, _regionManager);
                            PlanPropertiesViewModel propertiesViewModel = Utils.GetViewModelFromRegion<PlanPropertiesViewModel>(Common.RegionNames.PlanogramProperties, _regionManager);
                            treeViewModel.Init(_drawingCanvas);
                            propertiesViewModel.Init(_drawingCanvas);

                            BuildPictures();

                            Utils.SetCursor(false);
                        });

                    BuildIturSumQuantity();
				}).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<PopupWindowCloseEvent>().Unsubscribe(PopupWindowClose);
            _eventAggregator.GetEvent<ApplicationDeactivatedEvent>().Unsubscribe(PopupWindowClose);

            if (_drawingCanvas != null)
            {
                _drawingCanvas.ToolChanged -= DrawingCanvas_ToolChanged;

                _drawingCanvas.CanvasSizeChanged -= DrawingCanvas_CanvasSizeChanged;
                _drawingCanvas.SelectedObjectsChanged -= DrawingCanvas_SelectedObjectsChanged;
                _drawingCanvas.CanvasCommandExecuted -= DrawingCanvas_CanvasCommandExecuted;
                _drawingCanvas.IsDirtyChanged -= DrawingCanvas_IsDirtyChanged;
                _drawingCanvas.DrawingChanged -= DrawingCanvas_DrawingChanged;

                if (_drawingCanvas.Scrolling != null)
                {
                    _drawingCanvas.Scrolling.ZoomChanged -= Scrolling_ZoomChanged;
                }

            }
        }

        public void Init(DrawingCanvas drawingCanvas)
        {
            _drawingCanvas = drawingCanvas;

            _drawingCanvas.ToolChanged += DrawingCanvas_ToolChanged;
            _drawingCanvas.Scrolling.ZoomChanged += Scrolling_ZoomChanged;
            _drawingCanvas.CanvasSizeChanged += DrawingCanvas_CanvasSizeChanged;
            _drawingCanvas.SelectedObjectsChanged += DrawingCanvas_SelectedObjectsChanged;
            _drawingCanvas.CanvasCommandExecuted += DrawingCanvas_CanvasCommandExecuted;
            _drawingCanvas.IsDirtyChanged += DrawingCanvas_IsDirtyChanged;
            _drawingCanvas.DrawingChanged += DrawingCanvas_DrawingChanged;
            _drawingCanvas.CanvasObjectAdded += DrawingCanvas_CanvasObjectAdded;
            _drawingCanvas.CanvasObjectLockChanged += DrawingCanvas_CanvasObjectLockChanged;
            _drawingCanvas.CanvasObjectRenamed += DrawingCanvas_CanvasObjectRenamed;
        }       

        void DrawingCanvas_CanvasCommandExecuted(object sender, enCommand command, DrawingCanvasCommandResult result)
        {
            switch (command)
            {
                case enCommand.Delete:
                    if (_deleteCommand.CanExecute())
                        _deleteCommand.Execute();
                    break;
                case enCommand.CodeGenerate:
                    result.Result = this.CodeNewGenerate();
                    break;
                case enCommand.Statistic:

                    PlanSpecialObject special = sender as PlanSpecialObject;
                    if (special != null)
                    {
                        ShowObjectStatistic(special);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("command");
            }
        }

        private void BuildTools()
        {
            CanvasToolItemViewModel pointer = new CanvasToolItemViewModel();
            pointer.IsChecked = true;
            pointer.Image = @"/Count4U.Media;component/Icons/plan/plan_cursor2.png";
            pointer.ToolType = enToolType.Pointer;
            pointer.Title = Localization.Resources.ViewModel_PlanCanvas_btnPointer;
            _tools.Add(pointer);

            CanvasToolItemViewModel hand = new CanvasToolItemViewModel();
            hand.IsChecked = false;
            hand.Image = @"/Count4U.Media;component/Icons/plan/plan_hand.png";
            hand.ToolType = enToolType.Hand;
            hand.Title = Localization.Resources.ViewModel_PlanCanvas_btnHand;
            _tools.Add(hand);

            CanvasToolItemViewModel zoom = new CanvasToolItemViewModel();
            zoom.IsChecked = false;
            zoom.Image = @"/Count4U.Media;component/Icons/plan/plan_zoom.png";
            zoom.ToolType = enToolType.Zoom;
            zoom.Title = Localization.Resources.ViewModel_PlanCanvas_btnZoom;
            _tools.Add(zoom);

            CanvasToolItemViewModel shelf = new CanvasToolItemViewModel();
            shelf.IsChecked = false;
            shelf.Image = @"/Count4U.Media;component/Icons/plan/plan_shelf.png";
            shelf.ToolType = enToolType.Shelf;
            shelf.Title = Localization.Resources.ViewModel_PlanCanvas_btnShelf;
            _tools.Add(shelf);

            CanvasToolItemViewModel wall = new CanvasToolItemViewModel();
            wall.IsChecked = false;
            wall.Image = @"/Count4U.Media;component/Icons/plan/plan_wall.png";
            wall.ToolType = enToolType.Wall;
            wall.Title = Localization.Resources.ViewModel_PlanCanvas_btnWall;
            _tools.Add(wall);

            CanvasToolItemViewModel window = new CanvasToolItemViewModel();
            window.IsChecked = false;
            window.Image = @"/Count4U.Media;component/Icons/plan/plan_window.png";
            window.ToolType = enToolType.Window;
            window.Title = Localization.Resources.ViewModel_PlanCanvas_btnWindow;
            _tools.Add(window);

            CanvasToolItemViewModel location = new CanvasToolItemViewModel();
            location.IsChecked = false;
            location.Image = @"/Count4U.Media;component/Icons/plan/plan_location.png";
            location.ToolType = enToolType.Location;
            location.Title = Localization.Resources.ViewModel_PlanCanvas_btnLocation;
            _tools.Add(location);

            CanvasToolItemViewModel text = new CanvasToolItemViewModel();
            text.IsChecked = false;
            text.Image = @"/Count4U.Media;component/Icons/plan/plan_text.png";
            text.ToolType = enToolType.Text;
            text.Title = Localization.Resources.ViewModel_PlanCanvas_btnText;
            _tools.Add(text);

            CanvasToolItemViewModel picture = new CanvasToolItemViewModel();
            picture.IsChecked = false;
            picture.Image = @"/Count4U.Media;component/Icons/plan/plan_picture.png";
            picture.ToolType = enToolType.Picture;
            picture.Title = Localization.Resources.ViewModel_PlanCanvas_btnPicture;
            _tools.Add(picture);

            foreach (CanvasToolItemViewModel tool in _tools)
            {
                tool.PropertyChanged += Tool_PropertyChanged;
            }
        }

        private void BuildPictures()
        {
            return;

            foreach (string file in Helpers.EnumeratePictures(_dbSettings))
            {
                PlanPictureToolItemViewModel item = new PlanPictureToolItemViewModel();
                item.Path = file;

                _pictures.Add(item);
            }         
        }

        void DrawingCanvas_CanvasSizeChanged(object sender, EventArgs e)
        {
            BuildSizeText();
        }

        void DrawingCanvas_ToolChanged(object sender, System.EventArgs e)
        {
            _generatedByCode = true;

            CanvasToolItemViewModel tool = _tools.FirstOrDefault(r => r.ToolType == _drawingCanvas.Tool);
            if (tool != null)
            {
                tool.IsChecked = true;

                foreach (CanvasToolItemViewModel canvasTool in _tools)
                {
                    if (canvasTool != tool)
                    {
                        canvasTool.IsChecked = false;
                    }
                }
            }

            _generatedByCode = false;
        }

        void Tool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
                return;

            if (_generatedByCode)
                return;

            CanvasToolItemViewModel tool = _tools.FirstOrDefault(r => r == sender);
            if (tool == null) return;

            foreach (CanvasToolItemViewModel canvasTool in _tools)
            {
                if (canvasTool != tool)
                {
                    _generatedByCode = true;
                    canvasTool.IsChecked = false;
                    _generatedByCode = false;
                }
            }

            _drawingCanvas.Tool = tool.ToolType;
        }

        void Scrolling_ZoomChanged(object sender, System.EventArgs e)
        {
            AdjustZoomCombo(_drawingCanvas.Scrolling.ZoomPercentage);

            ScaleText = String.Format("{0}%", _drawingCanvas.Scrolling.ZoomPercentage);
        }

        void DrawingCanvas_IsDirtyChanged(object sender, EventArgs e)
        {
            _saveCommand.RaiseCanExecuteChanged();
        }

        void DrawingCanvas_DrawingChanged(object sender, EventArgs e)
        {

        }

        void DrawingCanvas_CanvasObjectAdded(object sender, DrawingObject drawingObject)
        {
            if (drawingObject.InnerSpecial != null)
            {
                SetBorderColorForSpecialObject(drawingObject.InnerSpecial);
                SetBackgroundColorForSpecialObject(drawingObject.InnerSpecial);
            }

            if (drawingObject.InnerLocation != null)
            {
                SetBackgroundColorForLocationObject(drawingObject.InnerLocation);
            }
        }

        void DrawingCanvas_CanvasObjectLockChanged(object sender, DrawingObject drawingObject)
        {
            _alignLeftCommand.RaiseCanExecuteChanged();
            _alignRightCommand.RaiseCanExecuteChanged();
            _alignTopCommand.RaiseCanExecuteChanged();
            _alignBottomCommand.RaiseCanExecuteChanged();
            _alignSameWidthCommand.RaiseCanExecuteChanged();
            _alignSameHeightCommand.RaiseCanExecuteChanged();

            _bringForwardommand.RaiseCanExecuteChanged();
            _sendBackwardCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged(() => CanEditDimensions);
            RaisePropertyChanged(() => CanEditName);

            if (_isGeneratedByCode) return;

            BuildIsLocked();
        }

        void DrawingCanvas_CanvasObjectRenamed(object sender, DrawingObject drawingObject)
        {

        }

        private void BuildSizeText()
        {
            double metersWidth = Helpers.PixelsToMeters(_drawingCanvas.GetWidth());
            double metersHeight = Helpers.PixelsToMeters(_drawingCanvas.GetHeight());

            SizeText = String.Format("{0}m x {1}m", (int)Math.Floor(metersWidth), (int)Math.Floor(metersHeight));
        }

        private void SizeChangeCommandExecuted()
        {
            var p = new Dictionary<string, string>();
            p.Add(Common.NavigationSettings.PlanogramWidth, _drawingCanvas.GetWidth().ToString());
            p.Add(Common.NavigationSettings.PlanogramHeight, _drawingCanvas.GetHeight().ToString());

            object result = _modalWindowLauncher.StartModalWindow(
                Common.ViewNames.PlanSizeChangeView,
                WindowTitles.PlanSizeChange,
                270, 140,
                ResizeMode.NoResize, p,
                null,
                270, 140);

            if (result is Size)
            {
                Size size = (Size)result;

                double pixelsWidth = Helpers.MetersToPixes(size.Width);
                double pixelsHeight = Helpers.MetersToPixes(size.Height);

                _drawingCanvas.SetSize(pixelsWidth, pixelsHeight);
                _drawingCanvas.IsDirty = true;
            }
        }

        private bool DeleteCommandCanExecute()
        {
            return _selected.Any();
        }

        private void DeleteCommandExecuted()
        {
            string message = Localization.Resources.ViewModel_PlanCanvas_msgDelete;
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Warning, _userSettingsManager);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.IsSelected).ToList())
                {
                    _drawingCanvas.Delete(drawingObject);

                    if (drawingObject.InnerSpecial != null)
                    {
                        foreach (IturPlanItem item in _iturPlan.Values.Where(r => r.PlanCode == drawingObject.InnerSpecial.Code))
                        {
                            item.PlanCode = String.Empty;
                            item.IsDirty = true;
                        }
                    }
                }
                _drawingCanvas.IsDirty = true;
            }

            
        }

        void DrawingCanvas_SelectedObjectsChanged(object sender, EventArgs e)
        {
            _selected = _drawingCanvas.Objects.Where(r => r.IsSelected).ToList();

            _deleteCommand.RaiseCanExecuteChanged();
            _iturAddCommand.RaiseCanExecuteChanged();
            _iturDeleteCommand.RaiseCanExecuteChanged();
            _infoCommand.RaiseCanExecuteChanged();
            _cloneCommand.RaiseCanExecuteChanged();
            _copyCommand.RaiseCanExecuteChanged();
            _iturClearCommand.RaiseCanExecuteChanged();
            _alignLeftCommand.RaiseCanExecuteChanged();
            _alignRightCommand.RaiseCanExecuteChanged();
            _alignTopCommand.RaiseCanExecuteChanged();
            _alignBottomCommand.RaiseCanExecuteChanged();
            _alignSameWidthCommand.RaiseCanExecuteChanged();
            _alignSameHeightCommand.RaiseCanExecuteChanged();
            _bringForwardommand.RaiseCanExecuteChanged();
            _sendBackwardCommand.RaiseCanExecuteChanged();
            _locationAssignCommand.RaiseCanExecuteChanged();
            _textAssignCommand.RaiseCanExecuteChanged();
            PictureAssignCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged(() => SingleObjectSelected);
            RaisePropertyChanged(() => NotSingleObjectSelected);
            RaisePropertyChanged(() => OneOrMoreObjectsSelected);
            RaisePropertyChanged(() => NoOneObjectSelected);

            RaisePropertyChanged(() => IsVisibleIturCommands);
            RaisePropertyChanged(() => IsVisibleLocationCommands);
            RaisePropertyChanged(() => IsVisibleTextCommands);
            RaisePropertyChanged(() => IsVisiblePictureCommands);

            RaisePropertyChanged(() => CanEditDimensions);
            RaisePropertyChanged(() => CanEditName);

            IsLockedEnabled = _selected.Any();
            BuildIsLocked();
        }

        private void BuildIsLocked()
        {
            bool? isLocked = null;
            if (_selected.Any() == false)
            {
                isLocked = false;
            }

            if (_selected.All(r => r.Inner.IsLocked) == true)
            {
                isLocked = true;
            }
            else
            {
                if (_selected.All(r => r.Inner.IsLocked == false))
                {
                    isLocked = false;
                }
                else
                {
                    isLocked = null;
                }
            }

            _isLocked = isLocked;
            RaisePropertyChanged(() => IsLocked);
        }

        private bool IturAddCommandCanExecute()
        {
            return _drawingCanvas.Objects.Count(r => r.IsSelected) == 1 &&
                   _drawingCanvas.Objects.Single(r => r.IsSelected).InnerSpecial != null;
        }

        private void IturAddCommandExecuted()
        {
            DrawingObject selected = _drawingCanvas.Objects.FirstOrDefault(r => r.IsSelected);
            if (selected == null) return;
            PlanSpecialObject special = selected.InnerSpecial;
            if (special == null) return;

            var p = new Dictionary<string, string>();

            string firstPrefix = String.Empty;
            Itur firstItur = _itursCache.FirstOrDefault(r => !String.IsNullOrWhiteSpace(r.NumberPrefix));

            if (firstItur != null)
            {
                firstPrefix = firstItur.NumberPrefix;
            }

            p.Add(Common.NavigationSettings.IturPrefix, firstPrefix);

            object result = _modalWindowLauncher.StartModalWindow(
                Common.ViewNames.PlanIturAddView,
                WindowTitles.PlanIturAdd,
                270, 160,
                ResizeMode.NoResize, p,
                null,
                270, 160);

            Tuple<int, List<int>> res = result as Tuple<int, List<int>>;

            if (res != null)
            {
                int prefixN = res.Item1;
                List<int> numbers = res.Item2;

                List<int> numbersWhichNotInDb = new List<int>();
                Dictionary<int, string> numbersAttachedToOtherPlan = new Dictionary<int, string>();
                List<string> codesOK = new List<string>();

                foreach (int number in numbers)
                {
                    string suffix = UtilsItur.SuffixFromNumber(number);
                    string prefix = UtilsItur.PrefixFromString(prefixN.ToString());
                    string iturCode = UtilsItur.CodeFromPrefixAndSuffix(prefix, suffix);
                    bool addedToOther = false;

                    if (DbIturs.Any(r => r.IturCode == iturCode) == false)
                    {
                        numbersWhichNotInDb.Add(number);
                        continue;
                    }

                    if (_iturPlan.ContainsKey(iturCode))
                    {
                        if (!String.IsNullOrEmpty(_iturPlan[iturCode].PlanCode) && _iturPlan[iturCode].PlanCode != special.Code)
                        {
                            numbersAttachedToOtherPlan.Add(number, _iturPlan[iturCode].PlanCode);
                            addedToOther = true;
                        }
                    }

                    if (!addedToOther)
                    {
                        codesOK.Add(iturCode);
                    }
                }

                if (numbersWhichNotInDb.Any())
                {
                    string numbersWhichNotInDbStr = CommaDashStringParser.Reverse(numbersWhichNotInDb);
                    string message = String.Format(Localization.Resources.ViewModel_PlanCanvas_msgMissingInDb, numbersWhichNotInDbStr);
                    UtilsMisc.ShowMessageBox(message, MessageBoxButton.OK, MessageBoxImage.Warning, _userSettingsManager);
                }
                if (numbersAttachedToOtherPlan.Any())
                {
                    string numbersAttachedToOtherPlanStr = CommaDashStringParser.Reverse(numbersAttachedToOtherPlan.Keys.ToList());
                    string message = String.Format(Localization.Resources.ViewModel_PlanCanvas_msgLinkedToOtherPlan, numbersAttachedToOtherPlanStr);
                    MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, _userSettingsManager);

                    if (messageBoxResult == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    if (messageBoxResult == MessageBoxResult.No)
                    {

                    }
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        foreach (int number in numbersAttachedToOtherPlan.Keys.ToList())
                        {
                            string suffix = UtilsItur.SuffixFromNumber(number);
                            string prefix = UtilsItur.PrefixFromString(prefixN.ToString());
                            string iturCode = UtilsItur.CodeFromPrefixAndSuffix(prefix, suffix);

                            _iturPlan[iturCode].PlanCode = special.Code;
                            _iturPlan[iturCode].IsDirty = true;
                        }

                        foreach (string planCode in numbersAttachedToOtherPlan.Values.Distinct())
                        {
                            DrawingObject drawing = _drawingCanvas.Objects.FirstOrDefault(r => r.InnerSpecial != null && r.InnerSpecial.Code == planCode);
                            if (drawing != null && drawing.InnerSpecial != null)
                            {
                                SetBorderColorForSpecialObject(drawing.InnerSpecial);
                                SetBackgroundColorForSpecialObject(drawing.InnerSpecial);
                            }
                        }

                        _drawingCanvas.IsDirty = true;
                    }
                }

                if (codesOK.Any())
                {
                    foreach (string code in codesOK)
                    {
                        _iturPlan[code] = new IturPlanItem() { IsDirty = true, PlanCode = special.Code, Itur = DbIturs.FirstOrDefault(r => r.IturCode == code) };
                    }

                    _drawingCanvas.IsDirty = true;
                }

                SetBorderColorForSpecialObject(special);
                SetBackgroundColorForSpecialObject(special);
            }
        }

        private bool IturDeleteCommandCanExecute()
        {
            return _drawingCanvas.Objects.Count(r => r.IsSelected) == 1 &&
                    _drawingCanvas.Objects.Single(r => r.IsSelected).InnerSpecial != null;
        }

        private void IturDeleteCommandExecuted()
        {
            DrawingObject selected = _drawingCanvas.Objects.FirstOrDefault(r => r.IsSelected);
            if (selected == null) return;
            PlanSpecialObject special = selected.InnerSpecial;
            if (special == null) return;

            var p = new Dictionary<string, string>();

            string firstPrefix = String.Empty;
            if (_iturPlan.Any(r => r.Value.PlanCode == special.Code))
            {
                firstPrefix = _iturPlan.First(r => r.Value.PlanCode == special.Code).Value.Itur.NumberPrefix;
            }

            p.Add(Common.NavigationSettings.IturPrefix, firstPrefix);

            object result = _modalWindowLauncher.StartModalWindow(
                Common.ViewNames.PlanIturAddView,
                WindowTitles.PlanIturDelete,
                270, 160,
                ResizeMode.NoResize, p,
                null,
                270, 160);

            Tuple<int, List<int>> res = result as Tuple<int, List<int>>;

            if (res != null)
            {
                int prefixN = res.Item1;
                List<int> numbers = res.Item2;

                List<int> numbersWhichNotInDb = new List<int>();
                List<int> numbersAttachedToOtherPlan = new List<int>();
                List<string> codesOK = new List<string>();

                foreach (int number in numbers)
                {
                    string suffix = UtilsItur.SuffixFromNumber(number);
                    string prefix = UtilsItur.PrefixFromString(prefixN.ToString());
                    string iturCode = UtilsItur.CodeFromPrefixAndSuffix(prefix, suffix);
                    bool addedToOther = false;

                    if (DbIturs.Any(r => r.IturCode == iturCode) == false)
                    {
                        numbersWhichNotInDb.Add(number);
                        continue;
                    }

                    if (_iturPlan.ContainsKey(iturCode))
                    {
                        if (_iturPlan[iturCode].PlanCode != special.Code)
                        {
                            numbersAttachedToOtherPlan.Add(number);
                            addedToOther = true;
                        }
                    }

                    if (!addedToOther)
                    {
                        codesOK.Add(iturCode);
                    }
                }

                if (numbersWhichNotInDb.Any())
                {
                    string numbersWhichNotInDbStr = CommaDashStringParser.Reverse(numbersWhichNotInDb);
                    string message = String.Format(Localization.Resources.ViewModel_PlanCanvas_msgMissingInDb, numbersWhichNotInDbStr);
                    UtilsMisc.ShowMessageBox(message, MessageBoxButton.OK, MessageBoxImage.Warning, _userSettingsManager);
                }
                if (numbersAttachedToOtherPlan.Any())
                {

                }

                if (codesOK.Any())
                {
                    foreach (string code in codesOK)
                    {
                        _iturPlan[code].PlanCode = String.Empty;
                        _iturPlan[code].IsDirty = true;
                    }

                    SetBorderColorForSpecialObject(special);
                    SetBackgroundColorForSpecialObject(special);

                    _drawingCanvas.IsDirty = true;
                }
            }
        }

        private void CancelCommandExecuted()
        {
            using (new CursorWait())
            {
                this.Build();

                _drawingCanvas.IsDirty = false;

                _selected = new List<DrawingObject>();

                PlanPropertiesViewModel propertiesViewModel = Utils.GetViewModelFromRegion<PlanPropertiesViewModel>(Common.RegionNames.PlanogramProperties, _regionManager);
                propertiesViewModel.BuildSelectedInfo();
            }
        }

        private bool CancelCommandCanExecute()
        {
            return true;
        }

        private bool SaveCommandCanExecute()
        {
            return _drawingCanvas.IsDirty;
        }

        private void SaveCommandExecuted()
        {
            using (new CursorWait())
            {
                try
                {
                    UnitPlans dbUnitPlans = _unitPlanRepository.GetUnitPlans(base.GetDbPath);

                    UnitPlan planogramUnit = dbUnitPlans.FirstOrDefault(r => r.ObjectCode == enPlanObjectType.Planogram.ToString());

                    if (planogramUnit == null)
                    {
                        planogramUnit = new UnitPlan();
                        planogramUnit.UnitPlanCode = CodeNewGenerate();
                        planogramUnit.ObjectCode = enPlanObjectType.Planogram.ToString();
                        planogramUnit.Width = _drawingCanvas.GetWidth();
                        planogramUnit.Height = _drawingCanvas.GetHeight();
                        planogramUnit.Zoom = (int)_drawingCanvas.Scrolling.ZoomPercentage;
                        planogramUnit.StartX = _drawingCanvas.Scrolling.OffsetX;
                        planogramUnit.StartY = _drawingCanvas.Scrolling.OffsetY;

                        _unitPlanRepository.Insert(planogramUnit, base.GetDbPath);
                    }
                    else
                    {
                        planogramUnit.Width = _drawingCanvas.GetWidth();
                        planogramUnit.Height = _drawingCanvas.GetHeight();
                        planogramUnit.Zoom = (int)_drawingCanvas.Scrolling.ZoomPercentage;
                        planogramUnit.StartX = _drawingCanvas.Scrolling.OffsetX;
                        planogramUnit.StartY = _drawingCanvas.Scrolling.OffsetY;

                        _unitPlanRepository.Update(planogramUnit, base.GetDbPath);
                    }

                    foreach (UnitPlan unitPlan in dbUnitPlans.Where(r => r.ObjectCode != enPlanObjectType.Planogram.ToString()).ToList())
                    {
                        DrawingObject drawingObject = _drawingCanvas.Objects.FirstOrDefault(r => r.Inner.Code == unitPlan.UnitPlanCode);
                        if (drawingObject == null) //object was deleted
                        {
                            _unitPlanRepository.Delete(unitPlan, base.GetDbPath);
                        }
                        else
                        {
                            if (drawingObject.IsDirty) //udpate
                            {
                                FillUnitPlanFromDrawing(unitPlan, drawingObject);

                                _unitPlanRepository.Update(unitPlan, base.GetDbPath);

                                drawingObject.IsDirty = false;
                            }
                        }
                    }

                    //insert
                    foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.IsDirty))
                    {
                        UnitPlan unitPlan = new UnitPlan();
                        unitPlan.UnitPlanCode = drawingObject.Inner.Code;

                        FillUnitPlanFromDrawing(unitPlan, drawingObject);

                        _unitPlanRepository.Insert(unitPlan, base.GetDbPath);

                        drawingObject.IsDirty = false;
                    }

                    SaveIturs();

                    _unitPlanValueRepository.FillUnitPlanValues(base.GetDbPath);

                    _drawingCanvas.IsDirty = false;

                    _unitPlansCache = null;
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("SaveCommandExecuted", exc);
                }
            }
        }

        private void SaveIturs()
        {
            try
            {
                Iturs itursToSave = new Iturs();

                foreach (IturPlanItem item in _iturPlan.Where(r => r.Value.IsDirty).Select(r => r.Value))
                {
                    item.Itur.UnitPlanCode = item.PlanCode;
                    itursToSave.Add(item.Itur);
                    item.IsDirty = false;
                }

                _iturRepository.Update(itursToSave, base.GetDbPath);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("SaveIturs", exc);
            }
        }

        private void FillUnitPlanFromDrawing(UnitPlan unitPlan, DrawingObject drawing)
        {
            unitPlan.ObjectCode = drawing.Inner.PlanType.ToString();
            unitPlan.StartX = Canvas.GetLeft(drawing);
            unitPlan.StartY = Canvas.GetTop(drawing);
            unitPlan.Width = drawing.Width;
            unitPlan.Height = drawing.Height;
            unitPlan.Rotate = (int)drawing.RotateTransform.Angle;
            unitPlan.Name = drawing.Inner.PlanName;
            unitPlan.Lock = drawing.Inner.IsLocked;
            unitPlan.ZIndex = Panel.GetZIndex(drawing) == Helpers.ZIndexBack ? Helpers.ZIndexBack : Helpers.ZIndexFront;

            if (drawing.InnerLocation != null)
            {
                unitPlan.LocationCode = drawing.InnerLocation.LocationCode;
            }

            if (drawing.InnerText != null)
            {
                unitPlan.Value = drawing.InnerText.Text;
                unitPlan.FontSize = drawing.InnerText.TextFontSize;
                unitPlan.Color = drawing.InnerText.TextFontColor;
            }

            if (drawing.InnerPicture != null)
            {
                unitPlan.Picture = drawing.InnerPicture.FileName;
            }
        }

        private void Build()
        {
            try
            {
                _drawingCanvas.Clear();
                _iturPlan.Clear();

                UnitPlan planogramUnit = this.DbUnitPlans.FirstOrDefault(r => r.ObjectCode == enPlanObjectType.Planogram.ToString());

                if (planogramUnit == null)
                {
                    _drawingCanvas.SetSize(Helpers.DefaultCanvasWidth, Helpers.DefaultCanvasHeight);
                    _drawingCanvas.Scrolling.SetZoom(Helpers.DefaultCanvasZoom, false);
                }
                else
                {
                    _drawingCanvas.SetSize(planogramUnit.Width, planogramUnit.Height);
                    _drawingCanvas.Scrolling.SetZoom(planogramUnit.Zoom, false);
                    _drawingCanvas.Scrolling.SetOffset(planogramUnit.StartX, planogramUnit.StartY);
                }

                foreach (UnitPlan unitPlan in this.DbUnitPlans.Where(r => r.ObjectCode != enPlanObjectType.Planogram.ToString()))
                {
                    enPlanObjectType planObjectType;
                    if (Enum.TryParse(unitPlan.ObjectCode, true, out planObjectType) == false)
                    {
                        continue;
                    }

                    PlanObjectDecorator decorator = new PlanObjectDecorator(_drawingCanvas);

                    decorator.Width = unitPlan.Width;
                    decorator.Height = unitPlan.Height;

                    Canvas.SetLeft(decorator, unitPlan.StartX);
                    Canvas.SetTop(decorator, unitPlan.StartY);

                    decorator.RotateTransform.Angle = unitPlan.Rotate;

                    PlanObject planObject = Helpers.CreatePlanObjectByType(planObjectType, _drawingCanvas, _container, unitPlan.UnitPlanCode);                    
                    planObject.PlanName = unitPlan.Name;
                    planObject.IsLocked = unitPlan.Lock.HasValue ? unitPlan.Lock.Value : false;

                    int zIndex = unitPlan.ZIndex == Helpers.ZIndexBack ? Helpers.ZIndexBack : Helpers.ZIndexFront;
                    Panel.SetZIndex(decorator, zIndex);

                    decorator.Add(planObject);

                    PlanSpecialObject special = planObject as PlanSpecialObject;
                    if (special != null)
                    {
                        foreach (Itur itur in DbIturs.Where(r => r.UnitPlanCode == unitPlan.UnitPlanCode))
                        {
                            _iturPlan[itur.IturCode] = new IturPlanItem() { IsDirty = false, Itur = itur, PlanCode = unitPlan.UnitPlanCode };
                        }
                    }

                    PlanLocation location = planObject as PlanLocation;
                    if (location != null)
                    {
                        location.LocationCode = unitPlan.LocationCode;
                        location.Location = this.DbLocations.FirstOrDefault(r => r.Code == unitPlan.LocationCode);
                    }

                    PlanText text = planObject as PlanText;
                    if (text != null)
                    {
                        text.Text = unitPlan.Value;
                        text.TextFontSize = unitPlan.FontSize;
                        text.TextFontColor = unitPlan.Color;
                    }

                    PlanPicture picture = planObject as PlanPicture;
                    if (picture != null)
                    {
                        picture.FileName = unitPlan.Picture;
                    }

                    _drawingCanvas.Add(decorator);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
            }
        }

        private string CodeNewGenerate()
        {
            List<string> list = _drawingCanvas.Objects.Select(r => r.Inner.Code.ToLower()).ToList();
            list.AddRange(this.DbUnitPlans.Select(r => r.UnitPlanCode.ToLower()));
            list = list.Distinct().ToList();

            string result;
            do
            {
                result = Helpers.CodeGenerate();

            } while (list.Contains(result.ToLower()));

            return result;
        }

        private void ShowObjectStatistic(PlanSpecialObject specialObject)
        {
            if (specialObject == null) return;

            if (_drawingCanvas.Tool != enToolType.Pointer) return;

            List<Itur> itursOfSpecial = new List<Itur>();
            itursOfSpecial.AddRange(_iturPlan.Where(r => r.Value.PlanCode == specialObject.Code).Select(r => r.Value.Itur));

            List<PlanInfoItem> info = new List<PlanInfoItem>();

            foreach (Itur itur in itursOfSpecial)
            {
                PlanInfoItem item = new PlanInfoItem();
                item.Itur = itur;

                if (_iturAnalyzesSimple != null)
                {
                    IturAnalyzesSimple simple = _iturAnalyzesSimple.FirstOrDefault(r => r.IturCode == itur.IturCode);
                    item.IturAnalyze = simple ?? new IturAnalyzesSimple() { IturCode = itur.IturCode, QuantityEdit = 0 };
                }

                info.Add(item);
            }

            PlanInfo planInfo = new PlanInfo();
            planInfo.Info = info;
            planInfo.TotalIturs = info.Count;
            if (info.Any(r => r.IturAnalyze == null))
            {
                planInfo.TotalItems = -1;
            }
            else
            {
                planInfo.TotalItems = info.Sum(r => r.IturAnalyze == null ? 0 : (r.IturAnalyze.QuantityEdit.HasValue ? r.IturAnalyze.QuantityEdit.Value : 0));
            }

            planInfo.Process = CalculateDoneForSpecialObject(specialObject);

            _popup = new Popup();
            _popup.Width = 200;
            _popup.Height = 350;
            _popup.Placement = PlacementMode.Mouse;
            _popup.StaysOpen = true;
            _popup.AllowsTransparency = true;

            ContentControl cc = new ContentControl();
            _popup.Child = cc;

            RegionManager.SetRegionManager(_popup, _regionManager);
            RegionManager.SetRegionName(cc, Common.RegionNames.PlanogramInfo);

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            UtilsConvert.AddObjectToQuery(query, _navigationRepository, planInfo);
            _regionManager.RequestNavigate(Common.RegionNames.PlanogramInfo, new Uri(Common.ViewNames.PlanInfoView + query, UriKind.Relative));

            _popup.MouseDown += (s, e) => e.Handled = true;

            _popup.IsOpen = true;
        }

        private void PopupWindowClose(object o)
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _regionManager.Regions.Remove(Common.RegionNames.PlanogramInfo);
            }
        }

        private bool InfoCommandCanExecute()
        {
            return _selected.Count == 1 &&
                  _selected.First().InnerSpecial != null;
        }

        private void InfoCommandExecuted()
        {
            DrawingObject drawing = _drawingCanvas.Objects.FirstOrDefault(r => r.IsSelected);
            if (drawing != null && drawing.InnerSpecial != null)
            {
                ShowObjectStatistic(drawing.InnerSpecial);
            }
        }

        private bool CloneCommandCanExecute()
        {
            return _selected.Any();
        }

        private void CloneCommandExecuted()
        {
            try
            {
                using (new CursorWait())
                {
                    List<PlanObjectDecorator> added = new List<PlanObjectDecorator>();

                    foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.IsSelected).ToList())
                    {
                        PlanObjectDecorator decorator = new PlanObjectDecorator(_drawingCanvas);

                        decorator.Width = drawingObject.GetWidth();
                        decorator.Height = drawingObject.GetHeight();
                        Canvas.SetLeft(decorator, drawingObject.GetLeft() + Helpers.CopiedObjectShiftX);
                        Canvas.SetTop(decorator, drawingObject.GetTop() + Helpers.CopiedObjectShiftY);
                        decorator.RotateTransform.Angle = drawingObject.GetAngle();
                        decorator.IsDirty = true;

                        string code = CodeNewGenerate();
                        PlanObject planObject = Helpers.CreatePlanObjectByType(drawingObject.Inner.PlanType, _drawingCanvas, _container, code);
                        planObject.PlanName = drawingObject.Inner.PlanName;
                        planObject.IsLocked = false;

                        Panel.SetZIndex(decorator, Helpers.GetDefaultZIndex(planObject.PlanType));

                        if (drawingObject.InnerText != null)
                        {
                            PlanText planText = planObject as PlanText;
                            if (planText != null)
                            {
                                planText.Text = drawingObject.InnerText.Text;
                                planText.TextFontSize = drawingObject.InnerText.TextFontSize;
                                planText.TextFontColor = drawingObject.InnerText.TextFontColor;
                            }
                        }

                        if (drawingObject.InnerPicture != null)
                        {
                            PlanPicture planPicture = planObject as PlanPicture;
                            if (planPicture != null)
                            {
                                planPicture.FileName = drawingObject.InnerPicture.FileName;
                            }
                        }

                        decorator.Add(planObject);
                        _drawingCanvas.Add(decorator);

                        added.Add(decorator);
                    }

                    _drawingCanvas.IsDirty = true;

                    _drawingCanvas.UnselectAll();
                    foreach (PlanObjectDecorator decorator in added)
                    {
                        decorator.IsSelected = true;
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("CloneCommandExecuted", exc);
            }
        }

        private bool CopyCommandCanExecute()
        {
            return _selected.Any();
        }

        private void CopyCommandExecuted()
        {
            _copyInfo.Clear();

            foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.IsSelected))
            {
                DrawingInfo info = new DrawingInfo();

                info.SourceObjectCode = drawingObject.Inner.Code;
                info.Left = drawingObject.GetLeft();
                info.Top = drawingObject.GetTop();
                info.Width = drawingObject.GetWidth();
                info.Height = drawingObject.GetHeight();
                info.Angle = drawingObject.GetAngle();
                info.PlanType = drawingObject.Inner.PlanType;
                info.PlanName = drawingObject.Inner.PlanName;

                if (drawingObject.InnerText != null)
                {
                    info.PlanText = drawingObject.InnerText.Text;
                    info.PlanFontSize = drawingObject.InnerText.TextFontSize;
                    info.PlanFontColor = drawingObject.InnerText.TextFontColor;
                }

                if (drawingObject.InnerPicture != null)
                {
                    info.PlanPicture = drawingObject.InnerPicture.FileName;
                }

                _copyInfo.Add(info);
            }

            _pasteCommand.RaiseCanExecuteChanged();
        }

        private bool PasteCommandCanExecute()
        {
            return _copyInfo.Any();
        }

        private void PasteCommandExecuted()
        {
            List<PlanObjectDecorator> added = new List<PlanObjectDecorator>();

            foreach (DrawingInfo info in _copyInfo)
            {
                PlanObjectDecorator decorator = new PlanObjectDecorator(_drawingCanvas);

                decorator.Width = info.Width;
                decorator.Height = info.Height;
                Canvas.SetLeft(decorator, info.Left + Helpers.CopiedObjectShiftX);
                Canvas.SetTop(decorator, info.Top + Helpers.CopiedObjectShiftY);
                decorator.RotateTransform.Angle = info.Angle;
                decorator.IsDirty = true;

                string code = CodeNewGenerate();
                PlanObject planObject = Helpers.CreatePlanObjectByType(info.PlanType, _drawingCanvas, _container, code);                
                planObject.PlanName = info.PlanName;
                planObject.IsLocked = false;

                if (!String.IsNullOrWhiteSpace(info.PlanText))
                {
                    PlanText planText = planObject as PlanText;
                    if (planText != null)
                    {
                        planText.Text = info.PlanText;
                        planText.TextFontSize = info.PlanFontSize;
                        planText.TextFontColor = info.PlanFontColor;
                    }
                }

                if (!String.IsNullOrWhiteSpace(info.PlanPicture))
                {
                    PlanPicture planPicture = planObject as PlanPicture;
                    if (planPicture != null)
                    {
                        planPicture.FileName = info.PlanPicture;
                    }
                }

                Panel.SetZIndex(decorator, Helpers.GetDefaultZIndex(planObject.PlanType));

                decorator.Add(planObject);
                _drawingCanvas.Add(decorator);

                added.Add(decorator);

                //send source object "back" by zindex
//                DrawingObject sourceObject = _drawingCanvas.Objects.FirstOrDefault(r => r.Inner.Code == info.SourceObjectCode);
//                if (sourceObject != null)
//                {
//                    Panel.SetZIndex(sourceObject, Helpers.ZIndexBack);
//                }
            }

            _drawingCanvas.IsDirty = true;

            _drawingCanvas.UnselectAll();
            foreach (PlanObjectDecorator decorator in added)
            {
                decorator.IsSelected = true;
            }

            _copyInfo.Clear();
            _pasteCommand.RaiseCanExecuteChanged();
        }


        private void SetBorderColorForSpecialObject(PlanSpecialObject special)
        {
            string color = String.Empty;

            if (_iturPlan.Any(r => r.Value.PlanCode == special.Code))
            {
                Itur itur = _iturPlan.First(r => r.Value.PlanCode == special.Code).Value.Itur;

                Location location = DbLocations.FirstOrDefault(r => r.Code == itur.LocationCode);

                if (location != null)
                {
                    color = location.BackgroundColor;
                }
            }

            special.SetBorderColor(String.IsNullOrEmpty(color) ? null :
                stringToBrushConverter.Convert(color, null, null, null) as SolidColorBrush);
        }

        private void SetBackgroundColorForSpecialObject(PlanSpecialObject special)
        {
            bool isEmpty = _iturPlan.Any(r => r.Value.PlanCode == special.Code) == false;
            double done = CalculateDoneForSpecialObject(special);
            special.SetBackgroundColor(done, new SolidColorBrush(Helpers.FromPercentageToColor(done, _userSettingsManager, isEmpty)));
        }

        private void SetBackgroundColorForLocationObject(PlanLocation locationObject)
        {
            string color = String.Empty;

            if (locationObject.Location != null)
            {
                color = locationObject.Location.BackgroundColor;
            }

            locationObject.SetBackgroundColor(String.IsNullOrEmpty(color) ? null :
                stringToBrushConverter.Convert(color, null, null, null) as SolidColorBrush);
        }

        private void SetBackgroundColorForTextObject(PlanText textObject)
        {

        }

        private double CalculateDoneForSpecialObject(PlanSpecialObject special)
        {
            Iturs iturs = Iturs.FromEnumerable(_iturPlan.Where(r => r.Value.PlanCode == special.Code).Select(r => r.Value.Itur));
            double done = _iturRepository.GetIturTotalDone(iturs, base.GetDbPath);

            return done;
        }

        private void BuildIturSumQuantity()
        {
            try
            {
                _iturAnalyzesSimple = _iturAnalyzesRepository.GetIturSumQuantityEditByIturCode(null, base.GetDbPath, true).ToList();
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildIturSumQuantity", exc);
            }
        }

        private bool IturClearCommandCanExecute()
        {
            if (_drawingCanvas.Objects.Count(r => r.IsSelected) != 1)
                return false;

            DrawingObject drawing = _drawingCanvas.Objects.FirstOrDefault(r => r.IsSelected);
            if (drawing == null) return false;
            PlanSpecialObject special = drawing.InnerSpecial;
            if (special == null) return false;

            return _iturPlan.Values.Any(r => r.PlanCode == special.Code);
        }

        private void IturClearCommandExecuted()
        {
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(
                Localization.Resources.ViewModel_PlanCanvas_msgClearAll,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                _userSettingsManager);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (new CursorWait())
                {
                    DrawingObject drawing = _drawingCanvas.Objects.FirstOrDefault(r => r.IsSelected);
                    if (drawing == null) return;
                    PlanSpecialObject special = drawing.InnerSpecial;

                    foreach (KeyValuePair<string, IturPlanItem> kvp in _iturPlan.Where(r => r.Value.PlanCode == special.Code))
                    {
                        _iturPlan[kvp.Key].PlanCode = String.Empty;
                        _iturPlan[kvp.Key].IsDirty = true;
                    }

                    SetBorderColorForSpecialObject(special);
                    SetBackgroundColorForSpecialObject(special);

                    _drawingCanvas.IsDirty = true;
                }
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {

                }

                return String.Empty;
            }
        }

        public string Error { get; private set; }

        public DelegateCommand LocationAssignCommand
        {
            get { return _locationAssignCommand; }
        }

        public bool IsVisibleIturCommands
        {
            get { return _selected.Any() && _selected.All(r => r.InnerSpecial != null); }
        }

        public bool IsVisibleLocationCommands
        {
            get { return _selected.Any() && _selected.All(r => r.InnerLocation != null); }
        }

        public bool IsVisibleTextCommands
        {
            get { return _selected.Any() && _selected.All(r => r.InnerText != null); }
        }

        public bool IsVisiblePictureCommands
        {
            get { return _selected.Any() && _selected.All(r => r.InnerPicture != null); }
        }

        public DelegateCommand TextAssignCommand
        {
            get { return _textAssignCommand; }
        }

        public DelegateCommand PictureAssignCommand
        {
            get { return _pictureAssignCommand; }
        }
     
        private void AdjustZoomCombo(double dbZoom)
        {
            ScaleItemViewModel item = _scaleItems.FirstOrDefault(r => r.Value == dbZoom);

            _scaleSelectedItem = item;
            RaisePropertyChanged(() => ScaleSelectedItem);
        }

        private bool AlignLeftCommandCanExecute()
        {
            return _selected.Count > 1 && CanEditDimensions;
        }

        private void AlignLeftCommandExecuted()
        {
            _drawingCanvas.AlignLeft();
        }

        private bool AlignRightCommandCanExecute()
        {
            return _selected.Count > 1 && CanEditDimensions;
        }

        private void AlignRightCommandExecuted()
        {
            _drawingCanvas.AlignRight();
        }

        private bool AlignTopCommandCanExecute()
        {
            return _selected.Count > 1 && CanEditDimensions;
        }

        private void AlignTopCommandExecuted()
        {
            _drawingCanvas.AlignTop();
        }

        private bool AlignBottomCommandCanExecute()
        {
            return _selected.Count > 1 && CanEditDimensions;
        }

        private void AlignBottomCommandExecuted()
        {
            _drawingCanvas.AlignBottom();
        }

        private bool AlignSameWidthCommandCanExecute()
        {
            return _selected.Count > 1 && CanEditDimensions;
        }

        private void AlignSameWidthCommandExecuted()
        {
            _drawingCanvas.AlignSameWidth();
        }

        private bool AlignSameHeightCommandCanExecute()
        {
            return _selected.Count > 1 && CanEditDimensions;
        }

        private void AlignSameHeightCommandExecuted()
        {
            _drawingCanvas.AlignSameHeight();
        }

        private bool BringForwardCommandCanExecute()
        {
            return CanEditDimensions;
        }

        private void BringForwardCommandExecuted()
        {
            _drawingCanvas.BringForward();
        }

        private bool SendBackwardCommandCanExecute()
        {
            return CanEditDimensions;
        }

        private void SendBackwardCommandExecuted()
        {
            _drawingCanvas.SendBackward();
        }

        private bool LocationAssignCommandCanExecute()
        {
            return _selected.Count == 1 && _selected.Single().InnerLocation != null;
        }

        private void LocationAssignCommandExecuted()
        {
            DrawingObject drawing = _selected.FirstOrDefault();
            if (drawing == null || drawing.InnerLocation == null)
                return;

            Dictionary<string, string> p = new Dictionary<string, string>();

            Utils.AddContextToDictionary(p, base.Context);
            Utils.AddDbContextToDictionary(p, base.CBIDbContext);

            PlanAssignedLocations payload = new PlanAssignedLocations();
            payload.CurrentLocationCode = drawing.InnerLocation.LocationCode;
//            foreach (PlanLocation planLocation in _drawingCanvas.Objects.Where(r => r.InnerLocation != null).Select(r => r.InnerLocation))
//            {
//                payload.AssignedLocationCodes.Add(planLocation.LocationCode);
//            }

            UtilsConvert.AddObjectToDictionary(p, _navigationRepository, payload);

            object result = _modalWindowLauncher.StartModalWindow(
             Common.ViewNames.PlanLocationAssignView,
             WindowTitles.PlanogramLocationAssign,
             400, 300,
             ResizeMode.NoResize, p,
             null,
             400, 400);

            if (result == null)
                return;

            string locationCode = result as String;

            string locationName = String.Empty;
            Location location = this.DbLocations.FirstOrDefault(r => r.Code == locationCode);
            if (location != null)
            {
                locationName = location.Name;
            }

            drawing.InnerLocation.LocationCode = locationCode;
            drawing.InnerLocation.Location = location;
            _drawingCanvas.SetName(drawing, locationName);
            drawing.IsDirty = true;

            _drawingCanvas.IsDirty = true;

            SetBackgroundColorForLocationObject(drawing.InnerLocation);
        }

        private bool TextAssignCommandCanExecute()
        {
            return _selected.Count == 1 && _selected.Single().InnerText != null;
        }

        private void TextAssignCommandExecuted()
        {
            DrawingObject drawing = _selected.FirstOrDefault();
            if (drawing == null || drawing.InnerText == null)
                return;

            Dictionary<string, string> p = new Dictionary<string, string>();

            Utils.AddContextToDictionary(p, base.Context);
            Utils.AddDbContextToDictionary(p, base.CBIDbContext);

            PlanTextInfo textInfo = new PlanTextInfo();
            textInfo.Text = drawing.InnerText.Text;
            textInfo.FontSize = drawing.InnerText.TextFontSize;
            textInfo.FontColor = drawing.InnerText.TextFontColor;

            UtilsConvert.AddObjectToDictionary(p, _navigationRepository, textInfo);

            object result = _modalWindowLauncher.StartModalWindow(
             Common.ViewNames.PlanTextAssignView,
             WindowTitles.PlanogramTextAssign,
             350, 250,
             ResizeMode.NoResize, p,
             null,
             350, 250);

            if (result == null)
                return;

            textInfo = result as PlanTextInfo;
            if (textInfo == null) return;

            _drawingCanvas.SetText(drawing, textInfo.Text);
            drawing.InnerText.TextFontSize = textInfo.FontSize;
            drawing.InnerText.TextFontColor = textInfo.FontColor;

            drawing.IsDirty = true;
            _drawingCanvas.IsDirty = true;
        }

        private bool PictureAssignCommandCanExecute()
        {
            return _selected.Count == 1 && _selected.Single().InnerPicture != null;
        }

        private void PictureAssignCommandExecuted()
        {
            DrawingObject drawing = _selected.FirstOrDefault();
            if (drawing == null || drawing.InnerPicture == null)
                return;

            Dictionary<string, string> p = new Dictionary<string, string>();

            Utils.AddContextToDictionary(p, base.Context);
            Utils.AddDbContextToDictionary(p, base.CBIDbContext);

            PlanPictureInfo pictureInfo = new PlanPictureInfo();
            pictureInfo.FileName = drawing.InnerPicture.FileName;

            UtilsConvert.AddObjectToDictionary(p, _navigationRepository, pictureInfo);

            object result = _modalWindowLauncher.StartModalWindow(
             Common.ViewNames.PlanPictureAssignView,
             WindowTitles.PlanogramPictureAssign,
             920, 520,
             ResizeMode.CanResize, p,
             null,
             500, 500);

            if (result == null)
                return;

            pictureInfo = result as PlanPictureInfo;
            if (pictureInfo == null) return;

            _drawingCanvas.SetPicture(drawing, pictureInfo.FileName);

            drawing.IsDirty = true;
            _drawingCanvas.IsDirty = true;
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1)
            {
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
            }
            else if (itemCount > 1)
            {
                dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
            }

            dragInfo.Effects = (dragInfo.Data != null) ?
                                   DragDropEffects.Copy | DragDropEffects.Move :
                                   DragDropEffects.None;

            dragInfo.NonStandardAdorner = true;
        }

        public void Dropped(IDropInfo dropInfo)
        {
            
        }
    }
}