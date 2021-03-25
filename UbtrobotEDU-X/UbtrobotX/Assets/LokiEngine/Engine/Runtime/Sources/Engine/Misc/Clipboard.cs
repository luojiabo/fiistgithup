using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public class Clipboard
	{
		public static bool Read(out Vector3 result)
		{
			result = Vector3.zero;
			string buffer = GUIUtility.systemCopyBuffer;
			if (!string.IsNullOrEmpty(buffer) && buffer.Contains(','))
			{
				var array = buffer.Split(',');
				if (array.Length >= 3)
				{
					for (int i = 0; i < 3; ++i)
					{
						float.TryParse(array[i], out var v);
						result[i] = v;
					}
					return true;
				}
			}
			return false;
		}

		public static void Write(Vector3 v)
		{
			GUIUtility.systemCopyBuffer = string.Format("{0},{1},{2}", v.x.ToFixedString(), v.y.ToFixedString(), v.z.ToFixedString());
		}

		public static bool Read(out Quaternion result)
		{
			result = Quaternion.identity;
			string buffer = GUIUtility.systemCopyBuffer;
			if (!string.IsNullOrEmpty(buffer) && buffer.Contains(','))
			{
				var array = buffer.Split(',');
				if (array.Length >= 4)
				{
					for (int i = 0; i < 4; ++i)
					{
						float.TryParse(array[i], out var v);
						result[i] = v;
					}
					return true;
				}
			}
			return false;
		}

		public static void Write(Quaternion v)
		{
			GUIUtility.systemCopyBuffer = string.Format("{0},{1},{2},{3}", v.x.ToFixedString(), v.y.ToFixedString(), v.z.ToFixedString(), v.w.ToFixedString());
		}
	}
}
