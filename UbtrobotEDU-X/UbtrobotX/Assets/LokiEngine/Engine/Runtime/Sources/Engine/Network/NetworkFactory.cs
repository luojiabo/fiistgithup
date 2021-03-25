using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public class NetworkFactory
	{
		public delegate IWebSocketClient Creator(string url, bool connectImmediatly, Action onConnected, Action<ENetCode> onDisconnected, Action<byte[]> onRecv, Action<string> onError);

		private static readonly List<KeyValuePair<string, Creator>> msCreators = new List<KeyValuePair<string, Creator>>
		{
			new KeyValuePair<string, Creator>( "ws://",  CreateWebSocket),
			new KeyValuePair<string, Creator>( "wss://",  CreateWebSocket),
			new KeyValuePair<string, Creator>( "webrtc://",  CreateWebRTCClient),
		};

		public static void Register(string protocolHeader, Creator creator)
		{
			msCreators.Union(protocolHeader, creator);
		}

		public static IWebSocketClient CreateWebClient(string url, bool connectImmediatly, Action onConnected, Action<ENetCode> onDisconnected, Action<byte[]> onRecv, Action<string> onError)
		{
			foreach (var item in msCreators)
			{
				if (url.StartsWith(item.Key, StringComparison.OrdinalIgnoreCase))
				{
					return item.Value(url, connectImmediatly, onConnected, onDisconnected, onRecv, onError);
				}
			}

			return null;
		}

		/// <summary>
		/// Create webrtc instance
		/// </summary>
		/// <param name="url">e.g. "webrtc://echo.websocket.org"</param>
		/// <param name="connectImmediatly">connect to host immediately or not</param>
		/// <param name="onConnected">the connected event</param>
		/// <param name="onDisconnected">the disconnected event</param>
		/// <param name="onRecv">on recv message event</param>
		/// <param name="onError">on error event</param>
		/// <returns>the interface of web-socket wrapper</returns>
		public static IWebSocketClient CreateWebRTCClient(string url, bool connectImmediatly, Action onConnected, Action<ENetCode> onDisconnected, Action<byte[]> onRecv, Action<string> onError)
		{
			return null;
		}

		/// <summary>
		/// Create webbridge instance
		/// </summary>
		/// <param name="url">e.g. "bridge://echo.websocket.org"</param>
		/// <param name="connectImmediatly">connect to host immediately or not</param>
		/// <param name="onConnected">the connected event</param>
		/// <param name="onDisconnected">the disconnected event</param>
		/// <param name="onRecv">on recv message event</param>
		/// <param name="onError">on error event</param>
		/// <returns>the interface of web-socket wrapper</returns>
		public static IWebSocketClient CreateWebClient<TWebClient>(
			string url, 
			bool connectImmediatly, 
			Action onConnected, 
			Action<ENetCode> onDisconnected, 
			Action<byte[]> onRecv, 
			Action<string> onError) where TWebClient : IWebSocketClient
		{
			IWebSocketClient ws = Activator.CreateInstance<TWebClient>();
			ws.onConnected = onConnected;
			ws.onRecv = onRecv;
			ws.onError = onError;
			ws.onDisconnected = onDisconnected;
			if (connectImmediatly)
			{
				ws.Connect(url);
			}
			return ws;
		}

		/// <summary>
		/// Create web socket instance
		/// </summary>
		/// <param name="url">e.g. "ws://echo.websocket.org"</param>
		/// <param name="connectImmediatly">connect to host immediately or not</param>
		/// <param name="onConnected">the connected event</param>
		/// <param name="onDisconnected">the disconnected event</param>
		/// <param name="onRecv">on recv message event</param>
		/// <param name="onError">on error event</param>
		/// <returns>the interface of web-socket wrapper</returns>
		public static IWebSocketClient CreateWebSocket(string url, bool connectImmediatly, Action onConnected, Action<ENetCode> onDisconnected, Action<byte[]> onRecv, Action<string> onError)
		{
			IWebSocketClient ws = new WebSocketClient();
			ws.onConnected = onConnected;
			ws.onRecv = onRecv;
			ws.onError = onError;
			ws.onDisconnected = onDisconnected;
			if (connectImmediatly)
			{
				ws.Connect(url);
			}
			return ws;
		}

		/// <summary>
		/// Create web socket instance
		/// </summary>
		/// <param name="url">e.g. "ws://echo.websocket.org"</param>
		/// <param name="connectImmediatly">connect to host immediately or not</param>
		/// <param name="onConnected">the connected event</param>
		/// <param name="onDisconnected">the disconnected event</param>
		/// <param name="onRecv">on recv message event</param>
		/// <param name="onError">on error event</param>
		/// <returns>the interface of web-socket wrapper</returns>
		public static IWebSocketClient CreateWebSocketSimulator(string url, bool connectImmediatly, Action onConnected, Action<ENetCode> onDisconnected, Action<byte[]> onRecv, Action<string> onError)
		{
			IWebSocketClient ws = new GameObject("WebSocketSimulator").AddComponent<WebSocketClientSimulator>();
			ws.onConnected = onConnected;
			ws.onRecv = onRecv;
			ws.onError = onError;
			ws.onDisconnected = onDisconnected;
			if (connectImmediatly)
			{
				ws.Connect(url);
			}
			return ws;
		}

		/// <summary>
		/// Create web socket instance
		/// </summary>
		/// <param name="url">e.g. "ws://echo.websocket.org"</param>
		/// <param name="connectImmediatly">connect to host immediately or not</param>
		/// <param name="onConnected">the connected event</param>
		/// <param name="onDisconnected">the disconnected event</param>
		/// <param name="onRecv">on recv message event</param>
		/// <param name="onError">on error event</param>
		/// <returns>the interface of web-socket wrapper</returns>
		public static IWebSocketClient CreateWebSocketSimulator<TWebSocketClientSimulator>(string url, bool connectImmediatly, Action onConnected, Action<ENetCode> onDisconnected, Action<byte[]> onRecv, Action<string> onError) where TWebSocketClientSimulator : Component, IWebSocketClient
		{
			IWebSocketClient ws = new GameObject("WebSocketSimulator").AddComponent<TWebSocketClientSimulator>();
			ws.onConnected = onConnected;
			ws.onRecv = onRecv;
			ws.onError = onError;
			ws.onDisconnected = onDisconnected;
			if (connectImmediatly)
			{
				ws.Connect(url);
			}
			return ws;
		}
	}
}
