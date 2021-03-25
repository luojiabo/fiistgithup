using UnityEngine;
using System.Collections;

namespace Ubtrobot
{
	public class FixedConnectivity : Connectivity
	{
		public override EPhysicalType connType
		{
			get { return EPhysicalType.Fixed; }
		}
	}
}
