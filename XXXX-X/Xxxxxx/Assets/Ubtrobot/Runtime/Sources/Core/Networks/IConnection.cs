using System;
using Loki;

namespace Ubtrobot
{
	public interface IConnection : IDisposable
	{
		ENetState state { get; }
		IDispatcher GetDispatcher();
		void Connect(string host);
		void Disconnect();
		void SendMessage(IProtocol message);
	}

	public interface IServerListener : IDisposable
	{
		ENetState state { get; }
		void StartListener(string host);
		void StopListener();
		void SendMessage(IProtocol message);
		void SendMessage(string host, byte[] message);
	}
}
