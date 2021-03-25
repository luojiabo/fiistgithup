#define LOKI_CONSOLE

using System.Text;
using UnityEngine;
using Loki;
using WebSocketSharp;
using System.Collections;
using System;

namespace Ubtrobot
{
	public class WebSocketServerHandleDemo : WebSocketHandle
	{
		private DateTime mConnectedTime;
		private DateTime mDisconnectedTime;

		protected override void OnMessage(MessageEventArgs e)
		{
			base.OnMessage(e);

			DebugUtility.LogTrace(LoggerTags.Online, "¡¾Server¡¿  Status : {0}, OnMessage : {1}", this.ConnectionState, Encoding.UTF8.GetString(e.RawData));
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			mConnectedTime = DateTime.Now;

			DebugUtility.LogTrace(LoggerTags.Online, "¡¾Server¡¿  Status : {0}, OnOpen, Time : {1}", this.ConnectionState, mConnectedTime.ToLongTimeString());
		}

		protected override void OnClose(CloseEventArgs e)
		{
			base.OnClose(e);
			mDisconnectedTime = DateTime.Now;

			DebugUtility.LogTrace(
				LoggerTags.Online,
				"¡¾Server¡¿  Status : {0}, OnClose : [Code({1}), Reason({2})], Time : {3}, Alive : {4}(seconds)",
				this.ConnectionState, e.Code, e.Reason, mDisconnectedTime.ToLongTimeString(), (mDisconnectedTime - mConnectedTime).TotalSeconds);
		}

		protected override void OnError(ErrorEventArgs e)
		{
			base.OnError(e);

			DebugUtility.LogTrace(LoggerTags.Online, "¡¾Server¡¿  Status : {0}, OnClose : [Message({1}), Exception({2})]", this.ConnectionState, e.Message, e.Exception);
		}
	}

	/// <summary>
	/// Attach to game to test websocket
	/// </summary>
#if LOKI_CONSOLE
	public class WebSocketDemo : UComponent
#else
	public class WebSocketDemo : MonoBehaviour
#endif
	{
		private WebSocketServer mServer = null;
		private WebSocketClient mClient = null;

		public string host = "ws://echo.websocket.org";

		public string serverHost = "ws://127.0.0.1";
		public int port = 8081;
		public string serverService = "/service_demo0";

		public bool createServer = true;

		private DateTime mConnectedTime;
		private DateTime mDisconnectedTime;

#if LOKI_CONSOLE
		[ConsoleField(aliasName = "demo.pause")]
#endif
		public bool pauseMessages = false;

		public void CreateServer()
		{
			DebugUtility.LogTrace(LoggerTags.Online, "¡¾Common¡¿ CreateServer : {0}", port);
			var server = new WebSocketServer(port);
			server.AddService<WebSocketServerHandleDemo>(serverService);
			server.StartListening();

			mServer = server;
		}

		private void CreateClient()
		{
			DebugUtility.LogTrace(LoggerTags.Online, "¡¾Common¡¿ CreateClient...");
			// Create WebSocket instance
			var ws = new WebSocketClient();
			// Add OnOpen event listener
			ws.onConnected += () =>
			{
				mConnectedTime = DateTime.Now;

				DebugUtility.LogTrace(LoggerTags.Online, "¡¾Client¡¿ connected : {0}", mConnectedTime.ToLongTimeString());
				DebugUtility.LogTrace(LoggerTags.Online, "¡¾Client¡¿ State : {0}", ws.state);

				ws.SendMessage(Encoding.UTF8.GetBytes("WebSocketDemo from Unity Client."));

			};

			// Add OnMessage event listener
			ws.onRecv += (byte[] msg) =>
			{
				DebugUtility.LogTrace(LoggerTags.Online, "¡¾Client¡¿ received message: " + Encoding.UTF8.GetString(msg));

				ws.Disconnect();
			};

			// Add OnError event listener
			ws.onError += (string errMsg) =>
			{
				DebugUtility.LogTrace(LoggerTags.Online, "¡¾Client¡¿ error: " + errMsg);
			};

			// Add OnClose event listener
			ws.onDisconnected += (code) =>
			{
				mDisconnectedTime = DateTime.Now;

				DebugUtility.LogTrace(LoggerTags.Online, "¡¾Client¡¿ closed with code: {0}, DisconnectedTime : {1}, Alive : {2}(seconds)", code, mDisconnectedTime.ToLongTimeString(), (mDisconnectedTime - mConnectedTime).TotalSeconds);
			};

			// Connect to the server
			if (createServer)
			{
				string url = string.Concat(serverHost, ":", port.ToString(), serverService);
				DebugUtility.Log(LoggerTags.Online, "¡¾Common¡¿ CreateClient {0}", url);
				ws.Connect(url);
			}
			else
			{
				DebugUtility.Log(LoggerTags.Online, "¡¾Common¡¿ CreateClient {0}", host);
				ws.Connect(host);
			}
			mClient = ws;
		}

		private void Pause()
		{

		}

		private void OnEnable()
		{
			if (createServer)
			{
				CreateServer();
			}
			CreateClient();
		}

		private IEnumerator Start()
		{
			while (!pauseMessages)
			{
				yield return new WaitForSeconds(0.5f);
				if (mClient.state.IsConnected())
				{
					DebugUtility.Log(LoggerTags.Online, "¡¾Client¡¿ Send message");
					mClient.SendMessage(Encoding.UTF8.GetBytes("Client Message : " + DateTime.Now.ToLongTimeString()));
				}
			}
		}

		private void OnDisable()
		{
			if (mServer != null)
			{
				mServer.Dispose();
				mServer = null;
			}

			if (mClient != null)
			{
				mClient.Dispose();
				mClient = null;
			}
		}

	}
}

