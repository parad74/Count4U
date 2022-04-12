using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using Count4U.Common.Helpers;

namespace Count4U.Common.Behaviours
{
    public class TextBoxPropertyBehavior : Behavior<TextBox>
    {
        private string _backup;
        private bool _backupOK;

        public TextBoxPropertyBehavior()
        {
            _backupOK = false;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

			if (this.AssociatedObject != null) {
				this.AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
				this.AssociatedObject.LostFocus += AssociatedObject_LostFocus;
				this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
			}
        }

		protected override void OnDetaching()
		{
			base.OnDetaching();

			if (this.AssociatedObject != null)
			{
				this.AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
				this.AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
				this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
			}
		}

		void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            _backupOK = true;
        }

        void AssociatedObject_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateSource();            
        }

        void AssociatedObject_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateSource();              
            }

            if (e.Key == Key.Escape && _backupOK)
            {
                this.AssociatedObject.Text = _backup;
                UpdateSource();
            }
        }

        private void UpdateSource()
        {
            TextBox textBox = this.AssociatedObject as TextBox;
            BindingExpression expression = textBox.GetBindingExpression(TextBox.TextProperty);
            expression.UpdateSource();

            if (UtilsConvert.IsOkAsDouble(textBox.Text))
            {
                _backup = textBox.Text;
                _backupOK = false;
            }
        }
    }
}