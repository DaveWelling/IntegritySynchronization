using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntervalTreeClocksCSharp;
using Sync;

namespace ItcActionWebApi.Models
{
	public class Node
	{
		public Node()
		{
			
		}

		public Node(Stamp stamp, int someValue0)
		{
			Stamp = stamp;
			SomeValue0 = someValue0;
		}

		public Stamp Stamp { get; set; }
		public int SomeValue0 { get; set; }
		private Guid _id = Guid.Empty;

		public Guid Id
		{
			get
			{
				if (_id == Guid.Empty)
				{
					_id = Guid.NewGuid();
				}
				return _id;
			}
			set { _id = value; }
		}

		public bool HasConflict { get; set; }

		public int ConflictValue1 { get; set; }
		public int ConflictValue2 { get; set; }
	}
}