using System;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync;
using TechTalk.SpecFlow;
using TestUtilities;
using TinyIoC;

namespace Specifications
{
	[Binding]
	public class SynchronizationSteps
	{
		private readonly DateTime _time1 = DateTime.Now.Subtract(TimeSpan.FromDays(1));
		private readonly DateTime _time2 = DateTime.Now;

		protected TestItem TestItemR0
		{
			get
			{
				if (!S.C.ContainsKey("TestItemR0"))
				{
					S.C["TestItemR0"] = new TestItem {FieldA = Guid.NewGuid().ToString()};
				}
				return (TestItem) S.C["TestItemR0"];
			}
			private set { S.C["TestItemR0"] = value; }
		}
										  
		protected TestItem TestItemR1
		{
			get { return (TestItem) S.C["TestItemR1"]; }
			private set { S.C["TestItemR1"] = value; }
		}

		protected string FieldAClientChangedValue
		{
			get { return (string)S.C["FieldAClientChangedValue"]; }
			private set { S.C["FieldAClientChangedValue"] = value; }
		}

		protected string FieldBClientChangeValue
		{
			get { return (string)S.C["FieldBClientChangeValue"]; }
			private set { S.C["FieldBClientChangeValue"] = value; }
		}

		protected string FieldAServerChangedValue
		{
			get { return (string) S.C["FieldAServerChangedValue"]; }
			private set { S.C["FieldAServerChangedValue"] = value; }
		}

		protected string FieldBServerChangedValue
		{
			get { return (string) S.C["FieldBServerChangedValue"]; }
			private set { S.C["FieldBServerChangedValue"] = value; }
		}

		protected TinyIoCContainer Container
		{
			get
			{
				if (!S.C.ContainsKey("Container"))
				{
					S.C["Container"] = new TinyIoCContainer();
				}
				return (TinyIoCContainer)S.C["Container"];
			}
		}

		protected TinyIoCContainer ClientContainer
		{
			get
			{
				if (!S.C.ContainsKey("ClientContainer"))
				{
					S.C["ClientContainer"] = Container.GetChildContainer();
				}
				return (TinyIoCContainer) S.C["ClientContainer"];
			}
		}

		protected TinyIoCContainer ServerContainer
		{
			get
			{
				if (!S.C.ContainsKey("ServerContainer"))
				{
					S.C["ServerContainer"] = Container.GetChildContainer();
				}
				return (TinyIoCContainer) S.C["ServerContainer"];
			}
		}

		protected SendingSynchronizer<TestItem, Guid> ClientSendingSynchronizer
		{
			get
			{
				if (!S.C.ContainsKey("ClientSynchronizor"))
				{
					S.C["ClientSynchronizor"] = new SendingSynchronizer<TestItem, Guid>(ClientContainer);
				}
				return (SendingSynchronizer<TestItem, Guid>) S.C["ClientSynchronizor"];
			}
		}

		protected ReceivingSynchronizer<TestItem, Guid> ServerSendingSynchronizer
		{
			get
			{
				if (!S.C.ContainsKey("ServerSynchronizor"))
				{
					S.C["ServerSynchronizor"] = new ReceivingSynchronizer<TestItem, Guid>(ServerContainer);
				}
				return (ReceivingSynchronizer<TestItem, Guid>)S.C["ServerSynchronizor"];
			}
		}

		protected TestRepository ClientRepository
		{
			get
			{
				if (!S.C.ContainsKey("ClientRepository"))
				{
					S.C["ClientRepository"] = new TestRepository();
				}
				return (TestRepository)S.C["ClientRepository"];
			}
		}

		protected TestRepository ServerRepository
		{
			get
			{
				if (!S.C.ContainsKey("ServerRepository"))
				{
					S.C["ServerRepository"] = new TestRepository();
				}
				return (TestRepository)S.C["ServerRepository"];
			}
		}

		protected Replica ServerReplica
		{
			get
			{
				if (!S.C.ContainsKey("ServerReplica"))
				{
					S.C["ServerReplica"] = new Replica("Server");
				}
				return (Replica)S.C["ServerReplica"];
			}
		}


		[BeforeScenario]
		private void SetupSync()
		{
			
			// Set up client
			ILog clientLogger = LogManager.GetLogger("Client");
			log4net.Config.XmlConfigurator.Configure(clientLogger.Logger.Repository);
			ClientContainer.Register<ILog>(clientLogger);
			ClientContainer.Register<IRepository<TestItem, Guid>>(ClientRepository);
			ClientContainer.Register<ISyncResult<TestItem, Guid>, SyncResult<TestItem, Guid>>();
			ClientContainer.Register<IReplica>(new Replica("Client"));

			// Set up server
			ILog serverLogger = LogManager.GetLogger("Server");
			log4net.Config.XmlConfigurator.Configure(serverLogger.Logger.Repository);
			ServerContainer.Register<ILog>(clientLogger);
			ServerContainer.Register<IRepository<TestItem, Guid>>(ServerRepository);
			ServerContainer.Register<ISyncResult<TestItem, Guid>, SyncResult<TestItem, Guid>>();
			ServerContainer.Register<IReplica>(ServerReplica);	
			ServerRepository.Insert(TestItemR0); // Start with an item on the server

			// Used for both client and server
			Container.Register<ISyncConflictResolver<TestItem, Guid>, TestItemConflictResolver>(); 

			// Just pass the sync requests directly between the server and client
			// Ordinarily these would by serialized and sent over a wire, but the 
			// TestSyncRequestSender, just passes the object directly.
			ClientContainer.Register<ISyncRequestSender<TestItem, Guid>>(new TestSyncRequestSender<TestItem, Guid>(ServerSendingSynchronizer));

			ClientSendingSynchronizer.BeginSync(ServerReplica);
		}
			   
		[Given(@"a client app adds a new record R1")]
		public void GivenAClientAppAddsANewRecordR1()
		{
			var newRecord = new TestItem();
			ClientRepository.Insert(newRecord);
			TestItemR1 = newRecord;
		}

		[Given(@"the record R1 does not exist on the server")]
		public void GivenTheRecordR1DoesNotExistOnTheServer()
		{
			if (ServerRepository.Find(TestItemR1.Id) != null)
			{
				ServerRepository.Delete(TestItemR1.Id);
			}
		}

		[Given(@"a client app deletes a record R0")]
		public void GivenAClientAppDeletesARecordR0()
		{
			ClientRepository.Delete(TestItemR0.Id);
		}

		[Given(@"the R0 record exists on the server")]
		public void GivenTheR0RecordExistsOnTheServer()
		{
			if (ServerRepository.Find(TestItemR0.Id) == null)
			{
				ServerRepository.Insert(TestItemR0);
			}
		}

		[Given(@"the R0 record has been deleted from the server")]
		public void GivenTheR0RecordHasBeenDeletedFromTheServer()
		{
			ServerRepository.Delete(TestItemR0.Id);
		}

		[Given(@"a client has a change record R1 in field A")]
		public void GivenAClientHasAChangeRecordR1InFieldA()
		{
			TestItemR1 = new TestItem();
			ClientRepository.Insert(TestItemR1);
			var r1 = ClientRepository.Find(TestItemR1.Id);
			r1.FieldA = FieldAClientChangedValue = Guid.NewGuid().ToString();
			ClientRepository.Update(r1);
		}

		[Given(@"a client has a change record R0 in field A")]
		public void GivenAClientHasAChangeRecordR0InFieldA()
		{
			var r0 = ClientRepository.Find(TestItemR0.Id);
			r0.FieldA = FieldAClientChangedValue = Guid.NewGuid().ToString();
			ClientRepository.Update(r0);
		}

		[Given(@"field A of the record R0 has been changed on the server")]
		public void GivenFieldAOfTheRecordR0HasBeenChangedOnTheServer()
		{
			var r0 = ServerRepository.Find(TestItemR0.Id);
			r0.FieldA = FieldAServerChangedValue = Guid.NewGuid().ToString();
			ServerRepository.Update(r0);
		}

		[Given(@"field B of the record R0 has been changed on the server")]
		public void GivenFieldBOfTheRecordR0HasBeenChangedOnTheServer()
		{
			var r0 = ServerRepository.Find(TestItemR0.Id);
			r0.FieldB = FieldBServerChangedValue = Guid.NewGuid().ToString();
			ServerRepository.Update(r0);
		}



		[Given(@"the timestamps do not exist on the client")]
		public void GivenTheTimestampsDoNotExistOnTheClient()
		{
			// Tests default this way
		}

		[When(@"the client app submits the record for synchronization")]
		public void WhenTheClientAppSubmitsTheRecordForSynchronization()
		{
			ClientSendingSynchronizer.BeginSync(ServerReplica);
		}

		[Then(@"the record R1 should be added to the server")]
		public void ThenTheRecordR1ShouldBeAddedToTheServer()
		{
			var result = ServerRepository.Find(TestItemR1.Id);
			Assert.IsNotNull(result);
		}

		[Then(@"the R0 record should be removed from the server")]
		public void ThenTheR0RecordShouldBeRemovedFromTheServer()
		{
			var result = ServerRepository.Find(TestItemR0.Id);
			Assert.IsNull(result);
		}

		[Then(@"nothing should happen to record R0 on the server")]
		public void ThenNothingShouldHappenToRecordR0TheServer()
		{
			var r0LogRecordFound = ServerLogContains("R0");
			Assert.IsFalse(r0LogRecordFound);
		}

		private bool ServerLogContains(string messageContains)
		{
			return SyncLogContains(messageContains, ServerContainer);
		}

		private bool ClientLogContains(string messageContains)
		{
			return SyncLogContains(messageContains, ClientContainer);
		}

		private bool SyncLogContains(string messageContains, TinyIoCContainer container)
		{
			ILog log = container.Resolve<ILog>();
			var appender = (MemoryAppender) log.Logger.Repository.GetAppenders().FirstOrDefault(a => a.Name == "MemoryAppender");
			if (appender == null) Assert.Fail("Memory appender for log4net not found.");
			var events = appender.GetEvents();
			bool r0LogRecordFound = events.Any(loggingEvent => loggingEvent.RenderedMessage.Contains(messageContains));
			return r0LogRecordFound;
		}

		[Then(@"the R0 record should be deleted from the client")]
		public void ThenTheR0RecordShouldBeDeletedFromTheClient()
		{
			var item = ClientRepository.Find(TestItemR0.Id);
			Assert.IsTrue(item.EndTime <= DateTime.Now);
		}

		[Then(@"the R0 record field A should be updated on the server")]
		public void ThenTheR0RecordFieldAShouldBeUpdatedOnTheServer()
		{
			var r0 = ServerRepository.Find(TestItemR0.Id);
			Assert.AreEqual(FieldAClientChangedValue, r0.FieldA);
		}

		[Then(@"the server R0 record should have client value in field A and server value in field B")]
		public void ThenTheServerR0RecordShouldHaveClientValueInFieldAAndServerValueInFieldB()
		{
			var r0 = ServerRepository.Find(TestItemR0.Id);
			Assert.AreEqual(FieldAClientChangedValue, r0.FieldA);
			Assert.AreEqual(FieldBServerChangedValue, r0.FieldB);
		}
		


		[Then(@"the fieldA conflict should be saved in the server record R0")]
		public void ThenTheFieldAConflictShouldBeSavedInTheServerRecordR0()
		{
			var r0 = ServerRepository.Find(TestItemR0.Id);
			Assert.AreEqual(1, r0.Conflicts.Count());
			foreach (Conflict<Guid>.ConflictItem conflict in r0.Conflicts.First().ConflictItems)
			{
				string fieldA = ((TestItem) conflict.Item).FieldA;
				if (conflict.Replica.Name == "Server")
				{
					Assert.AreEqual(FieldAServerChangedValue, fieldA);
				}
				else
				{
					Assert.AreEqual(FieldAClientChangedValue, fieldA);
				}
				
			}

		}
	}
}
