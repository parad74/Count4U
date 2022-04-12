using System.Windows.Input;
using System.Windows.Interactivity;

namespace Count4U.CustomControls.Behaviors
{
    public class TextSelectBehavior : Behavior<System.Windows.Controls.TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

			if (AssociatedObject != null)
			{
				AssociatedObject.Loaded += AssociatedObject_Loaded;
			//	AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;

			}
        }

		void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			
		}

        protected override void OnDetaching()
        {
            base.OnDetaching();

			if (AssociatedObject != null)
			{
				AssociatedObject.Loaded -= AssociatedObject_Loaded;
				//AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
			}
        }

        void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AssociatedObject != null)
                AssociatedObject.SelectAll();
        }
    }
}