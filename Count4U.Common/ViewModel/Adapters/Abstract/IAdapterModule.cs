using System;
using System.Text;

namespace Count4U.Common.ViewModel.Adapters.Abstract
{
    public interface IAdapterModule
    {
        Encoding Encoding { get; set; }
        Action<string> UpdateLog { set; }
        Action<bool> SetIsBusy { set; }
        Action<long> UpdateProgress { set; }
        Action<string> UpdateBusyText { set; }
        Action<bool> SetIsCancelOk { set; } 
    }
}