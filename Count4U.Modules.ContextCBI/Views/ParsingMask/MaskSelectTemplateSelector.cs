using System.Windows;
using System.Windows.Controls;
using Count4U.Modules.ContextCBI.ViewModels.ParsingMask;

namespace Count4U.Modules.ContextCBI.Views.ParsingMask
{
    public class MaskSelectTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }
        public DataTemplate Edit { get; set; }

        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            ContentPresenter presenter = container as ContentPresenter;
            if (presenter != null)
            {
                DataGridCell cell = presenter.Parent as DataGridCell;
                if (cell != null)
                {
                    MaskSelectItemViewModel node = cell.DataContext as MaskSelectItemViewModel;
                    if (node != null)
                        return node.IsEdit ? Edit : Normal;
                }
            }
            return null;
        }
    }
}