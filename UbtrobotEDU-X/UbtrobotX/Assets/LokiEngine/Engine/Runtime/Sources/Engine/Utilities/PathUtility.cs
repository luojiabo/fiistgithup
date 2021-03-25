using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Loki
{
	public static class PathUtility
	{
		public static readonly char AltDirectorySeparatorChar = '/';
		public static readonly char DirectorySeparatorChar = '\\';
		public static readonly char PathSeparatorChar = ';';
		public static readonly char VolumeSeparatorChar = ':';
		public static readonly char ExtensionSeparatorChar = '.';

		public static string NormalizePath(string path)
		{
			return path.ToUNIXStyle();
		}

		public static string Combine(string path1, string path2)
		{
			string path = Path.Combine(path1, path2);
			return NormalizePath(path);
		}

		public static string GetRelativePath(string fullPath, string path)
		{
			path = NormalizePath(path);
			fullPath = NormalizePath(fullPath);
			return fullPath.Replace(string.Concat(path, AltDirectorySeparatorChar), string.Empty);
		}

		public static string GetExtension(string path)
		{
			return Path.GetExtension(path);
		}

		public static string GetFullPath(string path)
		{
			var fullPath = Path.GetFullPath(path);
			return NormalizePath(fullPath);
		}

		public static string GetFullPathWithoutExt(string path)
		{
			path = NormalizePath(path);
			int index = path.LastIndexOf(ExtensionSeparatorChar);
			if (index < 0)
			{
				return path;
			}
			return path.Substring(0, index);
		}

		public static string GetFileName(string path)
		{
			path = Path.GetFileName(path);
			return NormalizePath(path);
		}

		public static string GetFileNameWithoutExt(string path)
		{
			return Path.GetFileNameWithoutExtension(path);
		}

		public static string GetDirectoryName(string path)
		{
			return Path.GetDirectoryName(path);
		}

		public static string GetTempPath()
		{
			return Path.GetTempPath();
		}

		public static string GetTempFilePath(string fileName)
		{
			return Combine(Path.GetTempPath(), fileName);
		}

		public static string GetPathFromDataPath(string relativePath)
		{
			if (string.IsNullOrEmpty(relativePath))
			{
				return Application.dataPath;
			}
			return NormalizePath(Path.Combine(Application.dataPath, relativePath));
		}

		public static string GetPathFromPersistentPath(string relativePath)
		{
			if (string.IsNullOrEmpty(relativePath))
			{
				return Application.persistentDataPath;
			}
			return NormalizePath(Path.Combine(Application.persistentDataPath, relativePath));
		}

		public static string GetTopLevelPath(params string[] paths)
		{
			for (int i = 0; i < paths.Length; ++i)
			{
				for (int j = 0; j < paths.Length; ++j)
				{
					if (!paths[j].StartsWith(paths[i]))
					{
						break;
					}

					if (j == paths.Length - 1)
					{
						return paths[i];
					}
				}
			}

			return string.Empty;
		}

		public static List<string> GetFileNamesWithoutExt(string path, string[] filters, bool reverse, SearchOption option, List<string> resuleCache = null)
		{
			List<string> paths = GetFilePaths(path, filters, reverse, true, option);
			if (resuleCache == null)
			{
				resuleCache = new List<string>(paths.Count);
			}
			for (int i = 0; i < paths.Count; i++)
			{
				string p = paths[i];
				int index = p.LastIndexOf(ExtensionSeparatorChar);
				if (index < 0)
				{
					resuleCache.Add(p);
				}
				else
				{
					resuleCache.Add(p.Substring(0, index));
				}
			}
			return resuleCache;
		}

		public static List<string> GetFilesRelativePaths(string path, string[] filters, bool reverse, SearchOption option)
		{
			return GetFilePaths(path, filters, reverse, true, option);
		}

		public static List<string> GetFilePaths(string path, string[] filters, bool reverse, SearchOption option)
		{
			return GetFilePaths(path, filters, reverse, false, option);
		}

		public static List<string> GetFilePaths(string path, string[] filters, bool reverse, bool relative, SearchOption option)
		{
			List<string> filePaths = new List<string>();
			try
			{
				if (FileSystem.Get().ExistsFile(path))
				{
					path = GetFileName(path);
				}

				if (!Directory.Exists(path))
				{
					return filePaths;
				}

				List<string> fullPathList = new List<string>();

				if (filters == null ||
					filters.Length == 0)
				{
					fullPathList.AddRange(Directory.GetFiles(path, "*", option));
				}
				else
				{
					for (int i = 0; i < filters.Length; ++i)
					{
						var results = Directory.GetFiles(path, filters[i], option);
						for (int j = 0; j < results.Length; j++)
						{
							var result = results[j];
							if (!fullPathList.Contains(result))
							{
								fullPathList.Add(result);
							}
						}
					}

					if (reverse)
					{
						List<string> reverseList = new List<string>();

						string[] allFiles = Directory.GetFiles(path, "*", option);
						for (int j = allFiles.Length - 1; j >= 0; --j)
						{
							if (!fullPathList.Contains(allFiles[j]))
							{
								reverseList.Add(allFiles[j]);
							}
						}

						fullPathList = reverseList;
					}
				}

				for (int index = 0; index < fullPathList.Count; index++)
				{
					var fullPath = fullPathList[index];
					filePaths.Add(NormalizePath(relative ? fullPath.Substring(path.Length + 1) : fullPath));
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return filePaths;
		}
	}
}

