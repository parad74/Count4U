using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IdSip2.Common.Protocol
{
	//<field name="status_code" id="" datacontract="1" description="" sent="Y"
	//  datasample="1"  datadefault="1">
		public class Field
	{
		public string name { get; set; }
		public string id { get; set; }
		public string datacontract { get; set; }
		public string description { get; set; }
		public string sent { get; set; }
		public string datasample { get; set; }
		public string datadefault { get; set; }
		public XElement xElement { get; set; }
		public XElement xElementFormat { get; set; }

		public Field()
		{
			this.name = "";
			this.id = "";
			this.datacontract = "";
			this.description = "";
			this.sent = "";
			this.datasample = "";
			this.datadefault = "";

			this.xElement = new XElement("field");
			this.xElementFormat = new XElement("format");
		}

		public Field(XElement fieldXElement)
		{
			this.xElement = fieldXElement;
			this.name = (string)fieldXElement.Attribute("name") ?? "";
			this.id = (string)fieldXElement.Attribute("id") ?? "";
			this.datacontract = (string)fieldXElement.Attribute("datacontract") ?? "";
			this.description = (string)fieldXElement.Attribute("description") ?? "";
			this.sent = (string)fieldXElement.Attribute("sent") ?? "";
			this.datasample = (string)fieldXElement.Attribute("datasample") ?? "";
			this.datadefault = (string)fieldXElement.Attribute("datadefault") ?? ""; 
			
			 //<format type="fixed" length="1" startindex="1"  required="0|1|2">
			 //<format type="variablelength_startindex" startindex="9"  required="">
			// <format type="variablelength" start="AM" required=""> 
			XElement xElementFormat = fieldXElement.Descendants("format").FirstOrDefault();
			this.xElementFormat = xElementFormat;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Field)) return false;
			return Equals((Field)obj);
		}

		public bool Equals(Field other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.name, this.name);
		}

		public override int GetHashCode()
		{
			return (this.name != null ? this.name.GetHashCode() : 0);
		}


	}
}

