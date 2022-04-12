using Count4U.Common.Web;
using System;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    [Serializable]
    public class CustomerPostData
    {
        public bool IsNew { get; set; }
        public string CustomerCode { get; set; }
        public FtpCommandResult ftpCommandResult { get; set; }
    }
}