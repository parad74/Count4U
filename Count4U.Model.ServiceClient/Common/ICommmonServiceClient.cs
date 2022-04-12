using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.ServiceContract;

namespace Count4U.Model.ServiceClient.Common
{
	public interface ICommmonServiceClient
	{
		IRequest2ResponseProxy GetSelectedClientProxy();
	}
}
