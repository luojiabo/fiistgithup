using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Loki
{
	public class OutlineStorage
	{
		private static OutlineStorage _instance;
		private static int MaxCameraCount = 16;
		private OutlineStorage()
		{

		}
		public static OutlineStorage Instance()
		{
			if (_instance == null)
			{
				_instance = new OutlineStorage();
			}
			return _instance;
		}

		private readonly Dictionary<int, List<Outline>> outlineDict = new Dictionary<int, List<Outline>>();
		public List<Outline> OutlinesForKey(int key)
		{
			List<Outline> outlines = null;
			outlineDict.TryGetValue(key, out outlines);
			return outlines;
		}

		public void AddOutlineForKey(Outline outline, int key)
		{
			// 如果相机新增了需要显示的效果，通知相机开启outline处理
			bool needNotifyCameraSystem = false;
			for (int i = 0; i < OutlineStorage.MaxCameraCount; i++)
			{
				var bit = 1 << i;
				if ((key & bit) != 0)
				{
					if (!outlineDict.TryGetValue(bit, out var outlines))
					{
						outlines = new List<Outline>();
						outlineDict.Add(bit, outlines);
					}
					// 不会检查是否有重复，由调用者保证
					outlines.Add(outline);
					// 新增outline时，通知对应的相机
					if (outlines.Count == 1)
					{
						needNotifyCameraSystem = true;
					}
				}
			}

			if (needNotifyCameraSystem)
			{
				CameraSystem cameraSystem = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				cameraSystem.CheckOutline();
			}			
		}

		public void RemoveOutlineForKey(Outline outline, int key)
		{
			// 如果移除过相机显示的最后一个效果，通知相机停止outline处理
			bool needNotifyCameraSystem = false;
			for (int i = 0; i < OutlineStorage.MaxCameraCount; i++)
			{
				var bit = 1 << i;
				if ((key & bit) != 0)
				{
					if (!outlineDict.TryGetValue(bit, out var outlines))
					{
						continue;
					}
					outlines.Remove(outline);
					// 移除最后一个outline时，通知对应的相机
					if (outlines.Count == 0)
					{
						needNotifyCameraSystem = true;
					}
				}
			}

			if (needNotifyCameraSystem)
			{
				CameraSystem cameraSystem = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				cameraSystem.CheckOutline();
			}
		}
	}
}
