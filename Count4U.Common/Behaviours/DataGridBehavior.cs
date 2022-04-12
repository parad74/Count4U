using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Count4U.Common.Behaviours
{
    public class DataGridBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(DataGridBehavior));

        public static readonly DependencyProperty CommitCommandProperty =
            DependencyProperty.Register("CommitCommand", typeof(ICommand), typeof(DataGridBehavior));

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(DataGridBehavior));

        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        public ICommand CommitCommand
        {
            get { return (ICommand)GetValue(CommitCommandProperty); }
            set { SetValue(CommitCommandProperty, value); }
        }

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.BeginningEdit += DataGrid_BeginningEdit;
            AssociatedObject.RowEditEnding += DataGrid_RowEditEnding;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.BeginningEdit -= DataGrid_BeginningEdit;
            AssociatedObject.RowEditEnding -= DataGrid_RowEditEnding;
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var command = EditCommand;

            if (command != null)
            {
                var parameter = e.Row.Item;

                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var command = GetCommand(e.EditAction);

            if (command != null)
            {
                var parameter = e.Row.Item;

                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private ICommand GetCommand(DataGridEditAction action)
        {
            switch (action)
            {
                case DataGridEditAction.Commit:
                    return CommitCommand;

                case DataGridEditAction.Cancel:
                    return CancelCommand;

                default:
                    return null;
            }
        }
    }
}