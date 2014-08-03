using System;
using IntervalTreeClocksCSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Sychronization.Unit.Tests.IntervalTreeClock
{
	[TestClass]
	public class StampTests
	{
		[TestMethod]
		public void Fork_NewStamp_Event0OnBoth()
		{
			var target = new Stamp();
			var newStamp = target.Fork();
			Assert.AreEqual(0, target.Event.Value);
			Assert.AreEqual(0, newStamp.Event.Value);
		}

		[TestMethod]
		public void Fork_EventValue1_BothWithEventValue1()
		{
			var target = new Stamp();
			target.CreateEvent();
			var newStamp = target.Fork();
			Assert.AreEqual(1, target.Event.Value);
			Assert.AreEqual(1, newStamp.Event.Value);
		}


		[TestMethod]
		public void Ctor_NoParms_Id0()
		{
			var target = new Stamp();
			Assert.AreEqual(1, target.Id.Value);
		}

		[TestMethod]
		public void Ctor_NoParms_IsLeaf()
		{
			var target = new Stamp();
			Assert.IsTrue(target.Event.IsLeaf);
		}


		[TestMethod]
		public void Fork_NewStamp_NotIsLeaf()
		{
			var target = new Stamp();
			var newStamp = target.Fork();
			Assert.IsFalse(newStamp.Id.IsLeaf);
		}

		[TestMethod]
		public void Fork_NewStamp_Id0Id0()
		{
			var target = new Stamp();
			var newStamp = target.Fork();
			Assert.AreEqual(0, target.Id.Value);
			Assert.AreEqual(0, newStamp.Id.Value);
		}

		[TestMethod]
		public void Join_TwoNewStamps_Fail()
		{
			var stamp1 = new Stamp();
			var stamp2 = new Stamp();

			try
			{
				stamp1.Join(stamp2);
				Assert.Fail("Expected exception");
			}
			catch (Exception e)
			{
				Assert.IsTrue(e.Message.Contains("Sum operation on IDs failed"));
			}

		}


	}
}
