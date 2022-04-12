using System.IO;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc
{
    public class CBIObjectPropertiesViewModel : CBIContextBaseViewModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IInventorRepository _inventorRepository;
        private readonly IStatusInventorRepository _statusInventorRepository;

        private readonly DelegateCommand _dbPathOpenCommand;
        private readonly DelegateCommand _importPathOpenCommand;
        private readonly DelegateCommand _exportPdaPathOpenCommand;

        private readonly DelegateCommand _codeCopyCommand;
        private readonly DelegateCommand _dbPathCopyCommand;
        private readonly DelegateCommand _importPathCopyCommand;
        private readonly DelegateCommand _exportPdaPathCopyCommand;

        private string _code;
        private string _dbPath;
        private string _importPath;
        private string _exportPdaPath;
        private string _createDate;
        private string _modifyDate;
        private string _inventorStatus;

        public CBIObjectPropertiesViewModel(
            IContextCBIRepository contextCbiRepository,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            IInventorRepository inventorRepository,
            IStatusInventorRepository statusInventorRepository)
            : base(contextCbiRepository)
        {
            this._statusInventorRepository = statusInventorRepository;
            this._inventorRepository = inventorRepository;
            this._branchRepository = branchRepository;
            this._customerRepository = customerRepository;

            this._dbPathOpenCommand = new DelegateCommand(DbPathOpenCommandExecuted, DbPathOpenCommandCanExecute);
            this._importPathOpenCommand = new DelegateCommand(ImportPathOpenCommandExecuted, ImportPathOpenCommandCanExecute);
            this._exportPdaPathOpenCommand = new DelegateCommand(ExportPdaPathOpenCommandExecuted, ExportPdaPathOpenCommandCanExecute);

            this._codeCopyCommand = new DelegateCommand(CodeCopyCommandExecuted);
            this._dbPathCopyCommand = new DelegateCommand(DbPathCopyCommandExecuted);
            this._importPathCopyCommand = new DelegateCommand(ImportPathCopyCommandExecuted);
            this._exportPdaPathCopyCommand = new DelegateCommand(ExportPdaPathCopyCommandExecuted);
        }    

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        public string DbPath
        {
            get { return _dbPath; }
            set
            {
                _dbPath = value;
                RaisePropertyChanged(() => DbPath);
            }
        }

        public string ImportPath
        {
            get { return _importPath; }
            set
            {
                _importPath = value;
                RaisePropertyChanged(() => ImportPath);
            }
        }

        public string ExportPdaPath
        {
            get { return _exportPdaPath; }
            set
            {
                _exportPdaPath = value;
                RaisePropertyChanged(() => ExportPdaPath);
            }
        }

        public string CreateDate
        {
            get { return _createDate; }
            set
            {
                _createDate = value;
                RaisePropertyChanged(() => CreateDate);
            }
        }

        public string ModifyDate
        {
            get { return _modifyDate; }
            set
            {
                _modifyDate = value;
                RaisePropertyChanged(() => ModifyDate);
            }
        }

        public string InventorStatus
        {
            get { return _inventorStatus; }
            set
            {
                _inventorStatus = value;
                RaisePropertyChanged(() => InventorStatus);
            }
        }

        public DelegateCommand DbPathOpenCommand
        {
            get { return _dbPathOpenCommand; }
        }

        public DelegateCommand ImportPathOpenCommand
        {
            get { return _importPathOpenCommand; }
        }

        public DelegateCommand ExportPdaPathOpenCommand
        {
            get { return _exportPdaPathOpenCommand; }
        }

        public bool IsInventor { get; set; }

        public DelegateCommand CodeCopyCommand
        {
            get { return _codeCopyCommand; }
        }

        public DelegateCommand DbPathCopyCommand
        {
            get { return _dbPathCopyCommand; }
        }

        public DelegateCommand ImportPathCopyCommand
        {
            get { return _importPathCopyCommand; }
        }

        public DelegateCommand ExportPdaPathCopyCommand
        {
            get { return _exportPdaPathCopyCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            object domainObject = base.GetCurrentDomainObject();
            string dbPath = base.ContextCBIRepository.GetDBPath(domainObject);
            string fullPath = string.Empty;

            switch (base.CBIDbContext)
            {
                case Common.NavigationSettings.CBIDbContextCustomer:
                    fullPath = this._customerRepository.Connection.BuildCount4UDBFilePath(dbPath);
                    break;
                case Common.NavigationSettings.CBIDbContextBranch:
                    fullPath = this._branchRepository.Connection.BuildCount4UDBFilePath(dbPath);
                    break;
                case Common.NavigationSettings.CBIDbContextInventor:
                    fullPath = this._inventorRepository.Connection.BuildCount4UDBFilePath(dbPath);
                    break;
            }

            FileInfo fi = new FileInfo(fullPath);
            DbPath = fi.FullName;

            //string importPath = base.ContextCBIRepository.GetImportFolderPath(domainObject).Trim('\\') + @"\" + FileSystem.inData; 
            string importPath = base.ContextCBIRepository.GetImportFolderPath(domainObject);
            fi = new FileInfo(importPath);
            ImportPath = fi.FullName;

            string exportPdaPath = base.ContextCBIRepository.GetExportToPDAFolderPath(domainObject, true);

            if (!Directory.Exists(exportPdaPath))
                Directory.CreateDirectory(exportPdaPath);

            fi = new FileInfo(exportPdaPath);
            ExportPdaPath = fi.FullName;

            Customer customer = domainObject as Customer;
            if (customer != null)
            {
                this.Code = customer.Code;
                this.CreateDate = String.Empty;
                this.ModifyDate = String.Empty;
            }

            Branch branch = domainObject as Branch;
            if (branch != null)
            {
                this.Code = branch.Code;
                this.CreateDate = String.Empty;
                this.ModifyDate = String.Empty;
            }

            Inventor inventor = domainObject as Inventor;
            if (inventor != null)
            {
                this.Code = inventor.Code;
                this.IsInventor = true;
                this.CreateDate = UtilsConvert.DateToStringLong(inventor.CreateDate);
                this.ModifyDate = UtilsConvert.DateToStringLong(inventor.InventorDate);

                var auditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(base.Context, true);
                if (auditConfig != null)
                {
                    if (!String.IsNullOrEmpty(auditConfig.StatusInventorCode))
                    {
                        StatusInventor status = _statusInventorRepository.GetStatusByCode(auditConfig.StatusInventorCode);
                        if (status != null)
                            this.InventorStatus = status.Name;
                    }
                }

            }
        }

        private bool DbPathOpenCommandCanExecute()
        {
            return File.Exists(this._dbPath);
        }

        private void DbPathOpenCommandExecuted()
        {
            FileInfo fi = new FileInfo(_dbPath);

            Utils.OpenFolderInExplorer(fi.DirectoryName);
        }

        private bool ImportPathOpenCommandCanExecute()
        {
            return Directory.Exists(this._importPath);
        }

        private void ImportPathOpenCommandExecuted()
        {
            Utils.OpenFolderInExplorer(this._importPath);
        }


        private bool ExportPdaPathOpenCommandCanExecute()
        {
            return Directory.Exists(this._exportPdaPath);
        }

        private void ExportPdaPathOpenCommandExecuted()
        {
            Utils.OpenFolderInExplorer(this._exportPdaPath);
        }

        private void ExportPdaPathCopyCommandExecuted()
        {
            Clipboard.SetText(this._exportPdaPath);
        }

        private void ImportPathCopyCommandExecuted()
        {
            Clipboard.SetText(this._importPath);
        }

        private void DbPathCopyCommandExecuted()
        {
            Clipboard.SetText(this._dbPath);
        }

        private void CodeCopyCommandExecuted()
        {
            Clipboard.SetText(this._code);
        }
    }
}