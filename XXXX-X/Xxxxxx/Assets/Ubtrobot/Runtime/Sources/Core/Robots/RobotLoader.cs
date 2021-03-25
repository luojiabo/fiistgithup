using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class RobotLoader : UComponent
	{
		[AssetPathToObject]
		[SerializeField]
		private string robotAssetPath;

		[InspectorMethod]
		public void TestAnyPath()
		{
			DebugUtility.Log(LoggerTags.Project, FileSystem.AnyPathToResourcesPath(robotAssetPath, true));
		}

		private void Start()
		{
			if (string.IsNullOrEmpty(robotAssetPath))
				return;
			
			RobotFactory.GetOrAlloc().LoadRobotFromResources(FileSystem.AnyPathToResourcesPath(robotAssetPath, true), (robot)=>
			{
				if (this == null)
				{
					Destroy(robot.gameObject);
				}
				if (robot != null)
				{
					robot.transform.SetParent(transform);
				}
			});
		}
	}
}
