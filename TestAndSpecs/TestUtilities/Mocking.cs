using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Sync;
using Telerik.JustMock;
using TinyIoC;

namespace TestUtilities
{
	public static class Mocking
	{
		public static T MockAndBind<T>(TinyIoCContainer container) where T: class
		{
		
			T constant = Mock.Create<T>();
			container.Register<T>(constant);
			return constant;
		}

		public static ISyncContext<TItem, TId> GetSyncContext<TItem, TId>(TinyIoCContainer container) where TItem : class, ISynchronizable<TId>
		{ 
			var result = new SyncContext<TItem, TId>(container);
			MockAndBind<IRepository<TItem, TId>>(container);
			MockAndBind<ILog>(container);
			MockAndBind<IReplica>(container);
			result.ToReplica = new Replica("TestReceivingReplica") {LastSyncTime = DateTime.MinValue};
			return result;

		}

	}
}
