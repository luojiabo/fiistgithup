using System;
using System.Reflection;
using UnityEngine;

namespace Loki
{
	/// <summary>
	/// The ScripatableObject request the file name of the type is the type's name.
	/// </summary>
	public abstract class UAssetObject : ScriptableObject
	{
		public virtual string category { get { return "EngineConfig"; } }

		public virtual bool isSubAsset { get { return true; } }

		public static string GetAssetName<T>(bool ext = true) where T : UAssetObject
		{
			return GetAssetName(typeof(T), ext);
		}

		public static string GetAssetName(Type type, bool ext = true)
		{
			var attr = type.GetCustomAttribute<CreateAssetMenuAttribute>();
			if (attr != null)
			{
				if (!string.IsNullOrEmpty(attr.fileName))
				{
					if (ext)
						return string.Concat(attr.fileName, ".asset");
					return attr.fileName;
				}
			}

			if (ext)
				return string.Concat(type.Name, ".asset");
			return type.Name;
		}

		public string GetAssetName(bool ext = true)
		{
			return GetAssetName(GetType(), ext);
		}

		public virtual string GetAddressName()
		{
			return GetAssetName(false);
		}

		public virtual string GetAssetFileName(bool ext = true)
		{
			return FileSystem.Combine(category, GetAssetName(ext));
		}

		public virtual void OnCreated()
		{
			// you can hide this object in it's view panel with assigning hideFlags.
		}
	}

	/// <summary>
	/// The ScripatableObject request the file name of the type is the type's name.
	/// </summary>
	public abstract class UEditorAssetObject : UAssetObject
	{
		public override string category { get { return "EditorConfig"; } }

		public static bool isProSkin
		{
			get
			{
#if UNITY_EDITOR
				return UnityEditor.EditorGUIUtility.isProSkin;
#else
				return false;
#endif
			}
		}

	}
}
