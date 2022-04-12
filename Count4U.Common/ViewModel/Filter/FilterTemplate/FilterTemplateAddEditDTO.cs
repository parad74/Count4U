using System;

namespace Count4U.Common.ViewModel.Filter.FilterTemplate
{
    [Serializable]
    public class FilterTemplateAddEditDTO
    {
        public string Context { get; set; } 
        public string DisplayName { get; set; }
        public string FileName { get; set; }
    }
}