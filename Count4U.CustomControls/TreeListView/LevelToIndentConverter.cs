﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Count4U.CustomControls.TreeListView
{
    public class LevelToIndentConverter : IValueConverter
    {
        public double IndentSize { get; set; }

        public object Convert(object o, Type type, object parameter,
                                                    CultureInfo culture)
        {
            return new Thickness((int)o * IndentSize, 0, 0, 0);
        }

        public object ConvertBack(object o, Type type, object parameter,
                                                            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}