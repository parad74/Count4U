using System;
using System.ComponentModel;
using Count4U.Model.Transfer;
using Ionic.Zip;

namespace Count4U.Modules.ContextCBI.ViewModels.Zip
{
    public class ZipImportItemViewModel : ZipItemViewModel
    {
        public ZipImportItemViewModel(PropertyChangedEventHandler propertyChangedEventHandler)
            :base(propertyChangedEventHandler)
        {
            

        }

        public ZipEntry ZipEntry { get; set; }

        public override object Image
        {
            get
            {
                if (this._image is String)
                    return String.Format("/Count4U.Media;component/Icons/{0}.png", this._image);
                return this._image;
            }
            set
            {
                this._image = value;
                RaisePropertyChanged(() => Image);
            }
        }

        public ZipRootFolder? ZipRootFolder { get; set; }
        public ZipImportItemViewModel Parent { get; set; }
    }
}