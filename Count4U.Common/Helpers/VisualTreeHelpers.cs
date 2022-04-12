using System;
using System.Windows;
using System.Windows.Media;

namespace Count4U.Common.Helpers
{
    public static class VisualTreeHelpers
    {
        public static T FindParent<T>(DependencyObject cur) where T : DependencyObject
        {
            DependencyObject current = cur;
            while (cur != null)
            {
                if (cur is T)
                    return (T)cur;

                cur = VisualTreeHelper.GetParent(cur);
            }

            return null;
        }

        public static T FindInVisualTreeByType<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject xObj = VisualTreeHelper.GetChild(parent, i);
                if (xObj is T)
                {
                    return (T)xObj;
                }
                else
                {
                    T res = FindInVisualTreeByType<T>(xObj);
                    if (res != null)
                        return res;
                }
            }
            return default(T);
        }

        public static DependencyObject FindInVisualTreeFunc(DependencyObject parent, Func<DependencyObject, bool> func)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject xObj = VisualTreeHelper.GetChild(parent, i);
                if (func(xObj))
                {
                    return xObj;
                }
                else
                {
                    DependencyObject res = FindInVisualTreeFunc(xObj, func);
                    if (res != null)
                        return res;
                }
            }
            return null;
        }
    }
}