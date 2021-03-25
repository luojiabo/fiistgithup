using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Loki
{
	public partial class AssetManager
	{
		public static ResourceRequest LoadFromResourcesAsync<TObject>(string path) where TObject : UnityObject
		{
			return Resources.LoadAsync<TObject>(path);
		}

		public static void UnloadAsset(UnityObject o)
		{
			if (o == null)
				return;

			if (o is GameObject)
				return;

			if (o is Component)
				return;

			Resources.UnloadAsset(o);
		}
	}
}
