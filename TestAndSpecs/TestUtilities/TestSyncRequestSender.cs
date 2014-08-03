using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync;

namespace TestUtilities
{
	public class TestSyncRequestSender<TItem, TId> : ISyncRequestSender<TItem, TId> where TItem : class, ISynchronizable<TId>
	{
		private readonly ReceivingSynchronizer<TItem, TId> _receiver;

		public TestSyncRequestSender(ReceivingSynchronizer<TItem, TId> receiver)
		{
			_receiver = receiver;
		}

		public ISyncResult<TItem, TId> SendInitialRequest(ISyncRequest<TId> request)
		{
			return _receiver.FulfillSyncRequest(request);
		}

		public void SendChangedItems(IEnumerable<TItem> changes)
		{
			_receiver.ReceiveChangedItemsAsRemoteReplica(changes);
		}
	}
}
