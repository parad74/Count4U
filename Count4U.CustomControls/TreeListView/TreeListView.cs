using System.Windows;
using System.Windows.Controls;

namespace Count4U.CustomControls.TreeListView
{
    public class TreeListView : TreeView
    {
        #region Fields

        private static ResourceKey mvMainColumnStyleKey;

        public static readonly DependencyProperty ColumnsProperty;

        #endregion Fields

        #region Constructor

        static TreeListView()
        {
            ColumnsProperty = DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(TreeListView));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        public TreeListView()
        {
            Columns = new GridViewColumnCollection();
        }

        #endregion Constructor

        #region Properties

        //		public static ResourceKey MainColumnStyleKey
        //		{
        //			get
        //			{
        //				if (mvMainColumnStyleKey == null)
        //					mvMainColumnStyleKey = new CustomThemeKey(CustomResourceKeyID.TreeListViewColumnStyle);
        //
        //				return mvMainColumnStyleKey;
        //			}
        //		}

        public GridViewColumnCollection Columns
        {
            get
            {
                return (GridViewColumnCollection)GetValue(ColumnsProperty);
            }
            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        #endregion Methods
    }
}