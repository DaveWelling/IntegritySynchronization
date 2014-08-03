using System;

namespace Sync
{
	public interface IReplica
	{
		Guid Id { get; }
		string Name { get; }
		DateTime LastSyncTime { get; set; }
	}

	public class Replica : IReplica
	{
		// Create from persisted replica
		public Replica(string replicaName, Guid id)
		{
			Name = replicaName;
			Id = id;
		}

		// Create new replica
		public Replica(string replicaName) : this(replicaName, Guid.NewGuid())
		{
		}

		// Default to local replica
		public Replica() : this("LocalReplica", Guid.NewGuid())
		{
		}

		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public DateTime LastSyncTime { get; set; }
	}
}