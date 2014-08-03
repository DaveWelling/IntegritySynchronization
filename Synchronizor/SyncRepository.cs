using System;
using System.Collections.Generic;
using IntervalTreeClocksCSharp;
using TinyIoC;

namespace Sync
{
	public abstract class SyncRepository<TItem, TId> : IRepository<TItem, TId>	where TItem : class, ISynchronizable<TId>
	{
		private IRepository<TItem, TId> _localStorageRepository;

		private TinyIoCContainer _container;

		protected TinyIoCContainer Container
		{
			get 
			{ 
				if (_container == null) throw new ApplicationException("IoC container must be passed in by constructor.");
				return _container;
			}
			set { _container = value; }
		}

		protected IRepository<TItem, TId> LocalStorageRepository
		{
			get { return _localStorageRepository ?? (_localStorageRepository = Container.Resolve<IRepository<TItem, TId>>()); }
			set { _localStorageRepository = value; }
		}

		public virtual TItem Find(TId id)
		{
			return LocalStorageRepository.Find(id);
		}

		public virtual IEnumerable<TItem> Get()
		{
			return LocalStorageRepository.Get();
		}

		public virtual void Insert(TItem item)
		{
			if (item.Stamp == null)
			{
				item.Stamp = new Stamp();
			}
			item.Stamp.CreateEvent();
			LocalStorageRepository.Insert(item);
		}

		public virtual void Update(TItem item)
		{
			if (item.Stamp == null)
			{
				throw new ApplicationException("Attempting to update an item that has no synchronization stamp.  This item should have received a stamp when it was inserted.");
			}
			item.Stamp.CreateEvent();
			LocalStorageRepository.Update(item);
		}

		public virtual void Delete(TId id)
		{
			var item = Find(id);
			if (item != null)
			{
				if (item.Stamp == null)
				{
					throw new ApplicationException("Attempting to delete an item that has no synchronization stamp.  This item should have received a stamp when it was inserted.");
				}
				item.Stamp.CreateEvent();
				LocalStorageRepository.Update(item);
			}
			LocalStorageRepository.Delete(id);
#if DEBUG	
			item = Find(id);
			if (item == null)
			{
				throw new ApplicationException("This repository removed the record completely during the Delete method.  This will prevent the delete action from being propogated to other replicas.  Instead consider creating a tombstone by placing a begin and end time for the item type in the same or an affiliated repository created for this purpose.");
			}
#endif
		}

		public virtual IEnumerable<ISynchronizable<TId>> GetChangesSince(DateTime lastSyncTime)
		{
			return LocalStorageRepository.GetChangesSince(lastSyncTime);
		}
	}
}
