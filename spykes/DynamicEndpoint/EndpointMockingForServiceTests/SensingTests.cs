using System;
using GenericEndpoint;
using NServiceBus;
using NUnit.Framework;
using Wonga.QA.Framework.Core;
using Wonga.QA.Framework.Msmq.Messages.Risk.Business;


namespace EndpointMockingForServiceTests
{
	[TestFixture]
	public class SensingTests
	{
		private EndPoint _salesforce;
		[TestFixtureSetUp]
		public void Setup()
		{
			_salesforce = new EndPoint("salesforce");
			_salesforce.Start();
		}

		[Test]
		public void AMessageIsSentToSalesforceTest()
		{
			var aMessageWasSentToSalesforce = false;
			_salesforce.AddHandler<IMessage>(action: (x,bus)=> aMessageWasSentToSalesforce = true);

			Do.Until(() => aMessageWasSentToSalesforce);
		}

		[Test]
		public void AMessageOfThisTypeIsSentToSalesforceTest()
		{
			var aMessageWasSentToSalesforce = false;
			_salesforce.AddHandler<IBusinessApplicationAcceptedEvent>(action: (x,bus) => aMessageWasSentToSalesforce = true);

			Do.Until(() => aMessageWasSentToSalesforce);
		}

		[Test]
		public void ThisMessageIsSentToSalesforceTest()
		{
			var appId = new Guid("cdaf96f4-7d57-4e91-ab7d-d1dbd6908e2c");
			
			var thisMessageWasSentToSalesforce = false;

			_salesforce.AddHandler<IBusinessApplicationAcceptedEvent>(
			    filter: x => x.ApplicationId == appId,
			    action: (x,bus) => thisMessageWasSentToSalesforce = true);
			
			Do.Until(() => thisMessageWasSentToSalesforce);
		}

	}
}
