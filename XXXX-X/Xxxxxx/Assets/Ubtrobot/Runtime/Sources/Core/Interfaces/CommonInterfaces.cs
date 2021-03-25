using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ubtrobot
{
	public interface IBoundBox
	{
		Bounds bounds { get; }
		void MakeBoundsDirty();
	}
}
