using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync;
using Telerik.JustMock;
using TestUtilities;
using TinyIoC;

namespace Sychronization.Unit.Tests
{
	[TestClass]
	public class ReceivingSynchronizerTests
	{
		private TinyIoCContainer _container;

		[TestInitialize]
		public void Setup()
		{
			_container = new TinyIoCContainer();
		}

		[TestMethod]
		public void FulfillSyncRequest_ValidRequest_SyncResultBuildCalled()
		{
			var syncResult = Mocking.MockAndBind<ISyncResult<TestItem, Guid>>(_container);

			var replica = new Replica("test");
			var syncRequest = Mock.Create<ISyncRequest<Guid>>();

			var target = new ReceivingSynchronizer<TestItem, Guid>(_container);

			var result = target.FulfillSyncRequest(syncRequest);

			Mock.Assert(() => syncResult.Build(syncRequest), Occurs.Once());

		}

		[TestMethod]
		public void ReceiveChangedItemsAsRemoteReplica_ChangeToExistingRecord_ChangeIsUpdated()
		{

			var testItem = new TestItem();
			var repository = Mocking.MockAndBind<IRepository<TestItem, Guid>>(_container);
			Mock.Arrange(() => repository.Find(testItem.Id)).Returns(testItem);
			var changes = new List<TestItem> { testItem };
			var target = new ReceivingSynchronizer<TestItem, Guid>(_container);
			target.ReceiveChangedItemsAsRemoteReplica(changes);

			Mock.Assert(() => repository.Update(testItem), Occurs.Once());
		}

		[TestMethod]
		public void ReceiveChangedItemsAsRemoteReplica_ChangesExist_ChangesAreSaved()
		{
			var testItem = new TestItem();
			TestItem nullTestItem = null;
			var repository = Mocking.MockAndBind<IRepository<TestItem, Guid>>(_container);
			// ReSharper disable once ExpressionIsAlwaysNull
			Mock.Arrange(() => repository.Find(testItem.Id)).Returns(nullTestItem);
			var changes = new List<TestItem> { testItem };
			var target = new ReceivingSynchronizer<TestItem, Guid>(_container);
			target.ReceiveChangedItemsAsRemoteReplica(changes);

			Mock.Assert(() => repository.Insert(testItem), Occurs.Once());
		}
	}
}