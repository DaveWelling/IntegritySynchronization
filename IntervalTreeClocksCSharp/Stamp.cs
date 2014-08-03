using System;

namespace IntervalTreeClocksCSharp
{
	public class Stamp
	{
		public Event Event { get; private set; }
		public Id Id { get; private set; }

		public Stamp()
		{
			Event = new Event();
			Id = new Id();
		}

		private Stamp(Id id, Event @event)
		{
			Id = id;
			Event = @event;
		}

		public Stamp Fork()
		{
			var newStamp = new Stamp {Event = Event.Clone()};

			Id[] newIds = Id.Split();
			Id = newIds[0];
			newStamp.Id = newIds[1];

			return newStamp;
		}

		public void Join(Stamp anotherStamp)
		{
			// joins two stamps becoming itself the result stamp
			Id.Sum(Id, anotherStamp.Id);
			Event.Join(Event, anotherStamp.Event);
		}

		public Stamp Peek()
		{
			var id = new Id(0);
			Event @event = Event.Clone();
			return new Stamp(id, @event);
		}

		public void CreateEvent()
		{
			Event heldEventState = Event.Clone();
			Fill(Id, Event);
			bool notFilled = heldEventState.Equals(Event);

			if (notFilled)
			{
				Grow(Id, Event);
			}
		}

		public bool Leq(Stamp anotherStamp)
		{
			return Event.Leq(anotherStamp.Event);
		}

		private static void Fill(Id id, Event @event)
		{
			if (id.IsLeaf && id.Value == 1)
			{
				@event.Height();
			}
			else if (@event.IsLeaf || (id.IsLeaf && id.Value == 0)) // do nothing
			{
			}
			else if (!id.IsLeaf)
			{
				if (id.Left.IsLeaf && id.Left.Value == 1)
				{
					Fill(id.Right, @event.Right);
					@event.Left.Height();
					@event.Left.Value = Math.Max(@event.Left.Value, @event.Right.Value);
					@event.Normalize();
				}
				else if (id.Right.IsLeaf && id.Right.Value == 1)
				{
					Fill(id.Left, @event.Left);
					@event.Right.Height();
					@event.Right.Value = Math.Max(@event.Right.Value, @event.Left.Value);
					@event.Normalize();
				}
				else
				{
					Fill(id.Left, @event.Left);
					Fill(id.Right, @event.Right);
					@event.Normalize();
				}
			}
			else
			{
				throw new Exception("Problem with the fill method \n ID:" + id + "\n Ev:" + @event);
			}
		}
						  
		private static int Grow(Id id, Event @event)
		{
			int cost;

			if (@event.IsLeaf)
			{
				if (id.IsLeaf && id.Value == 1)
				{
					@event.Value++;
					return 0;
				}

				@event.SetAsNode();
				cost = Grow(id, @event);
				return cost + 1000;
			}

			if (!id.IsLeaf)
			{
				if (id.Left.IsLeaf && id.Left.Value == 0)
				{
					cost = Grow(id.Right, @event.Right);
					return cost + 1;
				}
				if (id.Right.IsLeaf && id.Right.Value == 0)
				{
					cost = Grow(id.Left, @event.Left);
					return cost + 1;
				}
			}  
			else  // i.IsLeaf
			{
				throw new Exception("Error in the grow operation \n ID:" + id + "\n Event:" + @event);
			}

			int costRight = Grow(id.Right, @event.Right);
			int costLeft = Grow(id.Left, @event.Left);
			if (costLeft < costRight)
			{
				@event.Right = @event.Right.Clone();
				return costLeft + 1;
			}
			@event.Left = @event.Left.Clone();
			return costRight + 1;
		}

		public override String ToString()
		{
			return "( " + Id + ", " + Event + " )";
		}

	}
}