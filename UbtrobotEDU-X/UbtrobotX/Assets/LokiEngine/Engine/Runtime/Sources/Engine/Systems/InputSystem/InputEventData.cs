using System;
using System.Collections.Generic;

namespace Loki
{
	public abstract class InputEventData
	{
		protected bool mUsed;

		public virtual bool used { get { return mUsed; } }

		public virtual void Reset()
		{
			mUsed = false;
		}

		public virtual void Use()
		{
			mUsed = true;
		}
	}
}
