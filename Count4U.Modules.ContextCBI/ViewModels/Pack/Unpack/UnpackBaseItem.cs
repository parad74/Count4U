using System;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Pack.Unpack
{
    public class UnpackBaseItem : NotificationObject
    {
        private string _image;
        private string _path;

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public string Image
        {
            get
            {
                return String.Format("/Count4U.Media;component/Icons/{0}.png", this._image);
            }
            set
            {
                this._image = value;
                RaisePropertyChanged(() => Image);
            }
        }

        public string ImageShort
        {
            get { return _image; }
        }
    }
}