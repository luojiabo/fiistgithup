using System;
using Loki;
using WebSocketSharp;

namespace Ubtrobot
{
	public class ScratchLabDispatcher
	{
		/// <summary>
		/// lab处理来自于控制端转发过来的消息
		/// </summary>
		/// <param name="protocol"></param>
		public void OnMessage(IProtocol protocol)
		{
			if (RobotManager.Get())
			{
				RobotManager.Get().PushMessage(protocol);
			}
		}
	}

	public class ScratchLabControlDispatcher
	{
		private ScratchServerConnection mServer;

		public ScratchLabControlDispatcher(ScratchServerConnection server)
		{
			mServer = server;
		}

		/// <summary>
		/// 通过服务总控发送消息
		/// </summary>
		/// <param name="path">转发路径</param>
		/// <param name="message"></param>
		public void Forward(string path, byte[] message)
		{
			if (mServer != null && mServer.state == ENetState.Connected)
			{
				mServer.SendMessage(path, message);
			}
		}
	}
	
	/// <summary>
	/// 供仿真端连接
	/// </summary>
	public class ScratchLabService : WebSocketHandle
	{
		public static readonly string Path = "/lab";

		private DateTime mConnectedTime;
		private DateTime mDisconnectedTime;

		/// <summary>
		/// 消息回应路径
		/// </summary>
		public string forwardPath { get; set; }

		public ScratchLabDispatcher labDispatcher { get; set; }

		/// <summary>
		/// 来自于控制端的转发，一般是控制Robot怎么执行指令
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMessage(MessageEventArgs e)
		{
			base.OnMessage(e);

			DebugUtility.Log(LoggerTags.Online, "【Server】 Status : {0}, OnMessage.", this.ConnectionState);

			var dispatcher = labDispatcher;
			if (dispatcher == null)
			{
				return;
			}

			var protocol = ProtocolFactory.Generate(ProtocolOutput.ScratchToExplore, e.RawData, 0, e.RawData.Length);
			if (protocol == null)
			{
				DebugUtility.LogError(LoggerTags.Online, "【Server】OnMessage Failed : The protocol is null.");
				return;
			}
			DebugUtility.Log(LoggerTags.Online, "【Server】 recv message");
			protocol.host = forwardPath;
			dispatcher.OnMessage(protocol);
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			//mDispatcher = new ScratchProtocolDispatcher(this);
			mConnectedTime = DateTime.Now;

			DebugUtility.Log(LoggerTags.Online, "【Server】 Status : {0}, OnOpen, Time : {1}", this.ConnectionState, mConnectedTime.ToLongTimeString());
		}

		protected override void OnClose(CloseEventArgs e)
		{
			base.OnClose(e);
			mDisconnectedTime = DateTime.Now;

			DebugUtility.Log(
				LoggerTags.Online,
				"【Server】 Status : {0}, OnClose : [Code({1}), Reason({2})], Time : {3}, Alive : {4}(seconds)",
				this.ConnectionState, e.Code, e.Reason, mDisconnectedTime.ToLongTimeString(), (mDisconnectedTime - mConnectedTime).TotalSeconds);
		}

		protected override void OnError(ErrorEventArgs e)
		{
			base.OnError(e);

			DebugUtility.Log(LoggerTags.Online, "【Server】 Status : {0}, OnClose : [Message({1}), Exception({2})]", this.ConnectionState, e.Message, e.Exception);
		}
	}

	/// <summary>
	/// 供控制端连接（scratch等）
	/// 控制端接收到消息之后转发到仿真端
	/// </summary>
	public class ScratchLabControlService : WebSocketHandle
	{
		public static readonly string Path = "/labctrl";

		private DateTime mConnectedTime;
		private DateTime mDisconnectedTime;

		public string forwardPath { get; set; }

		public ScratchLabControlDispatcher labDispatcher { get; set; }
		
		/// <summary>
		/// 接收到控制端的消息之后转发到仿真端
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMessage(MessageEventArgs e)
		{
			base.OnMessage(e);

			DebugUtility.Log(LoggerTags.Online, "【Server】 {0} Status : {1}, OnMessage.", Path, this.ConnectionState);

			var dispatcher = labDispatcher;
			if (dispatcher == null)
			{
				return;
			}

			// 获取分发器进行转发
			dispatcher.Forward(forwardPath, e.RawData);
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			//mDispatcher = new ScratchProtocolDispatcher(this);
			mConnectedTime = DateTime.Now;

			DebugUtility.Log(LoggerTags.Online, "【Server】 {0}  Status : {1}, OnOpen, Time : {2}", Path, this.ConnectionState, mConnectedTime.ToLongTimeString());
		}

		protected override void OnClose(CloseEventArgs e)
		{
			base.OnClose(e);
			mDisconnectedTime = DateTime.Now;

			DebugUtility.Log(
				LoggerTags.Online,
				"【Server】 {0} Status : {1}, OnClose : [Code({2}), Reason({3})], Time : {4}, Alive : {5}(seconds)", Path,
				this.ConnectionState, e.Code, e.Reason, mDisconnectedTime.ToLongTimeString(), (mDisconnectedTime - mConnectedTime).TotalSeconds);
		}

		protected override void OnError(ErrorEventArgs e)
		{
			base.OnError(e);

			DebugUtility.Log(LoggerTags.Online, "【Server】{0} Status : {1}, OnClose : [Message({2}), Exception({3})]", Path, this.ConnectionState, e.Message, e.Exception);
		}
	}

	public class ScratchServerConnection : IDisposable, IServerListener
	{
		protected bool mDisposedValue = false;
		private IWebSocketServer mServer = null;

		public ENetState state { get { return mServer != null && mServer.isListening ? ENetState.Connected : ENetState.Disconnected; } }

		public ScratchServerConnection()
		{
		}

		~ScratchServerConnection()
		{
			Dispose(false);
		}

		public void StartListener(string host)
		{
			if (state == ENetState.Connected)
			{
				throw new Exception("Please disconnect at first.");
			}

			// 注意：不直接处理的原因是因为后面可能会把Server+Services独立到另外的进程
			// 否则通过Session直接可以转发到另外的对象

			var ws = new WebSocketServer(host);
			// 用于仿真端的连接, 仿真
			ws.AddService<ScratchLabService>(ScratchLabService.Path, (service) =>
			{
				service.labDispatcher = new ScratchLabDispatcher();
				// 仿真端协议的消息应答通过lab ctrl 进行回应
				service.forwardPath = ScratchLabControlService.Path;
			});
			// 用于控制端的连接，控制端发送消息到CtrlService，CtrlService分发到仿真端的控制
			ws.AddService<ScratchLabControlService>(ScratchLabControlService.Path, (service) =>
			{
				service.labDispatcher = new ScratchLabControlDispatcher(this);
				// 控制端协议的消息应答通过lab 进行回应
				service.forwardPath = ScratchLabService.Path;
			});
			ws.StartListening();
			mServer = ws;
		}

		public void StopListener()
		{
			if (mServer != null)
			{
				mServer.StopListening();
				mServer.Dispose();
				mServer = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SendMessage(IProtocol message)
		{
			if (state == ENetState.Connected)
			{
				message.ToBytes(ProtocolOutput.ExploreToScratch, out var e, out var bytes);
				if (bytes.Length > 0)
				{
					mServer.SendMessage(message.host, bytes);
				}
			}
		}

		/// <summary>
		/// 指定发送host path跟消息内容
		/// </summary>
		/// <param name="host"></param>
		/// <param name="message"></param>
		public void SendMessage(string host, byte[] message)
		{
			if (state == ENetState.Connected)
			{
				mServer.SendMessage(host, message);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!mDisposedValue)
			{
				mDisposedValue = true;
				if (disposing)
				{
					StopListener();
				}
			}
		}
	}
}
