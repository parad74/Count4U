using System;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Count4U.CustomControls.RenameTreeViewControl
{
    public class RenameTreeViewItem : TreeViewItem
    {
        public static readonly DependencyProperty IsRenameProperty;
        public static readonly DependencyProperty RenameTextProperty;
        public static readonly DependencyProperty CanRenameProperty;

        private const int RenameDelay = 500;

        private bool _canBeEdit = false;
        private bool _isMouseWithinScope = false;
        private bool _isDouble = false;
     //   private DispatcherTimer _timer;

        private bool _initForRename = false;

        static RenameTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RenameTreeViewItem), new FrameworkPropertyMetadata(typeof(RenameTreeViewItem)));

            FrameworkPropertyMetadata m = new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None);
            IsRenameProperty = DependencyProperty.Register("IsRename", typeof(bool), typeof(RenameTreeViewItem), m);

            FrameworkPropertyMetadata m1 = new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None);
            RenameTextProperty = DependencyProperty.Register("RenameText", typeof(string), typeof(RenameTreeViewItem), m1);

            FrameworkPropertyMetadata m2 = new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, CanRenameChanged);
            CanRenameProperty = DependencyProperty.Register("CanRename", typeof(bool), typeof(RenameTreeViewItem), m2);
        }

        private static void CanRenameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RenameTreeViewItem item = dependencyObject as RenameTreeViewItem;
            if (item != null)
            {
                if ((bool)e.NewValue)
                {
                    item.InitForRenamed();
                }
                else
                {
                    item.ClearFromRename();
                }
            }
        }

        public bool IsRename
        {
            get { return (bool)this.GetValue(IsRenameProperty); }
            set
            {
                this.SetValue(IsRenameProperty, value);
            }
        }

        public string RenameText
        {
            get { return (string)GetValue(RenameTextProperty); }
            set
            {
                SetValue(RenameTextProperty, value);
            }
        }

        public bool CanRename
        {
            get { return (bool)this.GetValue(CanRenameProperty); }
            set
            {
                this.SetValue(CanRenameProperty, value);
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RenameTreeViewItem;
        }

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new RenameTreeViewItem();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (CanRename == false) return;

            ClearFromRename();
            InitForRenamed();
        }

        private void InitForRenamed()
        {
            if (_initForRename) return;

            Border border = GetTemplateChild("Bd") as Border;
            if (border != null)
            {
                border.MouseEnter += Border_MouseEnter;
                border.MouseLeave += Border_MouseLeave;
            }

//            _timer = new DispatcherTimer();
//            _timer.Interval = TimeSpan.FromMilliseconds(RenameDelay);
//            _timer.Tick += Timer_Tick;

            System.Windows.Controls.TextBox textBox = GetTemplateChild("renameTb") as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                textBox.LostMouseCapture += TextBox_LostMouseCapture;
                textBox.ContextMenu = null;
            }

            _initForRename = true;
        }

        private void ClearFromRename()
        {
            Border border = GetTemplateChild("Bd") as Border;
            if (border != null)
            {
                border.MouseEnter -= Border_MouseEnter;
                border.MouseLeave -= Border_MouseLeave;
            }

//            if (_timer != null)
//            {
//                _timer.Tick -= Timer_Tick;
//                _timer = null;
//            }

            System.Windows.Controls.TextBox textBox = GetTemplateChild("renameTb") as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                textBox.LostMouseCapture -= TextBox_LostMouseCapture;
                textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
                textBox.LostFocus -= TextBox_LostFocus;
            }

            _initForRename = false;
        }


        private void TextBox_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (IsRename)
            {
                System.Windows.Controls.TextBox textBox = GetTemplateChild("renameTb") as System.Windows.Controls.TextBox;
                Mouse.Capture(textBox, CaptureMode.Element);
            }
        }

//        private void Timer_Tick(object sender, EventArgs e)
//        {
//            _timer.Stop();
//
//            IsRename = true;
//            System.Windows.Controls.TextBox textBox = GetTemplateChild("renameTb") as System.Windows.Controls.TextBox;
//            textBox.Text = RenameText;
//            textBox.Focus();
//            Mouse.Capture(textBox, CaptureMode.Element);
//            textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
//            textBox.LostFocus += TextBox_LostFocus;
//
//        }

        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsRename)
            {
                IsRename = false;
                Mouse.Capture(null);
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.Print("TextBox_PreviewMouseDown");

            if (IsRename)
            {
                System.Diagnostics.Debug.Print("IsRename");
                System.Diagnostics.Debug.Print("----------------");

                System.Windows.Controls.TextBox textBox = GetTemplateChild("renameTb") as System.Windows.Controls.TextBox;
                Point position = e.GetPosition(textBox);

                if (position.X > textBox.ActualWidth || position.Y > textBox.ActualHeight || position.X < 0 || position.Y < 0)
                {
                    IsRename = false;
                    Mouse.Capture(null);
                }
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsRename && IsSelected)
            {
                _canBeEdit = true;
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseWithinScope = false;
            _canBeEdit = false;
        //    _timer.Stop();
            _isDouble = false;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (CanRename == false) return;

            if (_isDouble)
            {
                _isDouble = false;
                return;
            }

            if (!IsRename)
            {
                if (!e.Handled && (_canBeEdit || _isMouseWithinScope))
                {
             //       _timer.Start();
                }

                if (IsSelected)
                    _isMouseWithinScope = true;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (CanRename == false) return;

            //_timer.Stop();
            _isDouble = true;
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (CanRename == false) return;

            if (e.Key == Key.F2)
            {                             
              //  _timer.Stop();

                IsRename = true;
                System.Windows.Controls.TextBox textBox = GetTemplateChild("renameTb") as System.Windows.Controls.TextBox;
                textBox.Text = RenameText;
                textBox.Focus();
                Mouse.Capture(textBox, CaptureMode.Element);
                textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
                textBox.LostFocus += TextBox_LostFocus;
                e.Handled = true;
            }

            if (e.Key == Key.Escape)
            {
                IsRename = false;
                Mouse.Capture(null);
            }

            if (e.Key == Key.Enter && IsRename)
            {
                IsRename = false;
                Mouse.Capture(null);
                System.Windows.Controls.TextBox textBox = GetTemplateChild("renameTb") as System.Windows.Controls.TextBox;
                SetValue(RenameTextProperty, textBox.Text);
            }
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);

        }
    }
}