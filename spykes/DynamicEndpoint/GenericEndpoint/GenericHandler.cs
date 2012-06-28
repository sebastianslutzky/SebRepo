using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace GenericEndpoint
{
	public class GenericHandler:IHandleMessages<IMessage>
	{
		private static List<OnTheFlyHandler> _handlers = new List<OnTheFlyHandler>();
		public static void Add(OnTheFlyHandler handler)
		{
			_handlers.Add(handler);
		}

		public IBus Bus { get; set; }

		public void Handle(IMessage message)
		{
			var relevantHandlers = _handlers.Where(x => x.IsFor(message)).ToList();
			Console.WriteLine("#{0} Handlers found for this {1}", relevantHandlers.Count, message.GetType().Name);

			relevantHandlers.ForEach(x=>x.Handle(message,Bus));

			

			_handlers.RemoveAll(x=>x.IsFor(message));
			
		}
	}

	
}
