using System;

namespace Count4U.Common.Services.Navigation.Data.SearchData
{
    [Serializable]
    public class InventProductSearchData
    {
        public string Makat { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
    }
}