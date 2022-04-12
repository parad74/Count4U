using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdSip2.Common.Protocol
{
	public class Operations : ObservableCollection<Operation>
    {
		public static Operations FromEnumerable(IEnumerable<Operation> list)
        {
			var collection = new Operations();
            return Fill(collection, list);
        }

		private static Operations Fill(Operations collection, IEnumerable<Operation> list)
        {
			foreach (Operation item in list)
                collection.Add(item);
            return collection;
        }
      
	}
}
