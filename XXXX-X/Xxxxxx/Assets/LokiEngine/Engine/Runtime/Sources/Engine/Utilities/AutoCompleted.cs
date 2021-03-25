using System;
using System.Collections.Generic;
using System.Linq;

namespace Loki
{
	public class AutoCompleted<TValue>
	{
		private readonly IList<TValue> mDict;

		public AutoCompleted(IList<TValue> dict)
		{
			mDict = dict;
		}

	}
}
