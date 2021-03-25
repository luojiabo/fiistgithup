using System;
using System.Collections.Generic;


namespace Loki
{
	public interface INetServer : IDisposable
	{
	}

	public interface IWebSocketServer : INetServer
	{
		bool isListening { get; }
		void StartListening();
		void StopListening();
		void SendMessage(string path, byte[] message);
	}
}

