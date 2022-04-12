using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Lib.MultiPoint;
using Count4U.Model.ProcessC4U;
using Count4U.Model.Repository.Product.WrapperMulti;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Count4U.Model.ExportImport.Items
{
    public class FileItemViewModel : NotificationObject
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string _file;
		private string _folder;
		private string _path;
		private string _code;	   //Почему-то используется для InventorDateTime 
		private string _objectCode;
		private string _size;
		private string _date;
		private string _description;
		private string _manager;
		private DateTime _dateTimeCreated;
		//private DateTime _dateTimeModifyed;
        private bool _isChecked;
        private bool _isCheckedEnabled;
		private Inventor _inventor;
		private AuditConfig _audit;
		private bool _isSource;
		private bool _isDestination;
		private bool _inProcess;
		private bool _canDelete;

		private string _name;
		private string _title;

		private string _tag1;
		private string _tag2;
		private string _tag3;

		private Process _process;	 

		public FileItemViewModel()
        {
			this._isSource = false;
			this._isDestination = true;
        }

		public Inventor InventorObject
		{
			get { return this._inventor; }
			set
			{
				this._inventor = value;
			}
		}

		public Process ProcessObject
		{
			get { return this._process; }
			set
			{
				this._process = value;
			}
		}

		public string Title
        {
			get { return this._title; }
            set
            {
				this._title = value;
				this.RaisePropertyChanged(() => this.Title);
            }
        }

		public string Name
		{
			get { return this._name; }
			set
			{
				this._name = value;
				this.RaisePropertyChanged(() => this.Name);
			}
		}

		public string Tag1
		{
			get { return this._tag1; }
			set
			{
				this._tag1 = value;
				this.RaisePropertyChanged(() => this.Tag1);
			}
		}

		public string Tag2
		{
			get { return this._tag2; }
			set
			{
				this._tag2 = value;
				this.RaisePropertyChanged(() => this.Tag2);
			}
		}

		public string Tag3
		{
			get { return this._tag3; }
			set
			{
				this._tag3 = value;
				this.RaisePropertyChanged(() => this.Tag3);
			}
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


		public bool IsSource
        {
			get { return this._isSource; }
            set
            {
				this._isSource = value;
				this.RaisePropertyChanged(() => this.IsSource);
            }
        }

		public bool IsDestination
		{
			get { return this._isDestination; }
			set
			{
				this._isDestination = value;
				this.RaisePropertyChanged(() => this.IsDestination);
			}
		}

		public bool InProcess
		{
			get { return this._inProcess; }
			set
			{
				this._inProcess = value;
				this._canDelete = !value;
				this.RaisePropertyChanged(() => this.InProcess);
				this.RaisePropertyChanged(() => this.CanDelete);
			}
		}

		//вторична от  InProcess
		public bool CanDelete
		{
			get { return this._canDelete; }
			set
			{
				this._canDelete= value;
			}
		}

		string _sourceColor = "200,200,200";
		public string SourceColor
		{
			get
			{
				//var color = this._userSettingsManager.InventProductMarkColorGet();
				return this._sourceColor;
			}
		}

		public AuditConfig Audit
		{
			get { return this._audit; }
			set
			{
				this._audit = value;
				this.RaisePropertyChanged(() => this.Audit);
			}
		}

		public string Description
		{
			get { return this._description; }
			set
			{
				this._description = value;
				this.RaisePropertyChanged(() => this.Description);
			}
		}

		public string Folder
		{
			get { return this._folder; }
			set
			{
				this._folder = value;
				this.RaisePropertyChanged(() => this.Folder);
			}
		}


		public string Manager
		{
			get { return this._manager; }
			set
			{
				this._manager = value;
				this.RaisePropertyChanged(() => this.Manager);
			}
		}

		public string Path
		{
			get { return this._path; }
			set
			{
				this._path = value;
				this.RaisePropertyChanged(() => this.Path);
			}
		}


		public string Code
		{
			get { return this._code; }
			set
			{
				this._code = value;
				this.RaisePropertyChanged(() => this.Code);
			}
		}


		public string ObjectCode
		{
			get { return this._objectCode; }
			set
			{
				this._objectCode = value;
				this.RaisePropertyChanged(() => this.ObjectCode);
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
			
		
		//public DateTime DateTimeModifyed
		//{
		//	get { return this._dateTimeModifyed; }
		//	set
		//	{
		//		this._dateTimeModifyed = value;
		//		this.RaisePropertyChanged(() => this.DateTimeModifyed);
		//	}
		//}
      

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