using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Loki.VisualStudio
{
	class CSProject
	{
		public string projectGuid { get; private set; }
		public string assemblyName { get; private set; }
		public string fileName { get; private set; }

		public string mainGuid { get; private set; }

		public CSProject(string file, string mainGuid)
		{
			fileName = Path.GetFileName(file);
			this.mainGuid = mainGuid;
			XmlDocument document = new XmlDocument();
			document.Load(file);
			XmlNamespaceManager nsMgr = new XmlNamespaceManager(document.NameTable);
			nsMgr.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");

			projectGuid = document.SelectSingleNode("/ns:Project/ns:PropertyGroup/ns:ProjectGuid", nsMgr).InnerText;
			assemblyName = document.SelectSingleNode("/ns:Project/ns:PropertyGroup/ns:AssemblyName", nsMgr).InnerText;

		}

		public override string ToString()
		{
			return $"Project(\"{mainGuid}\") = \"{assemblyName}\", \"{fileName}\", \"{projectGuid}\"\r\nEndProject";
		}

		public string[] ProjectDesc()
		{
			return new[]
			{
				$"Project(\"{mainGuid}\") = \"{assemblyName}\", \"{fileName}\", \"{projectGuid}\"",
				"EndProject"
			};
		}

		public string[] PostSolution()
		{
			return new[]
			{
				$"\t\t{projectGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
				$"\t\t{projectGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU",
				$"\t\t{projectGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU",
				$"\t\t{projectGuid}.Release|Any CPU.Build.0 = Release|Any CPU",
			};
		}
	}

	public class VSMenu
	{
		private static readonly Dictionary<string, CSProject> mAllProjects = new Dictionary<string, CSProject>();

		[MenuItem("Loki/VSMenu/Scan Visual Studio Projects")]
		private static void ScanProjects()
		{
			GenerateSolution(false);
		}

		[MenuItem("Loki/VSMenu/Generate new Visual Studio Solution")]
		private static void ScanProjects2()
		{
			GenerateSolution(true);
		}

		private static void GenerateSolution(bool newPath)
		{
			var projectPath = FileSystem.Get().projectPath.ToWindowsStyle();
			string slnPath = string.Concat(projectPath, "\\", projectPath.Substring(projectPath.LastIndexOf("\\") + 1), ".sln");
			DebugUtility.Log(LoggerTags.Engine, "Path {0}", slnPath);
			if (File.Exists(slnPath))
			{
				var slnText = File.ReadAllText(slnPath);
				int idx = slnText.IndexOf("Project(\"{");
				int endIdx = slnText.IndexOf("}\")", idx);
				int startIdx = idx + "Project(\"{".Length;
				var mainGuid = slnText.Substring(startIdx, endIdx - startIdx);
				DebugUtility.Log(LoggerTags.Engine, "GUID {0}", mainGuid);

				var csprojs = Directory.GetFiles(projectPath, "*.csproj");
				foreach (var item in csprojs)
				{
					if (mAllProjects.ContainsKey(item))
						continue;

					var csp = new CSProject(item, mainGuid);
					if (slnText.Contains(csp.projectGuid))
						continue;

					mAllProjects.Add(item, csp);
				}

				var reader = new StringReader(slnText);
				var slnAllLines = new List<string>();
				int lastEndProjectLineNum = 0;
				int postSolutionStart = 0;
				int postSolutionEnd = 0;
				string line;
				int lineNum = 0;
				while ((line = reader.ReadLine()) != null)
				{
					slnAllLines.Add(line);
					line = line.Trim();
					++lineNum;
					if (line == "EndProject")
					{
						lastEndProjectLineNum = lineNum;
					}
					else if (line == "GlobalSection(ProjectConfigurationPlatforms) = postSolution")
					{
						postSolutionStart = lineNum;
					}
					else if (postSolutionEnd <= 0 && line == "EndGlobalSection")
					{
						if (postSolutionStart > 0)
						{
							postSolutionEnd = lineNum;
						}
					}
				}
				DebugUtility.Log(LoggerTags.Engine, "Line {0}", lastEndProjectLineNum);
				DebugUtility.Log(LoggerTags.Engine, "postSolutionStart {0}", postSolutionStart);
				DebugUtility.Log(LoggerTags.Engine, "postSolutionEnd {0}", postSolutionEnd);

				foreach (var item in mAllProjects.Values)
				{
					var postStrs = item.PostSolution();
					slnAllLines.InsertRange(postSolutionEnd - 1, postStrs);
					postSolutionEnd += postStrs.Length;
				}

				foreach (var item in mAllProjects.Values)
				{
					var projDescs = item.ProjectDesc();
					slnAllLines.InsertRange(lastEndProjectLineNum, projDescs);
					lastEndProjectLineNum += projDescs.Length;
				}

				if (newPath)
					File.WriteAllLines(slnPath.Replace(".sln", "-generated.sln"), slnAllLines);
				else
					File.WriteAllLines(slnPath, slnAllLines);
			}
		}

	}
}
