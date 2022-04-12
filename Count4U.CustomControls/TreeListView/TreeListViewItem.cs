using System.Windows;
using System.Windows.Controls;

namespace Count4U.CustomControls.TreeListView
{
    public class TreeListViewItem : TreeViewItem
    {
        static TreeListViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem), new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
        }
        public TreeListViewItem()
            : base()
        {

        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        public int Level
        {
            get
            {
                TreeListViewItem parent = ItemsControlFromItemContainer(this) as TreeListViewItem;
                return (parent != null) ? parent.Level + 1 : 0;
            }
        }

    }
}