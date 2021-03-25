#define ENABLE_LITJSON

#if ENABLE_LITJSON
using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Loki
{
	public interface ILitJson
	{
		JsonData ToJsonData();
	}

	public static partial class LitJsonHelper
	{
		public const int kJsonAccuracy = 10000;
		public const int kDefaultJsonAccuracy = 0;

		public static T[] ParseJsonArray<T>(JsonData json, Func<JsonData, T> func, T[] empty)
		{
			if (json == null)
				return empty;

			if (json.IsArray)
			{
				int length = json.Count;
				if (length == 0)
					return empty;

				T[] array = new T[length];
				for (int idx = 0; idx < length; ++idx)
				{
					array[idx] = func(json[idx]);
				}
				return array;
			}
			return empty;
		}

		public static Dictionary<string, int> ParseJsonObject(JsonData jsonData, Dictionary<string, int> resultCache = null)
		{
			return ParseJsonObject(jsonData, json => (int)json, resultCache);
		}

		public static Dictionary<string, float> ParseJsonObject(JsonData jsonData, Dictionary<string, float> resultCache = null)
		{
			return ParseJsonObject(jsonData, json => (float)(double)json, resultCache);
		}

		public static Dictionary<string, double> ParseJsonObject(JsonData jsonData, Dictionary<string, double> resultCache = null)
		{
			return ParseJsonObject(jsonData, json => (double)json, resultCache);
		}

		public static Dictionary<string, T> ParseJsonObject<T>(JsonData json, Func<JsonData, T> func, Dictionary<string, T> resultCache = null)
		{
			if (resultCache == null)
			{
				resultCache = new Dictionary<string, T>();
			}

			if (json.GetJsonType() == JsonType.Object)
			{
				var collection = json.Keys;
				foreach (var key in collection)
				{
					resultCache[key] = func(json[key]);
				}
			}

			return resultCache;
		}

		public static JsonData CreateObjectJsonData(Dictionary<string, int> dict)
		{
			var node = new JsonData();
			node.SetJsonType(JsonType.Object);
			foreach (var item in dict)
			{
				node[item.Key] = item.Value;
			}
			return node;
		}

		public static JsonData CreateObjectJsonData(Dictionary<string, float> dict, float scale = kJsonAccuracy)
		{
			var node = new JsonData();
			node.SetJsonType(JsonType.Object);
			foreach (var item in dict)
			{
				node[item.Key] = CreateJsonData(item.Value);
			}
			return node;
		}

		public static JsonData CreateArrayJsonData<T>(List<T> array)
		{
			var node = new JsonData();
			node.SetJsonType(JsonType.Array);
			int length = array != null ? array.Count : 0;
			for (int idx = 0; idx < length; idx++)
			{
				var jd = array[idx];
				if (jd is ILitJson json)
				{
					node.Add(json.ToJsonData());
				}
				else
				{
					node.Add(jd);
				}
			}
			return node;
		}

		public static JsonData CreateArrayJsonData(float[] array)
		{
			var node = new JsonData();
			node.SetJsonType(JsonType.Array);
			int length = array != null ? array.Length : 0;
			for (int idx = 0; idx < length; idx++)
			{
				node.Add((double)array[idx]);
			}
			return node;
		}

		public static JsonData CreateArrayJsonData<T>(T[] array)
		{
			var node = new JsonData();
			node.SetJsonType(JsonType.Array);
			int length = array != null ? array.Length : 0;
			for (int idx = 0; idx < length; idx++)
			{
				var jd = array[idx];
				if (jd is ILitJson json)
				{
					node.Add(json.ToJsonData());
				}
				else
				{
					node.Add(jd);
				}
			}
			return node;
		}

		public static JsonData CreateArrayJsonData(Array array)
		{
			if (array is float[])
			{
				return CreateArrayJsonData((float[])array);
			}

			var node = new JsonData();
			node.SetJsonType(JsonType.Array);
			int length = array != null ? array.Length : 0;
			for (int idx = 0; idx < length; idx++)
			{
				JsonData data = null;
				var element = array.GetValue(idx);
				if (element is float)
				{
					data = new JsonData((double)(float)element);
				}
				else if (element is ILitJson json)
				{
					data = json.ToJsonData();
				}
				else
				{
					data = new JsonData(element);
				}

				node.Add(data);
			}
			return node;
		}

		public static JsonData CreateArrayJsonData<TSrc, TOutput>(TSrc[] array, Func<TSrc, TOutput> conv)
		{
			var node = new JsonData();
			node.SetJsonType(JsonType.Array);
			int length = array != null ? array.Length : 0;
			for (int idx = 0; idx < length; idx++)
			{
				var jd = conv(array[idx]);
				if (jd is ILitJson json)
				{
					node.Add(json.ToJsonData());
				}
				else
				{
					node.Add(jd);
				}
			}
			return node;
		}

		public static JsonData CreateArrayJsonData<T>(IEnumerable<T> array)
		{
			var node = new JsonData();
			node.SetJsonType(JsonType.Array);
			if (array != null)
			{
				var it = array.GetEnumerator();
				while (it.MoveNext())
				{
					var jd = it.Current;
					if (jd is ILitJson json)
					{
						node.Add(json.ToJsonData());
					}
					else
					{
						node.Add(jd);
					}
				}
			}
			return node;
		}

		public static JsonData CreateJsonData(Vector2 v, float scaleToInt = kJsonAccuracy)
		{
			var json = new JsonData();
			json.SetJsonType(JsonType.Array);
			json.Add(CreateJsonData(v.x, scaleToInt));
			json.Add(CreateJsonData(v.y, scaleToInt));
			return json;
		}

		public static JsonData CreateJsonData(Vector3 v, float scaleToInt = kJsonAccuracy)
		{
			var json = new JsonData();
			json.SetJsonType(JsonType.Array);
			json.Add(CreateJsonData(v.x, scaleToInt));
			json.Add(CreateJsonData(v.y, scaleToInt));
			json.Add(CreateJsonData(v.z, scaleToInt));
			return json;
		}

		public static JsonData CreateJsonData(Quaternion v, float scaleToInt = kJsonAccuracy)
		{
			var json = new JsonData();
			json.SetJsonType(JsonType.Array);
			json.Add(CreateJsonData(v.x, scaleToInt));
			json.Add(CreateJsonData(v.y, scaleToInt));
			json.Add(CreateJsonData(v.z, scaleToInt));
			json.Add(CreateJsonData(v.w, scaleToInt));
			return json;
		}

		public static JsonData CreateJsonData(Vector4 v, float scaleToInt = kJsonAccuracy)
		{
			var json = new JsonData();
			json.SetJsonType(JsonType.Array);
			json.Add(CreateJsonData(v.x, scaleToInt));
			json.Add(CreateJsonData(v.y, scaleToInt));
			json.Add(CreateJsonData(v.z, scaleToInt));
			json.Add(CreateJsonData(v.w, scaleToInt));
			return json;
		}

		public static bool ParseVector(JsonData json, out Vector2 result, float scaleToInt = kJsonAccuracy)
		{
			result = default;
			if (json.IsArray)
			{
				int childCount = json.Count;
				if (childCount >= 1)
					result.x = ParseJsonToFloat(json[0], scaleToInt);
				if (childCount >= 2)
					result.y = ParseJsonToFloat(json[1], scaleToInt);
				return true;
			}
			return false;
		}

		public static bool ParseVector(JsonData json, out Vector3 result, float scaleToInt = kJsonAccuracy)
		{
			result = default;
			if (json.IsArray)
			{
				int childCount = json.Count;
				if (childCount >= 1)
					result.x = ParseJsonToFloat(json[0], scaleToInt);
				if (childCount >= 2)
					result.y = ParseJsonToFloat(json[1], scaleToInt);
				if (childCount >= 3)
					result.z = ParseJsonToFloat(json[2], scaleToInt);
				return true;
			}
			return false;
		}

		public static bool ParseVector(JsonData json, out Vector4 result, float scaleToInt = kJsonAccuracy)
		{
			result = default;
			if (json.IsArray)
			{
				int childCount = json.Count;
				if (childCount >= 1)
					result.x = ParseJsonToFloat(json[0], scaleToInt);
				if (childCount >= 2)
					result.y = ParseJsonToFloat(json[1], scaleToInt);
				if (childCount >= 3)
					result.z = ParseJsonToFloat(json[2], scaleToInt);
				if (childCount >= 4)
					result.w = ParseJsonToFloat(json[3], scaleToInt);
				return true;
			}
			return false;
		}

		public static bool ParseQuaternion(JsonData json, out Quaternion result, float scaleToInt = kJsonAccuracy)
		{
			result = default;
			if (json.IsArray)
			{
				int childCount = json.Count;
				if (childCount >= 1)
					result.x = ParseJsonToFloat(json[0], scaleToInt);
				if (childCount >= 2)
					result.y = ParseJsonToFloat(json[1], scaleToInt);
				if (childCount >= 3)
					result.z = ParseJsonToFloat(json[2], scaleToInt);
				if (childCount >= 4)
					result.w = ParseJsonToFloat(json[3], scaleToInt);
				return true;
			}
			return false;
		}

		public static JsonData CreateJsonData(float v, float scaleToInt = kJsonAccuracy)
		{
			if (scaleToInt <= 0.0f)
			{
				return new JsonData((double)v);
			}
			return new JsonData(Mathf.RoundToInt(v * scaleToInt));
		}

		public static float ParseJsonToFloat(JsonData v, float scaleToInt = kJsonAccuracy)
		{
			if (scaleToInt <= 0.0f)
			{
				return (float)(double)v;
			}
			return ((int)v / scaleToInt);
		}
	}

	public static partial class LitJsonHelper
	{
		public static bool SafeConvert(JsonData data, out float result)
		{
			result = default;
			bool success = false;
			if (data.IsDouble)
			{
				result = (float)(double)data;
				success = true;
			}
			if (data.IsBoolean)
			{
				result = (bool)data ? 1.0f : 0.0f;
				success = true;
			}
			if (data.IsInt)
			{
				result = (int)data;
				success = true;
			}
			if (data.IsLong)
			{
				result = (long)data;
				success = true;
			}
			if (data.IsString)
			{
				success = float.TryParse((string)data, out result);
			}
			return success;
		}

		public static bool SafeConvert(JsonData data, out int result)
		{
			result = default;
			bool success = false;
			if (data.IsDouble)
			{
				result = (int)(double)data;
				success = true;
			}
			if (data.IsBoolean)
			{
				result = (bool)data ? 1 : 0;
				success = true;
			}
			if (data.IsInt)
			{
				result = (int)data;
				success = true;
			}
			if (data.IsLong)
			{
				result = (int)(long)data;
				success = true;
			}
			if (data.IsString)
			{
				success = int.TryParse((string)data, out result);
			}
			return success;
		}

		public static bool SafeConvertEnum<T>(JsonData data, out T result)
		{
			result = default;
			bool success = true;
			try
			{

				if (data.IsInt)
				{
					result = (T)Enum.ToObject(typeof(T), (int)data);
				}
				else if (data.IsLong)
				{
					result = (T)Enum.ToObject(typeof(T), (long)data);
				}
				else if (data.IsString)
				{
					result = (T)Enum.Parse(typeof(T), (string)data);
				}
				else
				{
					success = false;
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				success = false;
			}
			return success;
		}

		public static bool SafeConvert(JsonData data, out long result)
		{
			result = default;
			bool success = false;
			if (data.IsDouble)
			{
				result = (long)(double)data;
				success = true;
			}
			if (data.IsBoolean)
			{
				result = (bool)data ? 1 : 0;
				success = true;
			}
			if (data.IsInt)
			{
				result = (int)data;
				success = true;
			}
			if (data.IsLong)
			{
				result = (long)data;
				success = true;
			}
			if (data.IsString)
			{
				success = long.TryParse((string)data, out result);
			}
			return success;
		}

		public static bool SafeConvert(JsonData data, out double result)
		{
			result = default;
			bool success = false;
			if (data.IsDouble)
			{
				result = (double)data;
				success = true;
			}
			if (data.IsBoolean)
			{
				result = (bool)data ? 1.0 : 0.0;
				success = true;
			}
			if (data.IsInt)
			{
				result = (int)data;
				success = true;
			}
			if (data.IsLong)
			{
				result = (long)data;
				success = true;
			}
			if (data.IsString)
			{
				success = double.TryParse((string)data, out result);
			}
			return success;
		}

		public static bool SafeConvert(JsonData data, out string result)
		{
			result = default;
			bool success = false;
			if (data.IsDouble)
			{
				result = ((double)data).ToString();
				success = true;
			}
			if (data.IsBoolean)
			{
				result = ((bool)data).ToString();
				success = true;
			}
			if (data.IsInt)
			{
				result = ((int)data).ToString();
				success = true;
			}
			if (data.IsLong)
			{
				result = ((long)data).ToString();
				success = true;
			}
			if (data.IsString)
			{
				result = (string)data;
				success = true;
			}
			return success;
		}

		public static bool SafeConvert(JsonData data, out Vector2 result)
		{
			result = default;
			if (data.IsArray)
			{
				int count = data.Count;
				if (count >= 1)
				{
					SafeConvert(data[0], out float f);
					result.x = f;
				}
				if (count >= 2)
				{
					SafeConvert(data[1], out float f);
					result.y = f;
				}
				return true;
			}

			return false;
		}

		public static bool SafeConvert(JsonData data, out Vector3 result)
		{
			result = default;
			if (data.IsArray)
			{
				int count = data.Count;
				if (count >= 1)
				{
					SafeConvert(data[0], out float f);
					result.x = f;
				}
				if (count >= 2)
				{
					SafeConvert(data[1], out float f);
					result.y = f;
				}
				if (count >= 3)
				{
					SafeConvert(data[2], out float f);
					result.z = f;
				}
				return true;
			}

			return false;
		}

		public static bool SafeConvert(JsonData data, out Vector4 result)
		{
			result = default;
			if (data.IsArray)
			{
				int count = data.Count;
				if (count >= 1)
				{
					SafeConvert(data[0], out float f);
					result.x = f;
				}
				if (count >= 2)
				{
					SafeConvert(data[1], out float f);
					result.y = f;
				}
				if (count >= 3)
				{
					SafeConvert(data[2], out float f);
					result.z = f;
				}
				if (count >= 4)
				{
					SafeConvert(data[3], out float f);
					result.w = f;
				}
				return true;
			}

			return false;
		}

		public static bool SafeConvert(JsonData data, out Quaternion result)
		{
			result = default;
			if (data.IsArray)
			{
				int count = data.Count;
				if (count >= 1)
				{
					SafeConvert(data[0], out float f);
					result.x = f;
				}
				if (count >= 2)
				{
					SafeConvert(data[1], out float f);
					result.y = f;
				}
				if (count >= 3)
				{
					SafeConvert(data[2], out float f);
					result.z = f;
				}
				if (count >= 4)
				{
					SafeConvert(data[3], out float f);
					result.w = f;
				}
				return true;
			}

			return false;
		}

		public static bool SafeConvert(JsonData data, out object result)
		{
			result = null;
			bool success = false;
			if (data.IsDouble)
			{
				result = (double)data;
				success = true;
			}
			if (data.IsBoolean)
			{
				result = (bool)data;
				success = true;
			}
			if (data.IsInt)
			{
				result = (int)data;
				success = true;
			}
			if (data.IsLong)
			{
				result = (long)data;
				success = true;
			}
			if (data.IsString)
			{
				var str = (string)data;
				if (string.IsNullOrEmpty(str))
					result = string.Empty;
				else
					result = str;
				success = true;
			}
			return success;
		}

	}

	public static partial class LitJsonHelper
	{
		public static float Fix(float v, int accuracy)
		{
			int scale = 1;
			for (int i = 0; i < accuracy; i++)
				scale *= 10;
			return (int)(v * scale) / (float)scale;
		}

		public static Vector2 Fix(Vector2 v, int accuracy)
		{
			v.x = Fix(v.x, accuracy);
			v.y = Fix(v.y, accuracy);
			return v;
		}

		public static Vector3 Fix(Vector3 v, int accuracy)
		{
			v.x = Fix(v.x, accuracy);
			v.y = Fix(v.y, accuracy);
			v.z = Fix(v.z, accuracy);
			return v;
		}

		public static Vector4 Fix(Vector4 v, int accuracy)
		{
			v.x = Fix(v.x, accuracy);
			v.y = Fix(v.y, accuracy);
			v.z = Fix(v.z, accuracy);
			v.w = Fix(v.w, accuracy);
			return v;
		}

		public static Quaternion Fix(Quaternion v, int accuracy)
		{
			v.x = Fix(v.x, accuracy);
			v.y = Fix(v.y, accuracy);
			v.z = Fix(v.z, accuracy);
			v.w = Fix(v.w, accuracy);
			return v;
		}
	}
}
#endif
