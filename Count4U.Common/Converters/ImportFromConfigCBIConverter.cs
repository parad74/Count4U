using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Count4U.Common.Converters
{
	public class ImportFromConfigCBIConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string mode = value as String;
            if (String.IsNullOrEmpty(mode))
                return null;

            string radio = parameter as String;
            if (String.IsNullOrEmpty(radio))
                return null;

            switch (mode)
            {
                case Common.NavigationSettings.CBIDbContextCustomer:
                    switch (radio)
                    {
                        case "Customer":
							return Visibility.Visible;
                        case "Branch":
                            return Visibility.Collapsed;
                        case "Inventor":
                            return Visibility.Collapsed;
                    }
                    break;
                case Common.NavigationSettings.CBIDbContextBranch:
                    switch (radio)
                    {
                        case "Customer":
                            return Visibility.Visible;
                        case "Branch":
							return Visibility.Visible;
                        case "Inventor":
                            return Visibility.Collapsed;
                    }
                    break;
                case Common.NavigationSettings.CBIDbContextInventor:
                    switch (radio)
                    {
                        case "Customer":
                            return Visibility.Visible;
                        case "Branch":
                            return Visibility.Visible;
                        case "Inventor":
							return Visibility.Visible;
                    }
                    break;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}