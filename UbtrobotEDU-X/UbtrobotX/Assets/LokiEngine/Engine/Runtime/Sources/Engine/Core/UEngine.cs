using UnityEngine;

namespace Loki
{
	/// <summary>
	/// The Unity MonoBehaviour lifecycle
	/// <see cref="https://docs.unity3d.com/Manual/ExecutionOrder.html"/>
	/// </summary>
	public abstract class UEngine : UObject
	{
		protected virtual void OnApplicationFocus(bool focus)
		{
			GlobalEvents.OnApplicationFocus(focus);
		}

		protected virtual void OnApplicationPause(bool pause)
		{
			GlobalEvents.OnApplicationPause(pause);
		}

		protected virtual void OnApplicationQuit()
		{
			hasQuitApplication = true;
			GlobalEvents.OnApplicationQuit();
		}

		[ConsoleMethod(aliasName = "quit")]
		public void QuitApplication()
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
			}
#else
			Application.Quit(0);
#endif
		}
	}
}
