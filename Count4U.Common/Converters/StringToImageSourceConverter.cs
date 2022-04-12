using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Count4U.Common.Converters
{
    public class StringToImageSourceConverter : IValueConverter
    {
		//public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		//{
		//	string path = value as String;
		//	if (String.IsNullOrWhiteSpace(path))
		//		return null;


		//	BitmapImage logo = new BitmapImage();
		//	logo.BeginInit();
		//	//logo.UriSource = new Uri("pack://application:,,,/ApplicationName;component/Resources/logo.png");
		//	//emptyItem.Path = String.Format("pack://application:,,,/Count4U.Media;component/Icons/{0}.png", "empty");
		//	path = String.Format("pack://application:,,,/Count4U.Media;component/Icons/{0}.png", "back");
		//	logo.UriSource = new Uri(path);
		//	logo.EndInit();

		//	return logo;
		//}


		public object Convert(object value, Type targetType,
							object parameter, CultureInfo culture)
		{
			string val = value as string;
			//if (string.IsNullOrEmpty(val))
			//	return new BitmapImage(new Uri(val));
			if (String.IsNullOrWhiteSpace(val))
				return null;

			return value;
		}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}