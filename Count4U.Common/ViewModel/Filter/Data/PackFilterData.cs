using System;
using System.Xml.Serialization;

namespace Count4U.Common.ViewModel.Filter.Data
{
      [Serializable]
    public class PackFilterData : CommonFilterData, IFilterData
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

          [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchPackFieldView; } }

        public bool IsAnyField()
        {
            return !String.IsNullOrWhiteSpace(Name) || !String.IsNullOrWhiteSpace(Code) || From != null || To != null;
        }      
    }
}