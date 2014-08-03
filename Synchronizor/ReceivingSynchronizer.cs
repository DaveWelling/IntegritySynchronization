using System.Collections.Generic;
using TinyIoC;

namespace Sync
{
	public class ReceivingSynchronizer<TItem, TId> where TItem : class, ISynchronizable<TId>
	{	 
		private readonly TinyIoCContainer _container;
		private ISyncContext<TItem, TId> _context;

		public ReceivingSynchronizer(TinyIoCContainer container)
		{
			_container = container;
			_context = new SyncContext<TItem, TId>(container);
		}

		/// <summary>
		///     compares knowledge request to itself (local replica) by comparing individual
		///     item versions to the ITC stamps received (from destination).
		/// </summary>
		/// <param name="syncRequest"></param>
		/// <returns>
		///     SyncResult contains:
		///     1) all conflicting items
		///     (including normal item content and ID and ITC stamp info)
		///     2) local items changes with ID and ITC stamp info.
		///     (including normal item content and ID and ITC stamp info)
		///     3) ChangesRequest: IDs of items found in KnowledgeRequest
		///     that are not in conflict and do not already exist (with the same ITC stamp)
		///     at the local repository
		/// </returns>
		public ISyncResult<TItem, TId> FulfillSyncRequest(ISyncRequest<TId> syncRequest)
		{
			var syncResult = _container.Resolve<ISyncResult<TItem, TId>>();
			syncResult.Build(syncRequest);
			return syncResult;
		}

		public void ReceiveChangedItemsAsRemoteReplica(IEnumerable<TItem> changedItemsToSave)
		{
			foreach (TItem item in changedItemsToSave)
			{
				if (_context.Repository.Find(item.Id) != null)
				{
					_context.Repository.Update(item);
				}
				else
				{
					_context.Repository.Insert(item);
				}
			}
		}
		
	}
}