using System;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels
{
    public class DocumentHeaderAddEditViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDocumentHeaderRepository _documentHeaderRepository;
        private readonly IIturRepository _iturRepository;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private bool _isNew;

        private string _name;
        private string _workerGUID;
        private string _documentCode;
        private string _iturCode;
        private bool _isApprove;

        private DocumentHeader _documentHeader;

        public DocumentHeaderAddEditViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IDocumentHeaderRepository documentHeaderRepository,
            IIturRepository iturRepository)
            : base(contextCBIRepository)
        {
            this._iturRepository = iturRepository;
            this._documentHeaderRepository = documentHeaderRepository;
            this._eventAggregator = eventAggregator;
            this._okCommand = new DelegateCommand(OkCommandExecuted);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string WorkerGUID
        {
            get { return this._workerGUID; }
            set
            {
                this._workerGUID = value;
                this.RaisePropertyChanged(() => this.WorkerGUID);
            }
        }

        public string DocumentCode
        {
            get { return this._documentCode; }
            set
            {
                this._documentCode = value;
                this.RaisePropertyChanged(() => this.DocumentCode);
            }
        }

        public string IturCode
        {
            get { return this._iturCode; }
            set
            {
                this._iturCode = value;
                this.RaisePropertyChanged(() => this.IturCode);
            }
        }

        public bool IsApprove
        {
            get { return this._isApprove; }
            set
            {
                this._isApprove = value;
                this.RaisePropertyChanged(() => this.IsApprove);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.DocumentCode))
            {
                this._isNew = false;
                string documentCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.DocumentCode).Value;
                this._documentHeader = this._documentHeaderRepository.GetDocumentHeaderByCode(documentCode, base.GetDbPath);
            }
            else
            {
                this._isNew = true;
                if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.IturCode))
                    this._iturCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.IturCode).Value;

                this._documentHeader = new DocumentHeader();
                this._documentHeader.Code = Utils.CodeNewGenerate();
                this._documentHeader.DocumentCode = this._documentHeader.Code;
                this._documentHeader.IturCode = this._iturCode;
            }

            if (this._documentHeader != null) //load values on form
            {
                this._name = this._documentHeader.Name;
                this._workerGUID = this._documentHeader.WorkerGUID;
                this._documentCode = this._documentHeader.DocumentCode;
                this._iturCode = this._documentHeader.IturCode;
                this._isApprove = this._documentHeader.Approve ?? false;
            }
        }


        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
            //save values from form

            this._documentHeader.Name = this._name;
            this._documentHeader.WorkerGUID = this._workerGUID;
            this._documentHeader.DocumentCode = this._documentCode;
            this._documentHeader.ModifyDate = DateTime.Now;
            this._documentHeader.Approve = this._isApprove;

            if (this._isNew)
            {
                if (!String.IsNullOrEmpty(this._iturCode))
                {
                    this._documentHeader.CreateDate = DateTime.Now;
                    Itur itur = this._iturRepository.GetIturByCode(this._iturCode, base.GetDbPath);
                    this._documentHeaderRepository.Insert(itur, this._documentHeader, base.GetDbPath);

                    this._eventAggregator.GetEvent<DocumentHeaderAddedEditedEvent>().Publish(
                            new DocumentHeaderAddedEditedEventPayload() { DocumentHeader = this._documentHeader, IsNew = true });
                }
            }
            else
            {
                this._documentHeaderRepository.Update(this._documentHeader, base.GetDbPath);
                this._eventAggregator.GetEvent<DocumentHeaderAddedEditedEvent>().Publish(
                                      new DocumentHeaderAddedEditedEventPayload() { DocumentHeader = this._documentHeader, IsNew = false });
            }



            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }
    }
}