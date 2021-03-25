using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Ubtrobot
{
	[NameToType]
	public abstract class Group : Actor, IGroup
	{
		private readonly List<IPart> mParts = new List<IPart>();

		private Bounds mBounds;
		private bool mBoundsIsDirty = true;
		private Transform mOriginParent = null;
		private Transform mLastParent = null;
		private bool mRewinding = false;

		private Vector3 mStartPosition;
		private Quaternion mStartRotation;

		private bool mTransformRecorded = false;

		[SerializeField]
		protected Group[] mControlTargets;

		public AxisType upAxis = AxisType.Y;

		public bool inverseAxis = false;
		private bool mStopping = false;

		protected override bool initializeOnAwake => false;

		public Group[] controlTargets
		{
			get
			{
				return mControlTargets;
			}
			set
			{
				mControlTargets = value;
			}
		}

		public List<IPart> parts
		{
			get
			{
				return mParts;
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

		[PreviewMember]
		public ECommandType commandType { get; set; } = ECommandType.Unicast;

		public abstract IRobot robot { get; }

		protected override void OnInitialize()
		{
			base.OnInitialize();
			ForceUpdate();
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "robot.rewind")]
		public void Rewind()
		{
			if (mRewinding)
				return;
			mRewinding = true;

			foreach (var part in parts)
			{
				part.Rewind();
			}
			transform.SetPositionAndRotation(mStartPosition, mStartRotation);
			mRewinding = false;
		}

		public void SetParent(Transform parent)
		{
			DebugUtility.Assert(parent != transform, "The transform can't set to the parent");

			mLastParent = transform.parent;
			transform.SetParent(parent, true);
		}

		private int InverseToSign()
		{
			return inverseAxis ? -1 : 1;
		}

		public void Rotate(float y)
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

		public void MakeBoundsDirty()
		{
			mBoundsIsDirty = true;
		}

		public virtual void ForceUpdate()
		{
			if (!mTransformRecorded)
			{
				mTransformRecorded = true;
				mOriginParent = transform.parent;
				mStartPosition = transform.localPosition;
				mStartRotation = transform.localRotation;
			}

			if (mControlTargets != null)
			{
				foreach (var group in mControlTargets)
				{
					group.SetParent(transform);
				}
			}

			mParts.Clear();
			transform.GetComponentsInChildren<Part, Group, IPart>(true, mParts);
			foreach (Part p in mParts)
			{
				p.Initialize();
			}

			//DebugUtility.AssertFormat(!mGroups.Contains(this), "This Group is in the result");
		}

		public virtual void AddChildren(List<IPart> parts)
		{
			foreach (var p in parts)
			{
				p.transform.SetParent(transform, true);
			}
		}

		public virtual bool Verify(ICommand command)
		{
			if (!enabled)
				return false;
			return (parts.Count > 0) && (commandType != ECommandType.Disable);
		}

		public virtual ICommandResponseAsync Execute(ICommand command)
		{
			bool enableBroadcast = commandType == ECommandType.Broadcast;
			bool exitResponse = false;
			ICommandResponseAsync job = null;
			for (var i = 0; i < parts.Count; ++i)
			{
				var h = parts[i];
				if (h.Verify(command))
				{
					DebugUtility.Log(LoggerTags.Online, "Try to execute command in part  {0} - {1}", h.gameObject.name, command);
					var result = h.Execute(command);
					if (result != null)
					{
						if (job == null)
						{
							job = new CommandResponseAsync();
						}
						job.AddResponse(result);
						exitResponse = true;
						// 不是广播模式，立即停止
						if (!enableBroadcast)
						{
							break;
						}
					}
				}
				else
				{
					// DebugUtility.Log(LoggerTags.Online, "Fail to verify command : {0} - {1}", h.gameObject.name, command);
				}
			}

			return job;
		}

		public void Stop()
		{
			if (mStopping) return;
			mStopping = true;
			OnStop();
			mStopping = false;
		}

		protected virtual void OnStop()
		{

		}

#if UNITY_EDITOR
		protected virtual void OnEditorUpdate()
		{

		}

		public virtual void OnInspectorUpdate()
		{
			if (Application.isPlaying)
				return;

			ForceUpdate();

			for (var i = mParts.Count - 1; i >= 0; --i)
			{
				if (mParts[i] != null)
				{
					mParts[i].OnInspectorUpdate();
				}
			}
		}

		private void OnDrawGizmos()
		{
			GizmosColorUtility.NewStack();
			DoDrawGizmos();
			GizmosColorUtility.Revert();
		}

		protected virtual void DoDrawGizmos()
		{
			//Gizmos.color 
		}
#endif
	}
}
