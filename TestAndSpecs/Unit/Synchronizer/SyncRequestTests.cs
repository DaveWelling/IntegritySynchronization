using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync;
using Telerik.JustMock;
using TestUtilities;
using TinyIoC;

namespace Sychronization.Unit.Tests
{
	[TestClass]
	public class SyncRequestTests
	{
		private TinyIoCContainer _container;

		[TestInitialize]
		public void Setup()
		{
			_container = new TinyIoCContainer();
		}

		[TestMethod]
		public void GetChanges_RepositoryHasChanges_ChangesInSyncRequest()
		{
			// Make repository return some changes.
			var context = Mocking.GetSyncContext<TestItem, Guid>(_container);
			var changes = new List<TestItem> { new TestItem() };
			Mock.Arrange(() => context.Repository.GetChangesSince(Arg.AnyDateTime)).Returns(changes);

			var target = new SyncRequest<TestItem, Guid>(context);

			Assert.IsTrue(target.Any(si => si.Id == changes.First().Id));
		}
	}
}
