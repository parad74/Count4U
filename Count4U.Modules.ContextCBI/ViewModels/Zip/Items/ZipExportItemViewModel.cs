using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using Count4U.Common.Extensions;
using Count4U.Model.Transfer;

namespace Count4U.Modules.ContextCBI.ViewModels.Zip
{
    public class ZipExportItemViewModel : ZipItemViewModel
    {
        private readonly DirectoryInfo _directoryInfo;
        private readonly FileInfo _fileInfo;
        private readonly ZipRootFolder _zipRootFolder;
        private int _level;
        private ZipExportItemViewModel _parent;

        private string _tooltip;

        public ZipExportItemViewModel(
            DirectoryInfo di,
            FileInfo fi,
            PropertyChangedEventHandler propertyChangedEventHandler,
            ZipRootFolder zipRootFolder,
            int level,
            ZipExportItemViewModel parent)
            : base(propertyChangedEventHandler)
        {
            this._zipRootFolder = zipRootFolder;
            this._fileInfo = fi;
            this._directoryInfo = di;
            this._level = level;
            this._parent = parent;
        }

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


        public DirectoryInfo DirectoryInfo
        {
            get { return this._directoryInfo; }
        }

        public FileInfo FileInfo
        {
            get { return this._fileInfo; }
        }

        public ZipRootFolder ZipRootFolder
        {
            get { return _zipRootFolder; }
        }

        public int Level
        {
            get { return _level; }
        }

        public ZipExportItemViewModel Parent
        {
            get { return _parent; }
        }

        public string Tooltip
        {
            get { return _tooltip; }
            set
            {
                _tooltip = value;
                RaisePropertyChanged(() => Tooltip);
            }
        }
    }
}