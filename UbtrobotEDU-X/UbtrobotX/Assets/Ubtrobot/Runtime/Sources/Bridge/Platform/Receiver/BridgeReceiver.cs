using System;
using Loki;

namespace Ubtrobot
{
	/// <summary>
	/// 平台到Unity通讯
	/// </summary>
    public class BridgeReceiver : USingletonObject<BridgeReceiver>
    {
        private static readonly string ObjectName = "BridgeReceiver";
        private BridgeCaller caller;

		public override ELifetime lifetime => ELifetime.App;
		protected override void OnInitialize()
        {
			base.OnInitialize();
			gameObject.name = ObjectName;
        }

        public void SetCaller(BridgeCaller caller)
        {
            this.caller = caller;
		}

		[InspectorMethod]
		public void CallTest()
		{
			call("{\"id\": 11102,\"func\": \"onRecv\",\"args\": [{\"message\": {\"device\": 2,\"mode\": 127,\"id\": 1,\"data\": [255],\"uuid\": \"tSpeed\"}}] }");
			//{"device":2,"mode":127,"id":1,"data":[0],"uuid":"tSpeed"}
		}

		#region 平台调用
		#region 调用静态事件
		[ConsoleMethod(aliasName = "unity.call")]
		public void call(string args)
        {
            var request = BridgeRequest.Deserialize(args);
            if (request == null)
            {
                DebugUtility.LogError(LoggerTags.Module, "平台调用 -> 异常args:{0}", args);
                return;
            }
            DebugUtility.Log(LoggerTags.Module, "平台调用 -> 传入args:{0}", args);
            var response = BridgeResponse.CreateDefaultResponse(request.id, request.callbackMethod);
            var invokeMethod = request.invokeMethod.ToEnum(CallUnityMethod.Unknown);
            var method = caller.PullListener(invokeMethod);
            if (method != null)
            {
                try
                {
                    method(request, response);
                }
                catch (Exception ex)
                {
					DebugUtility.LogException(ex);
                    response.SetFailureResult(BridgeCode.Failure);
                    caller.DefaultCallBackPlatform(response);
                }
            }
            else
            {
                DebugUtility.LogError(LoggerTags.Module, "平台调用 -> 方法未监听！method:{0}, id:{1}", request.invokeMethod, request.id);
                response.SetFailureResult(BridgeCode.NorFunc);
                caller.DefaultCallBackPlatform(response);
            }
        }
        #endregion

        #region 回调动态事件
        public void onCallback(string result)
        {
            var response = BridgeResponse.Deserialize(result);
            if (response == null)
            {
                DebugUtility.LogError(LoggerTags.Module, "平台回调 -> 异常result:{0}", result);
                return;
            }
            DebugUtility.Log(LoggerTags.Module, "平台回调 -> result:{0}", result);
            //提取动态事件
            var method = caller.PullListener(response.callbackMethod, response.IsComplete);
            if (method != null)
            {
                method.SafeInvoke(response);
            }
            else
            {
                DebugUtility.LogError(LoggerTags.Module, "平台回调 -> 方法未监听！key:{0}", response.callbackMethod);
            }
        }
        #endregion
        #endregion
    }
}
