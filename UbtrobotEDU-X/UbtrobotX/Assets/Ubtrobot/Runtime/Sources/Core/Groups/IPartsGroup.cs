using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Loki;

using UnityEngine;

namespace Ubtrobot
{
	public interface IPartsGroup : IGroup
	{
		IRobot owner { get; }
	}
}
