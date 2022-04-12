using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels.Section
{
    public class SectionItemViewModel : NotificationObject
    {
        private string _code;
        private string _name;
        private string _description;
		private string _tag;
		private Count4U.Model.Count4U.Section _section;

        public SectionItemViewModel(Count4U.Model.Count4U.Section section)
        {
            Update(section);
        }

		public Count4U.Model.Count4U.Section Section
		{
			get { return this._section; }
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

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

		public string Tag
		{
			get { return _tag; }
			set
			{
				_tag = value;
				RaisePropertyChanged(() => Tag);
			}
		}

		public string TypeCode
		{
			get { return this._section.TypeCode; }
		}

		public string ParentSectionCode
		{
			get { return this._section.ParentSectionCode; }
		}

        public void Update(Count4U.Model.Count4U.Section section)
        {
			this._section = section;
            Code = section.SectionCode;
            Name = section.Name;
            Description = section.Description;
			Tag = section.Tag; 
        }

    }
}