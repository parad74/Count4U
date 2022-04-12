using System;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.ParsingMask;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Count4U.Model.Interface;
using Count4U.Model;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask
{
    public class MaskTemplateAddEditViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
		//private readonly IMaskTemplateRepository _maskTemplateRepository;
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private readonly IEventAggregator _eventAggregator;

        private bool _isNew;

		private MaskTemplate _maskTemplate;
        private string _name;
        private string _expression;
        private string _action;

        public MaskTemplateAddEditViewModel(IContextCBIRepository contextCbiRepository,
			//IMaskTemplateRepository maskTemplateRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            this._eventAggregator = eventAggregator;
			//_maskTemplateRepository = maskTemplateRepository;
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public string Error
        {
            get { return string.Empty; }
        }

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

        public string Expression
        {
            get { return this._expression; }
            set
            {
                this._expression = value;
                RaisePropertyChanged(() => Expression);
            }
        }

        public string Action
        {
            get { return this._action; }
            set
            {
                this._action = value;
                RaisePropertyChanged(() => Action);
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

        public override void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.MaskTemplateCode))
            {
                string maskCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.MaskTemplateCode).Value;
				//TODO:
				//_maskTemplate = this._maskTemplateRepository.GetMasks().FirstOrDefault(r => r.Code == maskCode);

				//if (this._maskTemplate != null)
				//{
				//    Name = this._maskTemplate.Name;
					//Expression = this._maskTemplate.RegExp;
					//Action = this._maskTemplate.Value;
				//}

                this._isNew = false;
            }
            else
            {
				_maskTemplate = new MaskTemplate();
                this._maskTemplate.Code = Utils.CodeNewGenerate();
                this._isNew = true;
            }
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName]
        {
            get
            {
//                switch (columnName)
//                {
//
//                }

                return String.Empty;
            }

        }



        #endregion

        private bool OkCommandCanExecute()
        {
            return !String.IsNullOrEmpty(this._name);
        }

        private void OkCommandExecuted()
        {
            this._maskTemplate.Name = this._name;
			//TODO:
			//_maskTemplate.RegExp = this._expression;
			//_maskTemplate.Value = this._action;

			//TODO:
			//if (this._isNew)
			//    this._maskRepository.Insert(this._maskTemplate, GetDbPath);
			//else
			//    this._maskRepository.Update(this._maskTemplate, GetDbPath);

            this._eventAggregator.GetEvent<MaskTemplateAddedEditedEvent>().Publish(this._maskTemplate);
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }
    }
}