using System;
using System.Collections.Generic;

namespace Ubtrobot
{
	public interface IConnectivity : IPhysical
	{
		EPhysicalType connType { get; }
#if UNITY_EDITOR
		void OnInspectorUpdate();
#endif
	}
}
