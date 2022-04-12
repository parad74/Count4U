using System.Windows;
using System.Windows.Controls;

namespace Count4U.CustomControls.RenameTreeViewControl
{
    public class RenameTreeView : TreeView
    {
        static RenameTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RenameTreeView), new FrameworkPropertyMetadata(typeof(RenameTreeView)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RenameTreeViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RenameTreeViewItem();
        }

    }
}