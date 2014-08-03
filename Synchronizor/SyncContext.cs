using log4net;
using TinyIoC;

namespace Sync
{
	public interface ISyncContext<TItem, TId> where TItem : class, ISynchronizable<TId>
	{
		IRepository<TItem, TId> Repository { get; }
		ILog Logger { get; }
		IReplica LocalReplica { get; }
		IReplica ToReplica { get; set; }
	}

	internal class SyncContext<TItem, TId> : ISyncContext<TItem, TId> where TItem : class, ISynchronizable<TId>
	{  
		private readonly TinyIoCContainer _container;
		private IReplica _localReplica;
		private ILog _logger;
		
		
		private IRepository<TItem, TId> _repository;

		public SyncContext(TinyIoCContainer container)
		{
			_container = container;
		}

		public IRepository<TItem, TId> Repository
		{
			get { return _repository ?? (_repository = _container.Resolve<IRepository<TItem, TId>>()); }
		}


		public ILog Logger
		{
			get { return _logger ?? (_logger = _container.Resolve<ILog>()); }
		}

		public IReplica LocalReplica
		{
			get { return _localReplica ?? (_localReplica = _container.Resolve<IReplica>()); }
		}

		public IReplica ToReplica { get; set; }

	}
}