using System;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.Services.Navigation.Data
{
    [Serializable]
    public class InventProductDetailsData
    {
        public SelectParams IturSelectParams { get; set; }
        public string IturCode { get; set; }
        public string DocumentCode { get; set; }
        public long InventProductId { get; set; }

        public string SearchItem { get; set; }
        public string SearchExpression { get; set; }
    }
}