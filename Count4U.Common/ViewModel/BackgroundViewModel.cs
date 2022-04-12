using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Count4U.Common.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel
{
    public class BackgroundViewModel : NotificationObject
    {
        protected string _backgroundFilePath;
        protected ImageBrush _localBackground;
        private double _opacityBackground;
        private readonly IEventAggregator _eventAggregator;

        public BackgroundViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

        }

        public string BackgroundFilePath
        {
            get
            {
                return this._backgroundFilePath;
            }
            set
            {
                this._backgroundFilePath = value;
                RaisePropertyChanged(() => LocalBackground);
            }
        }

        public double OpacityBackground
        {
            get { return _opacityBackground; }
            set
            {
                _opacityBackground = value;
                RaisePropertyChanged(() => LocalBackground);
            }
        }

        public ImageBrush LocalBackground
        {
            get
            {
                this._localBackground = new ImageBrush();
                if (File.Exists(BackgroundFilePath) == true)
                {
                    Uri uri = new Uri(BackgroundFilePath);
                    this._localBackground.ImageSource = new BitmapImage(uri);
                    this._localBackground.Opacity = OpacityBackground;
                }
                else
                {
                    this._localBackground = new ImageBrush();
                }
                return this._localBackground;
            }
//            set
//            {
//                this._localBackground = value;
//                RaisePropertyChanged(() => LocalBackground);
//            }
        }
    }
}