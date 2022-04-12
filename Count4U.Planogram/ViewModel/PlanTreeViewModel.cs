using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Planogram.Lib;
using Count4U.Planogram.Lib.Enums;
using Microsoft.Practices.Prism.Regions;
using System.Linq;
using System;
using Count4U.Common.Extensions;

namespace Count4U.Planogram.ViewModel
{
    public class PlanTreeViewModel : CBIContextBaseViewModel
    {
        private DrawingCanvas _drawingCanvas;

        private readonly ObservableCollection<PlanTreeItemViewModel> _items;
        private PlanTreeItemViewModel _rootShelf;
        private PlanTreeItemViewModel _rootWindow;
        private PlanTreeItemViewModel _rootWall;
        private PlanTreeItemViewModel _rootLocation;
        private PlanTreeItemViewModel _rootText;
        private PlanTreeItemViewModel _rootPicture;

        private bool _isGeneratedByCode;

        public PlanTreeViewModel(
            IContextCBIRepository contextCbiRepository)
            : base(contextCbiRepository)
        {
            _items = new ObservableCollection<PlanTreeItemViewModel>();
        }

        public ObservableCollection<PlanTreeItemViewModel> Items
        {
            get { return _items; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _drawingCanvas.CanvasObjectAdded -= DrawingCanvas_CanvasObjectAdded;
            _drawingCanvas.CanvasObjectDeleted -= DrawingCanvas_CanvasObjectDeleted;
            _drawingCanvas.CanvasObjectRenamed -= DrawingCanvas_CanvasObjectRenamed;
            _drawingCanvas.SelectedObjectsChanged -= DrawingCanvas_SelectedObjectsChanged;
        }

        public void Init(DrawingCanvas drawingCanvas)
        {
            _drawingCanvas = drawingCanvas;

            _drawingCanvas.CanvasObjectAdded += DrawingCanvas_CanvasObjectAdded;
            _drawingCanvas.CanvasObjectDeleted += DrawingCanvas_CanvasObjectDeleted;
            _drawingCanvas.CanvasObjectRenamed += DrawingCanvas_CanvasObjectRenamed;
            _drawingCanvas.SelectedObjectsChanged += DrawingCanvas_SelectedObjectsChanged;
            _drawingCanvas.CanvasObjectLockChanged += DrawingCanvas_CanvasObjectLockChanged;

            Build();
        }

        void DrawingCanvas_CanvasObjectAdded(object sender, DrawingObject drawingObject)
        {
            AddDrawingObject(drawingObject);
        }

        void DrawingCanvas_CanvasObjectDeleted(object sender, DrawingObject drawingObject)
        {
            PlanTreeItemViewModel item = _items.FlattenHierarchyNodes(r => r.Children).FirstOrDefault(r => r.Code == drawingObject.Inner.Code);

            if (item != null)
            {
                item.Parent.Children.Remove(item);
            }
        }

        void DrawingCanvas_CanvasObjectRenamed(object sender, DrawingObject drawingObject)
        {
            PlanTreeItemViewModel item = _items.FlattenHierarchyNodes(r => r.Children).FirstOrDefault(r => r.Code == drawingObject.Inner.Code);

            if (item != null)
            {
                string text = GetNodeTextFromDrawingObject(drawingObject);

                if (item.Text != text)
                {
                    item.Text = text;
                    item.SetRenameTextSilent(text);
                }
            }
        }

        void DrawingCanvas_SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (_isGeneratedByCode) return;

            _items.FlattenHierarchyNodes(r => r.Children).ToList().ForEach(r => r.SetIsCheckedSilent(false));

            foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.IsSelected))
            {
                PlanTreeItemViewModel item = _items.FlattenHierarchyNodes(r => r.Children).FirstOrDefault(r => r.Code == drawingObject.Inner.Code);
                if (item != null)
                {
                    item.SetIsCheckedSilent(true);
                }
            }
        }

        void DrawingCanvas_CanvasObjectLockChanged(object sender, DrawingObject drawingObject)
        {
            if (_isGeneratedByCode) return;

            PlanTreeItemViewModel item = _items.FlattenHierarchyNodes(r => r.Children).FirstOrDefault(r => r.Code == drawingObject.Inner.Code);

            if (item != null)
            {
                item.SetIsLockedSilent(drawingObject.Inner.IsLocked);
            }
        }

        private void Build()
        {
            _rootShelf = new PlanTreeItemViewModel(null);
            _rootShelf.IsExpanded = false;
            _rootShelf.Text = Localization.Resources.ViewModel_PlanTree_nodeShelves;
            _rootShelf.LockVisible = false;
            _rootShelf.CanRename = false;
            _rootShelf.CanNotRenameAtAll = true;

            _items.Add(_rootShelf);

            foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.Inner.PlanType == enPlanObjectType.Shelf))
            {
                AddDrawingObject(drawingObject);
            }

            _rootLocation = new PlanTreeItemViewModel(null);
            _rootLocation.IsExpanded = false;
            _rootLocation.Text = Localization.Resources.ViewModel_PlanTree_nodeLocations;
            _rootLocation.LockVisible = false;
            _rootLocation.CanRename = false;
            _rootLocation.CanNotRenameAtAll = true;

            _items.Add(_rootLocation);

            foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.Inner.PlanType == enPlanObjectType.Location))
            {
                AddDrawingObject(drawingObject);
            }

            _rootWall = new PlanTreeItemViewModel(null);
            _rootWall.IsExpanded = false;
            _rootWall.Text = Localization.Resources.ViewModel_PlanTree_nodeWalls;
            _rootWall.LockVisible = false;
            _rootWall.CanRename = false;
            _rootWall.CanNotRenameAtAll = true;

            _items.Add(_rootWall);

            foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.Inner.PlanType == enPlanObjectType.Wall))
            {
                AddDrawingObject(drawingObject);
            }

            _rootWindow = new PlanTreeItemViewModel(null);
            _rootWindow.IsExpanded = false;
            _rootWindow.Text = Localization.Resources.ViewModel_PlanTree_nodeWindows;
            _rootWindow.LockVisible = false;
            _rootWindow.CanRename = false;
            _rootWindow.CanNotRenameAtAll = true;

            _items.Add(_rootWindow);

            foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.Inner.PlanType == enPlanObjectType.Window))
            {
                AddDrawingObject(drawingObject);
            }

            _rootText = new PlanTreeItemViewModel(null);
            _rootText.IsExpanded = false;
            _rootText.Text = Localization.Resources.ViewModel_PlanTree_nodeText;
            _rootText.LockVisible = false;
            _rootText.CanRename = false;
            _rootText.CanNotRenameAtAll = true;

            _items.Add(_rootText);

            foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.Inner.PlanType == enPlanObjectType.Text))
            {
                AddDrawingObject(drawingObject);
            }

            _rootPicture = new PlanTreeItemViewModel(null);
            _rootPicture.IsExpanded = false;
            _rootPicture.Text = Localization.Resources.ViewModel_PlanTree_nodePicture;
            _rootPicture.LockVisible = false;
            _rootPicture.CanRename = false;
            _rootPicture.CanNotRenameAtAll = true;

            _items.Add(_rootPicture);

            foreach (DrawingObject drawingObject in _drawingCanvas.Objects.Where(r => r.Inner.PlanType == enPlanObjectType.Picture))
            {
                AddDrawingObject(drawingObject);
            }
        }

        private void AddDrawingObject(DrawingObject drawingObject)
        {
            PlanTreeItemViewModel rootNode = null;
            string text = GetNodeTextFromDrawingObject(drawingObject);
            bool canRename = false;
            switch (drawingObject.Inner.PlanType)
            {
                case enPlanObjectType.Shelf:
                    rootNode = _rootShelf;
                    canRename = true;
                    break;
                case enPlanObjectType.Wall:
                    rootNode = _rootWall;
                    canRename = true;
                    break;
                case enPlanObjectType.Window:
                    rootNode = _rootWindow;
                    canRename = true;
                    break;
                case enPlanObjectType.Location:
                    rootNode = _rootLocation;
                    canRename = false;
                    break;
                case enPlanObjectType.Text:
                    rootNode = _rootText;
                    canRename = true;
                    break;
                case enPlanObjectType.Picture:
                    rootNode = _rootPicture;
                    canRename = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            PlanTreeItemViewModel item = new PlanTreeItemViewModel(rootNode);
            item.Text = text;
            item.IsExpanded = true;
            item.Code = drawingObject.Inner.Code;
            item.IsCheckedVisible = true;
            item.IsCheckedChanged += TreeViewItem_IsCheckedChanged;
            item.IsLockedChanged += TreeViewItem_IsLockedChanged;
            item.TextChanged += TreeViewItem_TextChanged;
            item.LockVisible = true;
            item.IsLocked = drawingObject.Inner.IsLocked;
            item.CanRename = item.IsLocked == false && canRename;
            item.CanNotRenameAtAll = canRename == false;            
            item.SetRenameTextSilent(item.Text);

            rootNode.Children.Add(item);
        }

        private void TreeViewItem_IsLockedChanged(object sender, EventArgs e)
        {
            PlanTreeItemViewModel item = sender as PlanTreeItemViewModel;
            if (item != null)
            {
                DrawingObject drawing = _drawingCanvas.Objects.FirstOrDefault(r => r.Inner.Code == item.Code);
                if (drawing != null)
                {
                    _isGeneratedByCode = true;
                    _drawingCanvas.SetIsLocked(drawing, item.IsLocked);
                    _isGeneratedByCode = false;
                }
            }
        }

        void TreeViewItem_TextChanged(object sender, EventArgs e)
        {
            PlanTreeItemViewModel item = sender as PlanTreeItemViewModel;
            if (item != null)
            {
                DrawingObject drawing = _drawingCanvas.Objects.FirstOrDefault(r => r.Inner.Code == item.Code);
                if (drawing != null)
                {
                    _drawingCanvas.SetName(drawing, item.Text);
                }
            }
        }

        void TreeViewItem_IsCheckedChanged(object sender, EventArgs e)
        {
            PlanTreeItemViewModel item = sender as PlanTreeItemViewModel;
            if (item != null)
            {
                DrawingObject drawing = _drawingCanvas.Objects.FirstOrDefault(r => r.Inner.Code == item.Code);
                if (drawing != null)
                {
                    _isGeneratedByCode = true;
                    drawing.IsSelected = item.IsChecked;
                    _isGeneratedByCode = false;
                    if (item.IsChecked)
                    {
                        _drawingCanvas.ScrollIntoView(drawing);
                    }
                }
            }
        }

        private string GetNodeTextFromDrawingObject(DrawingObject drawingObject)
        {
            string text = String.Empty;
             switch (drawingObject.Inner.PlanType)
            {
                case enPlanObjectType.Shelf:
                    text = String.IsNullOrEmpty(drawingObject.Inner.PlanName) ? Localization.Resources.ViewModel_PlanTreeItem_nodeUnnamed : drawingObject.Inner.PlanName;
                    break;
                case enPlanObjectType.Wall:
                    text = String.IsNullOrEmpty(drawingObject.Inner.PlanName) ? Localization.Resources.ViewModel_PlanTreeItem_nodeWall : drawingObject.Inner.PlanName;
                    break;
                case enPlanObjectType.Window:
                    text = String.IsNullOrEmpty(drawingObject.Inner.PlanName) ? Localization.Resources.ViewModel_PlanTreeItem_nodeWindow : drawingObject.Inner.PlanName;
                    break;
                case enPlanObjectType.Location:
                    text = String.IsNullOrEmpty(drawingObject.Inner.PlanName) ? Localization.Resources.ViewModel_PlanTreeItem_nodeLocation : drawingObject.Inner.PlanName;
                    break;
                case enPlanObjectType.Text:
                    text = String.IsNullOrEmpty(drawingObject.Inner.PlanName) ? Localization.Resources.ViewModel_PlanTreeItem_nodeText : drawingObject.Inner.PlanName;
                    break;
                case enPlanObjectType.Picture:
                    text = String.IsNullOrEmpty(drawingObject.Inner.PlanName) ? Localization.Resources.ViewModel_PlanTreeItem_nodePicture : drawingObject.Inner.PlanName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return text;
        }
    }
}