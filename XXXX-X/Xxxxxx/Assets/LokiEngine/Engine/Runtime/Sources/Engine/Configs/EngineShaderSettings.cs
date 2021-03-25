using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public enum EWireframeType
	{
		GSSolid,
		GSTransparent,
		GSSolidTransparentCulled,
	}

	[CreateAssetMenu(menuName = "Loki/Configs/Engine/Engine Shader Settings", fileName = "EngineShaderSettings", order = -999)]
	public class EngineShaderSettings : UAssetObject
	{
		[AssetPathToObject]
		public string partLibrary = "Assets/LokiEngine/Engine/Runtime/LokiResources/Shaders";

		public Shader wireframeSolid;
		public Shader wireframeTransparent;
		public Shader wireframeTransparentCulled;

		public Material wireframeSolidMat;
		public Material wireframeTransparentMat;
		public Material wireframeTransparentCulledMat;

		public Shader[] shaders;
		public Material[] materials;
	}
}
