using System;
using System.Collections.Generic;

using Loki;

using UnityEngine;

namespace Ubtrobot
{
	[NameToType]
	public abstract class JointComponent : UComponent
	{
		[SerializeField, HideInInspector]
		protected Group mControlTarget = null;

		[SerializeField, HideInInspector]
		private Connectivity mController = null;

		[PreviewMember]
		public Group controlTarget
		{
			get
			{
				return mControlTarget;
			}
			set
			{
				if (mControlTarget != value)
				{
					if (mControlTarget != null)
					{
						mControlTarget.name = "Group";
					}
					mControlTarget = value;
				}
			}
		}

		[PreviewMember]
		public Connectivity controller
		{
			get { return mController; }
			set { mController = value; }
		}

		[PreviewMember]
		public Part controlPart
		{
			get
			{
				if (controller == null)
				{
					return null;
				}
				return controller.GetComponent<Part>() ?? controller.GetComponentInParent<Part>();
			}
		}

#if UNITY_EDITOR
		[LokiTooltip("让controlTarget的Pivot对准Axis(当前选中)")]
		[InspectorMethod]
		public void ResetControlTarget()
		{
			if (mControlTarget != null)
			{
				mControlTarget.transform.ExeIgnoreChildren(tr =>
				{
					tr.CopyFrom(transform, Space.World);
				});
			}
		}

		public void OnInspectorUpdate()
		{
			if (mControlTarget != null && controller != null && controller.controller != null)
			{
				if (controller.controller is IPartIDComponent)
				{
					var partIDCom = (IPartIDComponent)controller.controller;
					mControlTarget.name = string.Concat("Group_", partIDCom.id.ToString());
				}
			}
		}
#endif
	}
}
