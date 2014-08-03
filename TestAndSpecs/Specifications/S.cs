using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace Specifications
{
	/// <summary>
	/// Shortcut to SpecFlow scenario context
	/// </summary>
	public class S
	{
		public static IDictionary<string, object> C
		{
			get { return ScenarioContext.Current; }
		}
	}
}