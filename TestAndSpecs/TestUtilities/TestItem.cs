using System;
using System.Collections.Generic;
using IntervalTreeClocksCSharp;
using Sync;

namespace TestUtilities
{
	public class TestItem : ISynchronizable<Guid>
	{
		public TestItem()
		{
			Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }
		public Stamp Stamp { get; set; }
		public string FieldA { get; set; }
		public string FieldB { get; set; }
		public IEnumerable<Conflict<Guid>> Conflicts { get; set; }
		public DateTime BeginTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime LastChangedTime { get; set; }

	}
}