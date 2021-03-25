using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	[CreateAssetMenu(menuName = "Loki/Configs/Engine/Engine Settings", fileName = "EngineSettings", order = -999)]
	public class EngineSettings : ModuleSettings<EngineSettings>
	{
		public override string category { get { return "EngineConfig"; } }

		public ReflectionBlacklistSettings reflectionBlacklist;
		public LogSettings loggerSettings;
		public PackageSettings packageSettings;
		public InputSystemSettings inputSystemSettings;
		public EngineShaderSettings shaderSetings;

		public static EngineSettings GetOrLoad()
		{
			return GetOrLoad("EngineConfig/EngineSettings");
		}

		public override string GetAddressName()
		{
			return "EngineConfig/EngineSettings";
		}
	}
}
