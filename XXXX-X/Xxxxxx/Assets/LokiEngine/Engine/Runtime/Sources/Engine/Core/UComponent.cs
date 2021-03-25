using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Loki
{
	[NameToType]
	public abstract class UComponent : UObject
	{
		protected override void Awake()
		{
			base.Awake();

		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

		}

	}
}
