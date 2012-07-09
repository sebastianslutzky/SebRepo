using System;
using NServiceBus;

namespace GenericEndpoint
{
		public class OnTheFlyHandler
		{
			public virtual bool IsFor(IMessage msg){return true;}
			public  virtual void Handle(IMessage msg,IBus bus){}
		}

		public class OnTheFlyHandler<T> : OnTheFlyHandler where T : IMessage
		{
			private Func<T, bool> _filter;
			private Action<T,IBus> _action;

			public OnTheFlyHandler(Func<T, bool> filter, Action<T,IBus> action)
			{
				_filter = filter;
				_action = action;
			}
			public OnTheFlyHandler(Action<T,IBus> action) : this(null, action) { }

			public override bool IsFor(IMessage msg)
			{
				return base.IsFor(msg) &&
					   msg is T &&
					   (_filter == null || _filter((T)msg));
			}

			public override void Handle(IMessage msg,IBus bus)
			{
				base.Handle(msg,bus);

				StronglyTypedHandle((T)msg,bus);
			}

			public void StronglyTypedHandle(T handle,IBus bus)
			{
				_action(handle,bus);
			}
		}
}