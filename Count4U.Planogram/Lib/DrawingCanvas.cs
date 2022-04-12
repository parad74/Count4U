using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Count4U.Common.View.DragDrop;
using Count4U.Planogram.Lib.Enums;
using Count4U.Planogram.Lib.Infrastructure;
using Count4U.Planogram.View;
using Count4U.Planogram.View.PlanObjects;
using Microsoft.Practices.Unity;
using Shapes = System.Windows.Shapes;
using Count4U.Planogram.ViewModel;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = Count4U.Common.View.DragDrop.IDropTarget;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Panel = System.Windows.Controls.Panel;

namespace Count4U.Planogram.Lib
{
    public class DrawingCanvas : Canvas, IDropTarget
    {
        public static readonly DependencyProperty ToolProperty;

        private readonly IUnityContainer _container;

        private readonly Scrolling _scrolling;
        private readonly Grid _grid;

        private readonly List<Tool> _tools;
        private Tool _toolCurrent;

        private readonly List<DrawingObject> _objects;

        private bool _isDirty;

        public DrawingCanvas(Scrolling scrolling, Grid grid, IUnityContainer container)
        {
            _container = container;
            this._grid = grid;
            this._scrolling = scrolling;
            this.Background = Brushes.White;

            this._objects = new List<DrawingObject>();
            this._tools = new List<Tool>();

            this._tools.Insert((int)enToolType.None, null);
            this._tools.Insert((int)enToolType.Pointer, new ToolPointer());
            this._tools.Insert((int)enToolType.Hand, new ToolHand(_scrolling));
            this._tools.Insert((int)enToolType.Zoom, new ToolZoom(_scrolling));
            this._tools.Insert((int)enToolType.Shelf, new ToolShelf());
            this._tools.Insert((int)enToolType.Wall, new ToolWall());
            this._tools.Insert((int)enToolType.Window, new ToolWindow());
            this._tools.Insert((int)enToolType.Location, new ToolLocation());
            this._tools.Insert((int)enToolType.Text, new ToolText());
            this._tools.Insert((int)enToolType.Picture, new ToolPicture(container));

            foreach (Tool tool in this._tools)
            {
                if (tool != null)
                    tool.DrawingChanged += Tool_DrawingChanged;
            }

            this.Tool = enToolType.Pointer;

            this.Loaded += DrawingCanvas_Loaded;

            this._scrolling.ScrollViewer.PreviewMouseDown += ScrollViewer_MouseDown;
            this._scrolling.ScrollViewer.MouseUp += ScrollViewer_MouseUp;
            this._scrolling.ScrollViewer.MouseMove += ScrollViewer_MouseMove;

            //            this.MouseDown += DrawingCanvas_MouseDown;
            //            this.MouseMove += DrawingCanvas_MouseMove;
            //            this.MouseUp += DrawingCanvas_MouseUp;
            //            this.LostMouseCapture += DrawingCanvas_LostMouseCapture;

            _grid.MouseDown += DrawingCanvas_MouseDown;
            _grid.MouseMove += DrawingCanvas_MouseMove;
            _grid.MouseUp += DrawingCanvas_MouseUp;


            // this.PreviewKeyDown += DrawingCanvas_KeyDown;
            // this.PreviewKeyUp += DrawingCanvas_KeyUp;
            this.LostMouseCapture += DrawingCanvas_LostMouseCapture;

            this.FocusVisualStyle = null;

            _isDirty = false;

            Count4U.Common.View.DragDrop.DragDrop.SetIsDropTarget(this, true);
            Count4U.Common.View.DragDrop.DragDrop.SetDropHandler(this, this);
        }

        static DrawingCanvas()
        {
            ToolProperty = DependencyProperty.Register("Tool", typeof(enToolType), typeof(DrawingCanvas), new PropertyMetadata(enToolType.Pointer));
        }

        public enToolType Tool
        {
            get
            {
                return (enToolType)GetValue(ToolProperty);
            }
            set
            {
                if ((int)value >= 0)
                {
                    SetValue(ToolProperty, value);

                    _toolCurrent = _tools[(int)Tool];
                    _toolCurrent.SetCursor(_grid);

                    _grid.Focus();

                    OnToolChanged();

                    if (Tool == enToolType.Pointer)
                    {
                        foreach (DrawingObject drawingObject in this.Objects)
                        {
                            drawingObject.SetCursor();
                        }
                    }
                    else
                    {
                        foreach (DrawingObject drawingObject in this.Objects)
                        {
                            drawingObject.UnsetCursor();
                        }
                    }
                }
            }
        }

        public delegate void CanvasCommandEventHandler(object sender, enCommand command, DrawingCanvasCommandResult result);
        public delegate void CanvasObjectAddedEventHandler(object sender, DrawingObject drawingObject);
        public delegate void CanvasObjectDeletedEventHandler(object sender, DrawingObject drawingObject);
        public delegate void CanvasObjectRenamedEventHandler(object sender, DrawingObject drawingObject);
        public delegate void CanvasObjectLockEventHandler(object sender, DrawingObject drawingObject);
        public delegate void CanvasObjectNameEventHandler(object sender, DrawingObject drawingObject);

        public event EventHandler ToolChanged = delegate { };
        public event EventHandler CanvasSizeChanged = delegate { };
        public event EventHandler SelectedObjectsChanged = delegate { };
        public event CanvasCommandEventHandler CanvasCommandExecuted = delegate { };
        public event EventHandler IsDirtyChanged = delegate { };
        public event EventHandler DrawingChanged = delegate { };
        public event CanvasObjectAddedEventHandler CanvasObjectAdded = delegate { };
        public event CanvasObjectDeletedEventHandler CanvasObjectDeleted = delegate { };
        public event CanvasObjectRenamedEventHandler CanvasObjectRenamed = delegate { };
        public event CanvasObjectLockEventHandler CanvasObjectLockChanged = delegate { };

        protected virtual void OnToolChanged()
        {
            EventHandler handler = ToolChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnCanvasSizeChanged()
        {
            EventHandler handler = CanvasSizeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        protected virtual void OnSelectedObjectsChanged()
        {
            EventHandler handler = SelectedObjectsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnCanvasCommandExecuted(object sender, enCommand command, DrawingCanvasCommandResult result)
        {
            CanvasCommandEventHandler handler = CanvasCommandExecuted;
            if (handler != null) handler(sender, command, result);
        }

        protected virtual void OnIsDirtyChanged()
        {
            EventHandler handler = IsDirtyChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnDrawingChanged()
        {
            EventHandler handler = DrawingChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnCanvasObjectAdded(DrawingObject drawingobject)
        {
            CanvasObjectAddedEventHandler handler = CanvasObjectAdded;
            if (handler != null) handler(this, drawingobject);
        }

        protected virtual void OnCanvasObjectDeleted(DrawingObject drawingobject)
        {
            CanvasObjectDeletedEventHandler handler = CanvasObjectDeleted;
            if (handler != null) handler(this, drawingobject);
        }

        protected virtual void OnCanvasObjectRenamed(DrawingObject drawingobject)
        {
            CanvasObjectRenamedEventHandler handler = CanvasObjectRenamed;
            if (handler != null) handler(this, drawingobject);
        }

        protected virtual void OnCanvasObjectLockChanged(DrawingObject drawingobject)
        {
            CanvasObjectLockEventHandler handler = CanvasObjectLockChanged;
            if (handler != null) handler(this, drawingobject);
        }

        public List<DrawingObject> Objects
        {
            get { return _objects; }
        }

        public Scrolling Scrolling
        {
            get { return _scrolling; }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnIsDirtyChanged();

                    if (_isDirty == false)
                    {
                        foreach (DrawingObject drawingObject in _objects)
                        {
                            drawingObject.IsDirty = false;
                        }
                    }
                }
            }
        }

        void DrawingCanvas_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Focusable = true;
            this.Focus();
            OnCanvasSizeChanged();
        }

        void DrawingCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Handled) return;

            if (_toolCurrent == null) return;

            //  this.Focus();

            if (e.ClickCount == 2)
            {

            }
            else
            {
                _toolCurrent.OnMouseDown(this, e);
            }
        }

        void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_toolCurrent == null) return;

            if (e.MiddleButton == MouseButtonState.Released)
            {
                _toolCurrent.OnMouseMove(this, e);
            }
            else
            {
                this.Cursor = Helpers.DefaultCursor;
            }
        }

        void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_toolCurrent == null) return;

            _toolCurrent.OnMouseUp(this, e);
        }

        public void DrawingCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (_toolCurrent == null) return;

            if (e.Key == Key.V)
            {
                Tool = enToolType.Pointer;
                return;
            }

            if (e.Key == Key.H)
            {
                Tool = enToolType.Hand;
                return;
            }

            if (e.Key == Key.Z)
            {
                Tool = enToolType.Zoom;
                return;
            }

            if (e.Key == Key.Escape)
            {
                Tool = enToolType.Pointer;
                return;
            }

            _toolCurrent.OnKeyDown(this, e);
        }

        public void DrawingCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (_toolCurrent == null) return;

            _toolCurrent.OnKeyUp(this, e);
        }


        void DrawingCanvas_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                CancelCurrentOperation();
            }
        }

        void ScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_toolCurrent == null) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _toolCurrent.OnScrollMouseDown(this, e);
            }
        }

        void ScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_toolCurrent == null) return;

            _toolCurrent.OnScrollMouseUp(this, e);
        }

        void ScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_toolCurrent == null) return;

            _toolCurrent.OnScrollMouseMove(this, e);
        }

        public void Add(DrawingObject drawing)
        {
            this.Children.Add(drawing);
            _objects.Add(drawing);

            drawing.PropertyChanged += DrawingObject_PropertyChanged;
            drawing.DrawingSizeChanged += Drawing_DrawingSizeChanged;

            OnCanvasObjectAdded(drawing);
        }

        public void Delete(DrawingObject drawing)
        {
            this.Children.Remove(drawing);
            _objects.Remove(drawing);

            drawing.PropertyChanged -= DrawingObject_PropertyChanged;
            drawing.DrawingSizeChanged -= Drawing_DrawingSizeChanged;

            OnCanvasObjectDeleted(drawing);
            OnSelectedObjectsChanged();
        }

        public void UnselectAll()
        {
            foreach (DrawingObject drawingObject in _objects)
            {
                drawingObject.IsSelected = false;
            }
        }

        public void Clear()
        {
            foreach (DrawingObject drawingObject in _objects.ToList())
            {
                this.Delete(drawingObject);
            }
        }

        public void AddHandle(FrameworkElement rec)
        {
            this.Children.Add(rec);
        }

        public void RemoveHandle(FrameworkElement rec)
        {
            this.Children.Remove(rec);
        }

        void CancelCurrentOperation()
        {
            if (_toolCurrent is ToolPointer)
            {
                if (Objects.Count > 0)
                {
                    ToolPointer toolPointer = _toolCurrent as ToolPointer;
                    toolPointer.RemoveSelectionRectangle(this);
                }
            }

            this.Tool = enToolType.Pointer;

            this.ReleaseMouseCapture();
            this.Cursor = Helpers.DefaultCursor;
        }

        public void SetSize(double width, double height)
        {
            if (width < Helpers.MinCanvasWidth)
            {
                width = Helpers.MinCanvasWidth;
            }

            if (width > Helpers.MaxCanvasWidth)
            {
                width = Helpers.MaxCanvasWidth;
            }

            if (height < Helpers.MinCanvasHeight)
            {
                height = Helpers.MinCanvasHeight;
            }

            if (height > Helpers.MaxCanvasHeight)
            {
                height = Helpers.MaxCanvasHeight;
            }


            this._grid.MinWidth = width + 200;
            this._grid.MinHeight = height + 200;
            this.Width = width;
            this.Height = height;

            OnCanvasSizeChanged();
        }

        public double GetWidth()
        {
            return this.Width;
        }

        public double GetHeight()
        {
            return this.Height;
        }

        public void CommandExecuted(object sender, enCommand command, DrawingCanvasCommandResult result)
        {
            switch (command)
            {
                case enCommand.Delete:
                case enCommand.CodeGenerate:
                case enCommand.Statistic:
                    OnCanvasCommandExecuted(sender, command, result);

                    break;
                default:
                    throw new ArgumentOutOfRangeException("command");
            }
        }

        void DrawingObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                OnSelectedObjectsChanged();
            }
        }

        void Drawing_DrawingSizeChanged(object sender, EventArgs e)
        {
            this.IsDirty = true;
            DrawingObject drawing = sender as DrawingObject;
            if (drawing != null)
            {
                drawing.IsDirty = true;
            }
            OnDrawingChanged();
        }

        void Tool_DrawingChanged(object sender, EventArgs e)
        {
            OnDrawingChanged();
        }

        public void SetSelectedX(double x)
        {
            List<DrawingObject> selected = _objects.Where(r => r.IsSelected).ToList();

            if (selected.Count == 0)
                return;

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in selected)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetLeft(x);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void SetSelectedY(double y)
        {
            List<DrawingObject> selected = _objects.Where(r => r.IsSelected).ToList();

            if (selected.Count == 0)
                return;

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in selected)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetTop(y);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void SetSelectedWidth(double width)
        {
            List<DrawingObject> selected = _objects.Where(r => r.IsSelected).ToList();

            if (selected.Count == 0)
                return;

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in selected)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetWidth(width);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void SetSelectedHeight(double height)
        {
            List<DrawingObject> selected = _objects.Where(r => r.IsSelected).ToList();

            if (selected.Count == 0)
                return;

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in selected)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetHeight(height);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void SetSelectedAngle(double angle)
        {
            List<DrawingObject> selected = _objects.Where(r => r.IsSelected).ToList();

            if (selected.Count == 0)
                return;

            if (angle < 0 || angle > 360)
                return;

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in selected)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetAngle(angle);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void SetSelectedName(string name)
        {
            DrawingObject selected = _objects.FirstOrDefault(r => r.IsSelected);

            if (selected == null)
                return;

            if (selected.Inner.IsLocked == false)
            {
                selected.SetName(name);

                OnCanvasObjectRenamed(selected);

                this.IsDirty = true;
            }
        }

        public void ScrollIntoView(DrawingObject drawing)
        {
            ScrollViewer sv = _scrolling.ScrollViewer;

            System.Diagnostics.Debug.Print(sv.HorizontalOffset.ToString());

            Point point = drawing.TranslatePoint(new Point(0, 0), _grid);

            double offsetX = 100 * sv.HorizontalOffset / _scrolling.ZoomPercentage;
            double offsetY = 100 * sv.VerticalOffset / _scrolling.ZoomPercentage;

            double viewportWidth = 100 * sv.ViewportWidth / _scrolling.ZoomPercentage;
            double viewportHeight = 100 * sv.ViewportHeight / _scrolling.ZoomPercentage;

            if (
                (offsetX < point.X) && (point.X + drawing.GetWidth() < offsetX + viewportWidth)
               )
            {

            }
            else
            {
                double pointX = _scrolling.ZoomPercentage * point.X / 100;
                sv.ScrollToHorizontalOffset(pointX - 50);
            }

            if (
                (offsetY < point.Y) && (point.Y + drawing.GetHeight() < offsetY + viewportHeight)
               )
            {

            }
            else
            {
                double pointY = _scrolling.ZoomPercentage * point.Y / 100;
                _scrolling.ScrollViewer.ScrollToVerticalOffset(pointY - 50);
            }
        }

        public void AlignLeft()
        {
            List<DrawingObject> objects = _objects.Where(r => r.IsSelected).ToList();

            if (objects.Count < 2) return;

            double left = objects.Min(r => r.GetLeft());

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in objects)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetLeft(left);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void AlignRight()
        {
            List<DrawingObject> objects = _objects.Where(r => r.IsSelected).ToList();

            if (objects.Count < 2) return;

            double right = objects.Min(r => this.ActualWidth - (r.GetLeft() + r.GetWidth()));

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in objects)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    double left = this.ActualWidth - right - drawingObject.GetWidth();
                    drawingObject.SetLeft(left);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void AlignTop()
        {
            List<DrawingObject> objects = _objects.Where(r => r.IsSelected).ToList();

            if (objects.Count < 2) return;

            double top = objects.Min(r => r.GetTop());

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in objects)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetTop(top);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void AlignBottom()
        {
            List<DrawingObject> objects = _objects.Where(r => r.IsSelected).ToList();

            if (objects.Count < 2) return;

            double bottom = objects.Min(r => this.ActualHeight - (r.GetTop() + r.GetHeight()));

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in objects)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    double top = this.ActualHeight - bottom - drawingObject.GetHeight();
                    drawingObject.SetTop(top);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void AlignSameWidth()
        {
            List<DrawingObject> objects = _objects.Where(r => r.IsSelected).ToList();

            if (objects.Count < 2) return;

            double largest = objects.Max(r => r.GetWidth());

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in objects)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetWidth(largest);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void AlignSameHeight()
        {
            List<DrawingObject> objects = _objects.Where(r => r.IsSelected).ToList();

            if (objects.Count < 2) return;

            double highest = objects.Max(r => r.GetHeight());

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in objects)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SetHeight(highest);
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void SetIsLocked(DrawingObject drawing, bool isLocked)
        {
            drawing.SetIsLocked(isLocked);
            this.IsDirty = true;

            OnCanvasObjectLockChanged(drawing);
        }

        public void SetName(DrawingObject drawing, string name)
        {
            if (drawing.Inner.IsLocked == false)
            {
                drawing.SetName(name);

                OnCanvasObjectRenamed(drawing);

                this.IsDirty = true;
            }
        }

        public void SetText(DrawingObject drawing, string text)
        {
            drawing.SetName(text);
            OnCanvasObjectRenamed(drawing);
            this.IsDirty = true;
        }

        public void SetPicture(DrawingObject drawing, string pictureFileName)
        {
            drawing.SetName(pictureFileName);
            OnCanvasObjectRenamed(drawing);
            this.IsDirty = true;
        }

        public void BringForward()
        {
            List<DrawingObject> objects = _objects.Where(r => r.IsSelected).ToList();

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in objects)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.BrintToFront();
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public void SendBackward()
        {
            List<DrawingObject> objects = _objects.Where(r => r.IsSelected).ToList();

            bool wasChanged = false;
            foreach (DrawingObject drawingObject in objects)
            {
                if (drawingObject.Inner.IsLocked == false)
                {
                    drawingObject.SendToBack();
                    wasChanged = true;
                }
            }

            if (wasChanged)
            {
                this.IsDirty = true;
            }
        }

        public new void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is PlanPictureToolItemViewModel)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public new void Drop(IDropInfo dropInfo)
        {
            PlanPictureToolItemViewModel item = dropInfo.Data as PlanPictureToolItemViewModel;
            if (item == null)
                return;

            PlanObjectDecorator planObjectDecorator = new PlanObjectDecorator(this);

            DrawingCanvasCommandResult codeResult = new DrawingCanvasCommandResult();
            this.CommandExecuted(this, enCommand.CodeGenerate, codeResult);
            if ((codeResult.Result is String) == false)
            {
                return;
            }
            string code = codeResult.Result as String;

            PlanObject element = Helpers.CreatePlanObjectByType(enPlanObjectType.Picture, this, _container, code);
           
            planObjectDecorator.Add(element);

            Size size = dropInfo.DragInfo.VisualSourceItem.RenderSize;
            double ratio = size.Width / size.Height;

            planObjectDecorator.Width = 200;
            planObjectDecorator.Height = 200 / ratio;

            Canvas.SetLeft(planObjectDecorator, dropInfo.DropPosition.X);
            Canvas.SetTop(planObjectDecorator, dropInfo.DropPosition.Y);
            Panel.SetZIndex(planObjectDecorator, Helpers.GetDefaultZIndex(element.PlanType));

            PlanPicture picture = element as PlanPicture;
            if (picture != null)
            {
                FileInfo fi = new FileInfo(item.Path);

                picture.FileName = fi.Name;                
            }           

            this.Add(planObjectDecorator);

            planObjectDecorator.IsDirty = true;
            this.IsDirty = true;

            this.UnselectAll();
            planObjectDecorator.IsSelected = true;
        }
    }
}