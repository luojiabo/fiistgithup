using System;
using System.Collections.Generic;
using Loki;

namespace Ubtrobot
{
	public interface IDispatcher
	{
		void OnConnected();
		void OnDisconnected(ENetCode error);
		void OnMessage(IProtocol protocol);
		void OnError(string errMsg);
		void Clear();
	}

	public interface IServerTransmit
	{
		//void Forward(string path, IMess)
	}
}
