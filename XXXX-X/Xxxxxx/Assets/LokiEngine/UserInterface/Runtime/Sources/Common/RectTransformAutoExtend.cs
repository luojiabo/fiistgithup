using System;
using UnityEngine;

namespace Loki.UI
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class RectTransformAutoExtend : UComponent
	{
		private RectTransform mRectTransform;

		public bool updateToExtend = false;

		public float extendHeight = 0.0f;

		public RectTransform rectTransform
		{
			get
			{
				if (ReferenceEquals(mRectTransform, null))
				{
					mRectTransform = GetComponent<RectTransform>();
				}
				return mRectTransform;
			}
		}

		public void ExtendHeight()
		{
			rectTransform.ExtendHeight(extendHeight);
		}

		private void Update()
		{
			if (updateToExtend)
			{
				rectTransform.ExtendHeight(extendHeight);
			}
		}
	}
}