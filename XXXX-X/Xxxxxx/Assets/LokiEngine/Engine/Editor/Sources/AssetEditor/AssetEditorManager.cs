using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;

namespace Loki
{
	public struct OpenAssetInfo
	{
		public Object instance;
		public string assetPath;
		public string fullPath;
		public int instanceID;
		public int line;
	}

	public interface IAssetEditor
	{
		bool ProcessAssetOpen(OpenAssetInfo info);
	}

	public abstract class AutoRegisterAssetEditor<T> : IAssetEditor where T : AutoRegisterAssetEditor<T>, new()
	{
		private static readonly Type msType = typeof(T);

		public abstract bool ProcessAssetOpen(OpenAssetInfo info);

		protected static void InitializeOnLoad()
		{
			var mgr = AssetEditorManager.Get();
			if (!mgr.Exist(msType, false))
			{
				mgr.Register(new T());
			}
		}
	}


	public class AssetEditorManager
	{
		private static readonly AssetEditorManager msIntance;
		private readonly List<IAssetEditor> mAssetEditors = new List<IAssetEditor>();

		public static AssetEditorManager Get() { return msIntance; }

		static AssetEditorManager()
		{
			msIntance = new AssetEditorManager();
		}

		public void Register(IAssetEditor editor)
		{
			mAssetEditors.Add(editor);
		}

		public bool Exist(Type type, bool subClassOfType)
		{
			foreach (var e in mAssetEditors)
			{
				if (subClassOfType)
				{
					if (e.GetType().IsSubclassOf(type))
					{
						return true;
					}
				}
				else if (e.GetType() == type)
				{
					return true;
				}
			}
			return false;
		}

		public bool Process(OpenAssetInfo info)
		{
			for (int i = 0; i < mAssetEditors.Count; ++i)
			{
				if (mAssetEditors[i].ProcessAssetOpen(info))
				{
					return true;
				}
			}
			return false;
		}

		[OnOpenAsset]
		static bool OnOpenAssetCallback(int instanceID, int line)
		{
			// FDebug.LogTrace(FLoggerTags.Console, "OnOpenAsset");

			var selected = EditorUtility.InstanceIDToObject(instanceID);
			if (!selected)
				return false;

			var assetPath = AssetDatabase.GetAssetPath(selected);
			if (string.IsNullOrEmpty(assetPath))
				return false;

			// FDebug.Log(LogType.Log, FLoggerTags.Engine, "OnOpenAsset {0}:{1}", assetPath, line);

			var assetFilePath = Path.GetFullPath(assetPath);
			if (string.IsNullOrEmpty(assetPath))
				return false;

			OpenAssetInfo info;
			info.instance = selected;
			info.instanceID = instanceID;
			info.line = line;
			info.assetPath = assetPath;
			info.fullPath = assetFilePath;
			var result = Get().Process(info);
			return result;
		}
	}
}
