using System;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
//using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Windows.Media;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Interface.ProcessC4U;
using Count4U.Model;
using Count4U.Model.Interface;

namespace Count4U.Modules.ContextCBI.ViewModels
{
	public class ProcessAddEditViewModel : NavigationAwareViewModel, IDataErrorInfo
    {
        private static readonly Random _rnd = new Random();

        private readonly IEventAggregator _eventAggregator;
		private readonly IProcessRepository _processRepository;
		private readonly IConnectionDB _connectionDB;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

		private Count4U.Model.ProcessC4U.Process _process;
		private Count4U.Model.ProcessC4U.Processes _processes;

        private string _code;
		private string _processCode;
        private string _name;
        private string _description;
		private string _title;
		
		private string _tag;
		private string _manager;
     //   private Color _color;

        private bool _isNewMode;
        private bool _isUnknown;

		public ProcessAddEditViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
			IProcessRepository processRepository,
			IConnectionDB connectionDB)
           // : base(contextCBIRepository)
        {
            this._processRepository = processRepository;
            this._eventAggregator = eventAggregator;
			this._connectionDB = connectionDB;
            this._okCommand = new DelegateCommand(this.OkCommandExecute, this.OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(this.CancelCommandExecute, this.CancelCommandCanExecute);
        }

        public bool IsOk { get; set; }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                RaisePropertyChanged(() => Name);
                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public string Description
        {
            get { return this._description; }
            set
            {
                this._description = value;
                RaisePropertyChanged(() => Description);
            }
        }

		public string Tag
		{
			get { return this._tag; }
			set
			{
				this._tag = value;
				RaisePropertyChanged(() => Tag);
			}
		}

		public string Title
		{
			get { return this._title; }
			set
			{
				this._title = value;
				RaisePropertyChanged(() => Title);
			}
		}

		public string Manager
		{
			get { return this._manager; }
			set
			{
				this._manager = value;
				RaisePropertyChanged(() => Manager);
			}
		}

		//public Color Color
		//{
		//	get { return this._color; }
		//	set
		//	{
		//		this._color = value;
		//		RaisePropertyChanged(() => Color);
		//	}
		//}

		//public string Code
		//{
		//	get { return this._code; }
		//	set
		//	{
		//		this._code = value;
		//		RaisePropertyChanged(() => Code);

		//		this._okCommand.RaiseCanExecuteChanged();
		//	}
		//}


		private bool _inheritEmptyAuditDB = true;
		public bool InheritEmptyAuditDB
		{
			get { return true; }
			//get { return _inheritEmptyAuditDB; }
			//set
			//{
			//	if (_inheritEmptyAuditDB != value)
			//	{
			//		_inheritEmptyAuditDB = value;
			//		RaisePropertyChanged(() => InheritEmptyAuditDB);
			//	}
			//}
		}

		private bool _inherittRootMainDB = true;
		public bool InheritRootMainDB
		{
			get { return _inherittRootMainDB; }
			set
			{
				if (_inherittRootMainDB != value)
				{
					_inherittRootMainDB = value;
					RaisePropertyChanged(() => InheritRootMainDB);

					_inheritEmptyMainDB = !value;
					RaisePropertyChanged(() => InheritEmptyMainDB);
				}
			}
		}


		private bool _inheritEmptyMainDB = false;
		public bool InheritEmptyMainDB
		{
			get { return _inheritEmptyMainDB; }
			set
			{
				if (_inheritEmptyMainDB != value)
				{
					_inheritEmptyMainDB = value;
					RaisePropertyChanged(() => InheritEmptyMainDB);

					_inherittRootMainDB = !value;
					RaisePropertyChanged(() => InheritRootMainDB);
				}
			}
		}

		private bool _inheritRootUserSetting = true;
		public bool InheritRootUserSetting
		{
			get { return _inheritRootUserSetting; }
			set
			{
				if (_inheritRootUserSetting != value)
				{
					_inheritRootUserSetting = value;
					RaisePropertyChanged(() => InheritRootUserSetting);

					_inheritEmptyUserSetting = !value;
					RaisePropertyChanged(() => InheritEmptyUserSetting);
				}
			}
		}
//			   InheritRootUserSetting
//InheritEmptyUserSetting


		private bool _inheritEmptyUserSetting = false;
		public bool InheritEmptyUserSetting
		{
			get { return _inheritEmptyUserSetting; }
			set
			{
				if (_inheritEmptyUserSetting != value)
				{
					_inheritEmptyUserSetting = value;
					RaisePropertyChanged(() => InheritEmptyUserSetting);

					_inheritRootUserSetting = !value;
					RaisePropertyChanged(() => InheritRootUserSetting);
				}
			}
		}

		public string ProcessCode
        {
			get { return this._processCode; }
            set
            {
				this._processCode = value;
				RaisePropertyChanged(() => ProcessCode);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }
		

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public bool IsNewMode
        {
            get { return this._isNewMode; }
        }

        public bool IsEditMode
        {
            get { return !this._isNewMode; }
        }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
					case "ProcessCode":
                        {
							string validation = ProcessValidate.ProcessCodeValidate(this._processCode);
                            if (!String.IsNullOrEmpty(validation))
                                return validation;

                            if (!this.IsProcessesCodeUnique())
                                return Model.ValidateMessage.Location.CodeExist;

                            break;
                        }
                    case "Name":
                        {
							string validation = ProcessValidate.NameValidate(this._name);

                            if (!String.IsNullOrEmpty(validation))
                                return validation;

                            break;
                        }
                }
                return null;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }


		bool IsProcessesCodeUnique()
        {
            if (this._isNewMode)
				return !this._processes.Any(r => r.ProcessCode.ToUpper() == this.ProcessCode.ToUpper());

            return true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //base.OnNavigatedTo(navigationContext);

            this._processes = this._processRepository.GetProcesses();

			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.ProcessCode))
            {
                string processCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.ProcessCode).Value;
                this._process = this._processRepository.GetProcessByProcessCode(processCode);
                this._isNewMode = false;
            }
            else
            {
                Count4U.Model.ProcessC4U.Process process = new Count4U.Model.ProcessC4U.Process();
                process.Name = String.Empty;
                process.ProcessCode = String.Empty;
                process.Description = String.Empty;
                this._process = process;
                this._isNewMode = true;
            }

          //  this._color = ColorParser.StringToColor(this._process.BackgroundColor);
            this._name = this._process.Name;
            this._description = this._process.Description;
			this._manager = this._process.Manager;
			this._title = this._process.Title;



			//this._tag = this._process.Tag;
            this.ProcessCode = this._process.ProcessCode;
        }

        private void OkCommandExecute()
        {
            //this._location.Name = this._name;
            this._process.Name = this._name;
            this._process.Description = this._description;
			this._process.Manager = this._manager;
			this._process.Title =	this._title;

			
			//this._process.Tag = this._tag;
         
            if (this.IsNewMode || this._isUnknown)
            {
				this._process.ProcessCode = this.ProcessCode;

                this._processRepository.Insert(this._process);
				this.AddOrUpdateFilesForProcess(this._process);
				this._eventAggregator.GetEvent<ProcessAddedEvent>().Publish(this._process);

            }
            else
            {
                this._processRepository.Update(this._process);
				this.AddOrUpdateFilesForProcess(this._process);
				this._eventAggregator.GetEvent<ProcessEditedEvent>().Publish(this._process);
            }

            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }


		public void AddOrUpdateFilesForProcess(Count4U.Model.ProcessC4U.Process process)
		{
			//AuditDB всегда новая 
			string pathAuditDB = _connectionDB.CopyEmptyAuditDBToProcess(process.ProcessCode);  //копируем, только если нет файла
			if (InheritEmptyMainDB == true)
			{
				_connectionDB.CopyEmptyMainDBToProcess(process.ProcessCode);  //копируем, только если нет файла
			}
			else //Копируем из корня Maindb.sdf
			{
				_connectionDB.CopyMainDBToProcess(process.ProcessCode);  //копируем, только если нет файла
			}
			 //Еще надо что-то сделать с userSetting!!! TODO
		}

        private bool OkCommandCanExecute()
        {
			return !String.IsNullOrWhiteSpace(_processCode) &&
				   this.IsProcessesCodeUnique() &&
                   !String.IsNullOrWhiteSpace(_name);
        }

		public void BuildProcessFiles(string processCodes)
		{
			
		}

        private void CancelCommandExecute()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool CancelCommandCanExecute()
        {
            return true;
        }

    }
}