using NServiceBus.MessageInterfaces;

namespace GenericEndpoint
{
	public class MyMessageMapper:IMessageMapper{


		public IMessageMapper Decorated { get; set; }

		public System.Type GetMappedTypeFor(string typeName)
		{

			if (typeName.Contains("IBusinessApplicationAccepted"))
			{

				var type = System.Type.GetType("Wonga.QA.Framework.Msmq.Messages.Risk.IApplicationDecisionEvent");
				type = typeof(Wonga.QA.Framework.Msmq.Messages.Risk.Business.IBusinessApplicationAcceptedEvent);
				return type;
			}
				
			if (typeName.Contains("IRiskEvent"))
			{

				var type = System.Type.GetType("Wonga.QA.Framework.Msmq.Messages.Risk.IRiskEvent");
				type = typeof (Wonga.QA.Framework.Msmq.Messages.Risk.IRiskEvent);
				return type;
			}
				

			return Decorated.GetMappedTypeFor(typeName);
			//return System.Type.GetType(typeName);
		}

		public System.Type GetMappedTypeFor(System.Type t)
		{
			return Decorated.GetMappedTypeFor(t);
		}

		public void Initialize(System.Collections.Generic.IEnumerable<System.Type> types)
		{
			//Decorated.Initialize(types);
		}

		public object CreateInstance(System.Type messageType)
		{
			return Decorated.CreateInstance(messageType);
		}

		public T CreateInstance<T>(System.Action<T> action) where T : NServiceBus.IMessage
		{
			return Decorated.CreateInstance<T>(action);
		}

		public T CreateInstance<T>() where T : NServiceBus.IMessage
		{
			return Decorated.CreateInstance<T>();
		}
	}
}