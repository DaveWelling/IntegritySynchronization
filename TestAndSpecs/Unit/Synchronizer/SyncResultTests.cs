using System;
using System.Collections.Generic;
using System.Linq;
using IntervalTreeClocksCSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Sync;
using Telerik.JustMock;
using TestUtilities;
using TinyIoC;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Sychronization.Unit.Tests
{
	[TestClass]
	public class SyncResultTests
	{
		private TinyIoCContainer _container;

		[TestInitialize]
		public void Setup()
		{
			_container = new TinyIoCContainer();
		}

		[TestMethod]
		public void GetConflictingItems_ConflictExists_ConflictAddedToSyncResult()
		{
			// Create conflicting items
			Guid id = Guid.NewGuid();
			var serverStamp = new Stamp();
			var clientStamp = serverStamp.Fork();
			serverStamp.CreateEvent();
			clientStamp.CreateEvent();
			var serverItem = new TestItem { Id = id, Stamp = serverStamp };
			var clientItem = new TestItem { Id = id, Stamp = clientStamp };

			// Arrange server repository to return server item
			var repository = Mocking.MockAndBind<IRepository<TestItem, Guid>>(_container);
			Mock.Arrange(() => repository.Find(id)).Returns(serverItem);

			// Arrange client sync request
			ISyncRequest<Guid> syncRequest = new SyncRequest<TestItem, Guid> { new SyncItem<Guid>(clientItem)};

			var target = new SyncResult<TestItem, Guid>(_container);
			target.GetConflictingItems(syncRequest);

			Assert.IsTrue(target.ConflictingItems.Contains(serverItem));
		}


		[TestMethod]
		public void GetConflictingItems_ChangesHaveNoConflicts_NoConflictsInSyncResult()
		{
			// Create non conflicting items
			Guid id = Guid.NewGuid();
			var serverStamp = new Stamp();
			var clientStamp = serverStamp.Fork();
			var serverItem = new TestItem { Id = id, Stamp = serverStamp };
			var clientItem = new TestItem { Id = id, Stamp = clientStamp };

			// Arrange server repository to return server item
			var repository = Mocking.MockAndBind<IRepository<TestItem, Guid>>(_container);
			Mock.Arrange(() => repository.Find(id)).Returns(serverItem);

			// Arrange client sync request
			ISyncRequest<Guid> syncRequest = new SyncRequest<TestItem, Guid> { new SyncItem<Guid>(clientItem) };

			var target = new SyncResult<TestItem, Guid>(_container);
			target.GetConflictingItems(syncRequest);

			Assert.IsFalse(target.ConflictingItems.Contains(serverItem));
		}

		[TestMethod]
		public void GetFulfillingReplicaItemChanges_RepositoryHasChanges_ChangesInSyncResult()
		{
			var repository = Mocking.MockAndBind<IRepository<TestItem, Guid>>(_container);
			var changes = new List<TestItem> { new TestItem() };
			Mock.Arrange(() => repository.GetChangesSince(Arg.AnyDateTime)).Returns(changes);

			var target = new SyncResult<TestItem, Guid>(_container);
			target.GetFulfillingReplicaItemChanges(Mock.Create<ISyncRequest<Guid>>());
			Assert.IsTrue(target.FulfillingReplicaItemChanges.Any(si => si.Id == changes.First().Id));

		}

		[TestMethod]
		public void GetChangesRequest_2ItemsRequested1HasConflict_1IteminChangesRequest()
		{
			// Create non conflicting items
			var conflictItem = new TestItem();
			var item = new TestItem();

			
			// Arrange client sync request
			var syncRequest = new SyncRequest<TestItem, Guid> {new SyncItem<Guid>(conflictItem), new SyncItem<Guid>(item)};

			var target = new SyncResult<TestItem, Guid>(_container);
			List<TestItem> conflicts = (List<TestItem>) target.ConflictingItems;
			conflicts.Add(conflictItem);

			target.GetChangesRequest(syncRequest);

			Assert.AreEqual(1, target.ChangesRequest.Count());
			Assert.IsTrue(target.ChangesRequest.Contains(item.Id));
		}
	}
}
