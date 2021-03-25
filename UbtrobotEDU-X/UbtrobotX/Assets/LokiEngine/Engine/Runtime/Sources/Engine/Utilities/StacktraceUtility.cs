using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace Loki
{
	public static class StacktraceUtility
	{
		private static readonly List<string> msCacheArray = new List<string>(8);

		public static string GetCurrentFile(string def = "")
		{
			return GetFile(2, 0, def);
		}

		public static string GetCurrentDirectory(bool keepAlt = true, string def = "")
		{
			string file = GetFile(2, 0, def);
			int idx = file.LastIndexOfAny(new[] { '\\', '/' });
			if (idx <= 0 || idx == file.Length - 1)
			{
				return file;
			}
			if (!keepAlt)
				return file.Substring(0, idx);
			return file.Substring(0, idx + 1);
		}

		public static string GetFile(int skipFrames, int index = 0, string def = "")
		{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			var st = new StackTrace(skipFrames, true);
			if (st.FrameCount > index)
			{
				var sfs = st.GetFrame(index);
				var file = sfs.GetFileName();
				if (string.IsNullOrEmpty(file))
				{
					return def;
				}
				return FileSystem.FullPathToAssetPath(file);
			}
#endif
			return def;
		}

		public static string GetStackFrameInfo(this StackFrame sfs)
		{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			if (sfs == null)
				return string.Empty;

			var method = sfs.GetMethod();
			var parameters = method.GetParameters();
			msCacheArray.Clear();
			foreach (var p in parameters)
			{
				msCacheArray.Add(p.ParameterType.ToString());
			}

			return string.Concat(
				method.DeclaringType.ToString(), ":",
				method.Name, "(", string.Join(",", msCacheArray.ToArray()), ")",
				" (at ", FileSystem.FullPathToAssetPath(sfs.GetFileName()), ":", sfs.GetFileLineNumber(), ") ");
#else
			return string.Empty;
#endif
		}
	}
}
