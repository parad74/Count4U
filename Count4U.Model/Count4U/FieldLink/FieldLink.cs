using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public class FieldLink
	{
		public long ID { get; set; }
		public string ViewName { get; set; }
		public string EditorTemplate { get; set; }
		public string DomainType { get; set; }							//key
		public string TableName { get; set; }
		public string PropertyNameInDomainType { get; set; }	//key
		public string FieldNameInTable { get; set; }
		public int NumStringInRecord { get; set; }
		public string Editor { get; set; }
		public string Validator { get; set; }
		public string CodeLocalizationEditorLable { get; set; }
		public string DefaultEditorLable { get; set; }
		public int NN { get; set; }
		public bool InGrid { get; set; }
		public bool InEdit { get; set; }
		public bool InAdd { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(FieldLink)) return false;
			return Equals((FieldLink)obj);
		}

		public bool Equals(FieldLink other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ViewName, this.ViewName);
		}

		public override int GetHashCode()
		{
			return (ViewName != null ? ViewName.GetHashCode() : 0);
		}

	}
}
