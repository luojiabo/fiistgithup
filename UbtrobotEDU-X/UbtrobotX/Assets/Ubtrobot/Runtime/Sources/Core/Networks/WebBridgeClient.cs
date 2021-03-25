using System;
using System.Text;
using Loki;

namespace Ubtrobot
{
	public class WebBridgeClient : IWebSocketClient
	{
		public static readonly string ProtocolHeader = "webrpc://";

		private bool mDisposed = false;

		public string host { get; private set; }
		public Action onConnected { get; set; }
		public Action<ENetCode> onDisconnected { get; set; }
		public Action<string> onRecvRaw { get; set; }
		public Action<byte[]> onRecv { get; set; }
		public Action<string> onError { get; set; }

		public ENetState state { get; private set; } = ENetState.Disconnected;

		public void Connect(string url)
		{
			this.host = url;
			DebugUtility.LogTrace(LoggerTags.Online, "Connect {0}", host);
			var center = BridgeCenter.Caller;
			center.onRecvMessageEvent -= OnRecvMessage;
			center.onRecvMessageEvent += OnRecvMessage;
			state = ENetState.Connected;
			BridgeCenter.Caller.ConnnectAsync(host, "");
		}

		public void Disconnect()
		{
			DebugUtility.LogTrace(LoggerTags.Online, "Disconnect {0}", host);
			state = ENetState.Disconnected;
			BridgeCenter.Caller.DisconnnectAsync(host, "");
			BridgeCenter.Caller.onRecvMessageEvent -= OnRecvMessage;
			Misc.SafeInvoke(onDisconnected, ENetCode.WSNormal);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (mDisposed)
			{
				return;
			}
			mDisposed = true;
			if (disposing)
			{
				onRecv = null;
				onError = null;
				onConnected = null;
				onDisconnected = null;

				Disconnect();
			}
		}

		public void SendMessage(byte[] datas)
		{
			new Exception("unsupported send message by bytes");
		}

		public void SendMessage(string datas, Encoding encoding)
		{
			if (state == ENetState.Connected)
			{
				DebugUtility.Log(LoggerTags.Online, "SendMessage to {0}", host);
				BridgeCenter.Caller.PostMessageAsync(host, "", datas, encoding);
			}
			else
			{
				DebugUtility.LogWarningTrace(LoggerTags.Online, "Failed to SendMessage to {0}", host);
			}
		}

		private void OnRecvMessage(string datas)
		{
			DebugUtility.Log(LoggerTags.Online, "OnRecvMessage(string) from {0}", host);
			if (onRecvRaw != null)
			{
				Misc.SafeInvoke(onRecvRaw, datas);
			}
			else
			{
				OnRecvMessage(Encoding.UTF8.GetBytes(datas));
			}
		}

		public void OnRecvMessage(byte[] datas)
		{
			//DebugUtility.Log(LoggerTags.Online, "OnRecvMessage(bytes) from {0}", host);
			Misc.SafeInvoke(onRecv, datas);
		}

	}
}
