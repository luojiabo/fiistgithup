using System.Text;
using UnityEngine;
using Loki;
using System;

using JsonData = LitJson.JsonData;
using JsonMapper = LitJson.JsonMapper;

namespace Ubtrobot
{
	public enum ExploreProtocolDataType
	{
		DTObject,
		DTInt,
		DTFloat,
	}

	public class ExploreProtocol : IProtocol
	{
		/// <summary>
		/// The protocol version communication form scratch to explore
		/// </summary>
		public static readonly Version CurrentScratchToExploreVersion = new Version(1, 7, 0);

		public string host { get; set; }
		public int device { get; set; }
		public int mode { get; set; }
		public int id { get; set; }
		public int code { get; set; }
		public string uuid { get; set; }
		public bool debug { get; set; }

		public bool res { get; set; } = false;

		/// <summary>
		/// The datas is array
		/// </summary>
		public Array datas { get; set; }

		public JsonData userData { get; set; }

		public ExploreProtocolDataType dataType
		{
			get; set;
		}

		public static ExploreProtocol Create(ExploreProtocolDataType type, byte[] jsonData, int offset, int length, bool encryption, string context = "")
		{
			string json = Encoding.UTF8.GetString(jsonData, offset, length);
			return Create(type, json, context);
		}

		public static ExploreProtocol Create(ExploreProtocolDataType type, string jsonStr, string context = "")
		{
			try
			{
				var node = JsonMapper.ToObject(jsonStr);
				if (node != null)
				{
					int device = -1;
					if (node.ContainsKey("device"))
						device = (int)node["device"];

					int mode = -1;
					if (node.ContainsKey("mode"))
						mode = (int)node["mode"];

					int id = -1;
					if (node.ContainsKey("id"))
						id = (int)node["id"];

					int code = -1;
					if (node.ContainsKey("code"))
						code = (int)node["code"];

					string uuid = string.Empty;
					if (node.ContainsKey("uuid"))
						uuid = ((string)node["uuid"]);//.ToLower();

					bool debug = false;
					if (node.ContainsKey("debug"))
						debug = (int)node["debug"] == 1;

					ExploreProtocol p = new ExploreProtocol();
					p.dataType = type;
					p.device = device;
					p.mode = mode;
					p.id = id;
					p.code = code;
					p.uuid = uuid;
					p.debug = debug;

					if (node.ContainsKey("data"))
					{
						var datas = node["data"]; // it's array of int
						switch (type)
						{
							case ExploreProtocolDataType.DTObject:
								{
									p.datas = LitJsonHelper.ParseJsonArray(datas, jsonParam =>
									{
										LitJsonHelper.SafeConvert(jsonParam, out object result);
										return result;
									}, ArrayHelper<object>.Empty);
									break;
								}
							case ExploreProtocolDataType.DTInt:
								{
									p.datas = LitJsonHelper.ParseJsonArray(datas, jsonParam =>
									{
										LitJsonHelper.SafeConvert(jsonParam, out int result);
										return result;
									}, ArrayHelper<int>.Empty);
									break;
								}
							case ExploreProtocolDataType.DTFloat:
								{
									p.datas = LitJsonHelper.ParseJsonArray(datas, jsonParam =>
									{
										LitJsonHelper.SafeConvert(jsonParam, out float result);
										return result;
									}, ArrayHelper<float>.Empty);
									break;
								}
						}

					}
					return p;
				}
			}
			catch (System.Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Module, "Failure to create json data , Context : {0}, Stack : {1}", context, ex.StackTrace);
			}
			return null;
		}

		private static ExploreProtocol CreateResponse(ProtocolCode code, int device, int mode, int id, string uuid)
		{
			var protocol = new ExploreProtocol();
			protocol.dataType = ExploreProtocolDataType.DTObject;
			protocol.code = code == ProtocolCode.Success ? 0 : 1;
			protocol.device = device;
			protocol.mode = mode;
			protocol.id = id;
			protocol.uuid = uuid;
			protocol.res = true;
			return protocol;
		}

		public static ExploreProtocol CreateResponse(ProtocolCode code, ICommand command)
		{
			var protocol = CreateResponse(code, command.device, command.cmdMode, command.id, command.uuid);
			return protocol;
		}

		public static ExploreProtocol CreateResponse(ProtocolCode code, ICommand command, params object[] values)
		{
			var protocol = CreateResponse(code, command.device, command.cmdMode, command.id, command.uuid);
			protocol.SetDatas(values);
			return protocol;
		}

		public ExploreProtocol()
		{

		}

		public void AllocDatas(int length)
		{
			datas = new object[length];
		}

		public void SetDatas<T>(params T[] values)
		{
			int length = values.Length;
			if (datas == null || datas.Length != length)
			{
				datas = new T[length];
			}
			Array.Copy(values, 0, datas, 0, length);
		}

		public bool GetParam(int index, out int param)
		{
			param = 0;
			if (datas == null)
				return false;
			if (index < 0 || index >= datas.Length)
				return false;

			bool result = false;
			if (datas is float[])
			{
				var array = (float[])datas;
				param = (int)array[index];
				result = true;
			}
			else if (datas is int[])
			{
				var array = (int[])datas;
				param = (int)array[index];
				result = true;
			}
			else
			{
				var o = datas.GetValue(index);
				result = Misc.SafeUnboxing(o, out param);
			}
			return result;
		}

		public bool GetParamf(int index, out float param)
		{
			param = 0.0f;
			if (datas == null)
				return false;
			if (index < 0 || index >= datas.Length)
				return false;

			bool result = false;
			if (datas is float[])
			{
				var array = (float[])datas;
				param = (float)array[index];
				result = true;
			}
			else if (datas is int[])
			{
				var array = (int[])datas;
				param = (float)array[index];
				result = true;
			}
			else
			{
				var o = datas.GetValue(index);
				result = Misc.SafeUnboxing(o, out param);
			}
			return result;
		}

		public bool GetParamAsVec(int startIndex, out Vector2 vec)
		{
			vec = Vector2.zero;
			if (GetParamf(startIndex, out var v0) && GetParamf(startIndex + 1, out var v1))
			{
				vec.x = v0;
				vec.y = v1;
				return true;
			}
			return false;
		}

		public bool GetParamAsVec(int startIndex, out Vector2Int vec)
		{
			vec = Vector2Int.zero;
			if (GetParam(startIndex, out var v0) && GetParam(startIndex + 1, out var v1))
			{
				vec.x = v0;
				vec.y = v1;
				return true;
			}
			return false;
		}

		public bool GetParamAsVec(int startIndex, out Vector3Int vec)
		{
			vec = Vector3Int.zero;
			if (GetParam(startIndex, out var v0) && GetParam(startIndex + 1, out var v1) && GetParam(startIndex + 2, out var v2))
			{
				vec.x = v0;
				vec.y = v1;
				vec.z = v2;
				return true;
			}
			return false;
		}

		public bool GetParamAsRGB(int startIndex, out Color color)
		{
			color = Color.white;
			if (GetParam(startIndex, out var v0) && GetParam(startIndex + 1, out var v1) && GetParam(startIndex + 2, out var v2))
			{
				color.r = v0 / 255.0f;
				color.g = v1 / 255.0f;
				color.b = v2 / 255.0f;
				color.a = 1.0f;
				return true;
			}
			return false;
		}

		public bool GetParamAsRGBA(int startIndex, out Color color)
		{
			color = Color.white;
			if (GetParam(startIndex, out var v0) && GetParam(startIndex + 1, out var v1) && GetParam(startIndex + 2, out var v2) && GetParam(startIndex + 3, out var v3))
			{
				color.r = v0 / 255.0f;
				color.g = v1 / 255.0f;
				color.b = v2 / 255.0f;
				color.a = v3 / 255.0f;
				return true;
			}
			return false;
		}

		public bool GetParamAsVec(int startIndex, out Vector3 vec)
		{
			vec = Vector3.zero;
			if (GetParamf(startIndex, out var v0) && GetParamf(startIndex + 1, out var v1) && GetParamf(startIndex + 2, out var v2))
			{
				vec.x = v0;
				vec.y = v1;
				vec.z = v2;
				return true;
			}
			return false;
		}

		public JsonData ToJsonData(JsonData cache = null)
		{
			if (userData != null)
			{
				return userData;
			}

			if (cache == null)
				cache = new JsonData();

			cache["device"] = device;
			cache["mode"] = mode;
			cache["id"] = id;
			cache["code"] = code;
			cache["uuid"] = uuid;
			if (datas != null && datas.Length > 0)
			{
				cache["data"] = LitJsonHelper.CreateArrayJsonData(datas);
			}
			if (res)
			{
				cache["sendTo"] = "uCode";
			}

			return cache;
		}

		public void ToBytes(ProtocolOutput output, out bool encryption, out byte[] bytes)
		{
			encryption = false;
			bytes = null;

			if (output == ProtocolOutput.ExploreToScratch)
			{
				var json = ToString();
				bytes = Encoding.UTF8.GetBytes(json);
			}
		}

		public void ToString(ProtocolOutput output, out string result)
		{
			result = "";
			if (output == ProtocolOutput.ExploreToScratch)
			{
				result = ToString();
			}
		}

		public override string ToString()
		{
			return ToJsonData().ToJson();
		}
	}
}
