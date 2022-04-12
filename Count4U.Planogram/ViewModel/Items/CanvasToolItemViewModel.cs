using Count4U.Planogram.Lib;
using Count4U.Planogram.Lib.Enums;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Planogram.ViewModel
{
    public class CanvasToolItemViewModel : NotificationObject
    {
        private string _image;
        private bool _isChecked;
        private string _title;
        private enToolType _toolType;

        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged(() => Image);
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        public enToolType ToolType
        {
            get { return _toolType; }
            set { _toolType = value; }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }
    }
}