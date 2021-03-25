using System;
using System.Collections.Generic;
using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace KCP
{
	public sealed class KcpClient : IKcpSetting, IKcpUpdate, IDisposable
	{
		private Kcp mConn;
		private IKcpCallback mHandler;

		public IKcpCallback handler => mHandler;

		public KcpClient(uint conv)
		{
			mHandler = new KcpHandler();

			mConn = new Kcp(conv, mHandler);
			mConn.NoDelay(1, 10, 2, 1);//fast
			mConn.WndSize(64, 64);
			mConn.SetMtu(512);
		}

		public KcpClient(uint conv, IKcpCallback handle)
		{
			if (handle == null)
				throw new Exception("The handler must be not null value");

			mHandler = handle;

			mConn = new Kcp(conv, mHandler);
			mConn.NoDelay(1, 10, 2, 1);//fast
			mConn.WndSize(64, 64);
			mConn.SetMtu(512);
		}

		public int Interval(int interval)
		{
			return mConn.Interval(interval);
		}

		public int NoDelay(int nodelay, int interval, int resend, int nc)
		{
			return mConn.NoDelay(nodelay, interval, resend, nc);
		}

		public int SetMtu(int mtu)
		{
			return mConn.SetMtu(mtu);
		}

		public int WndSize(int sndwnd, int rcvwnd)
		{
			return mConn.WndSize(sndwnd, rcvwnd);
		}

		public void Update(DateTime time)
		{
			mConn.Update(time);
		}

		public void Update(uint msFrom1970)
		{
			mConn.Update(msFrom1970);
		}

		public void Dispose()
		{
			mConn.Dispose();
		}
	}
}
