using IntervalTreeClocksCSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync;

namespace Sychronization.Unit.Tests
{
	[TestClass]
	public class ChangeEvaluatorTests
	{
		[TestMethod]
		public void GetWhichChanged_FirstHasOnlyEvent_FirstChanged()
		{
			Stamp first = new Stamp();
			Stamp second = first.Fork();	
			first.CreateEvent();


			Assert.AreEqual(ChangeOccurredIn.FirstObject, ChangeEvaluator.GetWhichChanged(first, second));
		}

		[TestMethod]
		public void GetWhichChanged_NeitherHasEvent_Neither()
		{
			Stamp first = new Stamp();
			Stamp second = first.Fork();
											   
			Assert.AreEqual(ChangeOccurredIn.Neither, ChangeEvaluator.GetWhichChanged(first, second));	
		}

		[TestMethod]
		public void GetWhichChanged_SecondHasEvent_SecondChanged()
		{
			Stamp first = new Stamp();
			Stamp second = first.Fork();

			second.CreateEvent();
											 
			Assert.AreEqual(ChangeOccurredIn.SecondObject, ChangeEvaluator.GetWhichChanged(first, second));
		}


		[TestMethod]
		public void GetWhichChanged_BothHaveEvent_Both()
		{
			Stamp first = new Stamp();
			Stamp second = first.Fork();
									  
			first.CreateEvent();
			second.CreateEvent();
												
			Assert.AreEqual(ChangeOccurredIn.Both, ChangeEvaluator.GetWhichChanged(first, second));
		}

		[TestMethod]
		public void GetWhichChanged_FirstHasEventThenJoinedToSecond_Neither()
		{
			Stamp first = new Stamp();
			Stamp second = first.Fork();

			first.CreateEvent();
			second.Join(first);
									   
			Assert.AreEqual(ChangeOccurredIn.Neither, ChangeEvaluator.GetWhichChanged(first, second));
		}

		[TestMethod]
		public void GetWhichChanged_BothChangedButJoinedBetweenEvents_Neither()
		{
			Stamp first = new Stamp();
			Stamp second = first.Fork();

			first.CreateEvent();
			second.Join(first.Peek());
			second.CreateEvent();
			first.Join(second.Peek());
											
			Assert.AreEqual(ChangeOccurredIn.Neither, ChangeEvaluator.GetWhichChanged(first, second));
		}


		[TestMethod]
		public void GetWhichChanged_FirstHasTwoEvents_FirstChanged()
		{
			Stamp first = new Stamp();
			Stamp second = first.Fork();

			first.CreateEvent();
			first.CreateEvent();

			Assert.AreEqual(ChangeOccurredIn.FirstObject, ChangeEvaluator.GetWhichChanged(first, second));
		}
	}
}
