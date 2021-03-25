using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using LitJson;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	/// <summary>
	/// Unity通讯
	/// </summary>
	public class CommonBridgeCaller : BridgeCaller
	{
		public event Action<string> onRecvMessageEvent;

		#region
#if !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern string Invoke(string args);
		[DllImport("__Internal")]
		private static extern void Callback(string result);
#else
		private static string Invoke(string args)
		{
			return "{\"id\": 0,\"complete\":1, \"code\":1}";
		}

		private static void Callback(string result)
		{
		}
#endif
		protected override string InvokePlatform(string args)
		{
			try
			{
				var result = Invoke(args);
				return result;
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
				return string.Empty;
			}
		}

		protected override void CallBackPlatform(string result)
		{
			try
			{
				Callback(result);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}
		#endregion


		public CommonBridgeCaller()
		{
			staticUnityMethods[CallUnityMethod.startup] = Startup;
			staticUnityMethods[CallUnityMethod.shutdown] = Shutdown;
			staticUnityMethods[CallUnityMethod.onRecv] = OnRecvMessage;
			staticUnityMethods[CallUnityMethod.requestSupportCmdInfo] = RequestSupportCmdInfo;
		}

		#region 监听相关
		private readonly Dictionary<CallUnityMethod, Action<BridgeRequest, BridgeResponse>> staticUnityMethods = new Dictionary<CallUnityMethod, Action<BridgeRequest, BridgeResponse>>();
		private readonly Dictionary<string, Action<BridgeResponse>> dynamicSyncMethods = new Dictionary<string, Action<BridgeResponse>>();
		protected sealed override void PushListener(string key, Action<BridgeResponse> method)
		{
			dynamicSyncMethods[key] = method;
		}

		protected sealed override void PushListener(CallUnityMethod key, Action<BridgeRequest, BridgeResponse> method)
		{
			staticUnityMethods[key] = method;
		}

		public sealed override Action<BridgeResponse> PullListener(string key, bool remove)
		{
			Action<BridgeResponse> method;
			if (!dynamicSyncMethods.TryGetValue(key, out method)) return null;
			if (remove) dynamicSyncMethods.Remove(key);
			return method;
		}

		public sealed override Action<BridgeRequest, BridgeResponse> PullListener(CallUnityMethod key)
		{
			Action<BridgeRequest, BridgeResponse> method;
			if (!staticUnityMethods.TryGetValue(key, out method)) return null;
			return method;
		}
		#endregion

		#region Unity调用
		#region 同步方式

		public virtual JsonData GetAppInfo()
		{
			return SyncInvokePlatform(CallPlatformMethod.getAppInfo).data;
		}

		public virtual JsonData GetConfigs()
		{
			return SyncInvokePlatform(CallPlatformMethod.getConfigs).data;
		}

		public void ShowLoading(string message)
		{
			var args = new JsonData();
			args["message"] = message;
			SyncInvokePlatform(CallPlatformMethod.showLoading, args);
		}

		public void HideLoading()
		{
			SyncInvokePlatform(CallPlatformMethod.hideLoading);
		}

		public void SetTitle(string title)
		{
			SyncInvokePlatform(CallPlatformMethod.setTitle, title);
		}
		#endregion

		#region 异步方式
		public void PostMessageAsync(string source, string target, string message, Encoding encoding)
		{
			if (encoding != Encoding.UTF8)
			{
				message = Encoding.UTF8.GetString(Encoding.Convert(encoding, Encoding.UTF8, Encoding.Default.GetBytes(message)));
			}
			var args = new JsonData();
			args["source"] = source;
			args["target"] = target;
			args["message"] = message;
			SyncInvokePlatform(CallPlatformMethod.postMessageAsync, args);
		}

		public void ConnnectAsync(string source, string target)
		{
			var args = new JsonData();
			args["source"] = source;
			args["target"] = target;
			SyncInvokePlatform(CallPlatformMethod.connectAsync, args);
		}

		public void DisconnnectAsync(string source, string target)
		{
			var args = new JsonData();
			args["source"] = source;
			args["target"] = target;
			SyncInvokePlatform(CallPlatformMethod.disconnectAsync, args);
		}

		public void SetFullScreen(int isFullScreen)
		{
			var args = new JsonData();
			args["full"] = isFullScreen;
			SyncInvokePlatform(CallPlatformMethod.setFullScreen, args);
		}

		public void StartLaunching()
		{
			SyncInvokePlatform(CallPlatformMethod.startLaunching);
		}

		public void StopLaunching()
		{
			SyncInvokePlatform(CallPlatformMethod.stopLaunching);
		}
		#endregion

		#endregion

		#region 平台调用
		internal void OnRecvMessage(BridgeRequest req, BridgeResponse rep)
		{
			var json = req.GetArgument();
			string message = "";
			if (json.IsObject)
			{
				if (json.ContainsKey("message"))
					message = json["message"].ToJson();
			}
			else if (json.IsString)
			{
				message = (string)json;
			}
			onRecvMessageEvent.SafeInvoke(message);
		}

		internal void RequestSupportCmdInfo(BridgeRequest req, BridgeResponse rep)
		{
			var json = req.GetArgument();
			if (json.IsObject)
			{
				if (json.ContainsKey("protocol"))
				{
					JsonData protocols = json["protocol"];
					if (protocols.IsArray)
					{
						JsonData result = new JsonData();
						result.SetJsonType(JsonType.Object);

						var protocolsCount = protocols.Count;
						for (int i = 0; i < protocolsCount; i++)
						{
							var protocol = (string)protocols[i];
							if (string.IsNullOrEmpty(protocol) || protocol == "Explorer")
							{
								protocol = "Explorer";
								var support = CommandFactory.GetCommandSupprtList(typeof(ExploreProtocol));
								result[protocol] = LitJsonHelper.CreateArrayJsonData(support);
							}
						}
						rep.SetResult(BridgeCode.Success, result);
						return;
					}
				}
			}

			DebugUtility.LogError(LoggerTags.Project, "The first argument is not JSON object, examples: 'protocol': [ 'Explorer' ] ");
			rep.SetFailureResult(BridgeCode.Failure);
		}

		internal void Startup(BridgeRequest req, BridgeResponse rep)
		{
			var json = req.GetArgument();
			string sceneID = "";
			if (json.IsObject)
			{
				if (json.ContainsKey("sceneID"))
					sceneID = json["sceneID"].ToJson();
			}

			// temp code
			if (!string.IsNullOrEmpty(sceneID))
			{
				Loki.UI.WindowManager.CloseAll();
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneID, UnityEngine.SceneManagement.LoadSceneMode.Single).completed += (h) =>
				{
					Loki.UI.WindowManager.Open<SimulationWindow>();
				};
			}
		}

		private void CommonBridgeCaller_completed(AsyncOperation obj)
		{
			throw new NotImplementedException();
		}

		internal void Shutdown(BridgeRequest req, BridgeResponse rep)
		{

		}
		#endregion
	}
}
