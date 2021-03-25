using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public interface IActorAttachment : IEqualityComparer<IActorAttachment>
	{
		string jointName { get; }
		string name { get; }
		Actor owner { get; }
		void OnAttach(Actor owner, object context, ActorAttachArgs args);
		void OnDetach(object context);
	}

	public struct ActorAttachArgs
	{
		public static readonly ActorAttachArgs Default;

		static ActorAttachArgs()
		{
			Default = new ActorAttachArgs
			{
				resetLocalPosition = true,
				resetLocalRotation = true,
				resetLocalScale = true
			};
		}

		public bool resetLocalPosition;
		public bool resetLocalRotation;
		public bool resetLocalScale;

	}

	/// <summary>
	/// Do not use JointInfo() / default(JointInfo)
	/// </summary>
	public struct JointInfo
	{
		private readonly Transform mJoint;
		private readonly List<IActorAttachment> mAttachments;

		public Transform joint
		{
			get
			{
				return mJoint;
			}
		}

		public List<IActorAttachment> attachments
		{
			get
			{
				return mAttachments;
			}
		}

		public int attachmentCount
		{
			get
			{
				return attachments.Count;
			}
		}

		public IActorAttachment attachment
		{
			get
			{
				if (attachments.Count > 0)
					return attachments[0];
				return null;
			}
		}

		public static implicit operator Transform(JointInfo info)
		{
			return info.joint;
		}

		public JointInfo(Transform t)
		{
			mJoint = t;
			mAttachments = new List<IActorAttachment>();
		}
	}

	public partial class Actor
	{
		private readonly Dictionary<string, JointInfo> mCacheJoints = new Dictionary<string, JointInfo>();

		public bool Attach(IActorAttachment attachment)
		{
			return Attach(attachment, null, ActorAttachArgs.Default);
		}

		public bool Attach(IActorAttachment attachment, object context)
		{
			return Attach(attachment, context, ActorAttachArgs.Default);
		}

		public bool Attach(IActorAttachment attachment, object context, ActorAttachArgs args)
		{
			DebugUtility.AssertFormat(attachment.owner == null, "The owner of attachment ({0}) must be null (Actor : [{1}])", attachment.name, name);
			if (FindJoint(attachment.jointName, true, out var info))
			{
				info.attachments.Add(attachment);
				attachment.OnAttach(this, context, args);
				return true;
			}
			return false;
		}

		public bool Detach(IActorAttachment attachment, object context)
		{
			DebugUtility.AssertFormat(attachment.owner == this, "The owner of attachment ({0}) must be this Actor : [{1}]", attachment.name, name);
			attachment.OnDetach(context);
			if (FindJoint(attachment.jointName, false, out var info))
			{
				info.attachments.Remove(attachment);
				return true;
			}

			DebugUtility.LogErrorTrace("Missing joint : [{0}], Actor : [{1}], Attachment : {2}", attachment.jointName, name, attachment.name);
			return false;
		}

		public void DetachAll(string jointName, object context)
		{
			if (FindJoint(jointName, false, out var info))
			{
				foreach (var attachment in info.attachments)
				{
					attachment.OnDetach(context);
				}
				info.attachments.Clear();
			}
		}

		public IActorAttachment FindAttachment(string jointName)
		{
			if (FindJoint(jointName, false, out var info))
			{
				return info.attachment;
			}
			return null;
		}

		public IActorAttachment FindAttachment(string jointName, Predicate<IActorAttachment> predicate)
		{
			if (FindJoint(jointName, false, out var info))
			{
				foreach (var attachment in info.attachments)
				{
					if (predicate(attachment))
					{
						return attachment;
					}
				}
			}
			return null;
		}

		public int FindAttachments(string jointName, out List<IActorAttachment> result)
		{
			if (FindJoint(jointName, false, out var info))
			{
				result = info.attachments;
				return info.attachmentCount;
			}
			result = null;
			return 0;
		}

		public bool ExistsAttachment(string jointName)
		{
			if (FindJoint(jointName, false, out var info))
			{
				return (info.attachmentCount > 0);
			}
			return false;
		}

		public bool IsRootJoint(Transform joint)
		{
			return this == joint;
		}

		public bool FindJoint(string jointName, bool autoAdd, out JointInfo info)
		{
			if (mCacheJoints.TryGetValue(jointName, out info))
			{
				if (info.joint == null)
				{
					DebugUtility.LogErrorTrace(LoggerTags.Engine, "The Joint info [{0}] is missing, please dont remove the joint of [{1}] before clear the JointInfo.", jointName, name);
				}
				return true;
			}

			if (autoAdd)
			{
				Transform joint = transform.FindUnique(jointName, StringComparison.Ordinal);
				if (joint != null)
				{
					info = new JointInfo(joint);
					mCacheJoints[jointName] = info;
					return true;
				}
				DebugUtility.LogErrorTrace(LoggerTags.Engine, "The Joint [{0}] is missing, please check the children : {1}", jointName, name);
			}
			return false;
		}
	}
}
