using System.Collections;
using System.Collections.Generic;

namespace Sync
{
	public interface ISyncRequest<TId> : IList<SyncItem<TId>>
	{
		IReplica RequestingReplica { get; }
	}

	public class SyncRequest<TItem, TId> : List<SyncItem<TId>>, ISyncRequest<TId> where TItem : class, ISynchronizable<TId>
	{
		private readonly ISyncContext<TItem, TId> _requestingContext;

		// Contructor for deserialization
		public SyncRequest()
		{
		}

		// Constructor for requesting replica
		internal SyncRequest(ISyncContext<TItem, TId> requestingContext)
		{
			_requestingContext = requestingContext;
			RequestingReplica = _requestingContext.LocalReplica;
			GetChanges();
		}

		internal void GetChanges()
		{
			var changes = _requestingContext.Repository.GetChangesSince(_requestingContext.ToReplica.LastSyncTime);
			foreach (var change in changes)
			{
				Add(new SyncItem<TId>(change));
			}
		}

		public IReplica RequestingReplica { get; private set; }
	}

}