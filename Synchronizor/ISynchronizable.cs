using System.Collections;
using System.Collections.Generic;
using IntervalTreeClocksCSharp;

namespace Sync
{
	public interface ISynchronizable<TId>
	{
		TId Id { get; set; }
		Stamp Stamp { get; set; }
		IEnumerable<Conflict<TId>> Conflicts { get; set; }
	}
}