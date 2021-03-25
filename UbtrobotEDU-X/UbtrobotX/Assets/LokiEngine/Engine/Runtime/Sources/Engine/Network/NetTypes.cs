using System;
using System.Collections.Generic;

namespace Loki
{
	public enum EProtocolType
	{
		TCP,
		UDP,
		WebSocket,
	}

	public enum ENetState
	{
		Disconnecting,
		Disconnected,
		Connecting,
		Connected,
	}

	public enum ENetCode
	{
		WSNotSet = 0,
		WSNormal = 1000,
		WSAway = 1001,
		WSProtocolError = 1002,
		WSUnsupportedData = 1003,
		WSUndefined = 1004,
		WSNoStatus = 1005,
		WSAbnormal = 1006,
		WSInvalidData = 1007,
		WSPolicyViolation = 1008,
		WSTooBig = 1009,
		WSMandatoryExtension = 1010,
		WSServerError = 1011,
		WSTlsHandshakeFailure = 1015
	}

	public static class NetTypeExtensions
	{
		public static bool IsConnected(this ENetState state)
		{
			return state == ENetState.Connected;
		}

		public static bool IsDisconnected(this ENetState state)
		{
			return state <= ENetState.Disconnected;
		}
	}
}
