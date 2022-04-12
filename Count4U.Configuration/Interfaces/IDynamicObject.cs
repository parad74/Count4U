using System.Collections.ObjectModel;
using Count4U.Configuration.Dynamic;

namespace Count4U.Configuration.Interfaces
{
    public interface IDynamicObject
    {
        ObservableCollection<DynamicProperty> DynamicList { get;  } 
    }
}