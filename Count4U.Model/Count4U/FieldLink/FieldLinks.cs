using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Count4U.Model.Count4U
{
    /// <summary>
	/// Collection class for Reports entitites.
    /// </summary>
	public class FieldLinks : ObservableCollection<FieldLink>
    {
		public static FieldLinks FromEnumerable(IEnumerable<FieldLink> list)
		{
			var collection = new FieldLinks();
			return Fill(collection, list);
		}

		public static FieldLinks Fill(FieldLinks collection, IEnumerable<FieldLink> list)
		{
			foreach (FieldLink item in list)
			{
				if (FieldLinks.Containts(collection, item) == false)
				{
					collection.Add(item);
				}
			}
			return collection;
		}

		public static bool Containts(FieldLinks collection, FieldLink fieldLink)
		{
			if (collection == null) return false;
			if (fieldLink == null) return false;
			var ch = @" \".ToCharArray();
			string domainType = fieldLink.DomainType.ToLower().Trim(ch);
			if (string.IsNullOrWhiteSpace(domainType) == true) return false;
			string propertyNameInDomainType = fieldLink.PropertyNameInDomainType.ToLower().Trim(ch);
			if (string.IsNullOrWhiteSpace(propertyNameInDomainType) == true) return false;

			foreach (FieldLink item in collection)
			{
				if ((item.DomainType.ToLower().Trim(ch)) == domainType &&
					(item.PropertyNameInDomainType.ToLower().Trim(ch)) == propertyNameInDomainType) 
				{
					return true;
				}
			}
			return false;
		}

		public FieldLink CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }

	}
}

