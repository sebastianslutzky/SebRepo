using System;
using System.Linq;
using System.Transactions;
using Common.Logging;
using Autofac;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.MessageInterfaces;
using NServiceBus.ObjectBuilder;
using NServiceBus.Serialization;
using NServiceBus.Serializers.XML;
using NServiceBus.Unicast.Transport;


namespace GenericEndpoint
{
	public class EndpointConfigurator
	{
		//private readonly ILog _log =  LogManager.GetCurrentClassLogger();

		public void InitialiseEndpoint()
		{
			//OpsBusConfiguration();
			MyBusConfiguration();
		}


		private static void MyBusConfiguration()
		{
			var builder = new ContainerBuilder();


			builder.RegisterType<MyConfig>().As<IConfigurationSource>().PropertiesAutowired().
				InstancePerLifetimeScope();
			var container = builder.Build();

			//builder.RegisterType<MyMessageMapper>().As<IMessageMapper>().PropertiesAutowired().InstancePerLifetimeScope();
			//var anotherBuilder = new ContainerBuilder();
			////anotherBuilder.RegisterModule<MySerializerModule>();
			//anotherBuilder.Update(container);

			var busConfigSource = container.Resolve<IConfigurationSource>() as MyConfig;
			

			var bus = Configure.With(
				typeof(IMessage).Assembly,
				typeof(CompletionMessage).Assembly,
				typeof(OnTheFlyHandler).Assembly,
				typeof(Wonga.QA.Framework.Msmq.Messages.Risk.IRiskEvent).Assembly
				);

			bus.Autofac2Builder(container).XmlSerializer();
			//ConfigureSerializer(bus);
			bus.MsmqTransport()
				.IsTransactional(true)
				.IsolationLevel(IsolationLevel.RepeatableRead)
				.PurgeOnStartup(false)
				.UnicastBus()
				.ImpersonateSender(true)
				.LoadMessageHandlers()
				.DoNotAutoSubscribe().CreateBus().Start();

			var g = container.Resolve<IMessageSerializer>();
			var origMapper = ((MessageSerializer) g).MessageMapper;
			((MessageSerializer) g).MessageMapper = new MyMessageMapper() {Decorated = origMapper};
			
		}

		private string EndpointName { get; set; }

		public EndpointConfigurator(string endpointName)
		{
			EndpointName = endpointName;
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

	public class MyConfig:IConfigurationSource
	{
		public T GetConfiguration<T>() where T : class
		{
			if (typeof(T) == typeof(MsmqTransportConfig))
				return MsmqConfig() as T;

			if (typeof(T) == typeof(UnicastBusConfig))
				return UnicastBusConfiguration() as T;

			
				throw new NotImplementedException(typeof(T).ToString());
		}

		public string Endpoint { get; set; }

		private MsmqTransportConfig MsmqConfig()
		{
			return new MsmqTransportConfig

			{
				ErrorQueue = Endpoint + "_error",
				InputQueue = Endpoint,
				MaxRetries = 5,
				NumberOfWorkerThreads = 1

			};
		}

		public UnicastBusConfig UnicastBusConfiguration()
		{
			return new UnicastBusConfig();

		}
	}
}
