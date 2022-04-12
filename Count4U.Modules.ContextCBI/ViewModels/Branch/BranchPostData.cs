using System;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    [Serializable]
    public class BranchPostData
    {
        public string BranchCode { get; set; }
        public bool IsNew { get; set; }
    }
}