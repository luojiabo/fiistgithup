using System;
using UnityEngine;
using System.Collections.Generic;

namespace Loki
{
	public class FPSCounter
	{
		public GUIStyle defaultNormalStyle { get; private set; } = new GUIStyle();
		public GUIStyle defaultWarningStyle { get; private set; } = new GUIStyle();

		public float updateInterval { get; private set; } = 0.5f;
		public float lastInterval { get; private set; }
		public int frames { get; private set; }
		public float fps { get; private set; }
		public float warningFPS { get; set; } = 30.0f;
		public Vector2 size { get; set; } = new Vector2(60.0f, 30.0f);

		public void OnInitialize()
		{
			defaultNormalStyle.fontSize = 20;
			defaultNormalStyle.normal.textColor = Color.green;
			defaultWarningStyle.fontSize = 20;
			defaultWarningStyle.normal.textColor = Color.red;
			lastInterval = Time.realtimeSinceStartup;
			frames = 0;
		}

		public void OnUpdate(float deltaTime)
		{
			++frames;
			float timeNow = Time.realtimeSinceStartup;
			if (timeNow >= lastInterval + updateInterval)
			{
				fps = frames / (timeNow - lastInterval);
				frames = 0;
				lastInterval = timeNow;
			}
		}

		public void OnGUI(Rect position)
		{
			GUI.Label(position, fps.ToFixedString("#0.00"), fps <= warningFPS ? defaultWarningStyle : defaultNormalStyle);
		}
	}
}
