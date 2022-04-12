using System;
using System.Globalization;
using System.Windows.Data;

namespace Count4U.Report.Views
{
    public class ReportNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Count4U.GenerationReport.Report report = value as Count4U.GenerationReport.Report;
            if (report != null)
            {
                return String.Format(@"{0}\{1} {2}", report.Path, report.FileName, report.CodeReport);
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}