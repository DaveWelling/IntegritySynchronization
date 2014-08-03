using System;
using System.Collections.Generic;
using IntervalTreeClocksCSharp;

namespace Sync
{
	public class SyncItem<TId> 
	{
		public SyncItem(ISynchronizable<TId> change)
		{
			Id = change.Id;
			Stamp = change.Stamp;
		}


		public TId Id { get; set; }
		public Stamp Stamp { get; set; }
	}
}