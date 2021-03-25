using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	/// <summary>
	/// Use DefaultFollowCameraController by attach component to GameObject
	/// </summary>
	public class DefaultFollowCameraController : CameraController, IUpdatable
	{
		public enum EFollowMode
		{
			Default,
			Focus,
		}

		[SerializeField]
		private EFollowMode mFollowMode = EFollowMode.Default;

		[SerializeField]
		private LayerMask mWallLayerMask;

		[SerializeField]
		private Transform mFollowTarget = null;
		[SerializeField, HideInInspector]
		private Vector3 mFollowTargetOffset = new Vector3(0.0f, -3.0f, -5.0f);

		[SerializeField]
		private Transform mMovementRoot = null;
		[SerializeField]
		private Transform mRotationRoot = null;

		private Vector3 mStartPosition;
		private Quaternion mStartRotation;

#if UNITY_EDITOR
		[PreviewMember]
		public Color gizmosColor { get; set; } = Color.yellow;
#endif

		public override Transform viewTarget
		{
			get
			{
				return mFollowTarget;
			}
		}

		[PreviewMember]
		public Vector3 followTargetOffset
		{
			get
			{
				return mFollowTargetOffset;
			}
			set
			{
				if (mFollowTargetOffset != value)
				{
					mFollowTargetOffset = value;
					UpdateFollowTarget();
				}
			}
		}

		[PreviewMember]
		public Transform movementRoot
		{
			get
			{
				return mMovementRoot != null ? mMovementRoot : this.transform;
			}
			set
			{
				mMovementRoot = value;
			}
		}

		[PreviewMember]
		public Transform rotationRoot
		{
			get
			{
				return mRotationRoot != null ? mRotationRoot : this.transform;
			}
			set
			{
				mRotationRoot = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			mStartPosition = transform.position;
			mStartRotation = transform.rotation;

			if (mFollowTarget != null)
			{
				mFollowTargetOffset = transform.position - mFollowTarget.position;
			}
		}

		public void UpdateFollowTarget()
		{
			if (mFollowTarget != null)
			{
				if (mFollowMode == EFollowMode.Default)
				{
					//transform.localPosition = mFollowTargetOffset;
				}
				else
				{
					transform.position = mFollowTarget.position + mFollowTargetOffset;
					transform.LookAt(mFollowTarget, Vector3.up);
				}

				if (Physics.Raycast(mFollowTarget.position, transform.position - mFollowTarget.position, out var hitInfo, mFollowTargetOffset.magnitude, mWallLayerMask.value))
				{
					transform.position = hitInfo.point;
					transform.LookAt(mFollowTarget, Vector3.up);
				}
			}
		}

		public void OnUpdate(float deltaTime)
		{
			UpdateFollowTarget();
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Color tempColor = Gizmos.color;
			Matrix4x4 tempMat = Gizmos.matrix;
			Camera c = controlled;
			Gizmos.color = gizmosColor;

			if (c.orthographic)
			{
				var size = c.orthographicSize;
				Gizmos.DrawWireCube(Vector3.forward * (c.nearClipPlane + (c.farClipPlane - c.nearClipPlane) / 2)
					, new Vector3(size * 2.0f, size * 2.0f * c.aspect, c.farClipPlane - c.nearClipPlane));
			}
			else
			{
				Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
				Gizmos.DrawFrustum(Vector3.zero, c.fieldOfView, c.farClipPlane, c.nearClipPlane, c.aspect);
			}

			Gizmos.color = tempColor;
			Gizmos.matrix = tempMat;
		}
#endif
	}
}
