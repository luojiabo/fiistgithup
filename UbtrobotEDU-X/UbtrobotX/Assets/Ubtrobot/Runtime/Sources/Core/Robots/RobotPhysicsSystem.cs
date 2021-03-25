using System.Collections;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class RobotPhysicsSystem : IRobotPhysicsSystem
	{
		private readonly Dictionary<IPart, HashSet<IPart>> mAllowOverlapRecords = new Dictionary<IPart, HashSet<IPart>>();

		private readonly Dictionary<IPart, HashSet<IPart>> mMotionOverlapRecords = new Dictionary<IPart, HashSet<IPart>>();

		public IEnumerator overlaps { get { return mMotionOverlapRecords.Keys.GetEnumerator(); } }

		public IEnumerator GetOverlaps(IPart part)
		{
			if (!mMotionOverlapRecords.TryGetValue(part, out var set))
			{
				return null;
			}
			return set.GetEnumerator();
		}

		public bool AddCollider(IPart part, IPart other)
		{
			if (!mAllowOverlapRecords.TryGetValue(part, out var set))
			{
				set = new HashSet<IPart>();
				mAllowOverlapRecords.Add(part, set);
			}
			if (set.Add(other))
			{
				DebugUtility.Log(LoggerTags.Project, "Add Collider : {0}, {1}, Frame ({2})", ((Part)part).GetComponentInfo(), ((Part)other).GetComponentInfo(), Time.frameCount);
				return true;
			}
			return false;
		}

		public bool ExistCollider(IPart part, IPart other)
		{
			if (mAllowOverlapRecords.TryGetValue(part, out var set))
			{
				return set.Contains(other);
			}
			return false;
		}

		public bool AddMotionCollider(IPart part, IPart other)
		{
			if (ExistCollider(part, other))
			{
				return false;
			}

			DebugUtility.Log(LoggerTags.Project, "Add Motion Collider : {0}, {1}, Frame ({2})", ((Part)part).GetComponentInfo(), ((Part)other).GetComponentInfo(), Time.frameCount);
			if (!mMotionOverlapRecords.TryGetValue(part, out var set))
			{
				set = new HashSet<IPart>();
				mMotionOverlapRecords.Add(part, set);
			}
			if (set.Add(other))
			{
				return true;
			}
			return false;
		}

		public bool RemoveMotionCollider(IPart part, IPart other)
		{
			DebugUtility.Log(LoggerTags.Project, "Remove Motion Collider : {0}, {1}, Frame ({2})", ((Part)part).GetComponentInfo(), ((Part)other).GetComponentInfo(), Time.frameCount);
			if (!mMotionOverlapRecords.TryGetValue(part, out var set))
			{
				return false;
			}
			if (set.Remove(other))
			{
				return true;
			}
			return false;
		}

		public bool ExistMotionCollider(IPart part, IPart other)
		{
			if (mMotionOverlapRecords.TryGetValue(part, out var set))
			{
				return set.Contains(other);
			}
			return false;
		}

		public bool ExistMotionCollider(IPart part)
		{
			if (mMotionOverlapRecords.TryGetValue(part, out var set))
			{
				return set.Count > 0;
			}
			return false;
		}
	}
}
