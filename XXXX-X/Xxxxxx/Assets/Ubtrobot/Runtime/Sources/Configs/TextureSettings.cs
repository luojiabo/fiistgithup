using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ubtrobot
{
	[CreateAssetMenu(menuName = "Loki/Configs/Project/Ubtrobot Textures Library", fileName = "TexturesLibrary", order = -999)]
	public class TextureSettings : UAssetObject
	{
		[AssetPathToObject]
		public string partLibrary = "Assets/Updatable/TexturesLibrary";


	}
}
