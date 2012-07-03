using System;
using System.Linq;
using System.Transactions;
using Common.Logging;
using Autofac;
using NServiceBus;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.ObjectBuilder;
using NServiceBus.Serializers.XML;
using NServiceBus.Unicast.Transport;
using Wonga.Ops.Configuration;
using Wonga.Ops.Configuration.Bus;
using Wonga.Risk.Business;

namespace GenericEndpoint
{
	public class EndpointConfigurator: BaseEndpointConfig
	{
		private readonly ILog _log =  LogManager.GetCurrentClassLogger();

		public void InitialiseEndpoint()
		{
			//OpsBusConfiguration();
			MyBusConfiguration();
		}


		private void OpsBusConfiguration()
		{
			IContainer container = base.Init();
			//now we autosubscribe, need it to rewrite for Sagas - because need to write Dummy Sagas handlers
			var bus = BusConfigurator.ConfigureEndpointBus(container, isAutoSubscribe: false, runTests: false);
			bus.CreateBus().Start();
		}

		private static void MyBusConfiguration()
		{
			var builder = new ContainerBuilder();


			builder.RegisterType<NServiceBusConfigSource>().As<IConfigurationSource>().PropertiesAutowired().
				InstancePerLifetimeScope();
			var container = builder.Build();


			var anotherBuilder = new ContainerBuilder();
			//anotherBuilder.RegisterModule<MySerializerModule>();
			anotherBuilder.Update(container);

			var busConfigSource = container.Resolve<IConfigurationSource>() as NServiceBusConfigSource;
			
			var bus = Configure.With(
				typeof(IMessage).Assembly,
				typeof(CompletionMessage).Assembly,
				typeof(IBusinessApplicationAccepted).Assembly,
				typeof(OnTheFlyHandler).Assembly
				);
			bus.CustomConfigurationSource(busConfigSource);
			bus.Autofac2Builder(container);
			ConfigureSerializer(bus);
			bus.MsmqTransport()
				.IsTransactional(true)
				.IsolationLevel(IsolationLevel.RepeatableRead)
				.PurgeOnStartup(false)
				.UnicastBus()
				.ImpersonateSender(true)
				.LoadMessageHandlers()
				.DoNotAutoSubscribe().CreateBus().Start();


			//        .LoadMessageHandlers()
			//    .CreateBus()
			//    .Start();
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

		private static Configure ConfigureSerializer(Configure config)
		{
			var messageTypes = Configure.TypesToScan.Where(t => typeof(IMessage).IsAssignableFrom(t)).ToList();
			var decoratedMapper = new NServiceBus.MessageInterfaces.MessageMapper.Reflection.MessageMapper();

			config.Configurer.ConfigureComponent<NServiceBus.MessageInterfaces.MessageMapper.Reflection.MessageMapper>(ComponentCallModelEnum.Singleton);
			config.Configurer.ConfigureComponent<MySerializer>(ComponentCallModelEnum.Singleton)
				.ConfigureProperty(ms => ms.DecoratedSerializer, new MessageSerializer() { MessageTypes = messageTypes ,MessageMapper = new MyMessageMapper()});

			return config;
		}
		
	}
}
