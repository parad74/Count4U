using System.Windows.Controls;
using System.Windows.Interactivity;
using Count4U.Common.Interfaces;

namespace Count4U.Common.Behaviours
{
    public class GridCancelEditBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.PreparingCellForEdit += AssociatedObject_PreparingCellForEdit;
                this.AssociatedObject.CellEditEnding += AssociatedObject_CellEditEnding;
                this.AssociatedObject.Unloaded += AssociatedObject_Unloaded;                
            }
        }
       
        void AssociatedObject_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
          //  Clear();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            Clear();
        }

        void AssociatedObject_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            IRowEdititing rowEdititing = e.Row.DataContext as IRowEdititing;
            if (rowEdititing != null)
            {
                rowEdititing.BeginEditing();
            }
        }

        void AssociatedObject_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                IRowEdititing rowEdititing = e.Row.DataContext as IRowEdititing;
                if (rowEdititing != null)
                {
                    rowEdititing.CancelEditing();
                }
            }
            if (e.EditAction == DataGridEditAction.Commit)
            {
                IRowEdititing rowEdititing = e.Row.DataContext as IRowEdititing;
                if (rowEdititing != null)
                {
                    rowEdititing.CommitEditing();
                }
            }
        }

        private void Clear()
        {
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.CellEditEnding -= AssociatedObject_CellEditEnding;
                this.AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
				this.AssociatedObject.PreparingCellForEdit -= AssociatedObject_PreparingCellForEdit;

			}
        }
    }
}