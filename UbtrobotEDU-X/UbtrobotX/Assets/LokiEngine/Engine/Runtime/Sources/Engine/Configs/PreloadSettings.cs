using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	[CreateAssetMenu(menuName = "Loki/Configs/Module Config/PreloadSettings", fileName = "PreloadSettings", order = -999)]
	public class PreloadSettings : UAssetObject
	{
		public override string category { get { return "EngineConfig"; } }

		public List<string> assetReferences;

//#if UNITY_EDITOR
//		/// <summary>
//		/// Used to create asset in editor.
//		/// </summary>
//		/// <returns></returns>
//		public static PreloadSettings GetOrLoad()
//		{
//			if (!Application.isPlaying)
//				return GetOrLoad("EngineConfig/EngineSettings");
//			return null;
//		}
//#endif
	}
}
