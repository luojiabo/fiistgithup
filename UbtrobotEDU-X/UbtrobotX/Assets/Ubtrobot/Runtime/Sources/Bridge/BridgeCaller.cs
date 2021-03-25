using System;
using LitJson;
using Loki;

namespace Ubtrobot
{
    public abstract class BridgeCaller
    {
        protected abstract string InvokePlatform(string args);
        protected abstract void CallBackPlatform(string result);

        protected abstract void PushListener(string key, Action<BridgeResponse> method);
        protected abstract void PushListener(CallUnityMethod key, Action<BridgeRequest, BridgeResponse> method);
        public abstract Action<BridgeResponse> PullListener(string key, bool remove);
        public abstract Action<BridgeRequest, BridgeResponse> PullListener(CallUnityMethod key);

        protected BridgeResponse SyncInvokePlatform(CallPlatformMethod invokeMethod, params JsonData[] args)
        {
            var request = BridgeRequest.Create(invokeMethod.ToString(), args);
            var requestJson = request.ToJson();

            BridgeResponse response = null;
            try
            {
                var result = InvokePlatform(requestJson);
                response = BridgeResponse.Deserialize(result);

                if (response == null)
                {
					DebugUtility.LogError(LoggerTags.Module, "同步调用 -> arg:{0}; 解析错误 -> result:{1}", requestJson, result);
                    response = BridgeResponse.CreateErrorResponse(request.id, BridgeCode.ReturnTypeError);
                    return response;
                }
                else
                {
                    DebugUtility.Log(LoggerTags.Module, "同步调用 -> arg:{0}; 返回 -> result:{1}", requestJson, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                DebugUtility.LogError(LoggerTags.Module, "同步调用 -> arg:{0}; 异常 -> Exception:{1}", requestJson, ex);
                response = BridgeResponse.CreateErrorResponse(request.id, BridgeCode.InvokeParamError);
                return response;
            }
        }

        protected void AsyncInvokePlatform(CallPlatformMethod invokeMethod, CallUnityMethod callbackMethod, Action<BridgeResponse> method, params JsonData[] args)
        {
            var request = BridgeRequest.Create(invokeMethod.ToString(), callbackMethod.ToString(), args);
            var requestJson = request.ToJson();
            try
            {
                DebugUtility.Log(LoggerTags.Module, "异步调用 -> arg:{0};", requestJson);
                PushListener(request.callbackMethod, method);
                InvokePlatform(requestJson);
            }
            catch (Exception ex)
            {
                DebugUtility.LogError(LoggerTags.Module, "异步调用 -> arg:{0}; 异常 -> Exception:{1}", requestJson, ex);
            }
        }

        public void DefaultCallBackPlatform(BridgeResponse response)
        {
            try
            {
                var jsonData = response.ToJsonData();
                var result = jsonData.ToJson();
                DebugUtility.Log(LoggerTags.Module, "Unity返回 -> result:{0}", result);
                CallBackPlatform(result);
            }
            catch (Exception ex)
            {
				DebugUtility.LogException(ex);
            }
        }
    }
}
