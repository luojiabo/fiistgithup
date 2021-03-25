using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Loki
{
	public static class ZipUtility
	{
		static ZipUtility()
		{
			ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
		}

		public static void CreateZipFile(string sourcePath, string[] filters, bool reverse, string zipPath, string passward = "")
		{
			using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipPath)))
			{
				if (!string.IsNullOrEmpty(passward))
				{
					zipStream.Password = passward;
				}
				WriteZipStream(sourcePath, filters, reverse, zipStream);
				zipStream.Finish();
				zipStream.Close();
			}
		}

		public static IEnumerator CreateZipFileAsync(string sourcePath, string[] filters, bool reverse, string zipPath, string passward = "")
		{
			using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipPath)))
			{
				if (!string.IsNullOrEmpty(passward))
				{
					zipStream.Password = passward;
				}
				yield return WriteZipStreamAsync(sourcePath, filters, reverse, zipStream);
				zipStream.Finish();
				zipStream.Close();
			}
		}

		public static bool UnZipFile(byte[] zipBytes, string passward, Action<string, byte[]> callback, Predicate<string> predicate = null)
		{
			try
			{
				using (ZipInputStream s = new ZipInputStream(new MemoryStream(zipBytes)))
				{
					if (!string.IsNullOrEmpty(passward))
					{
						s.Password = passward;
					}
					if (predicate == null) predicate = (str) => true;
					ZipEntry entry;
					while ((entry = s.GetNextEntry()) != null)
					{
						if (!entry.IsFile) continue;
						var entryName = entry.Name;
						if (!predicate(entryName)) continue;
						var bytes = new byte[entry.Size];
						s.Read(bytes, 0, bytes.Length);
						Misc.SafeInvoke(callback, entryName, bytes);
					}
					s.Close();
					return true;
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Engine, "解压异常:{0}", ex);
				return false;
			}
		}

		public static IEnumerator UnZipFileAsync(byte[] zipBytes, string passward, Action<string, byte[]> callback, Predicate<string> predicate = null)
		{
			using (ZipInputStream s = new ZipInputStream(new MemoryStream(zipBytes)))
			{
				if (!string.IsNullOrEmpty(passward))
				{
					s.Password = passward;
				}
				if (predicate == null) predicate = (str) => true;
				ZipEntry entry;
				while ((entry = s.GetNextEntry()) != null)
				{
					try
					{
						if (!entry.IsFile) continue;
						var entryName = entry.Name;
						if (!predicate(entryName)) continue;
						var bytes = new byte[entry.Size];
						s.Read(bytes, 0, bytes.Length);
						Misc.SafeInvoke(callback, entryName, bytes);
					}
					catch (Exception ex)
					{
						DebugUtility.LogError(LoggerTags.Engine, "解压异常:{0}", ex);
					}
					yield return null;
				}
				s.Close();
			}
		}
		
		private static void WriteZipStream(string sourcePath, string[] filters, bool reverse, ZipOutputStream zipStream)
		{
			var files = PathUtility.GetFilePaths(sourcePath, filters, reverse, SearchOption.AllDirectories);
			for (int i = 0; i < files.Count; i++)
			{
				var file = files[i];
				if (string.IsNullOrEmpty(file)) continue;

				byte[] bytes = FileSystem.Get().ReadAllBytes(file);
				var relativePath = PathUtility.GetRelativePath(file, sourcePath);
				ZipEntry entry = new ZipEntry(relativePath);
				entry.DateTime = DateTime.MinValue;
				entry.Size = bytes.Length;
				zipStream.PutNextEntry(entry);

				zipStream.Write(bytes, 0, bytes.Length);
			}
		}

		private static IEnumerator WriteZipStreamAsync(string sourcePath, string[] filters, bool reverse, ZipOutputStream zipStream)
		{
			var files = PathUtility.GetFilePaths(sourcePath, filters, reverse, SearchOption.AllDirectories);
			for (int i = 0; i < files.Count; i++)
			{
				var file = files[i];
				if (string.IsNullOrEmpty(file)) continue;

				byte[] bytes = FileSystem.Get().ReadAllBytes(file);
				var relativePath = PathUtility.GetRelativePath(file, sourcePath);
				ZipEntry entry = new ZipEntry(relativePath);
				entry.DateTime = DateTime.MinValue;
				entry.Size = bytes.Length;
				zipStream.PutNextEntry(entry);

				zipStream.Write(bytes, 0, bytes.Length);
				yield return null;
			}
		}

	}
}