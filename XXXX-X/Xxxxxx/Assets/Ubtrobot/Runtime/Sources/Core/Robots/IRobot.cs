using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	public interface IRobotPhysicsSystem
	{
		IEnumerator overlaps { get; }
		IEnumerator GetOverlaps(IPart part);
		bool AddCollider(IPart part, IPart other);
		bool ExistCollider(IPart part, IPart other);
		bool AddMotionCollider(IPart part, IPart other);
		bool RemoveMotionCollider(IPart part, IPart other);
		bool ExistMotionCollider(IPart part, IPart other);
		bool ExistMotionCollider(IPart part);
	}

	public interface IRobot : ICommandHandler, IGroup
	{
		string nameOnUI { get; set; }
		bool isKinematicReady { get; }
		IRobotPhysicsSystem physicsSystem { get; }
		List<PartComponent> GetCommandComponents(List<PartComponent> resultCache);
		List<IPartIDComponent> GetIDComponents(List<IPartIDComponent> resultCache);
		void AutoInitialize();
		void Initialize();
		void OnSelected();
		void OnUnselected();
		void CheckCommands();
	}
}
