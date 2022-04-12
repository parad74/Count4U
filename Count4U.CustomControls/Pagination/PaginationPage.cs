using System;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.CustomControls.Pagination
{
    public class PaginationPage : NotificationObject
    {
        private DelegateCommand<object> _pageCommand;

        public PaginationPage()
        {
            
        }

        private void PageCommandExecuted()
        {
            
        }

        public int Data { get; set; }

        public DelegateCommand<object> PageCommand
        {
            get { return this._pageCommand; }
            set { this._pageCommand = value; }
        }
    }
}