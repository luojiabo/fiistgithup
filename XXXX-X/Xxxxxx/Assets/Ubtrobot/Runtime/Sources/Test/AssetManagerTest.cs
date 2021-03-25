using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class AssetManagerTest : IConsoleObject
	{
		public string statID { get { return "AssetManagerTest"; } }
		public string name { get { return "AssetManagerTest"; } }


		[ConsoleMethod(aliasName = "Test.LoadAssetAuto")]
		public void LoadAssetAuto()
		{
			//var tex = AssetManager.GetOrAlloc().LoadAsset<Texture2D>("Assets/Projects/Art/FBX/Textures/A.png");
			//DebugUtility.Log(statID, "Texture : " + tex);
			//var so = AssetManager.GetOrAlloc().LoadAsset<EngineSettings>("Assets/Resources/EngineConfig/EngineSettings.asset");
			//DebugUtility.Log(statID, "EngineConfigSettings  : " + so);
		}
	}
}
