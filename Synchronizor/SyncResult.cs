using System;
using System.Collections.Generic;
using System.Linq;
using TinyIoC;

namespace Sync
{
	public interface ISyncResult<TItem, TId> where TItem : ISynchronizable<TId>
	{

		/// <summary>
		/// all conflicting items 
		/// (including normal item content and ID and ITC stamp info)
		/// </summary>
		IEnumerable<TItem> ConflictingItems { get; }

		/// <summary>
		/// IDs of items found in SyncRequest 
		/// that are not in conflict and do not already exist (with the same ITC stamp)
		/// at the local repository
		/// </summary>
		IEnumerable<TId> ChangesRequest { get; }

		/// <summary>
		/// Items changes found on the Replica receiving the SyncRequest
		/// (including normal item content and ID and ITC stamp info)
		/// </summary>
		IEnumerable<ISynchronizable<TId>> FulfillingReplicaItemChanges { get; }

		void Build(ISyncRequest<TId> request);
	}

	/// <summary>
	/// 1) all conflicting items 
	/// (including normal item content and ID and ITC stamp info)
	/// 2) local items changes with ID and ITC stamp info.
	/// (including normal item content and ID and ITC stamp info)
	/// 3) ChangesRequest: IDs of items found in SyncRequest 
	/// that are not in conflict and do not already exist (with the same ITC stamp)
	/// at the local repository
	/// </summary>
	public class SyncResult<TItem, TId> : ISyncResult<TItem, TId> where TItem : class, ISynchronizable<TId>
	{
		private readonly TinyIoCContainer _container;

		public SyncResult(TinyIoCContainer container)
		{
			_container = container;
		}

		private IRepository<TItem, TId> _repository;
		private readonly List<TItem> _conflictingItems = new List<TItem>();

		protected IRepository<TItem, TId> Repository
		{
			get { return _repository ?? (_repository = _container.Resolve<IRepository<TItem, TId>>()); }
		}

		/// <summary>
		/// all conflicting items 
		/// (including normal item content and ID and ITC stamp info)
		/// </summary>
		public IEnumerable<TItem> ConflictingItems
		{
			get { return _conflictingItems; }
		}

		/// <summary>
		/// IDs of items found in SyncRequest 
		/// that are not in conflict and do not already exist (with the same ITC stamp)
		/// at the local repository
		/// </summary>
		public IEnumerable<TId> ChangesRequest { get; internal set; }

		/// <summary>
		/// Items changes found on the Replica receiving the SyncRequest
		/// (including normal item content and ID and ITC stamp info)
		/// </summary>
		public IEnumerable<ISynchronizable<TId>> FulfillingReplicaItemChanges { get; internal set; }

		public void Build(ISyncRequest<TId> request)
		{
			GetConflictingItems(request);
			GetChangesRequest(request);
			GetFulfillingReplicaItemChanges(request);
		}

		internal void GetFulfillingReplicaItemChanges(ISyncRequest<TId> request)
		{
			FulfillingReplicaItemChanges = Repository.GetChangesSince(request.RequestingReplica.LastSyncTime);
		}

		internal void GetChangesRequest(ISyncRequest<TId> request)
		{
			var conflictIds = ConflictingItems.Select(i => i.Id);
			var requestIds = request.Select(i => i.Id);
			ChangesRequest = requestIds.Except(conflictIds);
		}

		internal void GetConflictingItems(ISyncRequest<TId> request)
		{
			foreach (SyncItem<TId> syncItem in request)
			{
				var serverItem = Repository.Find(syncItem.Id);
				if (serverItem != null)
				{
					if (ChangeEvaluator.GetWhichChanged(syncItem.Stamp, serverItem.Stamp) == ChangeOccurredIn.Both)
					{
						_conflictingItems.Add(serverItem);
					}
				}
			}
			
		}
	}
}