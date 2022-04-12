using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Count4U.Common.Behaviours
{
    public class TextBoxHebrewAlignmentBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "he")
            {
                this.AssociatedObject.FlowDirection = FlowDirection.LeftToRight;
                this.AssociatedObject.HorizontalContentAlignment = HorizontalAlignment.Right;
            }

        }
    }
}