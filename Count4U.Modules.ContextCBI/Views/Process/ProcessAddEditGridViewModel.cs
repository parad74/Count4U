using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Model.Interface;
using Count4U.Model.Transfer;
using Ionic.Zip;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using NLog;
using Count4U.Common.Extensions;
using Count4U.Common.Services.UICommandService;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using Count4U.Model.ExportImport.Items;
using Count4U.Model.Interface.ProcessC4U;
using Count4U.Model.ProcessC4U;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class ProcessAddEditGridViewModel : NotificationObject//, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;

		private readonly DelegateCommand _addProcessCommand;
		private readonly DelegateCommand _backToRootProcessCommand;
		private readonly UICommandRepository<FileItemViewModel> _commandRepositoryObject;

		private readonly DelegateCommand<FileItemViewModel> _editSelectedCommand;
		private readonly DelegateCommand<FileItemViewModel> _deleteSelectedCommand;
		private readonly DelegateCommand<FileItemViewModel> _inProcessSelectedCommand;
		private readonly DelegateCommand<FileItemViewModel> _setInProcessSelectedCommand;

		private readonly UICommandRepository _commandRepository;
		private readonly IServiceLocator _serviceLocator;
		private readonly ModalWindowLauncher _modalWindowLauncher;
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly DBSettings _dbSettings;
		private readonly IProcessRepository _processRepository;

       // private readonly ObservableCollection<ZipImportItemViewModel> _items;

        private readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;
        private readonly InteractionRequest<MessageBoxNotification> _messageBoxRequest;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

		protected readonly ObservableCollection<FileItemViewModel> _items;
		private FileItemViewModel _selectedItem;
		protected bool _isChecked;

      
		public ProcessAddEditGridViewModel(
			IProcessRepository processRepository,
            IEventAggregator eventAggregator,
			UICommandRepository commandRepository,
			UICommandRepository<FileItemViewModel> commandRepositoryObject,
			IServiceLocator serviceLocator,
			ModalWindowLauncher modalWindowLauncher	,
			DBSettings dbSettings,
            IUserSettingsManager userSettingsManager)
        {
             this._eventAggregator = eventAggregator;
			 this._modalWindowLauncher = modalWindowLauncher;
			 this._userSettingsManager = userSettingsManager;
			 this._serviceLocator = serviceLocator;
			 this._commandRepository = commandRepository;
			 this._commandRepositoryObject = commandRepositoryObject;
			 this._dbSettings = dbSettings;
			 this._processRepository = processRepository;
			 this._items = new ObservableCollection<FileItemViewModel>();

            this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
    		this._addProcessCommand = _commandRepository.Build(enUICommand.Add, AddProcessCommandExecuted, AddProcessCommandCanExecuted);
			this._deleteSelectedCommand = _commandRepositoryObject.Build(enUICommand.Delete, DeleteSelectedCommandExecuted);
			this._editSelectedCommand = _commandRepositoryObject.Build(enUICommand.Edit, EditSelectedCommandExecuted);
			this._setInProcessSelectedCommand = _commandRepositoryObject.Build(enUICommand.Accept, SetInProcessSelectedCommandExecuted);
			this._backToRootProcessCommand = _commandRepository.Build(enUICommand.Accept, BackToRootProcessCommandExecuted);
			
			this._inProcessSelectedCommand = _commandRepositoryObject.Build(enUICommand.Accept, InProcessSelectedCommandExecuted);			

            this._messageBoxRequest = new InteractionRequest<MessageBoxNotification>();

			this.Build();
        }

		public ObservableCollection<FileItemViewModel> Items
		{
			get { return this._items; }
		}

		public FileItemViewModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				RaisePropertyChanged(() => SelectedItem);
			}
		}

		public bool IsChecked
		{
			get { return this._isChecked; }
			set
			{
				this._isChecked = value;
				RaisePropertyChanged(() => IsChecked);
				foreach (FileItemViewModel item in this._items)
				{
					item.IsChecked = this._isChecked;
				}
			}
		}


		public DelegateCommand<FileItemViewModel> EditSelectedCommand
		{
			get { return this._editSelectedCommand; }
		}

		public DelegateCommand<FileItemViewModel> DeleteSelectedCommand
		{
			get { return this._deleteSelectedCommand; }
		}

		private void EditSelectedCommandExecuted(FileItemViewModel processItemViewModel)
		{
			Count4U.Model.ProcessC4U.Process  process = this._selectedItem.ProcessObject;
			          this._eventAggregator.GetEvent<ProcessAddEvent>().Publish(new ProcessAddedEventPayLoad()
                                                                           {
                                                                               Process = process,
                                                                               AddUnknownProcess = false
                                                                           });
					  this.Build();
		}

	
		public DelegateCommand<FileItemViewModel> SetInProcessSelectedCommand
		{
			get { return this._setInProcessSelectedCommand; }
		}


		public DelegateCommand BackToRootProcessCommand
		{
			get { return this._backToRootProcessCommand; }
		}

		public DelegateCommand<FileItemViewModel> InProcessSelectedCommand
		{
			get { return this._inProcessSelectedCommand; }
		}

		private void SetInProcessSelectedCommandExecuted(FileItemViewModel inventorItemViewModel)
		{
			if (string.IsNullOrWhiteSpace(inventorItemViewModel.ObjectCode) == false)
			{
				using (new CursorWait())
				{
					this._processRepository.SetStatusInProcess(inventorItemViewModel.ObjectCode);
					this._dbSettings.SettingsRepository.ProcessCode = inventorItemViewModel.ObjectCode;
						this.Build();

				}
			}

		}


		private void BackToRootProcessCommandExecuted()
		{
				using (new CursorWait())
				{
					this._processRepository.ResetStatusInProcess();
					this._dbSettings.SettingsRepository.ProcessCode = "";
					this.Build();
				}
		}

		private void InProcessSelectedCommandExecuted(FileItemViewModel inventorItemViewModel)
		{
			if (string.IsNullOrWhiteSpace(inventorItemViewModel.ObjectCode) == false)
			{
			}

		}

		
		protected void Build()
		{
			this._items.Clear();
			//IConnectionDB resreshConnection = new IConnectionDB(this._dbSettings);	 //так не правильно потому что другой экземпляр создается 		IConnectionDB
			//this._processRepository.RefreshBaseEFRepositoryConnectionDB(); не работает, другой экземпляр . Надо выйти и зайти 

			Processes processes = this._processRepository.GetProcesses();
			List<string> pathList = new List<string>();

			foreach (Count4U.Model.ProcessC4U.Process process in processes)
			{
				try
				{
					FileItemViewModel item = NewFileItemViewModel(process);
			 		this._items.Add(item);
				}
				catch { }
			}
			
		}

		


		private FileItemViewModel NewFileItemViewModel(Count4U.Model.ProcessC4U.Process process)
		{
			FileItemViewModel item = new FileItemViewModel();
			item.ProcessObject = process;
			item.Name = process.Name;
			item.Tag1 = process.Tag1;
			item.Tag2 = process.Tag2;
			item.Tag3 = process.Tag3;
			item.Title = process.Title;
			item.InProcess = false;
			if (process.StatusCode.ToLower().Trim() == "inprocess") item.InProcess = true;		 //StatusAuditConfigEnum.InProcess.ToString()
			item.Description = process.Description;
			item.Manager = process.Manager;
			DateTime dt = Convert.ToDateTime(process.CreateDate);
			item.DateTimeCreated = dt;
			//item.Date = ;
			item.Code = process.Code;
			item.ObjectCode = process.ProcessCode;
			return item;
		}

		public DelegateCommand AddProcessCommand
		{
			get { return this._addProcessCommand; }
		}

		private void AddProcessCommandExecuted()
		{
			ProcessAddedEventPayLoad payload = new ProcessAddedEventPayLoad();
			this._eventAggregator.GetEvent<ProcessAddEvent>().Publish(payload);

			this.Build();
		}

	
		private bool AddProcessCommandCanExecuted()
		{
			return true;
		}

		private void DeleteSelectedCommandExecuted(FileItemViewModel inventorItemViewModel)
		{
			if (string.IsNullOrWhiteSpace(inventorItemViewModel.ObjectCode) == false)
			{
				using (new CursorWait())
				{
					this._processRepository.Delete(inventorItemViewModel.ObjectCode);

					this.Build();
				}
			}

		}



        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return this._yesNoRequest; }
        }

        public InteractionRequest<MessageBoxNotification> MessageBoxRequest
        {
            get { return this._messageBoxRequest; }
        }

	
    }
}