using System;
using System.Text;
using HybridWebSocket;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Loki
{
	public abstract class WebSocketHandle : WebSocketBehavior
	{
		protected override void OnOpen()
		{
			base.OnOpen();
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			base.OnMessage(e);
		}

		protected override void OnClose(CloseEventArgs e)
		{
			base.OnClose(e);
		}

		protected override void OnError(ErrorEventArgs e)
		{
			base.OnError(e);
		}
	}

	public class WebSocketServiceHost
	{

	}

	/// <summary>
	/// Examples:
	/// <para>var server = new WebSocketServer("ws://your_url");</para>
	/// <para></para>
	/// <para>server.AddService<WebSocketHandleImpl>("/dispatch_path_0");</para>
	/// <para>server.AddService<WebSocketHandleImpl>("/dispatch_path_1");</para>
	/// <para>server.AddService<WebSocketHandleImpl>("/dispatch_path_2", (handle) => ExternalInitializer(handle));</para>
	/// <para>server.Start();</para>
	/// <para>//Do something</para>
	/// <para>server.Stop();</para>
	/// </summary>
	public class WebSocketServer : IWebSocketServer
	{
		private bool mDisposed = false;
		private WebSocketSharp.Server.WebSocketServer mServerInstance = null;

		public bool isListening
		{
			get
			{
				return mServerInstance != null && mServerInstance.IsListening;
			}
		}

		public WebSocketServer(int port)
		{
			mServerInstance = new WebSocketSharp.Server.WebSocketServer(port);
		}

		public WebSocketServer(string url)
		{
			mServerInstance = new WebSocketSharp.Server.WebSocketServer(url);
		}

		public void AddService<THandle>(string path) where THandle : WebSocketHandle, new()
		{
			mServerInstance.AddWebSocketService<THandle>(path);
		}

		public void AddService<THandle>(string path, Action<THandle> initializer) where THandle : WebSocketHandle, new()
		{
			mServerInstance.AddWebSocketService<THandle>(path, initializer);
		}

		/// <summary>
		/// 对指定host路径进行广播回应
		/// </summary>
		/// <param name="path"></param>
		/// <param name="message"></param>
		public void SendMessage(string path, byte[] message)
		{
			if (mServerInstance.WebSocketServices.TryGetServiceHost(path, out var host))
			{
				host.Sessions.Broadcast(message);
			}
		}

		/// <summary>
		/// 对指定host路径进行广播回应
		/// </summary>
		/// <param name="path"></param>
		/// <param name="message"></param>
		public void RecvMessage(string path, byte[] message)
		{
			if (mServerInstance.WebSocketServices.TryGetServiceHost(path, out var host))
			{
				//host.Sessions.SendTo(message);
				foreach (var id in host.Sessions.ActiveIDs)
				{
					host.Sessions.SendTo(message, id);
				}
			}
		}

		public void StartListening()
		{
			mServerInstance.Start();
		}

		public void StopListening()
		{
			mServerInstance.Stop();
		}

		//protected override void OnMessage(MessageEventArgs e)
		//{
		//	var msg = e.Data == "BALUS"
		//			  ? "I've been balused already..."
		//			  : "I'm not available now.";

		//	Send(msg);
		//}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (mDisposed)
			{
				mDisposed = true;
				if (disposing)
				{
					StopListening();
				}
			}
		}


	}
}

