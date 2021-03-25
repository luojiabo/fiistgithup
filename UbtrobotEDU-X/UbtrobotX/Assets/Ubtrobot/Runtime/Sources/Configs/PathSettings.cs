using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	[CreateAssetMenu(menuName = "Loki/Configs/Project/Ubtrobot Path Settings/PathSettings", fileName = "PathSettings", order = -999)]
	public class PathSettings : UAssetObject
	{
		[AssetPathToObject]
		public string labModelExportPath;

		public override bool isSubAsset
		{
			get
			{
				return true;
			}
		}

		[InspectorMethod(aliasName = "Default Settings")]
		public void Default()
		{
			labModelExportPath = FileSystem.Get().GetAssetPathCheck(EFilePathType.EngineDefaultResources);
		}

		public string GetExportFullPath()
		{
			if (string.IsNullOrEmpty(labModelExportPath))
			{
				return FileSystem.Get().GetFullPath(EFilePathType.EngineDefaultResources);
			}
			return FileSystem.AssetPathToFullPath(labModelExportPath);
		}

		public string GetExportAssetPath()
		{
			if (string.IsNullOrEmpty(labModelExportPath))
			{
				return FileSystem.Get().GetAssetPathCheck(EFilePathType.EngineDefaultResources);
			}
			return labModelExportPath;
		}
	}
}
