using IntervalTreeClocksCSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sychronization.Unit.Tests.IntervalTreeClock
{
	/// <summary>
	/// Summary description for IdTests
	/// </summary>
	[TestClass]
	public class IdTests
	{

		[TestMethod]
		public void Sum_BothAreLeaf1stValueIs1_NoChange()
		{
			var id1 = new Id(1);
			var id2 = new Id(0);

			Id.Sum(id1, id2);

			Assert.AreEqual(1, id1.Value);
		}

		[TestMethod]
		public void Sum_BothLeaf1stValueIs02ndIs1_1stValueIs1()
		{
			var id1 = new Id(0);
			var id2 = new Id(1);

			Id.Sum(id1, id2);

			Assert.AreEqual(1, id1.Value);
		}


		[TestMethod]
		public void Sum_LeftAndRightFromSplit_1stValueIs1()
		{
			var id1 = new Id(1);

			Id[] ids = id1.Split();

			Id.Sum(ids[0], ids[1]);

			Assert.AreEqual(1, ids[0].Value);
		}



		[TestMethod]
		public void Sum_LeftAndRightFromSplit_2ndValueIs0()
		{
			var id1 = new Id(1);

			Id[] ids = id1.Split();

			Id.Sum(ids[0], ids[1]);

			Assert.AreEqual(0, ids[1].Value);
		}
	}
}
