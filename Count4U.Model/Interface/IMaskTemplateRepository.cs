using System;
using System.Collections.Generic;
using Count4U.Model;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface
{
    
    public interface IMaskTemplateRepository
    {
		Dictionary<MaskTemplateEnum, MaskTemplate> MaskTemplateDictionary { get; }
		MaskRecord ToMaskRecord(string inputString);
	}
}
