using System;
using System.IO;
using System.Xml.Serialization;
using Count4U.Common.ViewModel.Filter.Data;
using Microsoft.Practices.Prism.ViewModel;
using NLog;

namespace Count4U.Common.ViewModel.Filter.FilterTemplate
{
    public class FilterTemplateItemViewModel : NotificationObject
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public string _name;
        private FileInfo _fileInfo;

        private string _displayName;
        private readonly Func<Type> _getFilterDataType;

        public FilterTemplateItemViewModel(FileInfo fileInfo, Func<Type> getFilterDataType)
        {
            _getFilterDataType = getFilterDataType;
            Update(fileInfo);
        }

		public FilterTemplateItemViewModel()
		{
			this._name = "";
			this._displayName = "";
			this._fileInfo = null;
		}

        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(DisplayName) ? _name : DisplayName;
            }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public FileInfo FileInfo
        {
            get { return _fileInfo; }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public void Update(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;

            ParseDisplayName(fileInfo);

            Name = Path.GetFileNameWithoutExtension(fileInfo.FullName);
        }

        private void ParseDisplayName(FileInfo fi)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(_getFilterDataType());
                using (var reader = new StreamReader(fi.FullName))
                {
                    CommonFilterData data = serializer.Deserialize(reader) as CommonFilterData;
                    if (data != null)
                    {
                        _displayName = data.DisplayName;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.ErrorException("ParseDisplayName", e);
            }
        }
    }
}