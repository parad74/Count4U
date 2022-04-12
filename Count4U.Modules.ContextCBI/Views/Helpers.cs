using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model;
using Count4U.Model.Audit;

namespace Count4U.Modules.ContextCBI.Views
{
	public class Helpers
	{		
		public static List<string> AddressParseIntoMultiple(string address)
		{
			if (String.IsNullOrEmpty(address) == true)
				return new List<string>();
			return address.Split(Environment.NewLine.ToCharArray()).
				Where(r => !String.IsNullOrEmpty(r)).ToList();
		}

		public static string AddressParseIntoSingle(List<string> address, int maxLength)
		{
			string address1 = String.Empty;
			string address2 = String.Empty;

			if (address.Count > 0)
				address1 = address[0];
			if (address.Count > 1)
				address2 = address[1];

			string res = String.Format("{0}{1}{2}", address1, Environment.NewLine, address2);
			if (res.Length > maxLength)
				res = res.Substring(0, maxLength);

			return res;
		}
		
	}
}