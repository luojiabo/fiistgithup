using System;
using System.Collections;
using System.Collections.Generic;
using Loki;

namespace Ubtrobot
{
	public sealed class NetworkManager : Loki.NetworkManager<Ubtrobot.NetworkManager>
	{
		private HostSettings mHostSettings = null;
		private readonly Dictionary<string, IConnection> mConns = new Dictionary<string, IConnection>();

		private IServerListener mServerListener = null;

		[PreviewMember]
		public HostSettings hostSettingRef
		{
			get
			{
				if (mHostSettings == null)
				{
					mHostSettings = UbtrobotSettings.GetOrLoad().hostSettings;
				}
				return mHostSettings;
			}
			set
			{
				mHostSettings = value;
			}
		}

		public override IEnumerator PostInitialize()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			// 无法在WEBLGL本地端上创建服务器
			mServerListener = new ScratchServerConnection();
			if (mServerListener != null)
			{
				// mServerListener.StartListener(hostSettingRef.GetHost("server"));
			}
#endif
			yield break;
		}

		public override void Uninitialize()
		{
			base.Uninitialize();
		}

		public override void OnFixedUpdate(float fixedDeltaTime)
		{
			base.OnFixedUpdate(fixedDeltaTime);
		}

		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
		}

		public override void Shutdown()
		{
			base.Shutdown();
			if (mServerListener != null)
			{
				mServerListener.Dispose();
				mServerListener = null;
			}
		}

		public override void Startup()
		{
			base.Startup();
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.connScratch")]
		public IConnection ConnectToScratch()
		{
			return ConnectToScratch(hostSettingRef.GetHost(), true);
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.disconnScratch")]
		public void DisconnectToScratch()
		{
			DisconnectToScratch(hostSettingRef.GetHost());
		}

		public void DisconnectToScratch(string host)
		{
			if (mConns.TryGetValue(host, out var conn))
			{
				conn.Dispose();
				mConns.Remove(host);
			}
		}

		public IConnection GetScratchConn(string host)
		{
			mConns.TryGetValue(host, out var conn);
			return conn;
		}

		public IConnection ConnectToScratch(string host, bool autoConnect)
		{
			if (string.IsNullOrEmpty(host))
			{
				DebugUtility.LogErrorTrace(LoggerTags.Online, "The scratch host is empty.");
				return null;
			}

			if (mConns.TryGetValue(host, out var conn) && conn.state != ENetState.Disconnected)
			{
				DebugUtility.LogErrorTrace(LoggerTags.Online, "Please destroy the scratch connection.");
				return conn;
			}

			mConns[host] = conn = new ScratchConnection();
			if (autoConnect)
			{
				conn.Connect(host);
			}
			return conn;
		}
	}
}
