using UnityEngine;

namespace Loki
{
	public abstract class UObjectBase : MonoBehaviour
	{
		private Transform mTransformCache;
		private GameObject mGameObjectCache;

		public new Transform transform
		{
			get
			{
				if (mTransformCache == null)
					mTransformCache = base.transform;
				return mTransformCache;
			}
		}

		public new GameObject gameObject
		{
			get
			{
				if (mGameObjectCache == null)
					mGameObjectCache = base.gameObject;
				return mGameObjectCache;
			}
		}

		/// <summary>
		/// Returns true if this MonoBehaviour/GameObject has been destroyed
		/// </summary>
		/// <returns>returns true if the MonoBehaviour has destroyed</returns>
		public bool HasDestroyed()
		{
			// it will return true if the MonoBehaviour has destroyed
			return this == null;
		}

	}
}
