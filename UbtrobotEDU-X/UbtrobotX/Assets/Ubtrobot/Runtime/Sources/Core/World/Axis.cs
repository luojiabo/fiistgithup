using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	[Serializable]
	public struct AxisMaterialPropertyBlock
	{
		public Color diffuse;
		public Color specular;
		public float gloss;
	}

	public enum ETRSMode
	{
		None,
		Translation,
		Rotation,
		Scale,
	}

	[DisallowMultipleComponent]
	public abstract class Axis : UComponent
	{
		[SerializeField]
		protected AxisMaterialPropertyBlock mPropertyX;
		[SerializeField]
		protected AxisMaterialPropertyBlock mPropertyY;
		[SerializeField]
		protected AxisMaterialPropertyBlock mPropertyZ;

		[AutoSerializeField]
		public Transform axisX;
		[AutoSerializeField]
		public Transform axisY;
		[AutoSerializeField]
		public Transform axisZ;
		[AutoSerializeField]
		public Transform pivot;

		private Renderer mRendererAxisX;
		private Renderer mRendererAxisY;
		private Renderer mRendererAxisZ;

		private MaterialPropertyBlock mMPBAxisX;
		private MaterialPropertyBlock mMPBAxisY;
		private MaterialPropertyBlock mMPBAxisZ;

		private bool mPropXDirty = true;
		private bool mPropYDirty = true;
		private bool mPropZDirty = true;

		private Space mSpace = Space.Self;

		[PreviewMember]
		public Space space
		{
			get
			{
				return mSpace;
			}
			set
			{
				if (mSpace != value)
				{
					ForceSetSpace(value);
				}
			}
		}

		public Vector3 scaleValue
		{
			get
			{
				if (pivot == null)
					return Vector3.one;
				return pivot.localScale;
			}
			set
			{
				if (pivot == null)
					return;
				pivot.localScale = value;
			}
		}

		public Renderer rendererAxisX
		{
			get
			{
				if (mRendererAxisX == null)
				{
					mRendererAxisX = axisX.GetComponentInChildren<Renderer>();
				}
				return mRendererAxisX;
			}
		}

		public Renderer rendererAxisY
		{
			get
			{
				if (mRendererAxisY == null)
				{
					mRendererAxisY = axisY.GetComponentInChildren<Renderer>();
				}
				return mRendererAxisY;
			}
		}

		public Renderer rendererAxisZ
		{
			get
			{
				if (mRendererAxisZ == null)
				{
					mRendererAxisZ = axisZ.GetComponentInChildren<Renderer>();
				}
				return mRendererAxisZ;
			}
		}

		[InspectorMethod]
		public void ApplyProperties()
		{
			ApplyProperties(true);
		}

		public virtual void ApplyProperties(bool force)
		{
			ApplyProperty(ref mPropXDirty, rendererAxisX, mMPBAxisX, mPropertyX, force);
			ApplyProperty(ref mPropYDirty, rendererAxisY, mMPBAxisY, mPropertyY, force);
			ApplyProperty(ref mPropZDirty, rendererAxisZ, mMPBAxisZ, mPropertyZ, force);
		}

		public void ForceSetSpace(Space value)
		{
			mSpace = value;
			transform.localPosition = Vector3.zero;
			if (mSpace == Space.Self)
			{
				transform.localRotation = Quaternion.identity;
			}
			else
			{
				transform.rotation = Quaternion.identity;
			}
		}

		protected void ApplyProperty(ref bool dirty, Renderer r, MaterialPropertyBlock mpb, AxisMaterialPropertyBlock block, bool force)
		{
			if (dirty || force)
			{
				dirty = false;
				if (r != null)
				{
					r.GetPropertyBlock(mpb);
					mpb.SetColor(ShaderIDs.Diffuse, block.diffuse);
					mpb.SetColor(ShaderIDs.Specular, block.specular);
					mpb.SetFloat(ShaderIDs.Gloss, block.gloss);
					r.SetPropertyBlock(mpb);
				}
			}
		}

		private void Reset()
		{
			mPropertyX.diffuse = new Color(1.0f, 0.0f, 0.0f);
			mPropertyX.specular = new Color(0.5f, 0.0f, 0.0f);
			mPropertyX.gloss = 30.0f;

			mPropertyY.diffuse = new Color(0.0f, 1.0f, 0.0f);
			mPropertyY.specular = new Color(0.0f, 0.5f, 0.0f);
			mPropertyY.gloss = 30.0f;

			mPropertyZ.diffuse = new Color(0.0f, 0.0f, 1.0f);
			mPropertyZ.specular = new Color(0.0f, 0.0f, 0.5f);
			mPropertyZ.gloss = 30.0f;
		}

		protected override void Awake()
		{
			base.Awake();
			mMPBAxisX = new MaterialPropertyBlock();
			mMPBAxisY = new MaterialPropertyBlock();
			mMPBAxisZ = new MaterialPropertyBlock();
		}

#if UNITY_EDITOR
		private void OnInspectorUpdate()
		{
			if (Application.isPlaying)
				ApplyProperties(true);
		}
#endif

		private void Update()
		{
			ApplyProperties();
		}
	}
}
