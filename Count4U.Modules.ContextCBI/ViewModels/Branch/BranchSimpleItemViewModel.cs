using Count4U.Common.Helpers;
using Count4U.Model.Main;
using System;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BranchSimpleItemViewModel
    {
        private readonly Branch _branch;

        private string _fancyName;

        public BranchSimpleItemViewModel(Branch branch)
        {
            _branch = branch;

            //_fancyName = UtilsConvert.HebrewText(String.Format("{0} [{1},{2}]", _branch.Name, _branch.BranchCodeLocal, _branch.BranchCodeERP));                                                            
            _fancyName = UtilsConvert.BranchFancyName(_branch);
        }

        public string FancyName
        {
            get { return _fancyName; }
            set
            {
                _fancyName = value;                
            }
        }

        public Branch Branch
        {
            get { return _branch; }
        }
    }
}