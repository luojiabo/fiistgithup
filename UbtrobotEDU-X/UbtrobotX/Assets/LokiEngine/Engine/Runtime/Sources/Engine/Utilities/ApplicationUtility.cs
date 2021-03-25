using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public static class ApplicationUtility
	{
		public static FastEnumEqualityComparer<RuntimePlatform> runtimePlatformComparer { get; private set; }

		/// <summary>
		//  Returns application identifier at runtime. On Apple platforms this is the 'bundleIdentifier'
		//  saved in the info.plist file, on Android it's the 'package' from the AndroidManifest.xml.
		/// </summary>
		public static string identifier
		{
			get
			{
				return Application.identifier;
			}
		}

		public static float screenWidth
		{
			get
			{
				return Screen.width;
			}
		}

		public static float screenHeight
		{
			get
			{
				return Screen.height;
			}
		}

		public static Vector2 screenCenterPosition
		{
			get
			{
				return new Vector2(screenWidth / 2, screenHeight / 2);
			}
		}

		public static Vector2 screenSize
		{
			get
			{
				return new Vector2(screenWidth, screenHeight);
			}
		}

		public static Vector2 halfScreenSize
		{
			get
			{
				return new Vector2(screenWidth / 2, screenHeight / 2);
			}
		}

		public static float halfScreenWidth
		{
			get
			{
				return screenWidth / 2.0f;
			}
		}

		public static float halfScreenHeight
		{
			get
			{
				return screenHeight / 2.0f;
			}
		}

		public static Rect screenRect
		{
			get
			{
				return new Rect(Vector2.zero, screenSize);
			}
		}

		public static Rect halfScreenRect
		{
			get
			{
				return new Rect(Vector2.zero, halfScreenSize);
			}
		}

		static ApplicationUtility()
		{
			runtimePlatformComparer = new FastEnumEqualityComparer<RuntimePlatform>((lhs, rhs) => lhs == rhs, platform => platform.GetHashCode());
		}

		private static readonly RuntimePlatform[] msWindowsPlatform =
		{
			RuntimePlatform.WindowsPlayer,
			RuntimePlatform.WindowsEditor,
		};

		public static bool IsWindowsPlatform()
		{
			if (msWindowsPlatform.Contains(Application.platform, runtimePlatformComparer))
			{
				return true;
			}
			return false;
		}

	}
}
