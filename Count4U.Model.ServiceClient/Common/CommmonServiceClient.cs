using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Count4U.Model.ServiceContract;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.ServiceClient.Common
{
	public class CommmonServiceClient : ICommmonServiceClient
	{
		public readonly IServiceLocator _serviceLocator;
		public CommmonServiceClient(IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
		}

		public IRequest2ResponseProxy GetSelectedClientProxy()
		{
			IClientPresenterInfo info = ServiceProxyClientModuleInit.ClientPresenterInfoCurrent;
			IRequest2ResponseProxy _bibliothecaProxyClient = this._serviceLocator.GetInstance<IRequest2ResponseProxy>(info.Name);
			return _bibliothecaProxyClient;
		}
	}
}
