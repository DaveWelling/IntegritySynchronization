using System.Collections;
using System.Collections.Generic;

namespace Sync
{
	public class ConflictResolutionsToSend<TItem, TId> : List<Resolution<TItem, TId>> where TItem : ISynchronizable<TId>
	{
	}
	public class Resolution<TItem, TId> where TItem : ISynchronizable<TId>
	{
		public Resolution(IReplica replica, TItem item)
		{
			Replica = replica;
			Item = item;
		}
		public IReplica Replica { get; private set; }
		public TItem Item { get; private set; }
	}
}