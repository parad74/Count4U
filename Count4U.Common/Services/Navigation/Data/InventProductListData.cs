using System;

namespace Count4U.Common.Services.Navigation.Data
{
    [Serializable]
    public class InventProductListData
    {
        public string Filter { get; set; }
        public string FilterExpression { get; set; }    
    }
}