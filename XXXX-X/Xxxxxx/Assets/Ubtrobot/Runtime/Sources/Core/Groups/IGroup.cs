using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ubtrobot
{
	public interface IGroup : ICommandHandler, IBoundBox
	{
		IRobot robot { get; }
		Transform transform { get; }
		GameObject gameObject { get; }
		List<IPart> parts { get; }
#if UNITY_EDITOR
		void OnInspectorUpdate();
#endif
		void ForceUpdate();
		void AddChildren(List<IPart> parts);
		void SetParent(Transform parent);
		void Rewind();
	}
}
