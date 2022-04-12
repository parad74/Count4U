using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Main;
using Count4U.Model.Audit;

namespace Count4U.Model.Interface
{
	public interface IProfileRepository 
	{
		string RemoteCreateProfileObject(object domainObject, string URL, Encoding outEncoding);
		string RemoteCreateProfileCustomer(Customer customer, string URL, Encoding outEncoding);
		string RemoteCreateProfileInventor(Inventor inventor, string URL, Encoding outEncoding) ;
	}
}
