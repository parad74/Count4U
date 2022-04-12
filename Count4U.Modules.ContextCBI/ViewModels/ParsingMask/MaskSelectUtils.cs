using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Main;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask
{
    public static class MaskSelectUtils
    {
        public static string FormatInputString(string inputString, string mask)
        {
            string outputString = inputString;
            try
            {
                MaskRecord maskRecord = MaskTemplateRepository.ToMaskRecord(mask);
                if (maskRecord != null)
                {
                    MaskTemplate maskTemplate = MaskTemplateRepository.GetMaskTemplateDictionary()[maskRecord.MaskTemplateType];
                    if (maskTemplate != null)
                        outputString = maskTemplate.FormatString(inputString, maskRecord.Value);
                }
            }
            catch { }
            return outputString;
        } 
    }
}