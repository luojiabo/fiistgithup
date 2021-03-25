using System;
using System.Collections.Generic;
using Loki;

using UnityEngine;

namespace Ubtrobot
{
	[NameToType]
	public abstract class Connectivity : UComponent, IConnectivity
	{
		[SerializeField]
		protected Transform mPivotLink = null;

		private Bounds mBounds;
		private bool mBoundsIsDirty = true;
		private PartComponent mController = null;

		public AxisType upAxis = AxisType.Y;

		public bool inverseAxis = false;

		public Transform pivotLink
		{
			get
			{
				if (mPivotLink != null)
				{
					return transform;
				}
				return mPivotLink;
			}
			set
			{
				mPivotLink = value;
			}
		}

		public abstract EPhysicalType connType { get; }

		[PreviewMember]
		public PartComponent controller
		{
			get
			{
				if (mController == null)
				{
					mController = GetComponent<PartComponent>() ?? GetComponentInParent<PartComponent>();
				}
				return mController;
			}
		}

		public Bounds bounds
		{
			get
			{
				if (mBoundsIsDirty)
				{
					mBounds = this.transform.CalcRenerersBounds();
					mBoundsIsDirty = false;
				}
				return mBounds;
			}
		}

		public void MakeBoundsDirty()
		{
			mBoundsIsDirty = true;
		}

		private int InverseToSign()
		{
			return inverseAxis ? -1 : 1;
		}

		public void RotateAxis(float y)
		{
			switch (upAxis)
			{
				case AxisType.Y:
					{
						transform.Rotate(Vector3.up * InverseToSign(), y);
						break;
					}
				case AxisType.X:
					{
						transform.Rotate(Vector3.right * InverseToSign(), y);
						break;
					}
				case AxisType.Z:
					{
						transform.Rotate(Vector3.forward * InverseToSign(), y);
						break;
					}
			}
		}


#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			EditorHelper.OnSceneGUIDrawTitle(this);
		}

		private void OnDrawGizmosSelected()
		{
			//GizmosUtility.DrawMeshs(this, true, Color.red, false);
		}

		[InspectorMethod]
		public void SetAsPivot()
		{
			Group group = GetComponentInParent<Group>();
			List<Transform> children = new List<Transform>();
			for (int i = 0; i < group.transform.childCount; ++i)
			{
				children.Add(group.transform.GetChild(i));
			}
			group.transform.DetachChildren();
			group.transform.CopyFrom(transform, Space.World);
			foreach (var tr in children)
			{
				tr.transform.SetParent(group.transform, true);
			}
		}

		public virtual void OnInspectorUpdate()
		{
		}
#endif
	}
}
