using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sync;
using TinyIoC;

namespace TestUtilities
{
	public class TestRepository : SyncRepository<TestItem, Guid>
	{
		public TestRepository(TinyIoCContainer container)
		{
			Container = container;
		}

		public TestRepository()
		{	 
			LocalStorageRepository = new TestRepositoryForLocalStorage(); 
		}
	}

	public class TestRepositoryForLocalStorage : IRepository<TestItem, Guid>
	{
		private readonly IDictionary<Guid, TestItem> _list = new Dictionary<Guid, TestItem>();


		public TestItem Find(Guid id)
		{
			return !_list.ContainsKey(id) ? null : _list[id];
		}

		public IEnumerable<TestItem> Get()
		{
			return _list.Values;
		}

		public void Insert(TestItem item)
		{
			item.LastChangedTime = DateTime.Now;
			_list.Add(item.Id, item);
		}

		public void Update(TestItem item)
		{
			item.LastChangedTime = DateTime.Now;
			if (_list.ContainsKey(item.Id))
			{
				_list[item.Id] = item;
			}
			else
			{
				_list.Add(item.Id, item);
			}
		}

		public void Delete(Guid id)
		{
			var changeTime = DateTime.Now;
			var item = Find(id);
			item.EndTime = changeTime;
			item.LastChangedTime = changeTime;
			Update(item);
			_list[item.Id] = item;
		}

		public IEnumerable<ISynchronizable<Guid>> GetChangesSince(DateTime lastSyncTime)
		{
			return _list.Values.Where(i => i.LastChangedTime >= lastSyncTime);
		}

		public bool Any(Func<TestItem, bool> func)
		{
			return _list.Values.Any(func);
		}
		
	}
}