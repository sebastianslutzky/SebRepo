using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NServiceBus;
using NServiceBus.Unicast.Transport;
using Wonga.Ops.Configuration.HostEndpoint;

namespace GenericEndpoint
{
	public class EndPoint
	{
		private string _name ;
		public EndPoint (string endpointName)
		{
			_name = endpointName;
		}

		public void Start()
		{


			var ep1 = new EndpointConfigurator(_name);
			ep1.InitialiseEndpoint();
			
			//IWantCustomInitialization ep2 = new EndpointConfig();
			//ep2.Init();

		}

		public void AddHandler<T>(Action<T,IBus> action) where T : IMessage
		{
			AddHandler<T>(null, action);
		}
		public void AddHandler<T>(Func<T, bool> filter, Action<T,IBus> action) where T : IMessage
		{
			GenericHandler.Add(new OnTheFlyHandler<T>(filter, action));
		}
	}
}
