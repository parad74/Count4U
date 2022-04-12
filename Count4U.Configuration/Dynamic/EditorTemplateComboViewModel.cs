using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Configuration.Interfaces;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Configuration.Dynamic
{
    public class EditorTemplateComboViewModel : NotificationObject
    {
        private readonly DynamicRepository _dynamicRepository;
        private readonly IEditorTemplateRepository _editorTemplateRepository;

        private readonly ObservableCollection<EditorTemplate> _editorTemplateList;
        private EditorTemplate _editorTemplateCurrent;

        public EditorTemplateComboViewModel(
            DynamicRepository dynamicRepository,
            IEditorTemplateRepository editorTemplateRepository)
        {
            _editorTemplateRepository = editorTemplateRepository;
            _dynamicRepository = dynamicRepository;
            _editorTemplateList = new ObservableCollection<EditorTemplate>();
        }

        public DataGrid DataGrid { get; set; }
        public CBIState State { get; set; }
        public Action RebuildAction { get; set; }
        public string ViewName { get; set; }

        public ObservableCollection<EditorTemplate> EditorTemplateList
        {
            get { return _editorTemplateList; }
        }

        public EditorTemplate EditorTemplateCurrent
        {
            get { return _editorTemplateCurrent; }
            set
            {
                _editorTemplateCurrent = value;
                RaisePropertyChanged(() => EditorTemplateCurrent);

                using (new CursorWait())
                {
                    _editorTemplateRepository.SetCurrent(ViewName, State.GetDbPath, _editorTemplateCurrent);
                    InitGridWithColumns();
                    RebuildAction();
                }
            }
        }

        public DynamicRepository DynamicRepository
        {
            get { return _dynamicRepository; }
        }

        public void Init(string code ="")
        {
            foreach (EditorTemplate editorTemplate in _editorTemplateRepository.Get(this.ViewName))
            {
                _editorTemplateList.Add(editorTemplate);
            }

			if (string.IsNullOrWhiteSpace(code) == true)
			{
				EditorTemplate currentEditorTemplate = _editorTemplateRepository.GetCurrent(this.ViewName, State.GetDbPath);

				_editorTemplateCurrent = currentEditorTemplate == null ?
					_editorTemplateList.FirstOrDefault() :
					_editorTemplateList.FirstOrDefault(r => r.Code == currentEditorTemplate.Code);
			}
			else
			{
				_editorTemplateCurrent =  _editorTemplateList.FirstOrDefault(r => r.Code == code);
				if (_editorTemplateCurrent == null) _editorTemplateCurrent = _editorTemplateList.FirstOrDefault();
			}

            InitGridWithColumns();
        }

        private void InitGridWithColumns()
        {
            this.DynamicRepository.EditorTemplate = _editorTemplateCurrent;
            this.DynamicRepository.CBIState = this.State;
            this.DynamicRepository.InsertDynamicColumns(this.DataGrid);
        }
    }
}