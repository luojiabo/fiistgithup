//using UnityEngine;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityObject = UnityEngine.Object;

//namespace Loki
//{
//	public class UObjectPool : USingletonObject<UObjectPool>
//	{
//		public override ELifetime lifetime => ELifetime.App;

//		private static readonly Dictionary<Type, ObjectPool> msObjectPools = new Dictionary<Type, ObjectPool>();

//		public static T GetOrAllocPool<T>() where T : ObjectPool, new()
//		{
//			Type type = typeof(T);
//			if (!msObjectPools.TryGetValue(type, out var pool))
//			{
//				var inst = GetOrAlloc();
//				pool = new T();
//				pool.transform = new GameObject(type.Name).transform;
//				pool.transform.SetParent(inst.transform);
//				msObjectPools.Add(type, pool);
//			}
//			return (T)pool;
//		}

//		protected override void OnDestroy()
//		{
//			base.OnDestroy();
//			foreach (var kv in msObjectPools)
//			{
//				kv.Value.OnDestroy();
//			}
//			msObjectPools.Clear();
//		}
//	}

//	public abstract class ObjectPool
//	{
//		protected readonly Dictionary<ObjectPoolID, UnityObject> mObjects = new Dictionary<ObjectPoolID, UnityObject>();

//		internal Transform transform { get; set; }

//		public virtual void OnDestroy()
//		{
//			foreach (var item in mObjects)
//			{
//				if (item.Value != null)
//				{
//					//AssetManager.Release(item.Value);
//				}
//			}
//			mObjects.Clear();
//		}

//		protected T InstantiateObject<T>(T asset) where T : UnityObject
//		{
//			if (asset == null)
//				return null;
//			ProfilingUtility.BeginSample("InstantiateObject");
//			var result = UnityObject.Instantiate(asset) as T;
//			ProfilingUtility.EndSample();
//			return result;
//		}

//		protected void DestroyInstance<T>(T inst) where T : UnityObject
//		{
//			if (inst == null)
//				return;
//			UnityObject.Destroy(inst);
//		}

//		public async Task<T> LoadAssetAsync<T>(ObjectPoolID id) where T : UnityObject
//		{
//			ProfilingUtility.BeginSample("ObjectPool.LoadAssetAsync");
//			if (!mObjects.TryGetValue(id, out var asset))
//			{
//				// asset = await AssetManager.LoadFromResourcesAsync<T>(id.address);
//				if (asset != null)
//				{
//					mObjects.Add(id, asset);
//				}
//			}
//			ProfilingUtility.EndSample();
//			return (T)asset;
//		}
//	}

//	public struct ObjectPoolID : IEqualityComparer<ObjectPoolID>
//	{
//		public readonly string address;
//		public readonly string guid;

//		public static readonly ObjectPoolID DefaultComparer = new ObjectPoolID();

//		public bool isFullyAssigned
//		{
//			get
//			{
//				return !string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(guid);
//			}
//		}

//		private ObjectPoolID(string address, string guid)
//		{
//			this.address = address;
//			this.guid = guid;
//		}

//		public override string ToString()
//		{
//			return string.Concat("{", !string.IsNullOrEmpty(guid) ? guid : "(GUID)", ":", !string.IsNullOrEmpty(address) ? address : "(Address)", "}");
//		}

//		public static bool operator ==(ObjectPoolID left, ObjectPoolID right)
//		{
//			return left.Equals(right);
//		}

//		public static bool operator !=(ObjectPoolID left, ObjectPoolID right)
//		{
//			return !(left == right);
//		}

//		public override bool Equals(object obj)
//		{
//			if (obj is ObjectPoolID)
//			{
//				return Equals(this, (ObjectPoolID)obj);
//			}
//			return false;
//		}

//		public override int GetHashCode()
//		{
//			if (string.IsNullOrEmpty(address))
//				return guid.GetHashCode();

//			if (string.IsNullOrEmpty(guid))
//				return address.GetHashCode();

//			return HashCodeCombiner.CombineHashCodes(address, guid);
//		}

//		public static ObjectPoolID Auto(string unknown)
//		{
//			DebugUtility.AssertFormat(!string.IsNullOrEmpty(unknown), "The argument is empty.");

//			if (unknown.StartsWith("Assets/"))
//			{
//				return FromAddress(unknown);
//			}
//			return FromGUID(unknown);
//		}

//		public static ObjectPoolID FromAddressOrGUID(string address, string guid)
//		{
//			DebugUtility.AssertFormat(!string.IsNullOrEmpty(address) || !string.IsNullOrEmpty(guid), "The address and guid are empty.");
//			return new ObjectPoolID(address, guid);
//		}

//		public static ObjectPoolID FromAddress(string address)
//		{
//			DebugUtility.AssertFormat(!string.IsNullOrEmpty(address), "The address is empty.");
//			return new ObjectPoolID(address, string.Empty);
//		}

//		public static ObjectPoolID FromGUID(string guid)
//		{
//			DebugUtility.AssertFormat(!string.IsNullOrEmpty(guid), "The guid is empty.");
//			return new ObjectPoolID(string.Empty, guid);
//		}

//		public bool Equals(ObjectPoolID x, ObjectPoolID y)
//		{
//			// while the address is not empty
//			// while the guid is not empty
//			return (string.IsNullOrEmpty(x.address) && string.IsNullOrEmpty(y.address) && string.Equals(x.address, y.address))
//				|| (string.IsNullOrEmpty(x.guid) && string.IsNullOrEmpty(y.guid) && string.Equals(x.guid, y.guid));
//		}

//		public int GetHashCode(ObjectPoolID obj)
//		{
//			DebugUtility.AssertFormat(!string.IsNullOrEmpty(address) || !string.IsNullOrEmpty(guid), "The address and guid are empty.");

//			return obj.GetHashCode();
//		}

//	}

//	public class ObjectPool<T> : ObjectPool where T : UnityEngine.Object
//	{
//		/// <summary>
//		/// The caches
//		/// </summary>
//		protected readonly Dictionary<ObjectPoolID, Queue<T>> mObjectCaches = new Dictionary<ObjectPoolID, Queue<T>>(ObjectPoolID.DefaultComparer);

//		public override void OnDestroy()
//		{
//			base.OnDestroy();
//			OnDestroyCacheQueues();
//		}

//		protected virtual void OnDestroyCacheQueues()
//		{
//			foreach (var item in mObjectCaches)
//			{
//				while (item.Value.Count > 0)
//				{
//					var o = item.Value.Dequeue();
//					DestroyInstance(o);
//				}
//			}
//			mObjectCaches.Clear();
//		}

//		/// <summary>
//		/// Get or alloc a queue by id, however it will fail and return null when allocating a new Queue if the id is not FullyAssigned
//		/// </summary>
//		/// <param name="id">id</param>
//		/// <returns>Queue or null</returns>
//		public Queue<T> GetOrAllocQueue(ObjectPoolID id)
//		{
//			if (!mObjectCaches.TryGetValue(id, out var queue))
//			{
//				if (!id.isFullyAssigned)
//				{
//					DebugUtility.LogErrorTrace(LoggerTags.ObjectPool, "Please ensure that your object queue has existed or the id is fully-assigned");
//				}
//				else
//				{
//					queue = new Queue<T>();
//					mObjectCaches.Add(id, queue);
//				}
//			}
//			return queue;
//		}

//		/// <summary>
//		/// Put the object to cache queue
//		/// </summary>
//		/// <param name="id">id</param>
//		/// <param name="o">the abandoned object</param>
//		public virtual bool Put(ObjectPoolID id, T o)
//		{
//			if (o == null)
//			{
//				DebugUtility.LogWarningTrace(LoggerTags.ObjectPool, "Please do not set the null object to ObjectPool");
//				return false;
//			}

//			Queue<T> queue = GetOrAllocQueue(id);
//			if (queue != null)
//			{
//				OnBeforePut(id, o);
//				if (o != null)
//				{
//					queue.Enqueue(o);
//					DebugUtility.Log(LoggerTags.ObjectPool, "The object({0}) has been put to the pool ({1}).", o.name, id);
//					return true;
//				}
//				else
//				{
//					DebugUtility.LogWarningTrace(LoggerTags.ObjectPool, "The object can not be put to the pool ({0}). It was destroyed.", id);
//					return false;
//				}
//			}

//			DebugUtility.LogWarningTrace(LoggerTags.ObjectPool, "The object({0}) can not be put to the pool ({1}).", o.name, id);
//			return false;
//		}

//		/// <summary>
//		/// Get cache object if it has instantiated in the cache queue.
//		/// </summary>
//		/// <param name="id">id</param>
//		/// <returns>The Object or null(Not found)</returns>
//		public T GetCacheObject(ObjectPoolID id)
//		{
//			Queue<T> queue;
//			if (!mObjectCaches.TryGetValue(id, out queue))
//			{
//				return null;
//			}
//			// it may be zero
//			if (queue.Count == 0)
//			{
//				return null;
//			}
//			// the cached object
//			T cachedObject = null;
//			// avoid destroy from editor
//			while (queue.Count > 0 && (cachedObject == null))
//			{
//				// dequeue the first not null cached object
//				cachedObject = queue.Dequeue();
//			}

//			if (cachedObject != null)
//			{
//				OnBeforePop(id, cachedObject);
//			}
//			return cachedObject;
//		}

//		/// <summary>
//		/// Get cache object if it has instantiated in the cache queue.
//		/// If the cache queue is empty, it will load asset with AssetManager.
//		/// </summary>
//		/// <param name="id">The id</param>
//		/// <returns></returns>
//		public async Task<T> GetObjectAsync(ObjectPoolID id)
//		{
//			var o = GetCacheObject(id);
//			if (o != null)
//			{
//				return o;
//			}

//			var result = await LoadAssetAsync<T>(id);
//			return result;
//		}

//		public virtual async Task<ObjectPoolID> Preload(ObjectPoolID id, int count, int intervalMS = -1)
//		{
//			DebugUtility.AssertFormat(id.isFullyAssigned, "Please ensure that the id is fully assigned");
//			ProfilingUtility.BeginSample("ObjectPoo.Preload");
//			var queue = GetOrAllocQueue(id);
//			if (queue.Count < count)
//			{
//				var asset = await GetObjectAsync(id);
//				if (asset != null)
//				{
//					while (queue.Count < count)
//					{
//						if (intervalMS > 0)
//						{
//							await Task.Delay(intervalMS);
//						}
//						var cloned = InstantiateObject(asset);
//						cloned.name = asset.name;
//						Put(id, cloned);
//					}
//				}
//			}
//			ProfilingUtility.EndSample();
//			return id;
//		}

//		protected virtual void OnBeforePut(ObjectPoolID id, T o)
//		{
//		}

//		protected virtual void OnBeforePop(ObjectPoolID id, T o)
//		{
//		}
//	}

//	public class GameObjectPool : ObjectPool<GameObject>
//	{
//		/// <summary>
//		/// Preload
//		/// </summary>
//		/// <param name="id">id</param>
//		/// <param name="count">count</param>
//		/// <param name="intervalMS">Ignore</param>
//		/// <returns></returns>
//		public override async Task<ObjectPoolID> Preload(ObjectPoolID id, int count, int intervalMS = -1)
//		{
//			DebugUtility.AssertFormat(id.isFullyAssigned, "Please ensure that the id is fully assigned");

//			var queue = GetOrAllocQueue(id);
//			if (queue.Count < count)
//			{
//				var asset = await GetObjectAsync(id);
//				if (asset != null)
//				{
//					var result = await AssetManager.InstantiateAsync(asset, count, null);
//					foreach (var cloned in result)
//					{
//						Put(id, cloned);
//					}
//				}
//			}
//			return id;
//		}

//		protected override void OnBeforePut(ObjectPoolID id, GameObject o)
//		{
//			base.OnBeforePut(id, o);
//			if (o != null)
//			{
//				o.SetActive(false);
//				if (o != null)
//					o.transform.SetParent(transform);
//			}
//		}

//		protected override void OnBeforePop(ObjectPoolID id, GameObject o)
//		{
//			base.OnBeforePop(id, o);
//			if (o != null)
//			{
//				o.transform.SetParent(null);
//				// maybe some components will destroy gameObject at OnEnable();
//				o.SetActive(true);
//			}
//		}

//	}
//}
