
using NServiceBus.MessageInterfaces;
using NServiceBus.Serialization;
using NServiceBus.Serializers.XML;

namespace GenericEndpoint
{
	public class MyMessageMapper:IMessageMapper{

		public System.Type GetMappedTypeFor(string typeName)
		{

			return System.Type.GetType(typeName);
		}

		public System.Type GetMappedTypeFor(System.Type t)
		{
			return t;
		}

		public void Initialize(System.Collections.Generic.IEnumerable<System.Type> types)
		{
			//throw new System.NotImplementedException();
		}

		public object CreateInstance(System.Type messageType)
		{
			throw new System.NotImplementedException();
		}

		public T CreateInstance<T>(System.Action<T> action) where T : NServiceBus.IMessage
		{
			throw new System.NotImplementedException();
		}

		public T CreateInstance<T>() where T : NServiceBus.IMessage
		{
			throw new System.NotImplementedException();
		}
	}

	public class MySerializer : IMessageSerializer
	{
		public MessageSerializer DecoratedSerializer { get; set; }

		public NServiceBus.IMessage[] Deserialize(System.IO.Stream stream)
		{
			return DecoratedSerializer.Deserialize(stream);
		}

		public void Serialize(NServiceBus.IMessage[] messages, System.IO.Stream stream)
		{
			DecoratedSerializer.Serialize(messages, stream);
		}
	}
}
