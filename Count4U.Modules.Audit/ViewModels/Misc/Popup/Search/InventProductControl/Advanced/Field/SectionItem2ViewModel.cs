using Microsoft.Practices.Prism.ViewModel;
using System;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl
{
    public class SectionItem2ViewModel : NotificationObject
    {
        private readonly Model.Count4U.Section _section;

        private bool _isChecked;
        private string _name;
        private string _code;

        public SectionItem2ViewModel(Count4U.Model.Count4U.Section section)
        {
            _section = section;
            _section = section;
            _code = _section.SectionCode;
        //    _name = Section.Name;
        _name = String.Format("{0}-{1}", _section.SectionCode, _section.Name);
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        public Model.Count4U.Section Section
        {
            get { return _section; }
        }
    }
}