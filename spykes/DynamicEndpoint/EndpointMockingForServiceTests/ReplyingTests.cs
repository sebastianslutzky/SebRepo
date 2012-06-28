using System;
using GenericEndpoint;
using NUnit.Framework;
using Wonga.QA.Framework.Core;
using Wonga.Risk.Business;

namespace EndpointMockingForServiceTests
{
	[TestFixture]
	public class ReplyingTests
	{
		private EndPoint _salesforce;
		[TestFixtureSetUp]
		public void Setup()
		{
			_salesforce = new EndPoint("salesforce");
			_salesforce.Start();
		}

		[Test]
		public void SendMsgToRiskFromSalesforce()
		{
			var appId = new Guid("cdaf96f4-7d57-4e91-ab7d-d1dbd6908e2c");

			var messageWasSentToRisk = false;

			_salesforce.AddHandler<IBusinessApplicationAccepted>(
				filter: x => x.ApplicationId == appId,
				action: (x, bus) =>
				        	{
				        		messageWasSentToRisk = true;
								// here is where we mock the external service
								// in this case we send a message to Risk  but we can alsoe
								//	* Bus.Publish (which will also test that our target service subscribes to an vent
								//  * Bus.ReplyToOriginator() (twe could build this method, applicable for sagamessages
				        		bus.Send<IBusinessApplicationDeclined>("riskservice",msg => msg.ApplicationId = appId);
				        	});

			Do.Until(() => messageWasSentToRisk);
		}

	}
}
