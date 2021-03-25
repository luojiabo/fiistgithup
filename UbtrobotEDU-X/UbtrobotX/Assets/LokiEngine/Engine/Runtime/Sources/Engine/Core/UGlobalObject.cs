using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Loki
{
	/// <summary>
	/// The global object means that is the immortal tree-root
	/// </summary>
	public sealed class UGlobalObject : UObject
	{
		private static UGlobalObject msGlobalObject;

		public static UGlobalObject Get()
		{
			if (msGlobalObject == null)
			{
				msGlobalObject = NewObject<UGlobalObject>("GlobalObject");
				if (msGlobalObject != null)
				{
					DontDestroyOnLoad(msGlobalObject.gameObject);
				}
			}

			return msGlobalObject;
		}

		public bool SetAutoDestroy(UObject obj, string moduleName)
		{
			if (obj.transform.parent == transform.Find(moduleName))
			{
				obj.transform.SetParent(null);
				return true;
			}
			DebugUtility.LogErrorTrace(LoggerTags.GlobalObject, "The object is not under the module {0}, Please ensure that the object has AddGlobalObject({1}, {0})", moduleName, obj.name);
			return false;
		}

		public UObject AddGlobalObject(UObject obj, string moduleName)
		{
			obj.transform.SetParent(transform.FindOrAdd(moduleName));
			return obj;
		}
	}
}
