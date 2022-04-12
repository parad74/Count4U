using System;

namespace Count4U.Common.View.DragDrop
{
    public class DropTargetAdorners
    {
        public static Type Highlight
        {
            get { return typeof(DropTargetHighlightAdorner); }
        }

        public static Type Insert
        {
            get { return typeof(DropTargetInsertionAdorner); }
        }
    }
}