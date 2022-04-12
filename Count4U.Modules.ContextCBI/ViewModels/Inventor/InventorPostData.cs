using Count4U.Common.Web;
using System;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    [Serializable]
    public class InventorPostData
    {
        public bool IsNew { get; set; }
        public string InventorCode;
        public FtpCommandResult ftpCommandResult { get; set; }
    }
}