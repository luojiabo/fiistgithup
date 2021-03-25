using System;
using System.Collections;
using UnityEngine;

namespace Loki.UI
{
	public partial class WindowManager
	{
		private static bool UnloadWindow(Window window)
		{
			if (window == null) return false;
			Destroy(window.gameObject);
			return true;
		}

		private static IEnumerator LoadPrefab(string prefabName, Action<GameObject> onLoaded)
		{
			var req = AssetManager.LoadFromResourcesAsync<GameObject>(prefabName);
			yield return req;
			if (req.asset != null)
			{
				var res = Instantiate(req.asset as GameObject);
				res.name = prefabName;
				Misc.SafeInvoke(onLoaded, res);
			}
			else
			{
				Misc.SafeInvoke(onLoaded, null);
			}
		}

		private static IEnumerator LoadWindow<T>(string windowName, Action<GameObject, T> onLoaded) where T : Window
		{
			var req = AssetManager.LoadFromResourcesAsync<GameObject>(windowName);
			yield return req;
			if (req.asset != null)
			{
				var res = Instantiate(req.asset as GameObject);
				res.name = windowName;
				var window = res.GetComponent<T>();
				Misc.SafeInvoke(onLoaded, res, window);
			}
			else
			{
				Misc.SafeInvoke(onLoaded, null, null);
			}
		}
	}
}
