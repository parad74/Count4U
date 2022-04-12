using System.Collections.Generic;

namespace Count4U.Common.Services.Ini
{
    public class IniFileData
    {
        public IniFileData()
        {
            Data = new Dictionary<string, string>();
        }

        public string SectionName { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}