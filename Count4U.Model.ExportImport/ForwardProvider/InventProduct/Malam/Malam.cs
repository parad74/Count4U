using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

//		<Rerords>
//<Rerord>
//<text>B|20160002|999|100001-1|1|K|23/04/2017|12:05:24</text>
//<ballotbox>100001</ballotbox>
//<itemtype>1</itemtype>
//<pdanum>010</pdanum>
//<location>999</location>
//<datetime>23/04/2017 12:05:24</datetime>
//</Rerord>
//			</Rerords>

namespace Count4U.Model.Malam
{
	[Serializable]
	public class Record
	{
		public string text { get; set; }
		public string ballotbox { get; set; }
		public string itemtype { get; set; }
		public string pdanum { get; set; }
		public string location { get; set; }
		public string insertdate { get; set; }

		public Record()
		{ }

		public Record(string _text,
		 string _ballotbox,
		 string _itemtype,
		 string _pdanum,
		 string _location,
		 string _insertdate)
		{
			text = _text;
			ballotbox = _ballotbox;
			itemtype = _itemtype;
			pdanum = _pdanum;
			location = _location;
			insertdate = _insertdate;
		}
	}

	[Serializable]
	[XmlRoot("Records")]
	public class Records : ObservableCollection<Record>   
	{
		public Records() { }
	}
}

