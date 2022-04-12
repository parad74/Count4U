using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
    public class InputTypes : ObservableCollection<InputType>
    {

        public static InputTypes FromEnumerable(System.Collections.Generic.IEnumerable<InputType> List)
        {
            InputTypes inputTypes = new InputTypes();
            foreach (InputType item in List)
            {
                inputTypes.Add(item);
            }
            return inputTypes;
        }

        public InputType CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
