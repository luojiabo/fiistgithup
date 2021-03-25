using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ubtrobot
{
	[CreateAssetMenu(menuName = "Loki/Configs/Project/Ubtrobot Icons Library", fileName = "IconsLibrary", order = -999)]
	public class IconSettings : UAssetObject
	{
		[AssetPathToObject]
		public string partLibrary = "Assets/Updatable/IconsLibrary";

		

	}
}
