using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync;
using TinyIoC;

namespace TestUtilities
{
	public class TestItemConflictResolver : ISyncConflictResolver<TestItem, Guid>
	{
		private readonly TinyIoCContainer _container;
		public TestItemConflictResolver(TinyIoCContainer container)
		{
			_container = container;
		}

		private IReplica _localReplica;

		protected IReplica LocalReplica
		{
			get { return _localReplica ?? (_localReplica = _container.Resolve<IReplica>()); }
		}

		public ConflictResolutionsToSend<TestItem, Guid> Resolve(IEnumerable<TestItem> conflictingItems)
		{
			var resolutions = new ConflictResolutionsToSend<TestItem, Guid>();
			foreach (var conflictingItem in conflictingItems)
			{
				if (conflictingItem.Conflicts.Count() > 1)
				{
					throw new ApplicationException(string.Format("Item {0} with ID {1} has more than one conflict.  This is not supported by this resolver.", conflictingItem, conflictingItem.Id));
				}
				var conflict1 = conflictingItem.Conflicts.First();
				// Find one with the greatest timestamp
				Conflict<Guid>.ConflictItem winner = null;
				foreach (Conflict<Guid>.ConflictItem conflictItem in conflict1.ConflictItems)
				{
					if (winner == null)
					{
						winner = conflictItem;
					}
					else
					{
						if (((TestItem) conflictItem.Item).LastChangedTime > ((TestItem) winner.Item).LastChangedTime)
						{
							winner = conflictItem;
						}
					}
				}
				if (winner == null)
				{
					throw new ApplicationException("conflict.ConflictItems must have at least 2 items.");
				}

				if (winner.Replica.Id != LocalReplica.Id)
				{
					resolutions.Add(new Resolution<TestItem,Guid>(LocalReplica, (TestItem)winner.Item));
				}
			}
			return resolutions;
		}
	}
}
