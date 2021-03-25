using System;
using System.Collections.Generic;

namespace Loki
{
	/// <summary>
	/// Make a singleton object
	/// </summary>
	/// <typeparam name="TMostDerived">The TMostDerived must be the most derived class</typeparam>
	public abstract class Singleton<TMostDerived> : ISingleton where TMostDerived : Singleton<TMostDerived>, new()
	{
		private static TMostDerived msInstance;
		private bool mReleased = false;

		public static bool Exists()
		{
			return msInstance != null;
		}

		public static TMostDerived GetOrAlloc()
		{
			if (msInstance == null)
			{
				msInstance = new TMostDerived();
				msInstance.OnInitialize();
			}
			return msInstance;
		}

		public static void SafeRelease()
		{
			if (msInstance != null)
			{
				msInstance.Release();
			}
		}

		protected Singleton()
		{

		}

		public void Release()
		{
			if (mReleased)
				return;
			mReleased = true;

			OnDestroy();
		}

		protected virtual void OnInitialize()
		{
		}

		/// <summary>
		/// Must call base.OnDestroy() after your overrided method
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (msInstance == this)
				msInstance = null;
		}
	}
}
