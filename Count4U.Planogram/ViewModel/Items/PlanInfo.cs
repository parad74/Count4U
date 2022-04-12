using System;
using System.Collections.Generic;

namespace Count4U.Planogram.ViewModel
{
    [Serializable]
    public class PlanInfo
    {
        public PlanInfo()
        {
            Info = new List<PlanInfoItem>();
        }

        public List<PlanInfoItem> Info { get; set; }
        public int TotalIturs { get; set; }
        public double TotalItems { get; set; }
        public double Process { get; set; }        
    }
}