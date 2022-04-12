using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Interfaces;
using Count4U.Modules.ContextCBI.Views.Report;
using Count4U.Report.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System.Linq;
using Microsoft.Practices.Unity;
//using Microsoft.Reporting.WinForms;
using Microsoft.Practices.ServiceLocation;
using Count4U.Common.Constants;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Reports
{
    public class ReportsDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IRegionManager _regionManager;
        private readonly IReportRepository _reportRepository;
        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly UICommandRepository _commandRepository;


        private readonly DelegateCommand _viewCommand;

        private readonly ObservableCollection<ReportTreeItemRootViewModel> _items;

        public ReportsDashboardPartViewModel(
            IContextCBIRepository contextCbiRepository,
            IReportRepository reportRepository,
            IRegionManager regionManager,
            IGenerateReportRepository generateReportRepository,
            UICommandRepository commandRepository
            )
            : base(contextCbiRepository)
        {
            _commandRepository = commandRepository;
            this._generateReportRepository = generateReportRepository;
            this._reportRepository = reportRepository;
            this._regionManager = regionManager;
            this._viewCommand = _commandRepository.Build(enUICommand.More, ViewCommandExecuted);

            this._items = new ObservableCollection<ReportTreeItemRootViewModel>();
        }

        public DelegateCommand ViewCommand
        {
            get { return this._viewCommand; }
        }

        public ObservableCollection<ReportTreeItemRootViewModel> Items
        {
            get { return this._items; }
        }

        private void ViewCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.All);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.All);

            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			Task.Factory.StartNew(BuildReports).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Clear();
        }

        private void BuildReports()
        {
            string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
            string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
            string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

            var reports = this._reportRepository.GetAllowedReportTemplateTag(currentCustomerCode, currentBranchCode, currentInventorCode, ViewDomainContextEnum.All,
                                                          new List<AllowedReportTemplate>() { AllowedReportTemplate.Audit, AllowedReportTemplate.Main });

            DelegateCommand<ReportTreeItemViewModel> generateCommand = new DelegateCommand<ReportTreeItemViewModel>(GenerateCommandExecuted);

            var tags = reports.Select(r => r.Tag).Distinct();

            var rootNodes = new List<ReportTreeItemRootViewModel>();
            foreach (var tag in tags)
            {
                ReportTreeItemRootViewModel rootNode = new ReportTreeItemRootViewModel();
                rootNode.Header = tag;

                foreach (var report in reports.Where(r => r.Tag == tag))
                {
                    ReportTreeItemViewModel childNode = new ReportTreeItemViewModel(report);
                    childNode.GenerateCommand = generateCommand;
                    rootNode.Children.Add(childNode);
                }

                rootNodes.Add(rootNode);
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(() => rootNodes.ForEach(r => this._items.Add(r))));
        }

        private void GenerateCommandExecuted(ReportTreeItemViewModel item)
        {
            GenerateReportArgs args = new GenerateReportArgs();
            args.Report = item.Report;
            args.ViewDomainContextType = ViewDomainContextEnum.All;
            args.Customer = base.CurrentCustomer;
            args.Branch = base.CurrentBranch;
            args.Inventor = base.CurrentInventor;
            args.DbPath = base.GetDbPath;

            this._generateReportRepository.GenerateReport(args);
            GC.Collect();
        }

        public void Refresh()
        {

        }

        public void Clear()
        {

        }
    }
}