using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	public static class PartPhysicsComponentExtension
	{
		public static Part GetOwnerPart(this Collider collider)
		{
			if (collider == null)
				return null;

			return collider.GetComponentInParent<Part>(false, true);
		}
	}

	public class PartPhysicsComponent : PartComponent
	{
		private Collider mCollider = null;
		private Rigidbody mRigidbody = null;

		[SerializeField, HideInInspector]
		private Transform mBoundsRange = null;

		[SerializeField, HideInInspector]
		private bool mIgnorePhysics = false;

		[SerializeField, HideInInspector]
		private Vector3 mExtendSize = Vector3.zero;

		[SerializeField, HideInInspector]
		private bool mUseMeshCollider = false;


		[PreviewMember]
		public new Collider collider
		{
			get
			{
				if (mCollider == null)
					mCollider = GetComponent<Collider>();
				return mCollider;
			}
		}

		[PreviewMember]
		public new Rigidbody rigidbody
		{
			get
			{
				if (mRigidbody == null)
					mRigidbody = GetComponent<Rigidbody>();
				return mRigidbody;
			}
		}

#if UNITY_EDITOR
		[PreviewMember]
		public Vector3 extendSize
		{
			get
			{
				return mExtendSize;
			}
			set
			{
				if (!Misc.Nearly(mExtendSize, value))
				{
					mExtendSize = value;

					PhysicalFeature();
				}
			}
		}

		[PreviewMember]
		public bool useMeshCollider
		{
			get { return mUseMeshCollider; }
			set
			{
				if (mUseMeshCollider != value)
				{
					mUseMeshCollider = value;

					PhysicalFeature();
				}
			}
		}


		[PreviewMember]
		public bool ignorePhysics
		{
			get
			{
				return mIgnorePhysics;
			}
			set
			{
				if (mIgnorePhysics != value)
				{
					mIgnorePhysics = value;

					PhysicalFeature();
				}
			}
		}

		[PreviewMember]
		public Transform boundsRange
		{
			get
			{
				if (mBoundsRange == null)
				{
					return transform;
				}
				return mBoundsRange;
			}
			set
			{
				// Don't set the default value
				if (value == transform)
				{
					value = null;
				}
				if (mBoundsRange != value)
				{
					mBoundsRange = value;

					PhysicalFeature();
				}
			}
		}
#endif

		private void OnTriggerEnter(Collider other)
		{
			var otherPart = other.GetOwnerPart();
			if (otherPart == null || (IPart)otherPart == this.owner)
				return;
			owner.OnPartOverlap(otherPart);
		}

		private void OnTriggerExit(Collider other)
		{
			var otherPart = other.GetOwnerPart();
			if (otherPart == null || (IPart)otherPart == this.owner)
				return;
			owner.OnPartSeparation(otherPart);
		}

#if UNITY_EDITOR
		public void OnClearPhysicsFeature()
		{
			if (collider != null)
			{
				DestroyImmediate(collider, true);
			}
			if (rigidbody != null)
			{
				DestroyImmediate(rigidbody, true);
			}
		}

		[InspectorMethod]
		private void PhysicalFeature()
		{
			UnityEditor.EditorUtility.SetDirty(gameObject);

			if (mIgnorePhysics)
			{
				if (collider != null)
				{
					DestroyImmediate(collider, true);
				}
				if (rigidbody != null)
				{
					DestroyImmediate(rigidbody, true);
				}
				return;
			}

			Rigidbody rb = gameObject.GetOrAllocComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.useGravity = false;

			if (collider != null)
			{
				DestroyImmediate(collider, true);
			}

			if (!useMeshCollider)
			{
				var bounds = this.bounds;
				BoxCollider bc = gameObject.GetOrAllocComponent<BoxCollider>();
				var center = bounds.center;
				if (center.x.NearlyZero())
				{
					center.x = 0.0f;
				}
				if (center.y.NearlyZero())
				{
					center.y = 0.0f;
				}
				if (center.z.NearlyZero())
				{
					center.z = 0.0f;
				}
				bc.center = center;
				bc.size = bounds.size + extendSize;
				bc.isTrigger = true;
			}
			else
			{
				var meshCollider = gameObject.GetOrAllocComponent<MeshCollider>();
				meshCollider.convex = true;
				meshCollider.isTrigger = true;
				if (meshCollider.sharedMesh == null)
				{
					meshCollider.sharedMesh = boundsRange.GetComponentInChildren<MeshFilter>().sharedMesh;
				}
			}
		}

		protected override void DoDrawGizmos()
		{
			if (owner.isKinematicReady)
			{
				var robot = owner.robot;
				if (robot == null)
					return;
				if (collider != null && robot.physicsSystem.ExistMotionCollider(owner))
				{
					GizmosUtility.DrawMeshs(this, true, Color.red);
				}
				return;
			}

			// UnityEditor.PrefabUtility.IsPartOfAnyPrefab(gameObject);
			if (transform.position.NearlyZero())
			{
				var bounds = this.bounds;
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
			}
			else
			{
				//var bounds = this.bounds;
				//Gizmos.color = Color.red;
				////Gizmos.dramesh
				//Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
				//GizmosUtility.DrawBounds(transform, bounds, true, Color.red);
				//GizmosUtility.DrawMeshs(this, true, Color.green);
			}
		}

#endif
	}
}
