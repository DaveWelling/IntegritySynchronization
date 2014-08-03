using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync;
using TestUtilities;
using TinyIoC;

namespace Sychronization.Unit.Tests
{
	[TestClass]
	public class TinyIocEvaluationTests
	{
		[TestMethod]
		public void Resolve_RegisteredGenericInterface_ReturnsRegisteredClass()
		{

			var container = TinyIoCContainer.Current;
			container.Register<IRepository<TestItem, Guid>, TestRepository>();
			var testClass = new TestClass<TestItem, Guid>();
			var result = testClass.Repository;
			Assert.IsNotNull(result);
		}

		// Singleton resolution is needed for tests
		[TestMethod]
		public void Resolve_RegisteredInstance_ReturnsSameInstanceAlways()
		{
			var container = new TinyIoCContainer();
			var testInstance = new Test();
			container.Register<ITest>(testInstance);
			var try1 = container.Resolve<ITest>();
			var try2 = container.Resolve<ITest>();
			Assert.AreEqual(try1.Id, try2.Id);
		}

		private interface ITest
		{
			Guid Id { get; }
		}

		private class Test : ITest
		{
			public Test()
			{
				Id = Guid.NewGuid();
			}
			public Guid Id { get; private set; }
		}
		private class TestRepository : IRepository<TestItem, Guid>
		{
			public TestItem Find(Guid id)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<TestItem> Get()
			{
				throw new NotImplementedException();
			}

			public void Insert(TestItem item)
			{
				throw new NotImplementedException();
			}

			public void Update(TestItem item)
			{
				throw new NotImplementedException();
			}

			public void Delete(Guid id)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<ISynchronizable<Guid>> GetChangesSince(DateTime lastSyncTime)
			{
				throw new NotImplementedException();
			}
		}
		private class TestClass<TItem, TId> where TItem : class, ISynchronizable<TId>
		{
			readonly TinyIoCContainer _container = TinyIoCContainer.Current;
			private IRepository<TItem, TId> _repository;

			public IRepository<TItem,TId> Repository
			{
				get { return _repository ?? (_repository = _container.Resolve<IRepository<TItem, TId>>()); }
			}
		}
	}
}
