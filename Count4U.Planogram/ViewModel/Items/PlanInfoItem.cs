using System;
using Count4U.Model.Count4U;

namespace Count4U.Planogram.ViewModel
{
    [Serializable]
    public class PlanInfoItem
    {
        public Itur Itur { get; set; }
        public IturAnalyzesSimple IturAnalyze { get; set; }
    }
}