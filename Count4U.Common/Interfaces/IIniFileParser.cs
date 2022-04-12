using System.Collections.Generic;
using Count4U.Common.Services;
using Count4U.Common.Services.Ini;

namespace Count4U.Common.Interfaces
{
    public interface IIniFileParser
    {
        IniFileData Get(string filePath, string sectionName);
        List<IniFileData> Get(string filePath);
    }
}