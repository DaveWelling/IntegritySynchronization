using IntervalTreeClocksCSharp;

namespace Sync
{
	public static class ChangeEvaluator
    {
	    public static ChangeOccurredIn GetWhichChanged(Stamp first, Stamp second)
	    {
		    bool firstLeqSecond = first.Leq(second);
		    bool secondLeqFirst = second.Leq(first);

			if (firstLeqSecond && secondLeqFirst)
		    {
			    return ChangeOccurredIn.Neither;
		    }
			if (firstLeqSecond)
		    {
			    return ChangeOccurredIn.SecondObject;
		    }
			if (secondLeqFirst)
		    {
			    return ChangeOccurredIn.FirstObject;
		    }
			return ChangeOccurredIn.Both;
	    }

    }
}
