using UnityEngine;

namespace Ubtrobot
{
	public interface IPart : ICommandHandler, IBoundBox
	{
		string addressName { get; }
		bool isKinematicReady { get; }
		IRobot robot { get; }
		IGroup owner { get; }
		Transform transform { get; }
		GameObject gameObject { get; }
		void Rewind();
		void OnPartOverlap(IPart other);
		void OnPartSeparation(IPart other);

#if UNITY_EDITOR
		void OnInspectorUpdate();
#endif
	}
}
