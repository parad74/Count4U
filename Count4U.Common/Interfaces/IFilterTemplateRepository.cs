using System;
using System.Collections.Generic;
using System.IO;

namespace Count4U.Common.Interfaces
{
    public interface IFilterTemplateRepository
    {
        List<FileInfo> GetFiles(string context);
        object GetData(FileInfo file, Type type);
        FileInfo Add(string name, object content, string context);
        void Update(FileInfo file, object content);
        FileInfo Rename(FileInfo oldFi, string name, object content, string context);
        void Delete(FileInfo file);
    }
}