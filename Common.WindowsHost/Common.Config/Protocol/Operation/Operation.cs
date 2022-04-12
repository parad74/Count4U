using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IdSip2.Common.Protocol
{

	public class Operation
	{
		//<operation code="99" name="SCStatusRequest" type="request" back="98">
		public string code { get; set; }
		public string name { get; set; }
		public string type { get; set; }
		public string back { get; set; }
		public Dictionary<string, Field> fieldDictionary { get; set; }
		public Fields fields { get; set; }
		public XElement xElement { get; set; }

		public Operation()
		{
			this.code = "";
			this.name = "";
			this.type = "";
			this.back = "";
			this.fields = new Fields();
			this.fieldDictionary = new Dictionary<string, Field>();
			this.xElement = new XElement("operation");
		}

		public Operation(XElement operationXElement)
		{
			this.xElement = operationXElement;
			this.fieldDictionary = new Dictionary<string, Field>();
			this.fields = new Fields();

			if (operationXElement != null)
			{
				this.code = (string)operationXElement.Attribute("code") ?? "";
				this.name = (string)operationXElement.Attribute("name") ?? "";
				this.type = (string)operationXElement.Attribute("type") ?? "";
				this.back = (string)operationXElement.Attribute("back") ?? "";
				IEnumerable<XElement> fields = operationXElement.Elements("field");
				foreach (XElement fieldXElement in fields)
				{
					Field field = new Field(fieldXElement);
					this.fieldDictionary[field.name] = field;
					this.fields.Add(field);
				}
			}
			else
			{
				this.code = "";
				this.name = "";
				this.type = "";
				this.back = "";
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Operation)) return false;
			return Equals((Operation)obj);
		}

		public bool Equals(Operation other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.code, this.code);
		}

		public override int GetHashCode()
		{
			return (this.code != null ? this.code.GetHashCode() : 0);
		}

	

	}
}

