using System;
using System.Collections.Generic;

namespace Sync
{
	public interface IRepository<TSynchronizable, TId> where TSynchronizable : class, ISynchronizable<TId>
	{
		TSynchronizable Find(TId id);
		IEnumerable<TSynchronizable> Get();
		void Insert(TSynchronizable item);
		void Update(TSynchronizable item);
		void Delete(TId id);
		IEnumerable<ISynchronizable<TId>> GetChangesSince(DateTime lastSyncTime);
	}
}