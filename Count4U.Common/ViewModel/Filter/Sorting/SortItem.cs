using System.Reflection;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.Filter.Sorting
{
    public class SortItem : NotificationObject
    {
        private string _name;
        private readonly PropertyInfo _propertyInfo;

        public SortItem(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
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

        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }
    }
}