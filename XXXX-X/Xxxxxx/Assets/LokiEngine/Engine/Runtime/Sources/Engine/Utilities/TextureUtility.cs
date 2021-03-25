using System;
using UnityEngine;
using System.Collections.Generic;

namespace Loki
{
	public class TextureUtility
	{
		private static readonly Dictionary<string, Texture2D> msMemoryTextures = new Dictionary<string, Texture2D>();

		public static Texture2D blackground
		{
			get
			{
				return TextureUtility.GetSharedTexture("Black2x2_A0.5", 2, 2, Color.black.Alpha(0.5f));
			}
		}

		public static Texture2D GetSharedTexture(string uniqueName, int width, int height, Color color, TextureFormat format = TextureFormat.BGRA32)
		{
			if (msMemoryTextures.TryGetValue(uniqueName, out var tex2D))
			{
				return tex2D;
			}

			tex2D = new Texture2D(width, height, format, false);
			msMemoryTextures[uniqueName] = tex2D;
			return tex2D;
		}
	}
}
