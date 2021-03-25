using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public class EngineEditorStylesSettings : UEditorAssetObject
	{
		public GUISkin guiSkin;
		public Texture2D iconX;
		public Texture2D iconY;
		public Texture2D iconZ;
		public Texture2D iconRevert;
		public Texture2D iconLocked;
		public Texture2D iconUnlocked;

		public Color personalBackgroundColor = Color.white;
		public Color proBackgroundColor = Color.white;

		public Color personalFontColor = Color.white;
		public Color proFontColor = Color.white;

		public Color backgroundColor
		{
			get
			{
				if (isProSkin)
					return proBackgroundColor;
				return personalBackgroundColor;
			}
		}

		public Color fontColor
		{
			get
			{
				if (isProSkin)
					return proFontColor;
				return personalFontColor;
			}
		}

		protected void OnEnable()
		{
			LoadAssets();
		}

#if UNITY_EDITOR
		public EngineEditorStylesSettings LoadAssets()
		{
			if (guiSkin == null)
				guiSkin = UnityEditor.AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/LokiEngine/Engine/Editor/LokiResources/uEditorGUI.guiskin");
			if (iconX == null)
				iconX = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LokiEngine/Engine/Editor/LokiResources/Transform/uEditor_X.png");
			if (iconY == null)
				iconY = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LokiEngine/Engine/Editor/LokiResources/Transform/uEditor_Y.png");
			if (iconZ == null)
				iconZ = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LokiEngine/Engine/Editor/LokiResources/Transform/uEditor_Z.png");
			if (iconRevert == null)
				iconRevert = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LokiEngine/Engine/Editor/LokiResources/Transform/uEditor_Revert_pro.png");
			if (iconLocked == null)
				iconLocked = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LokiEngine/Engine/Editor/LokiResources/Transform/uEditor_locked.png");
			if (iconUnlocked == null)
				iconUnlocked = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LokiEngine/Engine/Editor/LokiResources/Transform/uEditor_unlocked.png");
			return this;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			LoadAssets();
		}
#else
		public EngineEditorStylesSettings LoadAssets()
		{
			return this;
		}
#endif
	}
}
