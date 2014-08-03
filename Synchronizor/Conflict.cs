using System.Collections;
using System.Collections.Generic;

namespace Sync
{
	public class Conflict<TId>
	{
		public IEnumerable<ConflictItem> ConflictItems { get; set; }

		public class ConflictItem
		{
			public Replica Replica { get; set; }
			public ISynchronizable<TId> Item { get; set; }
		}
	}
}