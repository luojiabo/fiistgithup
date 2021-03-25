using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Loki
{
	public abstract class FileSystem
	{
		private static FileSystem msFileSystem;
		public static readonly char[] SplitOfPath;

		public string projectPath { get; private set; }
		public string projectPathWithAlt { get; private set; }
		public string dataPath { get; private set; }
		public string streamingAssetsPath { get; private set; }
		public string engineResourcePath { get; private set; }
		public string editorResourcePath { get; private set; }
		public string persistentDataPath { get; private set; }
		public string engineGeneratedConfigPath { get; private set; }

		static FileSystem()
		{
			SplitOfPath = new char[] { '\\', '/' };
		}

		public static FileSystem Get()
		{
			if (msFileSystem == null)
			{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				msFileSystem = new WindowsFileSystem();
#elif UNITY_WEBGL
				msFileSystem = new WebGLFileSystem();
#elif UNITY_IOS
				msFileSystem = new iOSFileSystem();
#elif UNITY_ANDROID
				msFileSystem = new AndroidFileSystem();
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
				msFileSystem = new OSXFileSystem();
#else
#error			FileSystem Unsupported Platform
#endif
				DebugUtility.Assert(msFileSystem != null, "The FileSystem must be not null.");
			}

			return msFileSystem;
		}

		public static string Combine(string leftPart, string rightPart)
		{
			if (string.IsNullOrEmpty(leftPart) && string.IsNullOrEmpty(rightPart))
				return string.Empty;

			if (string.IsNullOrEmpty(leftPart))
				return rightPart;

			if (string.IsNullOrEmpty(rightPart))
				return leftPart;

			bool leftPartSplash = leftPart.EndsWithAnyChar(SplitOfPath);
			bool rightPartSplash = rightPart.StartsWithAnyChar(SplitOfPath);

			if (leftPartSplash && rightPartSplash)
			{
				return string.Concat(leftPart, rightPart.Substring(1)).ToUNIXStyle();
			}

			if (!leftPartSplash && !rightPartSplash)
			{
				return string.Concat(leftPart, '/', rightPart).ToUNIXStyle();
			}

			return string.Concat(leftPart, rightPart).ToUNIXStyle();
		}

		public static string GetFullPath(string assetPath)
		{
			return Path.GetFullPath(assetPath).ToUNIXStyle();
		}

		protected FileSystem()
		{
			dataPath = Application.dataPath.ToUNIXStyle();
			persistentDataPath = Application.persistentDataPath.ToUNIXStyle();
			streamingAssetsPath = Application.streamingAssetsPath.ToUNIXStyle();
			engineResourcePath = Combine(dataPath, "/Resources");
			editorResourcePath = Combine(dataPath, "/Editor/Resources");
			projectPath = dataPath.Substring(0, dataPath.Length - "/Assets".Length);
			projectPathWithAlt = projectPath + "/";
			engineGeneratedConfigPath = Combine(dataPath, "/Resources/Common/EngineGenerated/Configs");
		}

		public string GetFullPath(EFilePathType pathType)
		{
			switch (pathType)
			{
				case EFilePathType.EditorProject:
					return projectPath;
				case EFilePathType.EditorAssetRoot:
					return dataPath;
				case EFilePathType.EngineDefaultResources:
					return engineResourcePath;
				case EFilePathType.EditorDefaultResources:
					return editorResourcePath;
				case EFilePathType.StreamingAssets:
					return streamingAssetsPath;
				case EFilePathType.EngineGeneratedConfigPath:
					return engineGeneratedConfigPath;
				case EFilePathType.PersistentDataPath:
					return persistentDataPath;
			}
			return string.Empty;
		}

		public string GetFullPathCheck(EFilePathType pathType, string subPath = "", bool isFile = false)
		{
			string path = Combine(GetFullPath(pathType), subPath);
			if (isFile)
			{
				CheckPath(path);
			}
			else
			{
				CheckDirectory(path);
			}
			return path;
		}

		public string GetAssetPathCheck(EFilePathType pathType, string subPath = "", bool isFile = false)
		{
			return FullPathToAssetPath(GetFullPathCheck(pathType, subPath, isFile));
		}

		public static string FullPathToAssetPath(string fullPath)
		{
			if (string.IsNullOrEmpty(fullPath))
				return fullPath;

			if (fullPath.StartsWith("Assets/"))
				return fullPath;

			fullPath = fullPath.ToUNIXStyle();
			string projectPath = Get().GetFullPath(EFilePathType.EditorProject);

			if (!string.IsNullOrEmpty(projectPath) && fullPath.StartsWith(projectPath))
			{
				return fullPath.Substring(projectPath.Length + 1);
			}
			return fullPath;
		}

		public static string AssetPathToFullPath(string assetPath)
		{
			string projectPath = Get().GetFullPath(EFilePathType.EditorProject);
			if (string.IsNullOrEmpty(assetPath))
				return projectPath;

			if (!assetPath.StartsWith("Assets/"))
				return assetPath;

			assetPath = assetPath.ToUNIXStyle();

			if (!string.IsNullOrEmpty(projectPath))
			{
				return string.Concat(projectPath, "/", assetPath);
			}
			return String.Empty;
		}

		public static string AnyPathToResourcesPath(string anyPath, bool trimExt)
		{
			if (string.IsNullOrEmpty(anyPath))
				return string.Empty;
			const string kResPath = "Resources/";
			int resourcesPath = anyPath.IndexOf(kResPath);
			var result = anyPath;
			if (resourcesPath >= 0)
				result = anyPath.Substring(resourcesPath + kResPath.Length);
			if (trimExt)
			{
				int extIndex = result.LastIndexOf('.');
				if (extIndex > 0)
				{
					result = result.Substring(0, extIndex);
				}
			}
			return result;
		}
		public string GetFullPath(EFilePathType pathType, string subPath)
		{
			return Combine(GetFullPath(pathType), subPath.ToUNIXStyle());
		}

		public bool ExistsFile(string path)
		{
			try
			{
				return File.Exists(path);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
				return false;
			}
		}

		public bool CheckPath(string filePath)
		{
			try
			{
				if (string.IsNullOrEmpty(filePath))
					return false;

				if (Directory.Exists(filePath))
					return true;

				int lastSplash = filePath.LastIndexOfAny(SplitOfPath);
				if (lastSplash < 0)
					return true;

				if (lastSplash == filePath.Length - 1)
					return CheckDirectory(filePath);

				return CheckDirectory(filePath.Substring(0, lastSplash));
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return false;
		}

		public bool CheckDirectory(string directory)
		{
			try
			{
				if (string.IsNullOrEmpty(directory))
					return false;

				if (Directory.Exists(directory))
					return true;

				DirectoryInfo info = Directory.CreateDirectory(directory);
#if UNITY_EDITOR
				if (directory.StartsWith(dataPath))
				{
					if (!Application.isPlaying)
					{
						UnityEditor.AssetDatabase.Refresh();
					}
				}
#endif
				return info != null && info.Exists;
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return false;
		}

		public virtual byte[] ReadAllBytes(string fullPath)
		{
			try
			{
				if (File.Exists(fullPath))
				{
					return File.ReadAllBytes(fullPath);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return null;
		}

		public string ReadAllText(string fullPath)
		{
			return ReadAllText(fullPath, Encoding.UTF8);
		}

		public virtual string ReadAllText(string fullPath, Encoding encoding)
		{
			try
			{
				if (File.Exists(fullPath))
				{
					return File.ReadAllText(fullPath, encoding);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return null;
		}

		public string[] ReadAllLines(string fullPath)
		{
			return ReadAllLines(fullPath, Encoding.UTF8);
		}

		public virtual string[] ReadAllLines(string fullPath, Encoding encoding)
		{
			try
			{
				if (File.Exists(fullPath))
				{
					return File.ReadAllLines(fullPath, encoding);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return null;
		}

		public virtual void WriteAllText(string fullPath, string content)
		{
			WriteAllText(fullPath, content, Encoding.UTF8);
		}

		public void WriteAllLines(string fullPath, string[] lines)
		{
			WriteAllLines(fullPath, lines, Encoding.UTF8);
		}

		public virtual void WriteAllLines(string fullPath, string[] lines, Encoding encoding)
		{
			try
			{
				File.WriteAllLines(fullPath, lines, encoding);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public virtual void WriteAllText(string fullPath, string content, Encoding encoding)
		{
			try
			{
				File.WriteAllText(fullPath, content, encoding);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public virtual void WriteAllBytes(string fullPath, byte[] content)
		{
			try
			{
				File.WriteAllBytes(fullPath, content);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public virtual void AppendAllText(string fullPath, string content)
		{
			AppendAllText(fullPath, content, Encoding.UTF8);
		}

		public virtual void AppendAllText(string fullPath, string content, Encoding encoding)
		{
			try
			{
				File.AppendAllText(fullPath, content, encoding);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public void AppendAllLines(string fullPath, string[] lines)
		{
			AppendAllLines(fullPath, lines, Encoding.UTF8);
		}

		public virtual void AppendAllLines(string fullPath, string[] lines, Encoding encoding)
		{
			try
			{
				File.AppendAllLines(fullPath, lines, encoding);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}


		public virtual void PrintFileSystemInfo()
		{

		}
	}
}
