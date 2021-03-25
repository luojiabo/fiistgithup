using System;
using System.Collections.Generic;
using System.Text;

namespace Loki
{
	public interface INetClient : IDisposable
	{

	}

	public interface ITCPClient : INetClient
	{

	}

	public interface IUDPClient : INetClient
	{

	}

	public interface IWebSocketClient : INetClient
	{
		string host { get; }
		ENetState state { get; }
		Action onConnected { get; set; }
		Action<ENetCode> onDisconnected { get; set; }
		Action<byte[]> onRecv { get; set; }
		Action<string> onError { get; set; }

		void Connect(string url);
		void Disconnect();
		void SendMessage(byte[] datas);
		void SendMessage(string datas, Encoding encoding);
	}
}
