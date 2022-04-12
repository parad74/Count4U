using System;
using System.Collections.Generic;
using Count4U.Configuration.Dynamic;

namespace Count4U.Configuration.Interfaces
{
    public interface IEditorTemplateRepository
    {        
        List<EditorTemplate> Get(string viewName);
        EditorTemplate GetCurrent(string viewName, string dbPath);
        void SetCurrent(string viewName, string dbPath, EditorTemplate editorTemplate);
    }
}