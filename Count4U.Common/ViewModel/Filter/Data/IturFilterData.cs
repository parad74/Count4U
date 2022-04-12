using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class IturFilterData : CommonFilterData, IFilterData
    {
        public string Number { get; set; }
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public string ERP { get; set; }

        public bool IsLocation { get; set; }
        public bool IsStatus { get; set; }
		public bool IsTag { get; set; }

        public List<string> Locations { get; set; } //code
        public List<int> Statuses { get; set; } //bit
		public List<string> Tags { get; set; } 

        public bool IsLocationExpanded { get; set; }
        public bool IsStatusExpanded { get; set; }
        public bool IsInventProductExpanded { get; set; }
		public bool IsTagExpanded { get; set; }

//        public bool? IsDisabled { get; set; }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchIturFieldView; } }

		public IturFilterData()
		{
			this.Number = "";
			this.Status = "";
			this.ERP = "";
		}

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            if (!String.IsNullOrWhiteSpace(Number))
                return true;

            if (!String.IsNullOrWhiteSpace(Status))
                return true;

            if (Date != null)
                return true;

            if (!String.IsNullOrWhiteSpace(ERP))
                return true;

            if (IsLocation)
                return true;

            if (IsStatus)
                return true;

			if (IsTag)
				return true;

//            if (IsDisabled.HasValue)
//                return true;

            return false;
        }

        public void ApplyToSelectParams(SelectParams selectParams, Locations locations/*, Dictionary<string, StatusItur> statuses*/)
        {
            base.ApplySortToSelectParams(selectParams);

            if (!String.IsNullOrEmpty(Number))
            {
                List<int> raw = CommaDashStringParser.Parse(Number);
                if (raw != null && raw.Any())
                {
                    List<long> searchNum = raw.Distinct().Select(r => (long)r).ToList();
                    selectParams.FilterListParams.Add("Number", new FilterListParam()
                        {
                            Values = searchNum
                        });
                }
            }

            //            if (!String.IsNullOrEmpty(this.Status))
            //            {
            //                List<int> searchStatus = statuses.Data.Where(r => r.Name.ToLower().Contains(this.Status.ToLower())).Select(r => r.Bit).ToList();
            //
            //                if (searchStatus.Count == 0)
            //                    searchStatus.Add(-999);
            //
            //                selectParams.FilterIntListParams.Add("StatusIturBit", new FilterIntListParam()
            //                    {
            //                        Data = searchStatus
            //                    });
            //            }

            if (!String.IsNullOrEmpty(this.ERP))
            {
                selectParams.FilterParams.Add("ERPIturCode", new FilterParam()
                {
                    Operator = FilterOperator.Contains,
                    Value = this.ERP
                });
            }

            if (this.Date != null && this.Date != DateTime.MinValue)
            {
                DateTime from = new DateTime(this.Date.Value.Year, this.Date.Value.Month, this.Date.Value.Day, 0, 0, 0);
                DateTime to = new DateTime(this.Date.Value.Year, this.Date.Value.Month, this.Date.Value.Day, 23, 59, 59);
                selectParams.FilterDateTimeParams.Add("CreateDate",
                                                new FilterParam()
                                                {
                                                    Operator = FilterOperator.DateTimeBetween,
                                                    Value = from,
                                                    Value1 = to
                                                });
            }

            if (IsLocation && Locations.Any())
            {
                selectParams.FilterStringListParams.Add("LocationCode", new FilterStringListParam()
                {
                    Values = Locations
                });
            }

			if (IsTag && Tags.Any())
			{
				selectParams.FilterStringListParams.Add("Tag", new FilterStringListParam()
				{
					Values = Tags
				});
			}

            if (IsStatus && Statuses.Any())
            {
                List<int> statuses = Statuses.ToList();

                //temporary hack
                if (statuses.Any(r => r == 6))
                    statuses.Add(999);

                selectParams.FilterIntListParams.Add("StatusIturGroupBit", new FilterIntListParam()
                {
                    Values = statuses
                });
            }

            //            if (IsDisabled.HasValue)
            //            {
            //                selectParams.FilterParams.Add("Disabled", new FilterParam()
            //                {
            //                    Operator = FilterOperator.Equal,
            //                    Value = this.IsDisabled.Value
            //                });
            //            }
        }
    }
}