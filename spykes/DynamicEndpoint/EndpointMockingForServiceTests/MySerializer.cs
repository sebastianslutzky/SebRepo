using NServiceBus.Serialization;
using NServiceBus.Serializers.XML;

namespace GenericEndpoint
{
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
