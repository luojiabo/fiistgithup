using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	/// <summary>
	/// The component is attached off-line, it used for setting this Part
	/// </summary>
	[NameToType]
	public abstract class PartComponent : UComponent, IPartComponent
	{
		[SerializeField]
		protected Transform mPivotLink = null;

		private readonly List<IConnectivity> mConnectivities = new List<IConnectivity>();
		private Bounds mBounds;
		private bool mBoundsIsDirty = true;
		private Part mOwnerPart = null;

		protected bool mDebug = false;

		[PreviewMember]
		public virtual bool debug
		{
			get
			{
				return mDebug;
			}
			set
			{
				if (mDebug != value)
				{
					mDebug = value;
					SetDebug(value);
				}
			}
		}

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

		public List<IConnectivity> connectivities
		{
			get
			{
				return mConnectivities;
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

		public IPart owner
		{
			get
			{
				return GetPart();
			}
		}

		public Part GetPart()
		{
			if (mOwnerPart == null)
			{
				mOwnerPart = this.GetComponentInParent<Part>(true);
			}
			return mOwnerPart;
		}

		protected virtual void SetDebug(bool flag)
		{
			mDebug = flag;
			var debugTransform = GetDebugTransform();
			if (debugTransform != null)
			{
				debugTransform.gameObject.SetActive(flag);
			}
		}

		protected virtual Transform GetDebugTransform()
		{
			var part = GetPart();
			if (part != null)
			{
				return part.FindRecursion("Debug");
			}
			return transform.FindRecursion("Debug");
		}

		private void Reset()
		{
			GetPart();
		}

		public void MakeBoundsDirty()
		{
			mBoundsIsDirty = true;
		}

		public IConnectivity GetBestConnectivity(IConnectivity other)
		{
			return null;
		}

		public virtual void Rewind()
		{

		}

		public void ForceUpdate()
		{
			mConnectivities.Clear();
			transform.GetComponentsInChildren<Connectivity, PartComponent, IConnectivity>(true, mConnectivities);
		}

		protected override void Awake()
		{
			base.Awake();
			SetDebug(false);
			ForceUpdate();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

#if UNITY_EDITOR
		protected void OnDrawGizmos()
		{
			GizmosColorUtility.NewStack();
			EditorHelper.OnSceneGUIDrawTitle(this);
			DoDrawGizmos();
			GizmosColorUtility.Revert();
		}

		protected virtual void DoDrawGizmos()
		{

		}

		protected virtual void OnEditorUpdate()
		{

		}

		public virtual void OnInspectorUpdate()
		{
			ForceUpdate();

			for (var i = mConnectivities.Count - 1; i >= 0; --i)
			{
				if (mConnectivities[i] != null)
				{
					mConnectivities[i].OnInspectorUpdate();
				}
			}
		}
#endif
	}
}
