using UnityEngine;
using System;
using System.Collections.Generic;

namespace Loki
{
	public abstract class UObjectUtility : UObjectBase
	{
		public long GetRuntimeMemorySizeLong()
		{
			return ProfilingUtility.GetRuntimeMemorySizeLong(this);
		}
	}
}
