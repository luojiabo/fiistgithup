using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Loki
{
	public class PreloadManager : USingletonObject<PreloadManager>
	{
		private static readonly string msPreloadSettings = "PreloadSettings";

		private readonly Dictionary<string, Preload> mPreloads = new Dictionary<string, Preload>();

		public override string immortalName { get { return "Preload"; } }

		public override ELifetime lifetime { get { return ELifetime.App; } }

		public IEnumerator Initialize()
		{
			yield return StartCoroutine(GetPreloadAsync(msPreloadSettings));

#if UNITY_EDITOR
			DebugUtility.Log(LoggerTags.AssetManager, "PreloadManager initialized. {0}", ToString());
#else
			DebugUtility.Log(LoggerTags.AssetManager, "PreloadManager initialized.");
#endif
		}

		public Preload GetPreload()
		{
			return GetPreload(msPreloadSettings);
		}

		public Preload GetPreload(string preloadAddress)
		{
			if (string.IsNullOrEmpty(preloadAddress))
			{
				preloadAddress = msPreloadSettings;
			}

			if (mPreloads.TryGetValue(preloadAddress, out var preload))
			{
				return preload;
			}

			return null;
		}

		public IEnumerator GetPreloadAsync(string preloadAddress)
		{
			if (string.IsNullOrEmpty(preloadAddress))
			{
				preloadAddress = msPreloadSettings;
			}

			if (mPreloads.TryGetValue(preloadAddress, out var preload))
			{
				yield break;
			}

			var handle = AssetManager.LoadFromResourcesAsync<PreloadSettings>(preloadAddress);
			yield return handle;
			if (handle.asset is PreloadSettings result)
			{
				preload = new Preload(preloadAddress, result);
			}
			if (preload != null)
			{
				yield return StartCoroutine(preload.Initialize());
				mPreloads[preloadAddress] = preload;
			}
		}

		public T GetObject<T>(string preloadAddress, string address) where T : UAssetObject
		{
			var preload = GetPreload(preloadAddress);
			return preload.GetObject<T>(address);
		}

		public T GetObject<T>(string address) where T : UAssetObject
		{
			var preload = GetPreload(msPreloadSettings);
			return preload.GetObject<T>(address);
		}

		[ContextMenu("PrintCurrentLoaded")]
		public void PrintCurrentLoaded()
		{
			DebugUtility.Log(LoggerTags.AssetManager, ToString());
		}

		public override string ToString()
		{
			var sb = new StringBuilder(512);
			sb.AppendLine("PreloadManager: ");
			foreach (var item in mPreloads.Values)
			{
				sb.AppendLine(item.ToString());
			}
			sb.AppendLine("");
			return sb.ToString();
		}
	}

	public class Preload
	{
		private readonly Dictionary<object, UAssetObject> mAssetObjects = new Dictionary<object, UAssetObject>();

		public PreloadSettings settings { get; private set; }

		public int count
		{
			get { return mAssetObjects.Count; }
		}

		public string key { get; private set; }

		public Preload(string preloadAddress, PreloadSettings settings)
		{
			this.key = preloadAddress;
			this.settings = settings;
		}

		public IEnumerator Initialize()
		{
			if (settings.assetReferences == null || settings.assetReferences.Count == 0)
			{
				yield break;
			}

			foreach (var item in settings.assetReferences)
			{
				var handle = AssetManager.LoadFromResourcesAsync<UAssetObject>(item);
				yield return handle;
				if (handle.asset is UAssetObject asset)
				{
					mAssetObjects[asset.GetAddressName()] = asset;
				}
			}
		}

		public T GetObject<T>(string address) where T : UAssetObject
		{
			if (mAssetObjects.TryGetValue(address, out var ao))
			{
				return (T)ao;
			}
			return null;
		}

		private string GetSumary()
		{
			int assetCount = count;
			var sb = new StringBuilder(count * 100);
			sb.Append("[");
			foreach (var item in mAssetObjects.Keys)
			{
				assetCount--;
				sb.Append("\"").Append(item.ToString()).Append("\"");
				if (assetCount != 0)
				{
					sb.Append(",");
				}
			}
			sb.Append("]");
			return sb.ToString();
		}

		public override string ToString()
		{
			string sumary = GetSumary();
			return $"{{ {key} : {{\"count\":{count}, \"sumary\":{sumary} }} }}";
		}
	}

}
