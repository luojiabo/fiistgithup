using System;
using System.Collections.Generic;
using Loki;

namespace Ubtrobot
{
	public enum EInputState
	{
		None,
		Released,
		Click,
		DoubleClick,
		LongPress,
	}

	public class PartInputEventData : InputEventData
	{
		public EInputState state { get; set; }
	}
}
