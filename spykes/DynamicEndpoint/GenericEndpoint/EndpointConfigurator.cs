using System;
using Common.Logging;
using Autofac;
using NServiceBus;
using Wonga.Ops.Configuration;
using Wonga.Ops.Configuration.Bus;

namespace GenericEndpoint
{
	public class EndpointConfigurator: BaseEndpointConfig
	{
		private readonly ILog _log =  LogManager.GetCurrentClassLogger();

		public void InitialiseEndpoint()
		{
			IContainer container = base.Init();
			//now we autosubscribe, need it to rewrite for Sagas - because need to write Dummy Sagas handlers
			var bus = BusConfigurator.ConfigureEndpointBus(container, isAutoSubscribe: false,runTests:false);
			bus.CreateBus().Start();
		}

		
		public EndpointConfigurator(string endpointName)
		{
			EndpointName = endpointName;
			ExecutingAssemblyPath = AppDomain.CurrentDomain.BaseDirectory;// Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "");
		}

		protected override void RegisterOtherServices(Autofac.IContainer container)
		{
			//throw new NotImplementedException();
		}

		
	}
}
