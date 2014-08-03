using System;

namespace IntervalTreeClocksCSharp
{
	public class Id
	{
		private Id _left;
		private Id _right;


		internal Id()
		{
			IsLeaf = true;
			Value = 1;
		}

		internal Id(int val)
		{
			IsLeaf = true;
			Value = val;
		}

		public bool IsLeaf { get; set; }

		public int Value { get; set; }

		public Id Left
		{
			get { return IsLeaf ? null : _left; }
			set { _left = value; }
		}

		public Id Right
		{
			get { return IsLeaf ? null : _right; }
			set { _right = value; }
		}
			   
		internal Id[] Split()
		{
			var id1 = new Id();
			var id2 = new Id();

			id1.SetAsNode();
			id1.Value = 0;
			id2.SetAsNode();
			id2.Value = 0;

			if (IsLeaf)
			{
				if (Value == 0)
				{
					throw new ApplicationException("ID value should not equal 0 if the node is a leaf");
				}
				if (Value == 1)
				{
					// id = 1
					id1.Left = new Id(1);
					id1.Right = new Id(0);

					id2.Left = new Id(0);
					id2.Right = new Id(1);
				}
			}
			else // !IsLeaf
			{
				if ((Left.IsLeaf && Left.Value == 0) && (!Right.IsLeaf || Right.Value == 1))
				{
					// id = (0, i)
					Id[] ip = Right.Split();

					id1.Left = new Id(0);
					id1.Right = ip[0];

					id2.Left = new Id(0);
					id2.Right = ip[1];
				}
				else if ((!Left.IsLeaf || Left.Value == 1) && (Right.IsLeaf && Right.Value == 0))
				{
					// id = (i, 0)
					Id[] ip = Left.Split();

					id1.Left = ip[0];
					id1.Right = new Id(0);

					id2.Left = ip[1];
					id2.Right = new Id(0);
				}
				else if ((!Left.IsLeaf || Left.Value == 1) && (!Right.IsLeaf || Right.Value == 1))
				{
					// id = (i1, i2)
					id1.Left = Left.Clone();
					id1.Right = new Id(0);

					id2.Left = new Id(0);
					id2.Right = Right.Clone();
				}
				else
				{
					throw new Exception("Bug..." + ToString());
				}
			}

			var ids = new Id[2];
			ids[0] = id1;
			ids[1] = id2;
			return ids;
		}

		/// <summary>
		///     this becomes the sum between i1 and i2
		/// </summary>
		/// <param name="id1"></param>
		/// <param name="id2"></param>
		/// <remarks>
		///     sum(0, X) -> X;
		///     sum(X, 0) -> X;
		///     sum({L1,R1}, {L2, R2}) -> norm_id({sum(L1, L2), sum(R1, R2)}).
		/// </remarks>
		internal static void Sum(Id id1, Id id2)
		{
			if (id1.IsLeaf && id1.Value == 0)
			{
				id1.Copy(id2);
			}
			else if (id2.IsLeaf && id2.Value == 0)
			{
				//i1 is the result
			}
			else if (!id1.IsLeaf && !id2.IsLeaf)
			{
				Sum(id1.Left, id2.Left);
				Sum(id1.Right, id2.Right);
				id1.Normalize();
			}
			else
			{
				throw new Exception("Sum operation on IDs failed  " +
				                    "first Id's value: " + id1.Value +
				                    " || second Id's value: " + id2.Value);
			}
		}

		/// <summary>
		///     If this is not a leaf, but the left and right nodes are leaves with the same values
		///     then just collapse them down and make this a leaf with that value.
		/// </summary>
		private void Normalize()
		{
			if (!IsLeaf && Left.IsLeaf && Right.IsLeaf)
			{
				if (Left.Value == 0 && Right.Value == 0)
				{
					SetAsLeaf();
					Value = 0;
					Left = Right = null;
				}
				else if (Left.Value == 1 && Right.Value == 1)
				{
					SetAsLeaf();
					Value = 1;
					Left = Right = null;
				}
			}
		}
		   
		private void Copy(Id id)
		{
			IsLeaf = id.IsLeaf;
			Value = id.Value;
			Left = id.Left;
			Right = id.Right;
		}

		private void SetAsLeaf()
		{
			IsLeaf = true;
			Left = Right = null;
		}

		private void SetAsNode()
		{
			IsLeaf = false;
			Value = -1;
			Left = new Id(1);
			Right = new Id(0);
		}
					
		public override String ToString()
		{
			if (IsLeaf)
			{
				return Value + "";
			}
			return "(" + Left + ", " + Right + ")";
		}

		public override bool Equals(Object anotherId)
		{
			var otherId = (Id) anotherId;
			if (anotherId == null)
			{
				return false;
			}
			if (IsLeaf && otherId.IsLeaf && Value == otherId.Value)
			{
				return true;
			}
			return !IsLeaf && !otherId.IsLeaf && Left.Equals(otherId.Left) && Right.Equals(otherId.Right);
		}

		public override int GetHashCode()
		{
			int hash = 7;
			hash = 79*hash + (IsLeaf ? 1 : 0);
			hash = 79*hash + Value;
			hash = 79*hash + (Left != null ? Left.GetHashCode() : 0);
			hash = 79*hash + (Right != null ? Right.GetHashCode() : 0);
			return hash;
		}

		private Id Clone()
		{
			return new Id
			{
				IsLeaf = IsLeaf,
				Value = Value,
				Left = (Left == null) ? null : Left.Clone(),
				Right = (Right == null) ? null : Right.Clone()
			};
		}
	}
}