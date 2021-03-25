using HybridWebSocket;

namespace Loki
{
	public static class WebSocketExtensions
	{
		public static ENetState ToNetState(this WebSocketState state)
		{
			switch (state)
			{
				case WebSocketState.Closed:
					return ENetState.Disconnected;
				case WebSocketState.Closing:
					return ENetState.Disconnecting;
				case WebSocketState.Connecting:
					return ENetState.Connecting;
				case WebSocketState.Open:
					return ENetState.Connected;
			}
			return ENetState.Disconnected;
		}

		public static ENetCode ToNetCode(this WebSocketCloseCode code)
		{
			switch (code)
			{
				case WebSocketCloseCode.NotSet:
					return ENetCode.WSNotSet;
				case WebSocketCloseCode.Normal:
					return ENetCode.WSNormal;
				case WebSocketCloseCode.Away:
					return ENetCode.WSAway;
				case WebSocketCloseCode.ProtocolError:
					return ENetCode.WSProtocolError;
				case WebSocketCloseCode.UnsupportedData:
					return ENetCode.WSUnsupportedData;
				case WebSocketCloseCode.Undefined:
					return ENetCode.WSUndefined;
				case WebSocketCloseCode.NoStatus:
					return ENetCode.WSNoStatus;
				case WebSocketCloseCode.Abnormal:
					return ENetCode.WSAbnormal;
				case WebSocketCloseCode.InvalidData:
					return ENetCode.WSInvalidData;
				case WebSocketCloseCode.PolicyViolation:
					return ENetCode.WSPolicyViolation;
				case WebSocketCloseCode.TooBig:
					return ENetCode.WSTooBig;
				case WebSocketCloseCode.MandatoryExtension:
					return ENetCode.WSMandatoryExtension;
				case WebSocketCloseCode.ServerError:
					return ENetCode.WSServerError;
				case WebSocketCloseCode.TlsHandshakeFailure:
					return ENetCode.WSTlsHandshakeFailure;
			}
			return ENetCode.WSNotSet;
		}
	}

}
