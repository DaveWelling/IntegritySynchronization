using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyIoC;

namespace Sync
{
	/// <summary>
	///     Steps (see pg 37 in "Pro Sync Framework" book by Singh and Kanjilal from Apress):
	///     1) Destination looks at itself(its replica) and gathers knowledge
	///     (aka watermark -- compact rep of changes)
	///     + Done by calling ||GetSyncBatchParameters|| on destination
	///     2) Destination sends knowledge to source.
	///     3) Source compares received knowledge to itself (local replica) by comparing individual
	///     item versions to the knowledge received (from destination).
	///     + Done by calling ||GetChangeBatch|| on source
	///     4) Source sends back changed versions (and knowledge?) to destination (as changeDataRetriever object).
	///     5) Destination retrieves local replica copies of changes reported by source
	///     , and Destination resolves conflicts
	///     + Done by calling ||ProcessChangeBatch|| on destination
	///     6) Destination requests actual changes from source
	///     7) Source receives request and fires ||LoadChangeData|| to retrieve the items from itself (its replica).
	///     8) Source sends items to the destination.
	///     9) Destination calls ||SaveItemChange|| to update itself (its replica) with the changes received.
	/// </summary>
	public class SendingSynchronizer<TItem, TId> where TItem : class, ISynchronizable<TId>
	{
		private readonly TinyIoCContainer _container;
		private ISyncContext<TItem, TId> _context;

		public SendingSynchronizer(TinyIoCContainer container)
		{
			_container = container;
			_context = new SyncContext<TItem, TId>(container);
		}


		public void BeginSync(IReplica toReplica)
		{
			_context.ToReplica = toReplica;
			_context.Logger.Info(string.Format("Begin sync from replica name: {0} ID: {1} | to replica name: {2} ID: {3}",
				_context.LocalReplica.Name, _context.LocalReplica.Id, toReplica.Name, toReplica.Id));

			var syncRequest = new SyncRequest<TItem, TId>(_context);
			var syncResult = SendSyncRequestToRemoteReplica(syncRequest);
			var conflictResolutions = ResolveItemConflicts(syncResult);
			SendChangedItemsToRemoteReplica(syncResult.ChangesRequest, conflictResolutions);

			_context.Logger.Info(string.Format("Finished sync from replica name: {0} ID: {1} | to replica name: {2} ID: {3}",
				_context.LocalReplica.Name, _context.LocalReplica.Id, toReplica.Name, toReplica.Id));

		}

		internal void SendChangedItemsToRemoteReplica(IEnumerable<TId> changesRequested,
			ConflictResolutionsToSend<TItem, TId> conflictResolutions)
		{
			var sender = _container.Resolve<ISyncRequestSender<TItem, TId>>();
			var localChangesRequested = changesRequested
				.Select(id => _context.Repository.Find(id))
				.Where(found => found != null);
			var toSend = localChangesRequested
				.Union(conflictResolutions.Select(r => r.Item));
			sender.SendChangedItems(toSend);
		}

		/// <summary>
		///     Resolve conflicts:
		///     1) Conflicts returned by remote replica
		///     2) New conflicts found in item ID/ITC Stamps sent from remote replica
		/// </summary>
		/// <param name="syncResult"></param>
		/// <returns>
		///     Items outdated on the remote replica
		///     because of conflict resolution
		/// </returns>
		internal ConflictResolutionsToSend<TItem, TId> ResolveItemConflicts(ISyncResult<TItem, TId> syncResult)
		{
			var conflictResolver = _container.Resolve<ISyncConflictResolver<TItem, TId>>();
			return conflictResolver.Resolve(syncResult.ConflictingItems);
		}


		internal ISyncResult<TItem, TId> SendSyncRequestToRemoteReplica(ISyncRequest<TId> syncRequest)
		{

			var sender = _container.Resolve<ISyncRequestSender<TItem, TId>>();
			return sender.SendInitialRequest(syncRequest);
		}

	}
}