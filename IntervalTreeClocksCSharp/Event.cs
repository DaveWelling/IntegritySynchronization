using System;
using System.Globalization;

namespace IntervalTreeClocksCSharp
{
	public class Event
	{
		internal Event()
		{
			Value = 0;
			IsLeaf = true;
		}

		private Event(int eventValue)
		{
			Value = eventValue;
			IsLeaf = true;
		}

		public Event Left { get; set; }

		public Event Right { get; set; }

		public int Value { get; set; }

		public bool IsLeaf { get; set; }

		/// <summary>
		///     Set left and right nodes of event1 to max(L) and
		///     max(R).
		///     max(L)& max(R) = max right and max left leaf values
		///     throughout entire event tree.
		///     Then Normalize <see cref="Event.Normalize" />
		/// </summary>
		internal static void Join(Event event1, Event event2)
		{
			// Node calculation
			if (!event1.IsLeaf && !event2.IsLeaf)
			{
				if (event1.Value > event2.Value)
				{
					// force code down "else" path by recursing with
					// switched parameters, then copy the results to event1
					Join(event2, event1);
					event1.Copy(event2);
				}
				else
				{
					int d = event2.Value - event1.Value;
					event2.Left.Lift(d);
					event2.Right.Lift(d);
					Join(event1.Left, event2.Left);
					Join(event1.Right, event2.Right);
				}
			}
			else if (event1.IsLeaf && !event2.IsLeaf)
			{
				// If only one event is a node, force both 
				// events into Node calculation (above)
				event1.SetAsNode();
				Join(event1, event2);
			}
			else if (!event1.IsLeaf && event2.IsLeaf)
			{
				// If only one event is a node, force both 
				// events into Node calculation (above)
				event2.SetAsNode();
				Join(event1, event2);
			}
			else if (event1.IsLeaf && event2.IsLeaf)
			{
				event1.Value = Math.Max(event1.Value, event2.Value);
			}
			else
			{
				throw new Exception("fail Event fork event1:" + event1 + " event2:" + event2);
			}
			event1.Normalize();
		}

		/// <summary>
		///     If this is a node and the children are also nodes,
		///     normalize by increasing event1 value by minimum
		///     of left and right,
		///     and decreasing event1.left and event1.right by
		///     that same value.
		///     If this is a node, but the children are leaves,
		///     accumulate thier values into this node value only if
		///     the leaves have equal values.
		/// </summary>
		internal void Normalize()
		{
			if (!IsLeaf && Left.IsLeaf && Right.IsLeaf && Left.Value == Right.Value)
			{
				Value += Left.Value;
				SetAsLeaf();
			}
			else if (!IsLeaf)
			{
				int minValue = Math.Min(Left.Value, Right.Value);
				Lift(minValue);
				Left.Drop(minValue);
				Right.Drop(minValue);
			}
		}

		private void Copy(Event eventToCopy)
		{
			IsLeaf = eventToCopy.IsLeaf;
			Value = eventToCopy.Value;
			Left = eventToCopy.Left;
			Right = eventToCopy.Right;
		}

		private void Lift(int increaseByValue)
		{
			Value += increaseByValue;
		}

		private static Event CloneAndLiftClone(int increaseByValue, Event eventToLift)
		{
			Event newEvent = eventToLift.Clone();
			newEvent.Value += increaseByValue;
			return newEvent;
		}

		private void Drop(int valueToDecreaseBy)
		{
			// drops itself for val
			if (valueToDecreaseBy <= Value)
			{
				Value = Value - valueToDecreaseBy;
			}
		}

		internal void Height()
		{
			if (IsLeaf) return;
			Left.Height();
			Right.Height();
			Value += Math.Max(Left.Value, Right.Value);
			SetAsLeaf();
		}

		public bool Leq(Event anotherEvent)
		{
			// Target is a node inside here
			if (!IsLeaf)
			{
				// If target value is greater than param value we are done.
				if (Value > anotherEvent.Value) return false;

				// If they are both Nodes, check which one has the greater
				// child (start with Left first)
				if (!anotherEvent.IsLeaf)
				{
					return CloneAndLiftClone(Value, Left)
						.Leq(CloneAndLiftClone(anotherEvent.Value, anotherEvent.Left))
					       && CloneAndLiftClone(Value, Right)
						       .Leq(CloneAndLiftClone(anotherEvent.Value, anotherEvent.Right));
				}

				// If param is not a leaf, compare leaves of target
				// to param leaf
				return CloneAndLiftClone(Value, Left).Leq(anotherEvent)
				       && CloneAndLiftClone(Value, Right).Leq(anotherEvent);
			}

			// Target IsLeaf below here

			// Both are leaves, just check value
			if (anotherEvent.IsLeaf) return Value <= anotherEvent.Value;

			// Target is leaf and param is node
			if (Value < anotherEvent.Value) return true;

			// Target is leaf, param is node, target value >= param value
			// Convert target to node and recurse
			Event tempEvent = Clone();
			tempEvent.SetAsNode();
			return tempEvent.Leq(anotherEvent);
		}

		private void SetAsLeaf()
		{
			IsLeaf = true;
			Left = null;
			Right = null;
		}

		internal void SetAsNode()
		{
			IsLeaf = false;
			Left = new Event(0);
			Right = new Event(0);
		}

		public override String ToString()
		{
			if (IsLeaf)
			{
				return Value.ToString(CultureInfo.InvariantCulture);
			}
			return "(" + Value + ", " + Left + ", " + Right + ")";
		}

		public override bool Equals(Object anotherEvent)
		{
			var otherEvent = (Event) anotherEvent;
			if (otherEvent == null)
			{
				return false;
			}
			if (IsLeaf && otherEvent.IsLeaf && Value == otherEvent.Value)
			{
				return true;
			}
			return !IsLeaf && !otherEvent.IsLeaf && Value == otherEvent.Value
			       && Left.Equals(otherEvent.Left) && Right.Equals(otherEvent.Right);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (Left != null ? Left.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (Right != null ? Right.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ Value;
				hashCode = (hashCode*397) ^ IsLeaf.GetHashCode();
				return hashCode;
			}
		}

		internal Event Clone()
		{
			return new Event
			{
				IsLeaf = IsLeaf,
				Value = Value,
				Left = (Left == null) ? null : Left.Clone(),
				Right = (Right == null) ? null : Right.Clone()
			};
		}
	}
}