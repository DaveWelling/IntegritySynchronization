using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sync
{
	public interface ISyncRequestSender<TItem, TId>	where TItem : ISynchronizable<TId>
	{
		ISyncResult<TItem, TId> SendInitialRequest(ISyncRequest<TId> request);
		void SendChangedItems(IEnumerable<TItem> changes);
	}
}