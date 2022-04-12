using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Count4U.Planogram.View.PlanObjects;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;
using Shapes = System.Windows.Shapes;
using Point = System.Windows.Point;

namespace Count4U.Planogram.Lib
{
    public abstract class DrawingObject : UserControl, INotifyPropertyChanged
    {
        protected static readonly double HandleSize = 6;
        protected static readonly Color HandleColor = Colors.DarkSlateGray;

        protected readonly DrawingCanvas _canvas;

        protected bool _isSelected;
        protected readonly List<FrameworkElement> _handles;

        private PlanObject _inner;
        private PlanSpecialObject _innerSpecial;
        private PlanLocation _innerLocation;
        private PlanText _innerText;
        private PlanPicture _innerPicture;

        private bool _isDirty;

        protected DrawingObject()
        {
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = new RotateTransform(0);
        }

        protected DrawingObject(DrawingCanvas canvas)
            : this()
        {
            _canvas = canvas;
            _handles = new List<FrameworkElement>();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    if (_isSelected)
                    {
                        ShowHandles();
                    }
                    else
                    {
                        HideHandles();
                    }

                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public abstract void ShowHandles();
        public abstract void HideHandles();

        public abstract int MakeHitTestHandle(Point point);
        public abstract void Move(double deltaX, double deltaY);
        public abstract Cursor GetHandleCursor(int handleNumber);
        public abstract bool MakeHitTest(Point point);
        public abstract bool IntersectsWith(System.Windows.Rect rectangle);

        public virtual void SetCursor()
        {
        }

        public virtual void UnsetCursor()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler DrawingSizeChanged;

        protected virtual void OnDrawingSizeChanged()
        {
            EventHandler handler = DrawingSizeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public PlanObject Inner
        {
            get { return _inner; }
        }

        public PlanSpecialObject InnerSpecial
        {
            get { return _innerSpecial; }
        }

        public PlanLocation InnerLocation
        {
            get { return _innerLocation; }
        }

        public PlanText InnerText
        {
            get { return _innerText; }
        }

        public PlanPicture InnerPicture
        {
            get { return _innerPicture; }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
            }
        }

        public void Add(PlanObject planObject)
        {
            _inner = planObject as PlanObject;
            _innerSpecial = planObject as PlanSpecialObject;
            _innerLocation = planObject as PlanLocation;
            _innerText = planObject as PlanText;
            _innerPicture = planObject as PlanPicture;

            UserControl uc = planObject as UserControl;
            if (uc == null)
            {
                throw new InvalidOperationException();
            }

            this.Add(uc);

            if (_innerSpecial == null)
            {
                this.MinWidth = Helpers.MinWidthToObject;
                this.MinHeight = Helpers.MinHeightToObject;
            }
            else
            {
                this.MinWidth = Helpers.MinWidthToSpecialObject;
                this.MinHeight = Helpers.MinHeightToSpecialObject; ;
            }
        }

        protected abstract void Add(UserControl uc);

        public RotateTransform RotateTransform
        {
            get { return this.RenderTransform as RotateTransform; }
        }

        public double GetLeft()
        {
            return Canvas.GetLeft(this);
        }

        public double GetTop()
        {
            return Canvas.GetTop(this);
        }

        public double GetWidth()
        {
            return this.ActualWidth;
        }

        public double GetHeight()
        {
            return this.ActualHeight;
        }

        public double GetAngle()
        {
            return this.RotateTransform.Angle;
        }

        public void SetLeft(double left)
        {
            Canvas.SetLeft(this, left);
            this.IsDirty = true;
        }

        public void SetTop(double top)
        {
            Canvas.SetTop(this, top);
            this.IsDirty = true;
        }

        public void SetWidth(double width)
        {
            this.Width = width;
            this.IsDirty = true;
        }

        public void SetHeight(double height)
        {
            this.Height = height;
            this.IsDirty = true;
        }

        public void SetAngle(double angle)
        {
            this.RotateTransform.Angle = angle;
            this.IsDirty = true;
        }

        public void SetIsLocked(bool isLocked)
        {
            this.Inner.IsLocked = isLocked;
            this.IsDirty = true;
        }

        public void SetName(string name)
        {
            this.Inner.PlanName = name;
            this.IsDirty = true;
        }

        public void BrintToFront()
        {
            Panel.SetZIndex(this, Helpers.ZIndexFront);
            this.IsDirty = true;
        }

        public void SendToBack()
        {
            Panel.SetZIndex(this, Helpers.ZIndexBack);
            this.IsDirty = true;
        }
    }
}