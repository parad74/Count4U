using System.Windows;

namespace Count4U.CustomControls.TextBox
{
    public class TextBoxV7 : System.Windows.Controls.TextBox
    {
        public static readonly DependencyProperty IsNecessaryProperty =
            DependencyProperty.Register("IsNecessary", typeof (bool), typeof (TextBoxV7), new PropertyMetadata(false));

        public bool IsNecessary
        {
            get { return (bool) GetValue(IsNecessaryProperty); }
            set { SetValue(IsNecessaryProperty, value); }
        }

        static TextBoxV7()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxV7), new FrameworkPropertyMetadata(typeof(TextBoxV7)));                
        }

        
    }
}