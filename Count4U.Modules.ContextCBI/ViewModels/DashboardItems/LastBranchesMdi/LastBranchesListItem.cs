using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Main;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class LastBranchesListItem : NotificationObject
    {
        private readonly Branch _branch;

        public LastBranchesListItem(Branch branch)
        {
            this._branch = branch;

            this.Code = this._branch.Code;
            this.Name = this._branch.Name;
            this.CodeLocal = this._branch.BranchCodeLocal;
        }

        public string CodeLocal { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Branch Branch
        {
            get { return this._branch; }
        }
    }
}