using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	[CreateAssetMenu(menuName = "Loki/Configs/Project/Ubtrobot Settings", fileName = "UbtrobotSettings", order = -999)]
	public class UbtrobotSettings : ModuleSettings<UbtrobotSettings>
	{
		public override string category { get { return "ProjectConfig"; } }

		#region The shared materials
		public NumMaterialSettings numMaterials;
		#endregion


		public PathSettings pathSettings;
		public PartsLibrary partsLibrary;
		public GizmosSettings gizmosSettings;
		public IconSettings iconSettings;
		public TextureSettings textureSettings;
		public SelectionSettings selectionSettings;
		public HostSettings hostSettings;

		public static UbtrobotSettings GetOrLoad()
		{
			return GetOrLoad("ProjectConfig/UbtrobotSettings");
		}

		public override string GetAddressName()
		{
			return "ProjectConfig/UbtrobotSettings";
		}
	}
}
