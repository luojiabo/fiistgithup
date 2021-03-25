using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loki
{
	/// <summary>
	/// The convert mode of euler angle
	/// </summary>
	public enum EEulerAngleConvertMode
	{
		N360P0,
		N0P360,
		NP180,
		NP360,
	}

	public static partial class Misc
	{
		private static readonly Dictionary<Type, Func<object>> msDefaultTypes = new Dictionary<Type, Func<object>>
		{
			{ typeof(string), ()=> "" },

		};

		public static T Default<T>(Type type)
		{
			return (T)Default(type);
		}

		public static void Register(Type type, Func<object> def)
		{
			msDefaultTypes[type] = def;
		}

		public static TMostDerived Default<TMostDerived>()
		{
			var type = typeof(TMostDerived);
			TMostDerived value = default(TMostDerived);
			if (msDefaultTypes.TryGetValue(type, out var func))
			{
				value = (TMostDerived)func();
			}
			else if (type.IsClass)
			{
				value = (TMostDerived)Activator.CreateInstance(type);
			}

			if (value == null)
			{
				throw new Exception(string.Format("The value is null, The Type {0} must support the Default constructor.", type.Name));
			}
			return value;
		}

		public static object Default(Type type)
		{
			object value = null;
			if (msDefaultTypes.TryGetValue(type, out var func))
			{
				value = func();
			}
			else if (type.IsEnum)
			{
				Array enumValues = Enum.GetValues(type);
				if (enumValues.Length > 0)
				{
					return enumValues.GetValue(0);
				}
				return null;
			}
			else if (type.IsClass)
			{
				value = Activator.CreateInstance(type);
			}
			else
			{
				value = default;
			}

			if (value == null)
			{
				throw new Exception(string.Format("The value is null, The Type {0} must support the Default constructor.", type.Name));
			}
			return value;
		}

		public static string RemoveAssetExtension(string assetPath)
		{
			int dotIdx = assetPath.LastIndexOf('.');
			if (dotIdx >= 0)
			{
				return assetPath.Substring(0, dotIdx);
			}
			return assetPath;
		}

		public static string RemoveAssetExtension(string assetPath, int startIndex)
		{
			int dotIdx = assetPath.LastIndexOf('.', startIndex);
			if (dotIdx >= 0)
			{
				return assetPath.Substring(0, dotIdx);
			}
			return assetPath;
		}

		public static int GetCurrentManagedThreadID()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T temp = a;
			a = b;
			b = temp;
		}

		public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
		{
			return new Vector3(
				Mathf.Clamp(value.x, min.x, max.x),
				Mathf.Clamp(value.y, min.y, max.y),
				Mathf.Clamp(value.z, min.z, max.z));
		}

		public static Transform Find(string hierarchyPath)
		{
			if (string.IsNullOrEmpty(hierarchyPath))
			{
				return null;
			}

			if (hierarchyPath[0] == '/')
			{
				var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
				int idx = hierarchyPath.IndexOf('/', 1);
				if (idx > 1)
				{
					string firstNode = hierarchyPath.Substring(1, idx - 1);
					foreach (var root in rootObjects)
					{
						if (root.name == firstNode)
						{
							var target = root.transform.Find(hierarchyPath);
							if (target != null)
								return target;
						}
					}
				}
			}
			return null;
		}
	}

	public static partial class Misc
	{
		/// <summary>
		/// Convert the angle to another presentation
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="angle">[0, 360]</param>
		/// <returns></returns>
		public static float Convert(EEulerAngleConvertMode mode, float angle)
		{
			float result = angle;
			switch (mode)
			{
				case EEulerAngleConvertMode.NP180:
					{
						if (angle > 180.0f)
						{
							result = angle - 360;
						}
						else if (angle < -180.0f)
						{
							result = angle + 360.0f;
						}
						break;
					}
				case EEulerAngleConvertMode.NP360:
					{
						break;
					}
				case EEulerAngleConvertMode.N360P0:
					{
						result = angle - 360.0f;
						break;
					}
				case EEulerAngleConvertMode.N0P360:
					{
						break;
					}
			}
			return result;
		}

		/// <summary>
		/// Convert angle to range [-180.0f, 180.0f]
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static float LimitAngleNP180(float angle)
		{
			angle = FixAngle(angle);
			if (angle > 180.0f)
			{
				angle -= 180.0f;
			}
			return angle;
		}

		/// <summary>
		/// Convert angle to range [0, 360.0f]
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static float FixAngle(float angle)
		{
			if (angle < 0.0f)
			{
				angle = ((angle % 360.0f) + 360.0f) % 360.0f;
			}
			else if (angle > 0.0f)
			{
				angle = (angle % 360.0f);
			}
			return angle;
		}

		public static Vector3 FixAngle(Vector3 angle)
		{
			return new Vector3(FixAngle(angle.x), FixAngle(angle.y), FixAngle(angle.z));
		}

		public static int To01(this bool b, bool asOne = true)
		{
			return b == asOne ? 1 : 0;
		}

		public static bool ToBool(this int v, int asTrue = 1)
		{
			return v == asTrue;
		}

	}

	public static partial class Misc
	{
		public static bool Nearly(float v1, float v2, float epsilon = 0.00001f)
		{
			if (Mathf.Abs(v1 - v2) <= epsilon)
			{
				return true;
			}
			return false;
		}

		public static bool Nearly(Color src, Color target)
		{
			if (!Nearly(src.r, target.r, 0.01f))
			{
				return false;
			}
			if (!Nearly(src.g, target.g, 0.01f))
			{
				return false;
			}
			if (!Nearly(src.b, target.b, 0.01f))
			{
				return false;
			}
			return true;
		}

		public static bool Nearly(Vector3 v1, Vector3 v2)
		{
			if ((v1 - v2).magnitude <= 0.00001f)
			{
				return true;
			}
			return false;
		}

		public static bool NearlyZero(this float v1)
		{
			return Nearly(v1, 0.0f);
		}

		public static bool NearlyZero(this Vector3 v)
		{
			if (v.magnitude <= 0.000001f)
			{
				return true;
			}
			return false;
		}

		public static bool NearlyZero(this Vector3 v, float epsilon)
		{
			if (v.magnitude <= epsilon)
			{
				return true;
			}
			return false;
		}
	}

	public static partial class Misc
	{
		public static bool SafeUnboxing(object src, out int value)
		{
			value = 0;
			if (src is int)
			{
				value = (int)src;
				return true;
			}

			if (src is float)
			{
				value = (int)(float)src;
				return true;
			}

			if (src is double)
			{
				value = (int)(double)src;
				return true;
			}

			if (src is long)
			{
				value = (int)(long)src;
				return true;
			}

			if (src is ulong)
			{
				value = (int)(long)src;
				return true;
			}

			if (src is ushort)
			{
				value = (int)(long)src;
				return true;
			}

			if (src is uint)
			{
				value = (int)(long)src;
				return true;
			}

			if (src is char)
			{
				value = (int)(char)src;
				return true;
			}

			if (src is byte)
			{
				value = (int)(byte)src;
				return true;
			}

			if (src is bool)
			{
				value = (int)((bool)src ? 1 : 0);
				return true;
			}

			if (src is string)
			{
				return int.TryParse((string)src, out value);
			}

			return false;
		}

		public static bool SafeUnboxing(object src, out float value)
		{
			value = 0;
			if (src is int)
			{
				value = (float)(int)src;
				return true;
			}

			if (src is float)
			{
				value = (float)(float)src;
				return true;
			}

			if (src is double)
			{
				value = (float)(double)src;
				return true;
			}

			if (src is long)
			{
				value = (float)(long)src;
				return true;
			}

			if (src is ulong)
			{
				value = (float)(long)src;
				return true;
			}

			if (src is ushort)
			{
				value = (float)(long)src;
				return true;
			}

			if (src is uint)
			{
				value = (float)(long)src;
				return true;
			}

			if (src is char)
			{
				value = (float)(char)src;
				return true;
			}

			if (src is byte)
			{
				value = (float)(byte)src;
				return true;
			}

			if (src is bool)
			{
				value = (float)((bool)src ? 1.0f : 0.0f);
				return true;
			}

			if (src is string)
			{
				return float.TryParse((string)src, out value);
			}

			return false;
		}

		public static bool SafeUnboxing(object src, out string value)
		{
			value = src.ToString();
			return true;
		}

		public static bool SafeUnboxing(object src, out long value)
		{
			value = 0;
			if (src is int)
			{
				value = (long)(int)src;
				return true;
			}

			if (src is float)
			{
				value = (long)(float)src;
				return true;
			}

			if (src is double)
			{
				value = (long)(double)src;
				return true;
			}

			if (src is long)
			{
				value = (long)(long)src;
				return true;
			}

			if (src is ulong)
			{
				value = (long)(long)src;
				return true;
			}

			if (src is ushort)
			{
				value = (long)(long)src;
				return true;
			}

			if (src is uint)
			{
				value = (long)(long)src;
				return true;
			}

			if (src is char)
			{
				value = (long)(char)src;
				return true;
			}

			if (src is byte)
			{
				value = (long)(byte)src;
				return true;
			}

			if (src is bool)
			{
				value = (long)((bool)src ? 1 : 0);
				return true;
			}

			if (src is string)
			{
				return long.TryParse((string)src, out value);
			}

			return false;
		}

		public static bool SafeUnboxing(object src, out double value)
		{
			value = 0;
			if (src is int)
			{
				value = (double)(int)src;
				return true;
			}

			if (src is float)
			{
				value = (double)(float)src;
				return true;
			}

			if (src is double)
			{
				value = (double)(double)src;
				return true;
			}

			if (src is long)
			{
				value = (double)(long)src;
				return true;
			}

			if (src is ulong)
			{
				value = (double)(long)src;
				return true;
			}

			if (src is ushort)
			{
				value = (double)(long)src;
				return true;
			}

			if (src is uint)
			{
				value = (double)(long)src;
				return true;
			}

			if (src is char)
			{
				value = (double)(char)src;
				return true;
			}

			if (src is byte)
			{
				value = (double)(byte)src;
				return true;
			}

			if (src is bool)
			{
				value = (double)((bool)src ? 1 : 0);
				return true;
			}

			if (src is string)
			{
				return double.TryParse((string)src, out value);
			}

			return false;
		}
	}

	public static partial class Misc
	{
		public static float ToGray(Color color)
		{
			return ToGray(color.r, color.g, color.b);
		}

		public static float ToGray(float r, float g, float b)
		{
			// Gray = R*0.299 + G*0.587 + B*0.114
			return r * 0.299f + g * 0.587f + b * 0.114f;
		}

		public static bool PickedColor(RaycastHit hitInfo, out Color color)
		{
			return PickedColor(hitInfo.collider.GetComponent<Renderer>(), hitInfo.textureCoord, out color);
		}

		public static bool PickedColorInChildren(RaycastHit hitInfo, Renderer renderer, out Color color)
		{
			return PickedColor(renderer, hitInfo.textureCoord, out color);
		}

		public static bool PickedColor(Renderer renderer, Vector2 textureCoord, out Color color)
		{
			color = Color.white;
			if (renderer == null)
				return false;

			if (renderer.sharedMaterial == null)
				return false;

			if (renderer.sharedMaterial.mainTexture == null)
				return false;

			var mainTexture = renderer.sharedMaterial.mainTexture;
			var tiling = renderer.sharedMaterial.mainTextureScale;
			var tex = mainTexture as Texture2D;
			if (tex == null)
				return false;

			var pos = new Vector2(textureCoord.x * tex.width * tiling.x, textureCoord.y * tex.height * tiling.y);
			color = tex.GetPixel(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), 0);
			return true;
		}
	}

	public static partial class Misc
	{
		public static void SafeInvoke(Action invoke)
		{
			try
			{
				if (invoke != null)
				{
					invoke();
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public static void SafeInvoke<T>(Action<T> invoke, T arg0)
		{
			try
			{
				if (invoke != null)
				{
					invoke(arg0);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public static void SafeInvoke<T0, T1>(Action<T0, T1> invoke, T0 arg0, T1 arg1)
		{
			try
			{
				if (invoke != null)
				{
					invoke(arg0, arg1);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}
	}

	public static partial class Misc
	{
		public static float Fix(this float v, int accuracy)
		{
			int scale = 1;
			for (int i = 0; i < accuracy; i++)
				scale *= 10;
			return (int)(v * scale) / (float)scale;
		}

		public static Vector2 Fix(this Vector2 v, int accuracy)
		{
			v.x = v.x.Fix(accuracy);
			v.y = v.y.Fix(accuracy);
			return v;
		}

		public static Vector3 Fix(this Vector3 v, int accuracy)
		{
			v.x = v.x.Fix(accuracy);
			v.y = v.y.Fix(accuracy);
			v.z = v.z.Fix(accuracy);
			return v;
		}

		public static Vector4 Fix(this Vector4 v, int accuracy)
		{
			v.x = v.x.Fix(accuracy);
			v.y = v.y.Fix(accuracy);
			v.z = v.z.Fix(accuracy);
			v.w = v.w.Fix(accuracy);
			return v;
		}

		public static Quaternion Fix(this Quaternion v, int accuracy)
		{
			v.x = v.x.Fix(accuracy);
			v.y = v.y.Fix(accuracy);
			v.z = v.z.Fix(accuracy);
			v.w = v.w.Fix(accuracy);
			return v;
		}
	}
}
