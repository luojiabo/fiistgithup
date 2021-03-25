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
		private static void LogException(Exception ex)
		{
			DebugUtility.LogException(ex);
		}

		public static AsyncOperationHandle InitializeAsync()
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.InitializeAsync");
			var h = Addressables.InitializeAsync();
			//h.Completed += opHandle =>
			//{

			//};
			return h;
		}

		/// <summary>
		/// Load a single asset
		/// </summary>
		/// <param name="key">The key of the location of the asset.</param>
		public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.LoadAssetAsync {0}", key);
			var h = Addressables.LoadAssetAsync<TObject>(key);

			return h;
		}

		/// <summary>
		/// Load a single asset
		/// </summary>
		/// <param name="key">The key of the location of the asset.</param>
		public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key, Action<AsyncOperationHandle<TObject>> onCompleted, Action<AsyncOperationHandle> onDestroy = null)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.LoadAssetAsync {0}", key);
			var h = Addressables.LoadAssetAsync<TObject>(key);
			if (onCompleted != null)
				h.Completed += onCompleted;
			if (onDestroy != null)
				h.Destroyed += onDestroy;
			return h;
		}

		/// <summary>
		/// Load a single asset
		/// </summary>
		/// <param name="key">The key of the location of the asset.</param>
		public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key, Action<AsyncOperationHandle> onCompletedTypeless, Action<AsyncOperationHandle> onDestroy = null)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.LoadAssetAsync {0}", key);
			var h = Addressables.LoadAssetAsync<TObject>(key);
			if (onCompletedTypeless != null)
				h.CompletedTypeless += onCompletedTypeless;
			if (onDestroy != null)
				h.Destroyed += onDestroy;
			return h;
		}

		/// <summary>
		/// Load a single asset
		/// </summary>
		/// <param name="key">The key of the location of the asset.</param>
		public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key, Action<AsyncOperationHandle<TObject>> onCompleted, Action<AsyncOperationHandle> onCompletedTypeless, Action<AsyncOperationHandle> onDestroy)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.LoadAssetAsync {0}", key);
			var h = Addressables.LoadAssetAsync<TObject>(key);
			if (onCompleted != null)
				h.Completed += onCompleted;
			if (onCompletedTypeless != null)
				h.CompletedTypeless += onCompletedTypeless;
			if (onDestroy != null)
				h.Destroyed += onDestroy;
			return h;
		}

		/// <summary>
		/// Load multiple assets
		/// </summary>
		/// <param name="keys">List of keys for the locations.</param>
		/// <param name="callback">Callback Action that is called per load operation.</param>
		/// <param name="mode">Method for merging the results of key matches.  See <see cref="MergeMode"/> for specifics</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<object> keys, Action<TObject> callback, MergeMode mode)
		{
			return Addressables.LoadAssetsAsync(keys, callback, mode);
		}

		/// <summary>
		/// Load multiple sprites
		/// </summary>
		/// <param name="keys">The key of the location of the asset.</param>
		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<string> keys, Action<TObject> onLoaded, MergeMode mode, Action<AsyncOperationHandle<IList<TObject>>> onCompleted)
		{
			var keysObj = new List<object>(keys);
			var h = Addressables.LoadAssetsAsync<TObject>(keysObj, onLoaded, mode);
			if (onCompleted != null)
				h.Completed += onCompleted;
			return h;
		}

		/// <summary>
		/// Load multiple assets
		/// </summary>
		/// <param name="locations">The locations of the assets.</param>
		/// <param name="callback">Callback Action that is called per load operation.</param>        
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<IResourceLocation> locations, Action<TObject> callback)
		{
			return Addressables.LoadAssetsAsync(locations, callback);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="key"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.LoadAssetAsync {0}", key);
			return Addressables.LoadAssetsAsync(key, callback);
		}

		/// <summary>
		/// Load multiple assets
		/// </summary>
		/// <param name="locations">The locations of the assets.</param>
		/// <param name="callback">Callback Action that is called per load operation.</param>        
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<IResourceLocation> locations, Action<TObject> callback, Action<AsyncOperationHandle<IList<TObject>>> onCompleted)
		{
			var h = LoadAssetsAsync(locations, callback);
			if (onCompleted != null)
			{
				h.Completed += onCompleted;
			}
			return h;
		}

		/// <summary>
		/// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.  
		/// </summary>
		/// <param name="location">The location of the Object to instantiate.</param>
		/// <param name="parent">Parent transform for instantiated object.</param>
		/// <param name="instantiateInWorldSpace">Option to retain world space when instantiated with a parent.</param>
		/// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
		{
			DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.InstantiateAsync {0}", location);
			var h = Addressables.InstantiateAsync(location, parent, instantiateInWorldSpace, trackHandle);

			return h;
		}

		/// <summary>
		/// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
		/// </summary>
		/// <param name="location">The location of the Object to instantiate.</param>
		/// <param name="position">The position of the instantiated object.</param>
		/// <param name="rotation">The rotation of the instantiated object.</param>
		/// <param name="parent">Parent transform for instantiated object.</param>
		/// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.InstantiateAsync {0}", location);
			return Addressables.InstantiateAsync(location, position, rotation, parent, trackHandle);
		}

		/// <summary>
		/// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
		/// </summary>
		/// <param name="key">The key of the location of the Object to instantiate.</param>
		/// <param name="parent">Parent transform for instantiated object.</param>
		/// <param name="instantiateInWorldSpace">Option to retain world space when instantiated with a parent.</param>
		/// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.InstantiateAsync {0}", key);
			var h = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);

			return h;
		}

		/// <summary>
		/// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
		/// </summary>
		/// <param name="key">The key of the location of the Object to instantiate.</param>
		/// <param name="position">The position of the instantiated object.</param>
		/// <param name="rotation">The rotation of the instantiated object.</param>
		/// <param name="parent">Parent transform for instantiated object.</param>
		/// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.InstantiateAsync {0}", key);
			var h = Addressables.InstantiateAsync(key, position, rotation, parent, trackHandle);

			return h;
		}
		
		/// <summary>
		/// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
		/// </summary>
		/// <param name="key">The key of the location of the Object to instantiate.</param>
		/// <param name="instantiateParameters">Parameters for instantiation.</param>
		/// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.InstantiateAsync {0}", key);
			var h = Addressables.InstantiateAsync(key, instantiateParameters, trackHandle);

			return h;
		}

		/// <summary>
		/// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
		/// </summary>
		/// <param name="location">The location of the Object to instantiate.</param>
		/// <param name="instantiateParameters">Parameters for instantiation.</param>
		/// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.InstantiateAsync {0}", location);
			var h = Addressables.InstantiateAsync(location, instantiateParameters, trackHandle);

			return h;
		}

		/// <summary>
		/// Loads the resource locations specified by the keys.
		/// The method will always return success, with a valid IList of results. If nothing matches keys, IList will be empty
		/// </summary>
		/// <param name="keys">The set of keys to use.</param>
		/// <param name="mode">The mode for merging the results of the found locations.</param>
		/// <param name="type">A type restriction for the lookup.  Only locations of the provided type (or derived type) will be returned.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(IList<object> keys, MergeMode mode, Type type = null)
		{
			return Addressables.LoadResourceLocationsAsync(keys, mode, type);
		}

		/// <summary>
		/// Request the locations for a given key.
		/// The method will always return success, with a valid IList of results. If nothing matches key, IList will be empty
		/// </summary>
		/// <param name="key">The key for the locations.</param>
		/// <param name="type">A type restriction for the lookup.  Only locations of the provided type (or derived type) will be returned.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(object key, Type type = null)
		{
			//DebugUtility.LogTrace(LoggerTags.AssetManager, "AssetManager.LoadResourceLocationsAsync {0}", key);
			return Addressables.LoadResourceLocationsAsync(key, type);
		}

		/// <summary>
		/// Release asset.
		/// </summary>
		/// <typeparam name="TObject">The type of the object being released</typeparam>
		/// <param name="obj">The asset to release.</param>
		public static void Release<TObject>(TObject obj)
		{
			Addressables.Release(obj);
		}

		/// <summary>
		/// Release the operation and its associated resources.
		/// </summary>
		/// <typeparam name="TObject">The type of the AsyncOperationHandle being released</typeparam>
		/// <param name="handle">The operation handle to release.</param>
		public static void Release<TObject>(AsyncOperationHandle<TObject> handle)
		{
			Addressables.Release(handle);
		}

		/// <summary>
		/// Release the operation and its associated resources.
		/// </summary>
		/// <param name="handle">The operation handle to release.</param>
		public static void Release(AsyncOperationHandle handle)
		{
			Addressables.Release(handle);
		}

		/// <summary>
		/// Releases and destroys an object that was created via Addressables.InstantiateAsync. 
		/// </summary>
		/// <param name="instance">The GameObject instance to be released and destroyed.</param>
		/// <returns>Returns true if the instance was successfully released.</returns>
		public static bool ReleaseInstance(GameObject instance)
		{
			if (!Addressables.ReleaseInstance(instance))
			{
				Destroy(instance);
			}
			return true;
		}

		/// <summary>
		/// Releases and destroys an object that was created via Addressables.InstantiateAsync. 
		/// </summary>
		/// <param name="instances">The GameObject instances to be released and destroyed.</param>
		/// <returns>Returns true if the instance was successfully released.</returns>
		public static void ReleaseInstances(IEnumerable<GameObject> instances)
		{
			foreach (var instance in instances)
			{
				ReleaseInstance(instance);
			}
		}

		/// <summary>
		/// Releases and destroys an object that was created via Addressables.InstantiateAsync. 
		/// </summary>
		/// <param name="handle">The handle to the game object to destroy, that was returned by InstantiateAsync.</param>
		/// <returns>Returns true if the instance was successfully released.</returns>
		public static bool ReleaseInstance(AsyncOperationHandle handle)
		{
			Addressables.ReleaseInstance(handle);
			return true;
		}

		/// <summary>
		/// Releases and destroys an object that was created via Addressables.InstantiateAsync. 
		/// </summary>
		/// <param name="handle">The handle to the game object to destroy, that was returned by InstantiateAsync.</param>
		/// <returns>Returns true if the instance was successfully released.</returns>
		public static bool ReleaseInstance(AsyncOperationHandle<GameObject> handle)
		{
			Addressables.ReleaseInstance(handle);
			return true;
		}

		/// <summary>
		/// Load a single sprite
		/// </summary>
		/// <param name="key">The key of the location of the asset.</param>
		/// <param name="spriteName">The spriteName of the sprite.</param>
		public static void LoadSprite(string key, string spriteName, Action<AsyncOperationHandle<Sprite>> onCompleted)
		{
			var h = Addressables.LoadAssetAsync<Sprite>(string.Concat(key, "[", spriteName, "]"));
			if (onCompleted != null)
				h.Completed += onCompleted;
		}

		/// <summary>
		/// Load multiple sprites
		/// </summary>
		/// <param name="key">The key of the location of the asset.</param>
		/// <param name="spriteNames">The spriteName of the sprite.</param>
		public static AsyncOperationHandle<IList<Sprite>> LoadSprites(string key, IList<string> spriteNames, Action<Sprite> onLoaded, MergeMode mode, Action<AsyncOperationHandle<IList<Sprite>>> onCompleted)
		{
			var spritesToObjects = new List<object>(spriteNames.Count);
			foreach (var item in spriteNames)
			{
				spritesToObjects.Add(string.Concat(key, "[", item, "]"));
			}
			var h = Addressables.LoadAssetsAsync<Sprite>(spritesToObjects, onLoaded, mode);
			if (onCompleted != null)
			{
				h.Completed += onCompleted;
			}
			return h;
		}

		/// <summary>
		/// Downloads dependencies of assets marked with the specified label or address.  
		/// </summary>
		/// <param name="key">The key of the asset(s) to load dependencies for.</param>
		/// <param name="autoReleaseHandle">Automatically releases the handle on completion</param>
		/// <returns>The AsyncOperationHandle for the dependency load.</returns>
		public static AsyncOperationHandle DownloadDependenciesAsync(object key, bool autoReleaseHandle = false)
		{
			return Addressables.DownloadDependenciesAsync(key, autoReleaseHandle);
		}

		/// <summary>
		/// Downloads dependencies of assets at given locations.  
		/// </summary>
		/// <param name="locations">The locations of the assets.</param>
		/// <param name="autoReleaseHandle">Automatically releases the handle on completion</param>
		/// <returns>The AsyncOperationHandle for the dependency load.</returns>
		public static AsyncOperationHandle DownloadDependenciesAsync(IList<IResourceLocation> locations, bool autoReleaseHandle = false)
		{
			return Addressables.DownloadDependenciesAsync(locations, autoReleaseHandle);
		}

		/// <summary>
		/// Downloads dependencies of assets marked with the specified labels or addresses.  
		/// </summary>
		/// <param name="keys">List of keys for the locations.</param>
		/// <param name="mode">Method for merging the results of key matches.  See <see cref="MergeMode"/> for specifics</param>
		/// <param name="autoReleaseHandle">Automatically releases the handle on completion</param>
		/// <returns>The AsyncOperationHandle for the dependency load.</returns>
		public static AsyncOperationHandle DownloadDependenciesAsync(IList<object> keys, MergeMode mode, bool autoReleaseHandle = false)
		{
			return Addressables.DownloadDependenciesAsync(keys, mode, autoReleaseHandle);
		}

		/// <summary>
		/// Determines the required download size, dependencies included, for the specified <paramref name="key"/>.
		/// Cached assets require no download and thus their download size will be 0.  The Result of the operation
		/// is the download size in bytes.
		/// </summary>
		/// <returns>The operation handle for the request.</returns>
		/// <param name="key">The key of the asset(s) to get the download size of.</param>
		public static AsyncOperationHandle<long> GetDownloadSizeAsync(object key)
		{
			return Addressables.GetDownloadSizeAsync(key);
		}

		/// <summary>
		/// Determines the required download size, dependencies included, for the specified <paramref name="keys"/>.
		/// Cached assets require no download and thus their download size will be 0.  The Result of the operation
		/// is the download size in bytes.
		/// </summary>
		/// <returns>The operation handle for the request.</returns>
		/// <param name="keys">The keys of the asset(s) to get the download size of.</param>
		public static AsyncOperationHandle<long> GetDownloadSizeAsync(IList<object> keys)
		{
			return Addressables.GetDownloadSizeAsync(keys);
		}

		/// <summary>
		/// Clear the cached AssetBundles for a given key.  Operation may be performed async if Addressables
		/// is initializing or updating.
		/// </summary>
		/// <param name="key">The key to clear the cache for.</param>
		public static void ClearDependencyCacheAsync(object key)
		{
			Addressables.ClearDependencyCacheAsync(key);
		}

		/// <summary>
		/// Clear the cached AssetBundles for a list of Addressable locations.  Operation may be performed async if Addressables
		/// is initializing or updating.
		/// </summary>
		/// <param name="locations">The locations to clear the cache for.</param>
		public static void ClearDependencyCacheAsync(IList<IResourceLocation> locations)
		{
			Addressables.ClearDependencyCacheAsync(locations);
		}

		/// <summary>
		/// Clear the cached AssetBundles for a list of Addressable keys.  Operation may be performed async if Addressables
		/// is initializing or updating.
		/// </summary>
		/// <param name="keys">The keys to clear the cache for.</param>
		public static void ClearDependencyCacheAsync(IList<object> keys)
		{
			Addressables.ClearDependencyCacheAsync(keys);
		}

		/// <summary>
		/// Checks all updatable content catalogs for a new version.
		/// </summary>
		/// <param name="autoReleaseHandle">If true, the handle will automatically be released when the operation completes.</param>
		/// <returns>The operation containing the list of catalog ids that have an available update.  This can be used to filter which catalogs to update with the UpdateContent.</returns>
		public static AsyncOperationHandle<List<string>> CheckForCatalogUpdates(bool autoReleaseHandle = true)
		{
			return Addressables.CheckForCatalogUpdates(autoReleaseHandle);
		}

		/// <summary>
		/// Update the specified catalogs.
		/// </summary>
		/// <param name="catalogs">The set of catalogs to update.  If null, all catalogs that have an available update will be updated.</param>
		/// <param name="autoReleaseHandle">If true, the handle will automatically be released when the operation completes.</param>
		/// <returns>The operation with the list of updated content catalog data.</returns>
		public static AsyncOperationHandle<List<IResourceLocator>> UpdateCatalogs(IEnumerable<string> catalogs = null, bool autoReleaseHandle = true)
		{
			return Addressables.UpdateCatalogs(catalogs, autoReleaseHandle);
		}

		/// <summary>
		/// Add a resource locator.
		/// </summary>
		/// <param name="locator">The locator object.</param>
		/// <param name="localCatalogHash">The hash of the local catalog. This can be null if the catalog cannot be updated.</param>
		/// <param name="remoteCatalogLocation">The location of the remote catalog. This can be null if the catalog cannot be updated.</param>
		public static void AddResourceLocator(IResourceLocator locator, string localCatalogHash = null, IResourceLocation remoteCatalogLocation = null)
		{
			Addressables.AddResourceLocator(locator, localCatalogHash, remoteCatalogLocation);
		}

		/// <summary>
		/// Remove a locator;
		/// </summary>
		/// <param name="locator">The locator to remove.</param>
		public static void RemoveResourceLocator(IResourceLocator locator)
		{
			Addressables.RemoveResourceLocator(locator);
		}

		/// <summary>
		/// Remove all locators.
		/// </summary>
		public static void ClearResourceLocators()
		{
			Addressables.ClearResourceLocators();
		}

		/// <summary>
		/// Additively load catalogs from runtime data. 
		/// The settings are not used.
		/// </summary>
		/// <param name="catalogPath">The path to the runtime data.</param>
		/// <param name="providerSuffix">This value, if not null or empty, will be appended to all provider ids loaded from this data.</param>
		/// <returns>The operation handle for the request.</returns>
		public static AsyncOperationHandle<IResourceLocator> LoadContentCatalogAsync(string catalogPath, string providerSuffix = null)
		{
			return Addressables.LoadContentCatalogAsync(catalogPath, providerSuffix);
		}
	}
}

//AddressablesImpl.cs
////添加2个新方法
////初始化完之后调用GetRemoteBundleSizeAsync方法
//AsyncOperationHandle<long> GetRemoteBundleSizeWithChain(IList<string> bundles)
//{
//	return ResourceManager.CreateChainOperation(InitializationOperation, op => GetRemoteBundleSizeAsync(key));
//}
////通过ab包名得到ab包大小
//public AsyncOperationHandle<long> GetRemoteBundleSizeAsync(IList<string> bundles)
//{
//	//如果还没初始化完那么等待初始化完
//	if (!InitializationOperation.IsDone)
//		return GetRemoteBundleSizeWithChain(key);
//	IList<IResourceLocation> locations = new IList<IResourceLocation>();
//	for (var i = 0; i < bundles.Count, i++)
//	{
//		IList<IResourceLocation> tmpLocations;
//		var key = bundles[i];
//		//寻找传入的包名对应的ab包，如果没找到那么警告
//		if (!GetResourceLocations(key, typeof(object), out locations))
//			return ResourceManager.CreateCompletedOperation<Long>(0, new InvalidKeyException(key).Message);
//		locations.Add(tmpLocations[0]);
//	}
//	//总的包大小
//	long size = 0;
//	for (var i = 0; i < locations.Count; i++)
//	{
//		if (locations[i].Data != null)
//		{
//			var sizeData = locations[i].Data as ILocationSizeData;
//			if (sizeData != null)
//			{
//				//计算包大小
//				size += sizeData.ComputeSize(locations[i]);
//			}
//		}
//	}
//	//返回总的包大小
//	return ResourceManager.CreateCompletedOperation<Long>(size, string.Empty)
//}
////在对应的Addressables外观类里也添加GetRemoteBundleSizeAsync方法

////使用方法
////在addressable初始化完成后遍历所有地址，如果地址的结尾是.bundle，那么他对应了一个ab包，然后把它缓存到列表，再使用添加的接口来获得所有需要更新包的大小。
//Addressables.InitializeAsync().Completed += opHandle =>
//{
//    var map = opHandle.Result as ResourceLocationMap;
//List<string> bundles = new List<string>();
//    foreach (object mapKey in map.keys)
//    {
//        string key = mapKey as string;
//        if(key != null && key.EndsWith(".bundle"))
//        {
//            bundles.Add(key);
//        }
//    }
//    Addressables.GetRemoteBundleSizeAsync(key).Completed += asyncOpHandle => print(asyncOpHandle.Result);
//};
#endif
