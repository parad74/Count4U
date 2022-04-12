using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Controls;

namespace Count4U.Common.View
{
    /// <summary>
    /// Interaction logic for PopupView.xaml
    /// </summary>
    public partial class PopupView : UserControl
    {
        public PopupView()
        {
            InitializeComponent();

            this.Loaded += PopupFilterView_Loaded;
            this.KeyUp += PopupView_KeyUp;
            pathClose.MouseLeftButtonDown += pathClose_MouseLeftButtonDown;
            pathReset.MouseLeftButtonDown += PathReset_MouseLeftButtonDown;
            pathApply.MouseLeftButtonDown += PathApply_MouseLeftButtonDown;

            border.MouseLeftButtonDown += border_MouseLeftButtonDown;

        }

        void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
              if (e.ClickCount == 1)
              {
                  Window wnd = Window.GetWindow(this);
                  if (wnd != null)
                  {
                      wnd.DragMove();
                  }
              }
        }

        void PathApply_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IPopupChildControl child = GetChild();
            if (child != null)
                child.Apply();
        }

        void PathReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IPopupChildControl child = GetChild();
            if (child != null)
                child.Reset();
        }

        void PopupView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        public Window Window { get; set; }

        void PopupFilterView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void pathClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                Close();
            }
        }

        public void SetOffset(double offset)
        {
            double halfWidth = bdTriangle.Width / 2;
            double newOffset = offset - halfWidth;
            bdTriangle.Margin = new Thickness(newOffset, 0, 0, 0);
            bdShield.Margin = new Thickness(newOffset + 2, 11, 0, 0);
        }

        public void Close()
        {
            if (Window != null)
                Window.Close();
        }

        private IPopupChildControl GetChild()
        {
            return content.Content as IPopupChildControl;
        }
    }
}
