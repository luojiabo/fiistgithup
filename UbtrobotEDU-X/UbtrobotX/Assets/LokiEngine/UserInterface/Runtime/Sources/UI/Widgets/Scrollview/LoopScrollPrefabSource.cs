using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Loki.UI
{
	[System.Serializable]
	public class LoopScrollPrefabSource
	{
		public string prefabName;
		public int poolSize = 5;

		private bool mInited = false;

		public virtual GameObject GetObject()
		{
			if (!mInited)
			{
				SG.ResourceManager.Instance.InitPool(prefabName, poolSize);
				mInited = true;
			}
			return SG.ResourceManager.Instance.GetObjectFromPool(prefabName);
		}

		public virtual void ReturnObject(Transform go)
		{
			go.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
			SG.ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
		}
	}
}
