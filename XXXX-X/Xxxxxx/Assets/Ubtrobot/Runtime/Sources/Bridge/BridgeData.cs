using System;
using LitJson;
using Loki;
using Ubtrobot;

namespace Ubtrobot
{
    public enum BridgeCode : int
    {
        Failure = -1,
        Success = 0,
        NorFunc = 901,
        InvokeParamError = 902,
        ReturnTypeError = 903,
        Unknown = 1000,
    }

    public class BridgeRequest
    {
        private static int requestId = 0;

        public int id;
        public string invokeMethod;
        public JsonData args;
        public string callbackMethod;

        public bool IsSync => string.IsNullOrEmpty(callbackMethod);

		public JsonData GetArgument(int idx = 0)
		{
			JsonData defResult = null;

			if (args == null) 
				return defResult;

			if (args.IsArray)
			{
				if (idx < 0 || idx >= args.Count)
					return defResult;
				return args[idx];
			}

			if (idx != 0)
				return defResult;
			return args;
		}

        public JsonData ToJsonData()
        {
            var json = new JsonData();
            json["id"] = id;
            json["func"] = invokeMethod;
            if (args != null) json["args"] = args;
            if (!string.IsNullOrEmpty(callbackMethod)) json["callback"] = callbackMethod;
            return json;
        }

        public string ToJson()
        {
            var json = ToJsonData();
            return json.ToJson();
        }

        private static JsonData ToArgs(params JsonData[] args)
        {
            if (args == null) return BridgeUtility.EmptyArray;
            var length = args.Length;
            if (length <= 0) return BridgeUtility.EmptyArray;
            var json = BridgeUtility.EmptyArray;
            for (int i = 0; i < length; i++)
            {
                json.Add(args[i]);
            }
            return json;
        }

        public static BridgeRequest Deserialize(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString)) return null;
            try
            {
                var ret = new BridgeRequest();
                var json = JsonMapper.ToObject(jsonString);
                var keys = json.Keys;

                var id = (int)json["id"];
                var invokeMethod = (string)json["func"];
                var args = keys.Contains("args") ? json["args"] : string.Empty;
                var callbackMethod = keys.Contains("callback") ? (string)json["callback"] : string.Empty;

                ret.id = id;
                ret.invokeMethod = invokeMethod;
                ret.args = args;
                ret.callbackMethod = callbackMethod;
                return ret;
            }
            catch (Exception ex)
            {
				DebugUtility.LogException(ex);
                return null;
            }
        }

        public static BridgeRequest Create(string invokeMethod, params JsonData[] args)
        {
            if (requestId >= int.MaxValue) requestId = 0;
            var id = ++requestId;

            var ret = new BridgeRequest();
            ret.id = id;
            ret.invokeMethod = invokeMethod;
            ret.args = ToArgs(args);
            return ret;
        }

        public static BridgeRequest Create(string invokeMethod, string callbackMethod, params JsonData[] args)
        {
            var ret = Create(invokeMethod, args);
            ret.callbackMethod = $"{callbackMethod}_{ret.id}";
            return ret;
        }
    }

    public class BridgeResponse
    {
        public int id;
        public int complete;
        public int code;
        public string msg;
        public string callbackMethod;
        public JsonData data;

        public bool IsComplete => complete.AsBool();

        public JsonData ToJsonData()
        {
            var json = new JsonData();
            json["id"] = id;
            json["complete"] = complete;
            json["code"] = code;
            json["msg"] = msg;
            if (!string.IsNullOrEmpty(callbackMethod)) json["callback"] = callbackMethod;
            json["data"] = data ?? BridgeUtility.EmptyObject;
            return json;
        }

        public string ToJson()
        {
            var json = ToJsonData();
            return json.ToJson();
        }

        public void SetResult(BridgeCode code, JsonData data, bool isComplete = true)
        {
            this.complete = isComplete.AsInt();
            this.code = (int)code;
            this.data = data;
        }

        public void SetFailureResult(BridgeCode code, bool isComplete = true)
        {
            SetResult(code, null, isComplete);
        }

        public void SetSuccessResult(JsonData data = null, bool isComplete = true)
        {
            SetResult(BridgeCode.Success, data, isComplete);
        }

        public static BridgeResponse Deserialize(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString)) return null;
            try
            {
                var ret = new BridgeResponse();

                var json = JsonMapper.ToObject(jsonString);
                var id = (int)json["id"];
                var complete = (int)json["complete"];
                var code = (int)json["code"];
                var keys = json.Keys;
                var msg = keys.Contains("msg") ? (string)json["msg"] : string.Empty;
                var callbackMethod = keys.Contains("callback") ? (string)json["callback"] : string.Empty;
                var data = keys.Contains("data") ? json["data"] : BridgeUtility.EmptyObject;

                ret.id = id;
                ret.complete = complete;
                ret.code = code;
                ret.msg = msg;
                ret.callbackMethod = callbackMethod;
                ret.data = data;
                return ret;
            }
            catch (Exception ex)
            {
				DebugUtility.LogException(ex);
                return null;
            }
        }

        public static BridgeResponse CreateErrorResponse(int id, BridgeCode code)
        {
            var ret = new BridgeResponse();
            ret.id = id;
            ret.complete = 1;//TODO
            ret.code = (int)code;
            ret.msg = string.Empty;
            ret.data = BridgeUtility.EmptyObject;
            return ret;
        }

        public static BridgeResponse CreateDefaultResponse(int id, string callbackMethod)
        {
            var ret = new BridgeResponse();
            ret.id = id;
            ret.complete = 1;//TODO
            ret.code = (int)BridgeCode.Unknown;
            ret.msg = string.Empty;
            ret.callbackMethod = callbackMethod;
            return ret;
        }
    }
}
