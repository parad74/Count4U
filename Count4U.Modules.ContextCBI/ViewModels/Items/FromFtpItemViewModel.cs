using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Count4U.Model.Count4U;
using Count4U.Model.Lib.MultiPoint;
using Count4U.Model.Repository.Product.WrapperMulti;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Count4U.Common.Web
{		 
        public class FromFtpItemViewModel : NotificationObject
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string _file;
		private string _size;
		private string _date;
		private DateTime _dateTimeCreated;
        private bool _isChecked;
        private bool _isCheckedEnabled;

		public readonly IServiceLocator _serviceLocator;
		public FromFtpItemViewModel(
			IServiceLocator serviceLocator)
        {
      
			this._serviceLocator = serviceLocator;
    
        }

   

        public string File
        {
			get { return this._file; }
            set
            {
				this._file = value;
				this.RaisePropertyChanged(() => this.File);
            }
        }

		public string Size
		{
			get { return this._size; }
			set
			{
				this._size = value;
				this.RaisePropertyChanged(() => this.Size);
			}
		}

		public string Date
		{
			get { return this._date; }
			set
			{
				this._date = value;
				this.RaisePropertyChanged(() => this.Date);
			}
		}

		public DateTime DateTimeCreated
		{
			get { return this._dateTimeCreated; }
			set
			{
				this._dateTimeCreated = value;
				this.RaisePropertyChanged(() => this.DateTimeCreated);
			}
		}
			
      

        public bool IsChecked
        {
			get { return this._isChecked; }
            set
            {
				this._isChecked = value;
			
				this.RaisePropertyChanged(() => this.IsChecked);
            }
        }

     

        public bool IsCheckedEnabled
        {
			get { return this._isCheckedEnabled; }
            set
            {
				this._isCheckedEnabled = value;
				this.RaisePropertyChanged(() => this.IsCheckedEnabled);
            }
        }

  
    }
}