using System;
namespace Loki
{
	public static class DelegateUtility
	{
		public static void SafeInvoke(this Action action)
		{
			try
			{
				if (action != null) action();
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Engine, "Fail to invoke SafeInvoke : {0}", ex);
			}
		}

		public static void SafeInvoke<T>(this Action<T> action, T t)
		{
			try
			{
				if (action != null) action(t);
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Engine, "Fail to invoke SafeInvoke : {0}, Ex : {1}", t, ex);
			}
		}

		public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2)
		{
			try
			{
				if (action != null) action(t1, t2);
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Engine, "Fail to invoke SafeInvoke : {0}, {1}, Ex : {2}", t1, t2, ex);
			}
		}

		public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
		{
			try
			{
				if (action != null) action(t1, t2, t3);
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Engine, "Fail to invoke SafeInvoke : {0}, {1}, {2}, Ex : {2}", t1, t2, t3, ex);
			}
		}
	}
}
