using Count4U.Configuration.Dynamic;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{
    public class DynamicColumnSettingsItem : NotificationObject
    {
        private string _title;
        private readonly EditorTemplate _editorTemplate;

        public DynamicColumnSettingsItem(EditorTemplate editorTemplate)
        {
            _editorTemplate = editorTemplate;
            _title = editorTemplate.Title;
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public EditorTemplate EditorTemplate
        {
            get { return _editorTemplate; }
        }
    }
}