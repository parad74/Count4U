using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common.Enums;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Common.Constants;

namespace Count4U.Report.ViewModels.ExportPda
{
    public class ExportPdaExtraSettingsViewModel : CBIContextBaseViewModel
    {
        private readonly IReportRepository _reportRepository;

        private bool _isAutoPrint;
        private readonly ObservableCollection<Count4U.GenerationReport.Report> _reports;
        private Count4U.GenerationReport.Report _selectedReport;
        private bool _isEditable;

        private Customer _customer;
        private Branch _branch;
        private Inventor _inventor;

        public ExportPdaExtraSettingsViewModel(
            IContextCBIRepository contextCbiRepository,
            IReportRepository reportRepository)
            : base(contextCbiRepository)
        {
            this._reportRepository = reportRepository;
            this._reports = new ObservableCollection<Count4U.GenerationReport.Report>();
            _isEditable = true;
        }

        public bool IsAutoPrint
        {
            get { return _isAutoPrint; }
            set
            {
                _isAutoPrint = value;
                RaisePropertyChanged(() => IsAutoPrint);
            }
        }

        public ObservableCollection<Count4U.GenerationReport.Report> Reports
        {
            get { return _reports; }
        }

        public GenerationReport.Report SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                _selectedReport = value;
                RaisePropertyChanged(() => SelectedReport);

                RaisePropertyChanged(() => IsAutoPrintEnabled);
            }
        }

        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(() => IsEditable);
            }
        }

        public bool IsAutoPrintEnabled
        {
            get { return this._selectedReport != null; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            foreach (var report in this._reportRepository.GetAllowedReportTemplate(String.Empty, String.Empty, String.Empty,
                                                                                   ViewDomainContextEnum.ItursIturDoc,
                                                                                   new List<AllowedReportTemplate> { AllowedReportTemplate.PrintInventProduct }))
            {
                this._reports.Add(report);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void SetCustomer(Customer customer)
        {
            if (customer == null) return;

            _customer = customer;

            IsAutoPrint = customer.Print;

            SelectedReport = this._reports.FirstOrDefault(r => r.FileName == customer.ReportName);
        }

        public void SetBranch(Branch branch, enBranchAdapterInherit mode)
        {
            if (branch == null) return;

            _branch = branch;

            SetSelectedAdapterStateForBranch(mode);
        }

        public void SetInventor(Inventor inventor, enInventorAdapterInherit mode)
        {
            if (inventor == null) return;

            _inventor = inventor;

            SetSelectedAdapterStateForInventor(mode);
        }

        public void SetSelectedAdapterStateForBranch(enBranchAdapterInherit mode)
        {
            Customer customer = this.CurrentCustomer;

            if (mode == enBranchAdapterInherit.InheritFromCustomer && customer != null)
            {
                IsAutoPrint = customer.Print;
                SelectedReport = this._reports.FirstOrDefault(r => r.FileName == customer.ReportName);
            }
            else
            {
				IsAutoPrint = false; //_branch.Print; используется для InProcess, потому что 
                SelectedReport = this._reports.FirstOrDefault(r => r.FileName == _branch.ReportName);
            }

        }


        public void SetSelectedAdapterStateForInventor(enInventorAdapterInherit mode)
        {
            Customer customer = this.CurrentCustomer;
            Branch branch = this.CurrentBranch;

            if (mode == enInventorAdapterInherit.InheritFromCustomer && customer != null)
            {
                IsAutoPrint = customer.Print;
                SelectedReport = this._reports.FirstOrDefault(r => r.FileName == customer.ReportName);
            }
            else if (mode == enInventorAdapterInherit.InheritFromBranch && branch != null)
            {
				IsAutoPrint = false; //_branch.Print; используется для InProcess, потому что 
                SelectedReport = this._reports.FirstOrDefault(r => r.FileName == branch.ReportName);
            }
            else
            {
                IsAutoPrint = _inventor.Print;
                SelectedReport = this._reports.FirstOrDefault(r => r.FileName == _inventor.ReportName);
            }
        }

        public void ApplyChanges()
        {
            if (_customer != null)
            {
				// исчпользуется для другого , потому что	   ??
				_customer.Print = this._isAutoPrint;

				_customer.ReportName = this._selectedReport == null ? String.Empty : this._selectedReport.FileName;
				_customer.ReportPath = this._selectedReport == null ? String.Empty : this._selectedReport.Path;
				_customer.ReportDS = this._selectedReport == null ? String.Empty : this._selectedReport.TypeDS;
				_customer.ReportContext = this._selectedReport == null ? String.Empty : this._selectedReport.DomainType;
            }

            if (_branch != null)
            {
				// _branch.Print = this._isAutoPrint;	 = false; //_branch.Print; используется для InProcess, потому что 
				// исчпользуется для другого , потому что
				//_branch.ReportName = this._selectedReport == null ? String.Empty : this._selectedReport.FileName;
				//_branch.ReportPath = this._selectedReport == null ? String.Empty : this._selectedReport.Path;
				//_branch.ReportDS = this._selectedReport == null ? String.Empty : this._selectedReport.TypeDS;
                // _branch.ReportContext = this._selectedReport == null ? String.Empty : this._selectedReport.DomainType;
            }

            if (_inventor != null)
            {
                _inventor.Print = this._isAutoPrint;

                _inventor.ReportName = this._selectedReport == null ? String.Empty : this._selectedReport.FileName;
                _inventor.ReportPath = this._selectedReport == null ? String.Empty : this._selectedReport.Path;
                _inventor.ReportDS = this._selectedReport == null ? String.Empty : this._selectedReport.TypeDS;
                _inventor.ReportContext = this._selectedReport == null ? String.Empty : this._selectedReport.DomainType;
            }
        }
    }
}