using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync;
using Telerik.JustMock;
using Telerik.JustMock.Expectations.Abstraction;
using TestUtilities;
using TinyIoC;

namespace Sychronization.Unit.Tests
{
	[TestClass]
	public class SendingSynchronizerTests
	{
		private TinyIoCContainer _container;

		[TestInitialize]
		public void Setup()
		{
			_container = new TinyIoCContainer();
		}
		

		[TestMethod]
		public void SendSyncRequestToRemoteReplica_GivenValidSyncRequest_CallsSyncRequestSender()
		{
			var mockSyncRequestSender = Mocking.MockAndBind<ISyncRequestSender<TestItem, Guid>>(_container);
			var syncRequest = Mock.Create<ISyncRequest<Guid>>();

			var target = new SendingSynchronizer<TestItem, Guid>(_container);
			target.SendSyncRequestToRemoteReplica(syncRequest);

			Mock.Assert(()=>mockSyncRequestSender.SendInitialRequest(syncRequest), Occurs.Once());
		}


		[TestMethod]
		public void ResolveItemConflicts__RequestsResolutionFromConflictResolverForItemType()
		{
			var conflictResolver = Mocking.MockAndBind<ISyncConflictResolver<TestItem, Guid>>(_container);
			var syncResult = Mock.Create<ISyncResult<TestItem, Guid>>();
			var target = new SendingSynchronizer<TestItem, Guid>(_container);
			target.ResolveItemConflicts(syncResult);

			Mock.Assert(()=>conflictResolver.Resolve(syncResult.ConflictingItems), Occurs.Once());
		}

		[TestMethod]
		public void SendChangedItemsToRemoteReplica__SendsChangesViaRequestSenderForType()
		{
			var requestSender = Mocking.MockAndBind<ISyncRequestSender<TestItem, Guid>>(_container);
			var syncResult = Mock.Create<ISyncResult<TestItem, Guid>>();
			var target = new SendingSynchronizer<TestItem, Guid>(_container);
			ConflictResolutionsToSend<TestItem, Guid> conflictResolutions = new ConflictResolutionsToSend<TestItem, Guid>();
			target.SendChangedItemsToRemoteReplica(syncResult.ChangesRequest, conflictResolutions);

			Mock.Assert(()=>requestSender.SendChangedItems(Arg.IsAny<IEnumerable<TestItem>>()), Occurs.Once());
		}


		[TestMethod]
		public void SendChangedItemsToRemoteReplica_BothParmsHaveChanges_ChangesAggregated()
		{
			IReplica replica = new Replica("Test");
			var testItem1 = new TestItem();
			var testItem2 = new TestItem();

			var requestSender = Mocking.MockAndBind<ISyncRequestSender<TestItem, Guid>>(_container);
			var repository = Mocking.MockAndBind<IRepository<TestItem, Guid>>(_container);
			Mock.Arrange(() => repository.Find(testItem2.Id)).Returns(testItem2);
			var syncResult = new SyncResult<TestItem, Guid>(_container) 
				{ChangesRequest = new List<Guid>{testItem2.Id}};
			var conflictResolutions =
				new ConflictResolutionsToSend<TestItem, Guid> { new Resolution<TestItem, Guid>(replica, testItem1) };

			var target = new SendingSynchronizer<TestItem, Guid>(_container);
			target.SendChangedItemsToRemoteReplica(syncResult.ChangesRequest, conflictResolutions);

			Mock.Assert(()=> requestSender.SendChangedItems(
				Arg.Matches<IEnumerable<TestItem>>(items=> items.Contains(testItem1) && items.Contains(testItem2))));
		}



	}
}
