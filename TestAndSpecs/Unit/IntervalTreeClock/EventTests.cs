using System;
using System.Runtime.InteropServices;
using IntervalTreeClocksCSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Sychronization.Unit.Tests.IntervalTreeClock
{
	[TestClass]
	public class EventTests
	{
		[TestMethod]
		public void Join_TwoLeafEvents_1stGetsMaxValueOfBoth()
		{
			var event1 = new Event();
			var event2 = new Event();

			event1.Value = 1;
			event2.Value = 3;

			Event.Join(event1, event2);

			Assert.AreEqual(3, event1.Value);
		}

		[TestMethod]
		public void Join_TwoNodeEventsSecondGreater_IncreaseEvent1ByMinLeafValueOfMaxNode()
		{
			var event1 = new Event();
			event1.SetAsNode();
			var event2 = new Event();
			event2.SetAsNode();

			event1.Left.Value = 2;
			event1.Right.Value = 1;
			event2.Left.Value = 4;
			event2.Right.Value = 3;

			Event.Join(event1, event2);

			Assert.AreEqual(3, event1.Value);

		}

		[TestMethod]
		public void Join_TwoNodeEventsFirstGreater_IncreaseEvent1ByMinLeafValueOfMaxNode()
		{
			var event1 = new Event();
			event1.SetAsNode();
			var event2 = new Event();
			event2.SetAsNode();

			event1.Left.Value = 6;
			event1.Right.Value = 5;
			event2.Left.Value = 4;
			event2.Right.Value = 3;

			Event.Join(event1, event2);

			Assert.AreEqual(5, event1.Value);

		}

		[TestMethod]
		public void Join_TwoNodeEvents1stHasGreaterLeft2ndHasGreaterRight_IncreaseEvent1ByMinLeafValueOfMaxNode()
		{
			var event1 = new Event();
			event1.SetAsNode();
			var event2 = new Event();
			event2.SetAsNode();

			event1.Left.Value = 6;
			event1.Right.Value = 3;
			event2.Left.Value = 4;
			event2.Right.Value = 5;

			Event.Join(event1, event2);

			Assert.AreEqual(5, event1.Value);

		}

		[TestMethod]
		public void Normalize_NodeWithLeftAndRightLeavesWithEqualValue_NodeBecomesLeafWithValueOfLeft()
		{
			var target = new Event
			{
				IsLeaf = false,
				Left = new Event(),
				Right = new Event()
			};
			target.Left.Value = 5;
			target.Right.Value = 5;

			target.Normalize();

			Assert.AreEqual(5, target.Value);
			Assert.IsNull(target.Left);
			Assert.IsNull(target.Right);
		}


		[TestMethod]
		public void Normalize_NodeEq3WithLeftEq5AndRightEq4_NodeEq7()
		{
			var target = new Event
			{
				IsLeaf = false,
				Left = new Event(),
				Right = new Event(),
				Value = 3
			};
			target.Left.Value = 5;
			target.Right.Value = 4;

			target.Normalize();

			Assert.AreEqual(7, target.Value);
			Assert.IsNotNull(target.Left);
			Assert.IsNotNull(target.Right);
			Assert.AreEqual(1, target.Left.Value);
		}

		/// <summary>
		/// Switch to NUnit here because the syntax is easier
		/// for data driven (a.k.a. parameterized tests)
		/// </summary>
		[TestCase(false, false, 5, 4, 0, 0, 0, 0, false, TestName = "Leq | Both Nodes; Target Value > Param Value | Returns false")]
		[TestCase(false, true, 5, 4, 0, 0, 0, 0, false, TestName = "Leq | Target node, Param leaf; Target Value > Param Value | Returns false")]
		[TestCase(false, false, 1, 1, 1, 0, 0, 0, false, TestName = "Leq | Both Nodes; Target left leaf value > Param | Returns false")]
		[TestCase(false, false, 1, 1, 0, 1, 0, 0, false, TestName = "Leq | Both Nodes; Target right leaf value > Param | Returns false")]
		[TestCase(false, false, 1, 1, 0, 0, 1, 0, true, TestName = "Leq | Both Nodes; Target left leaf value < Param | Returns true")]
		[TestCase(false, false, 1, 1, 1, 0, 1, 0, true, TestName = "Leq | Both Nodes; Target left leaf value = Param | Returns true")]
		[TestCase(false, false, 1, 1, 0, 0, 0, 1, true, TestName = "Leq | Both Nodes; Target right leaf value < Param | Returns true")]
		[TestCase(false, false, 1, 1, 0, 1, 0, 1, true, TestName = "Leq | Both Nodes; Target right leaf value = Param | Returns true")]
		[TestCase(false, true, 0, 0, 1, 0, 0, 0, false, TestName = "Leq | Target node, Param leaf; Target left leaf value > Param value | Returns false")]
		[TestCase(false, true, 0, 0, 0, 1, 0, 0, false, TestName = "Leq | Target node, Param leaf; Target right leaf value > Param value | Returns false")]
		[TestCase(false, true, 0, 1, 0, 0, 0, 0, true, TestName = "Leq | Target node, Param leaf; Target left leaf value < Param value | Returns true")]
		[TestCase(false, true, 0, 1, 0, 0, 0, 0, true, TestName = "Leq | Target node, Param leaf; Target right leaf value < Param value | Returns true")]
		[TestCase(false, true, 0, 1, 1, 0, 0, 0, true, TestName = "Leq | Target node, Param leaf; Target left leaf value = Param value | Returns true")]
		[TestCase(false, true, 0, 1, 0, 1, 0, 0, true, TestName = "Leq | Target node, Param leaf; Target right leaf value = Param value | Returns true")]
		[TestCase(true, true, 1, 0, 0, 0, 0, 0, false, TestName = "Leq | Both leaves; Target value > Param | Returns false")]
		[TestCase(true, true, 0, 1, 0, 0, 0, 0, true, TestName = "Leq | Both leaves; Target value < Param | Returns true")]
		[TestCase(true, true, 1, 1, 0, 0, 0, 0, true, TestName = "Leq | Both leaves; Target value = Param | Returns true")]
		[TestCase(true, false, 0, 1, 0, 0, 0, 0, true, TestName = "Leq | Target leaf, param node; Target value < Param | Returns true")]
		[TestCase(true, false, 1, 1, 0, 0, 0, 0, true, TestName = "Leq | Target leaf, param node; Target value = Param | Returns true")]
		[TestCase(true, false, 1, 0, 0, 0, 0, 0, false, TestName = "Leq | Target leaf, param node; Target value > Param | Returns false")]
		public void Leq_Tests(
			bool targetIsLeaf,
			bool paramIsLeaf,
			int targetValue,
			int paramValue,
			int targetLeftValue,
			int targetRightValue,
			int paramLeftValue,
			int paramRightValue,
			bool assertIsLeq)
		{
			var target = new Event
			{
				Value = targetValue,
				IsLeaf = targetIsLeaf,
				Left = (targetIsLeaf ? null :new Event { Value = targetLeftValue }),
				Right = (targetIsLeaf ? null :new Event { Value = targetRightValue })
			};
			var param = new Event
			{
				Value = paramValue,
				IsLeaf = paramIsLeaf,
				Left = (paramIsLeaf ? null :new Event { Value = paramLeftValue }),
				Right = (paramIsLeaf ? null :new Event { Value = paramRightValue })
			};

			bool result = target.Leq(param);

			Assert.AreEqual(assertIsLeq, result);

		}
	}
}
