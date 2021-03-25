namespace Loki
{
	public class GlobalEvents
	{
		public delegate void OnApplicationPauseEvent(bool pause);
		public delegate void OnApplicationFocusEvent(bool focus);
		public delegate void OnApplicationQuitEvent();

		public static OnApplicationPauseEvent onApplicationPause;
		public static OnApplicationFocusEvent onApplicationFocus;
		public static OnApplicationQuitEvent onApplicationQuit;

		public static void OnApplicationPause(bool pause)
		{
			try
			{
				if (onApplicationPause != null)
				{
					onApplicationPause(pause);
				}
			}
			catch (System.Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public static void OnApplicationFocus(bool focus)
		{
			try
			{
				if (onApplicationFocus != null)
				{
					onApplicationFocus(focus);
				}
			}
			catch (System.Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public static void OnApplicationQuit()
		{
			try
			{
				if (onApplicationQuit != null)
				{
					onApplicationQuit();
				}
			}
			catch (System.Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}
	}
}
