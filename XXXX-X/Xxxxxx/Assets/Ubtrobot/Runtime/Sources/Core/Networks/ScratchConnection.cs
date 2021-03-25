using System;
using Loki;
using System.Text;
using LitJson;

namespace Ubtrobot
{
	class ScratchProtocolDispatcher : IDispatcher
	{
		private ScratchConnection mScratchConn = null;

		public ScratchProtocolDispatcher(ScratchConnection conn)
		{
			mScratchConn = conn;
		}

		public void OnConnected()
		{
			JsonData register = new JsonData();
			register["register"] = true;
			register["user"] = "u3d";
			mScratchConn.SendMessage(register.ToJson());
		}

		public void OnDisconnected(ENetCode error)
		{

		}

		public void OnMessage(IProtocol protocol)
		{
			if (RobotManager.Get())
			{
				RobotManager.Get().PushMessage(protocol);
			}
		}

		public void OnError(string errMsg)
		{

		}

		public void Clear()
		{

		}
	}


	public class ScratchConnection : IConnection
	{
		private bool mDisposed = false;
		private IWebSocketClient mConn = null;
		private readonly ScratchProtocolDispatcher mDispatcher;

		public ENetState state
		{
			get
			{
				if (mConn == null)
					return ENetState.Disconnected;
				return mConn.state;
			}
		}

		public bool useByteStreaming { get; set; } = false;

		public ScratchConnection()
		{
			mDispatcher = new ScratchProtocolDispatcher(this);
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
				if (mConn != null)
				{
					mConn.Dispose();
					mConn = null;
				}
			}
		}

		public IDispatcher GetDispatcher()
		{
			return mDispatcher;
		}

		public void Disconnect()
		{
			if (mConn != null)
			{
				mConn.Disconnect();
				mConn = null;
			}
		}

		public void Connect(string host)
		{
			if (mConn != null && !mConn.state.IsDisconnected())
			{
				DebugUtility.LogError(LoggerTags.Online, "Please disconnect this conn.");
				return;
			}
			DebugUtility.LogTrace(LoggerTags.Online, "Connnect to {0}", host);

			const string kSimProtocolStr = "sim://";
			// sim : simulator
			if (host.StartsWith(kSimProtocolStr))
			{
				string name = host.Substring(kSimProtocolStr.Length);
				var simulators = UnityEngine.Object.FindObjectsOfType<ScratchWebSocketClientSimulator>();
				foreach (var simulator in simulators)
				{
					if (simulator.name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
					{
						mConn = simulator;
					}
					if (mConn != null)
					{
						break;
					}
				}

				if (mConn != null)
				{
					mConn.onConnected = OnConnected;
					mConn.onRecv = OnMessage;
					mConn.onError = OnError;
					mConn.onDisconnected = OnDisconnected;
					mConn.Connect(host);
				}
				else
				{
					mConn = NetworkFactory.CreateWebSocketSimulator<ScratchWebSocketClientSimulator>(host, true, OnConnected, OnDisconnected, OnMessage, OnError);
				}
				return;
			}
			if (host.StartsWith(WebBridgeClient.ProtocolHeader))
			{
				mConn = NetworkFactory.CreateWebClient<WebBridgeClient>(host, true, OnConnected, OnDisconnected, OnMessage, OnError);
				return;
			}
			mConn = NetworkFactory.CreateWebClient(host, true, OnConnected, OnDisconnected, OnMessage, OnError);
		}

		/// <summary>
		/// 从仿真到Scratch 发送消息
		/// </summary>
		/// <param name="message"></param>
		public void SendMessage(IProtocol message)
		{
			if (useByteStreaming)
			{
				if (mConn == null || !mConn.state.IsConnected())
				{
					DebugUtility.LogError(LoggerTags.Online, "SendMessage Failed : Please connect to a host. Message({0})", message);
					return;
				}

				message.ToBytes(ProtocolOutput.ExploreToScratch, out var encryption, out var bytes);
				if (bytes != null && bytes.Length > 0)
				{
					DebugUtility.Log(LoggerTags.Online, "SendMessage : {0}", message);
					mConn.SendMessage(bytes);
				}
				else
				{
					DebugUtility.LogError(LoggerTags.Online, "SendMessage Failed : Try to send empty message to scratch ({0})", message);
				}
			}
			else
			{
				message.ToString(ProtocolOutput.ExploreToScratch, out var result);
				SendMessage(result);
			}
		}

		/// <summary>
		/// 从仿真到Scratch 发送消息
		/// </summary>
		/// <param name="message"></param>
		public void SendMessage(string message)
		{
			if (mConn == null || !mConn.state.IsConnected())
			{
				DebugUtility.LogError(LoggerTags.Online, "SendMessage Failed : Please connect to a host. Message({0})", message);
				return;
			}
			DebugUtility.Log(LoggerTags.Online, "SendMessage : {0}", message);
			mConn.SendMessage(message, Encoding.UTF8);
		}

		protected void OnConnected()
		{
			mDispatcher.OnConnected();
		}

		protected void OnDisconnected(ENetCode error)
		{
			mDispatcher.OnDisconnected(error);
		}

		/// <summary>
		/// 接收 从Scratch到仿真接收的消息
		/// </summary>
		/// <param name="msg"></param>
		protected void OnMessage(byte[] msg)
		{
			var protocol = ProtocolFactory.Generate(ProtocolOutput.ScratchToExplore, msg, 0, msg.Length);
			if (protocol == null)
			{
				DebugUtility.LogError(LoggerTags.Online, "OnMessage Failed : The protocol is null.");
				return;
			}
			DebugUtility.Log(LoggerTags.Online, "Recv message : {0}", protocol.ToString());
			mDispatcher.OnMessage(protocol);
		}

		protected void OnError(string errMsg)
		{
			mDispatcher.OnError(errMsg);
		}
	}

}
