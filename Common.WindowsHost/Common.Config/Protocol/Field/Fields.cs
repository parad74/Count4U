using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdSip2.Common.Protocol
{
	public class Fields : ObservableCollection<Field>
    {
		public static Fields FromEnumerable(IEnumerable<Field> list)
        {
			var collection = new Fields();
            return Fill(collection, list);
        }

		private static Fields Fill(Fields collection, IEnumerable<Field> list)
        {
			foreach (Field item in list)
                collection.Add(item);
            return collection;
        }
      
	}
}
