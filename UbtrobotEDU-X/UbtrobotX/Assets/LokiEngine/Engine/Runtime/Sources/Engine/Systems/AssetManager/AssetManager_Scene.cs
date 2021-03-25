#if UNITY_ADDRESSABLE_SYSTEM
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using static UnityEngine.AddressableAssets.Addressables;
using UnityObject = UnityEngine.Object;

namespace Loki
{
	public partial class AssetManager
	{
		public static AsyncOperationHandle<SceneInstance> LoadScene(IResourceLocation location, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
		{
			return LoadSceneAsync(location, loadMode, activateOnLoad, priority);
		}

		/// <summary>
		/// Load scene.
		/// </summary>
		/// <param name="key">The key of the location of the scene to load.</param>
		/// <param name="loadMode">Scene load mode.</param>
		/// <param name="activateOnLoad">If false, the scene will load but not activate (for background loading).  The SceneInstance returned has an Activate() method that can be called to do this at a later point.</param>
		/// <param name="priority">Async operation priority for scene loading.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
		}

		/// <summary>
		/// Load scene.
		/// </summary>
		/// <param name="location">The location of the scene to load.</param>
		/// <param name="loadMode">Scene load mode.</param>
		/// <param name="activateOnLoad">If false, the scene will load but not activate (for background loading).  The SceneInstance returned has an Activate() method that can be called to do this at a later point.</param>
		/// <param name="priority">Async operation priority for scene loading.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation location, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.LoadSceneAsync(location, loadMode, activateOnLoad, priority);
		}

		/// <summary>
		/// Release scene
		/// </summary>
		/// <param name="scene">The SceneInstance to release.</param>
		/// <param name="autoReleaseHandle">If true, the handle will be released automatically when complete.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance scene, bool autoReleaseHandle = true)
		{
			return Addressables.UnloadSceneAsync(scene, autoReleaseHandle);
		}

		/// <summary>
		/// Release scene
		/// </summary>
		/// <param name="handle">The handle returned by LoadSceneAsync for the scene to release.</param>
		/// <param name="autoReleaseHandle">If true, the handle will be released automatically when complete.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle handle, bool autoReleaseHandle = true)
		{
			return Addressables.UnloadSceneAsync(handle, autoReleaseHandle);
		}

		/// <summary>
		/// Release scene
		/// </summary>
		/// <param name="handle">The handle returned by LoadSceneAsync for the scene to release.</param>
		/// <param name="autoReleaseHandle">If true, the handle will be released automatically when complete.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle<SceneInstance> handle, bool autoReleaseHandle = true)
		{
			return Addressables.UnloadSceneAsync(handle, autoReleaseHandle);
		}
	}
}
#endif
