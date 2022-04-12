using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels.Family
{
    public class FamilyItemViewModel : NotificationObject
    {
         private string _code;
        private string _name;
		private string _type;
		private string _size;
		private string _extra1;
		private string _extra2;
        private string _description;

		public FamilyItemViewModel(Count4U.Model.Count4U.Family family)
        {
			Update(family);
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

		public string Type
		{
			get { return _type; }
			set
			{
				_type = value;
				RaisePropertyChanged(() => Type);
			}
		}

		public string Size
		{
			get { return _size; }
			set
			{
				_size = value;
				RaisePropertyChanged(() => Size);
			}
		}

		public string Extra1
		{
			get { return _extra1; }
			set
			{
				_extra1 = value;
				RaisePropertyChanged(() => Extra1);
			}
		}

		public string Extra2
		{
			get { return _extra2; }
			set
			{
				_extra2 = value;
				RaisePropertyChanged(() => Extra2);
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

		public void Update(Count4U.Model.Count4U.Family family)
        {
			Code = family.FamilyCode;
			Name = family.Name;
			Description = family.Description;
			Type = family.Type;
			Size = family.Size;
			Extra1 = family.Extra1;
			Extra2 = family.Extra2;
        } 
    }
}