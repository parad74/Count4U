using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Count4U.Common.Behaviours
{
    public class ContextMenuLeftButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (this.AssociatedObject == null)
                return;

            if (this.AssociatedObject.ContextMenu == null)
                this.AssociatedObject.ContextMenu = new ContextMenu();
            
            this.AssociatedObject.Click += btn_Click;
            this.AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        void AssociatedObject_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
      
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            Clear();
        }

        void btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.AssociatedObject == null)
                return;

            ContextMenu cm = this.AssociatedObject.ContextMenu;

            //cm.PlacementTarget = this.AssociatedObject;
            Dispatcher.Invoke(new Action(() => cm.IsOpen = true), DispatcherPriority.Background);
        }       

        private void Clear()
        {
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.Click -= btn_Click;
                this.AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            }
        }
    }
}