using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Common.WCF
{
	public class FactoryWrapper<TChannel> where TChannel : class
	{
		private static ChannelFactory<TChannel> _factory;
		

		// или любой другой набор параметров, который  нужен
		// для корректного создания фабрики...
		//
		public FactoryWrapper(Binding binding, EndpointAddress endpointAddress)
		{
			_factory = new ChannelFactory<TChannel>(binding, endpointAddress);
		}



		public FactoryWrapper(string abcName)
		{
			_factory = new ChannelFactory<TChannel>(abcName);
			//ChannelFactory<ISimpleMessage> IISSimpleMessageFactory = new ChannelFactory<ISimpleMessage>("IISSimpleMessageService");
			//ISimpleMessage proxy = IISSimpleMessageFactory.CreateChannel();
		}

		public FactoryWrapper(string abcName, IEndpointBehavior endpointBehavior)
		{
			_factory = new ChannelFactory<TChannel>(abcName);
			if (endpointBehavior != null) _factory.Endpoint.Behaviors.Add(endpointBehavior);
		}

				
		public TResult Execute<TResult>(Func<TChannel, TResult> action)
		{
			var proxy = default(TChannel);
			TResult result;
			try
			{
				proxy = _factory.CreateChannel();
				((IClientChannel)proxy).Open();
				result = action(proxy);
				((IClientChannel)proxy).Close();
			}
			catch (Exception exp)
			{
				if (proxy != null)
					((IClientChannel)proxy).Abort();
				throw;
			}
			return result;
		}
	}

}
