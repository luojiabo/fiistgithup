using System;
using System.Text;
using HybridWebSocket;
using UnityEngine;

namespace Loki
{
	public class WebSocketClient : IWebSocketClient
	{
		private bool mDisposed = false;

		private string mHost;

		private IWebSocket mSocketClient;
		public string host { get { return mHost; } }
		public Action onConnected { get; set; }
		public Action<ENetCode> onDisconnected { get; set; }
		public Action<byte[]> onRecv { get; set; }
		public Action<string> onError { get; set; }

		public ENetState state
		{
			get
			{
				if (mSocketClient == null)
					return ENetState.Disconnected;
				return mSocketClient.GetState().ToNetState();
			}
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

		protected void OnConnected()
		{
			DebugUtility.LogTrace(LoggerTags.Online, "{0} Connected : {1}", mHost, state);

			if (onConnected != null)
			{
				onConnected();
			}
		}

		protected void OnDisconnected(WebSocketCloseCode error)
		{
			DebugUtility.LogTrace(LoggerTags.Online, "{0} Disconnected : {1}", mHost, error);

			if (onDisconnected != null)
			{
				onDisconnected(error.ToNetCode());
			}
		}

		protected void OnMessage(byte[] msg)
		{
			if (onRecv != null)
			{
				onRecv(msg);
			}
		}

		protected void OnError(string errMsg)
		{
			DebugUtility.LogErrorTrace(LoggerTags.Online, "{0} Error : {1}, State : {2}", mHost, errMsg, state);

			if (onError != null)
			{
				onError(errMsg);
			}
		}

		public void Connect(string url)
		{
			if (mSocketClient != null)
			{
				throw new Exception("Please disconnect the connected/connecting WebScoket.");
			}

			mHost = url;
			mSocketClient = WebSocketFactory.CreateInstance(url);
			mSocketClient.OnOpen += OnConnected;
			mSocketClient.OnClose += OnDisconnected;
			mSocketClient.OnMessage += OnMessage;
			mSocketClient.OnError += OnError;
			mSocketClient.Connect();
		}

		public void Disconnect()
		{
			if (mSocketClient == null)
				return;

			var state = mSocketClient.GetState();
			if (state == WebSocketState.Connecting || state == WebSocketState.Open)
			{
				mSocketClient.Close();
			}
			mSocketClient = null;
		}

		public void SendMessage(string datas, Encoding encoding)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			SendMessage(encoding.GetBytes(datas));
		}

		public void SendMessage(byte[] datas)
		{
			if (state.IsConnected())
			{
				mSocketClient.Send(datas);
			}
			else
			{
				DebugUtility.LogErrorTrace(LoggerTags.Online, "Try to send message before the web-socket connected.");
			}
		}

	}

	public class WebSocketClientSimulator : UComponent, IWebSocketClient
	{
		private bool mDisposed = false;

		private string mHost;
		public string host { get { return mHost; } }
		public Action onConnected { get; set; }
		public Action<ENetCode> onDisconnected { get; set; }
		public Action<byte[]> onRecv { get; set; }
		public Action<string> onError { get; set; }
		public ENetState state { get; private set; } = ENetState.Disconnected;

		[PreviewMember]
		public TextAsset sendContent { get; set; }
		[PreviewMember]
		public TextAsset recvContent { get; set; }

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
				Disconnect();
				Destroy(gameObject);
			}
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.simSend")]
		public void SimulateSendMessage()
		{
			string text = sendContent ? sendContent.text : string.Empty;
			if (string.IsNullOrEmpty(text))
			{
				DebugUtility.LogError(LoggerTags.Online, "Can't send empty message.");
				return;
			}

			SendMessage(Encoding.UTF8.GetBytes(text));
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.simRecv")]
		public void SimulateRecvMessage()
		{
			string text = recvContent ? recvContent.text : string.Empty;
			if (string.IsNullOrEmpty(text))
			{
				DebugUtility.LogError(LoggerTags.Online, "Can't recv empty message.");
				return;
			}

			OnMessage(Encoding.UTF8.GetBytes(text));
		}

		public void Connect(string url)
		{
			mHost = url;
			state = ENetState.Connected;
			OnConnected();
		}

		public void Disconnect()
		{
			state = ENetState.Disconnected;
			OnDisconnected(WebSocketCloseCode.Normal);

		}

		public void SendMessage(byte[] datas)
		{
			// DebugUtility.Log(LoggerTags.Online, "Send message");
			if (state.IsConnected())
			{
				DebugUtility.LogTrace(LoggerTags.Online, "Success to send message");
			}
			else
			{
				DebugUtility.LogErrorTrace(LoggerTags.Online, "Try to send message before the web-socket connected.");
			}
		}

		public void SendMessage(string datas, Encoding encoding)
		{
			// DebugUtility.Log(LoggerTags.Online, "Send message");
			if (state.IsConnected())
			{
				DebugUtility.LogTrace(LoggerTags.Online, "Success to send message");
			}
			else
			{
				DebugUtility.LogErrorTrace(LoggerTags.Online, "Try to send message before the web-socket connected.");
			}
		}

		protected void OnConnected()
		{
			DebugUtility.LogTrace(LoggerTags.Online, "{0} Connected : {1}", mHost, state);

			if (onConnected != null)
			{
				onConnected();
			}
		}

		protected void OnDisconnected(WebSocketCloseCode error)
		{
			DebugUtility.LogTrace(LoggerTags.Online, "{0} Disconnected : {1}", mHost, error);

			if (onDisconnected != null)
			{
				onDisconnected(error.ToNetCode());
			}
		}

		protected void OnMessage(byte[] msg)
		{
			if (onRecv != null)
			{
				onRecv(msg);
			}
		}

		protected void OnError(string errMsg)
		{
			DebugUtility.LogErrorTrace(LoggerTags.Online, "{0} Error : {1}, State : {2}", mHost, errMsg, state);

			if (onError != null)
			{
				onError(errMsg);
			}
		}
	}

}
