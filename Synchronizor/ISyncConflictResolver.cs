using System.Collections.Generic;

namespace Sync
{
	public interface ISyncConflictResolver<TItem, TId> where TItem : ISynchronizable<TId>
	{
		ConflictResolutionsToSend<TItem, TId> Resolve(IEnumerable<TItem> conflictingItems);
	}
}