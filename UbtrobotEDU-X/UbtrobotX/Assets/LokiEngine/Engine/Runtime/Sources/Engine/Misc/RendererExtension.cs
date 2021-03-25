using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public static class RendererExtension
	{
		public static Material[] GetRuntimeMaterials(this Renderer r)
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
				return r.materials;
			else
				return r.sharedMaterials;
#else
			return r.sharedMaterials;
#endif
		}


		public static void SetRuntimeMaterials(this Renderer r, Material[] mats)
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
				 r.materials = mats;
			else
				 r.sharedMaterials = mats;
#else
			r.sharedMaterials = mats;
#endif
		}
	}
}
