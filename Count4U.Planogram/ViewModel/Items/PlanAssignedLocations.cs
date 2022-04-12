using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Count4U.Planogram.ViewModel
{
    [Serializable]
    public class PlanAssignedLocations
    {
        private readonly List<string> _assignedLocationCodes;

        public PlanAssignedLocations()
        {
            _assignedLocationCodes = new List<string>();
        }
        public string CurrentLocationCode { get; set; }

//        public List<string> AssignedLocationCodes
//        {
//            get { return _assignedLocationCodes; }
//        }
    }
}