using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EndpointMockingForServiceTests;
using NServiceBus.MessageInterfaces;

namespace GenericEndpoint
{
	public class MyMessageMapper:IMessageMapper
    {
		public IMessageMapper Decorated { get; set; }

		public System.Type GetMappedTypeFor(string typeName)
		{
		    string pathToMsmqMessages = AssemblyDirectory + "\\Wonga.QA.Framework.Msmq.dll";

            List<Type> messagesTypes = ReflectionHelper.GetIMessageTypes(pathToMsmqMessages);

		    foreach (var messageTyp in messagesTypes)
		    {
		        if (typeName.Contains(messageTyp.Name))
		        {
                    return messageTyp;
		        }
		    }

			return Decorated.GetMappedTypeFor(typeName);
		}

        static private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
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