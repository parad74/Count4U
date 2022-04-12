using System;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Media;

namespace Count4U.Report.ViewModels.ReportButton
{    
    public class MenuTagItemViewModel : NotificationObject
    {
        private bool _isChecked;
        private string _text;
        private Color _color;        

        public MenuTagItemViewModel()
        {

        }

        //public MenuFilterItem State { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                RaisePropertyChanged(() => Color);
            }
        }
    }
}