using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	[DisallowMultipleComponent]
	public class AxisTranslation : Axis
	{
		[AutoSerializeField]
		public Transform mPlaneXY;
		[AutoSerializeField]
		public Transform mPlaneXZ;
		[AutoSerializeField]
		public Transform mPlaneYZ;

		[Range(0.0f, 1.0f)]
		public float mPlaneAlpha = 0.2f;

		private Renderer mRendererAxisXY;
		private Renderer mRendererAxisXZ;
		private Renderer mRendererAxisYZ;

		private MaterialPropertyBlock mMPBXY;
		private MaterialPropertyBlock mMPBXZ;
		private MaterialPropertyBlock mMPBYZ;

		private bool mPropXYDirty = true;
		private bool mPropXZDirty = true;
		private bool mPropYZDirty = true;

		public Renderer rendererAxisXY
		{
			get
			{
				if (mRendererAxisXY == null)
				{
					mRendererAxisXY = mPlaneXY.GetComponent<Renderer>();
				}
				return mRendererAxisXY;
			}
		}

		public Renderer rendererAxisXZ
		{
			get
			{
				if (mRendererAxisXZ == null)
				{
					mRendererAxisXZ = mPlaneXZ.GetComponent<Renderer>();
				}
				return mRendererAxisXZ;
			}
		}

		public Renderer rendererAxisYZ
		{
			get
			{
				if (mRendererAxisYZ == null)
				{
					mRendererAxisYZ = mPlaneYZ.GetComponent<Renderer>();
				}
				return mRendererAxisYZ;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			mMPBXY = new MaterialPropertyBlock();
			mMPBXZ = new MaterialPropertyBlock();
			mMPBYZ = new MaterialPropertyBlock();
		}

		public override void ApplyProperties(bool force)
		{
			base.ApplyProperties(force);

			ApplyProperty(ref mPropXYDirty, rendererAxisXY, mMPBXY, mPropertyZ.diffuse, force);
			ApplyProperty(ref mPropXZDirty, rendererAxisXZ, mMPBXZ, mPropertyY.diffuse, force);
			ApplyProperty(ref mPropYZDirty, rendererAxisYZ, mMPBYZ, mPropertyX.diffuse, force);
		}

		protected void ApplyProperty(ref bool dirty, Renderer r, MaterialPropertyBlock mpb, Color diffuse, bool force)
		{
			if (dirty || force)
			{
				dirty = false;
				if (r != null)
				{
					r.GetPropertyBlock(mpb);
					diffuse.a = mPlaneAlpha;
					mpb.SetColor(ShaderIDs.Diffuse, diffuse);
					r.SetPropertyBlock(mpb);
				}
			}
		}

	}
}
