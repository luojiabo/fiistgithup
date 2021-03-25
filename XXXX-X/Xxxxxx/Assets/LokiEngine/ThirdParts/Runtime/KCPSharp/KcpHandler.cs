using System;
using System.Collections.Generic;
using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace KCP
{
	public class KcpHandler : IKcpCallback
	{
		public Action<Memory<byte>> output;
		public Action<byte[]> recv;

		public void Receive(byte[] buffer)
		{
			if (recv == null)
				return;

			recv(buffer);
		}

		public IMemoryOwner<byte> RentBuffer(int lenght)
		{
			return null;
		}

		public void Output(IMemoryOwner<byte> buffer, int avalidLength)
		{
			if (output == null)
				return;

			using (buffer)
			{
				output(buffer.Memory.Slice(0, avalidLength));
			}
		}
	}
}
