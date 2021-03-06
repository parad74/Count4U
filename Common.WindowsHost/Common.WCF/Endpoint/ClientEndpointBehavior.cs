using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace Common.WCF
{
	// пример использования bibliothecaClient.Endpoint.Behaviors.Add(new ClientEndpointBehavior());
	public class ClientEndpointBehavior : IEndpointBehavior
	{
		public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{ }

		public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(
				new ClientMessageInspector()
				);
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
		{ }

		public void Validate(ServiceEndpoint endpoint)
		{ }
	}
}