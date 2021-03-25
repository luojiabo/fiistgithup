using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public interface IConsoleObject
	{
		string statID { get; }
		string name { get; }
	}

	public static class IConsoleObjectExtension
	{
		/// <summary>
		/// if RegisterToConsole called, the UnregisterFromConsole must call at the end of this object lifecircle
		/// </summary>
		/// <param name="o">object</param>
		public static void RegisterToConsole(this IConsoleObject o)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
#if LOKI_CONSOLE
			if (o is ConsoleManager)
			{
				((ConsoleManager)o).RegisterConsole(o);
				return;
			}

			ConsoleManager consoleMgr = ConsoleManager.GetOrAlloc();
			if (consoleMgr != null)
			{
				consoleMgr.RegisterConsole(o);
			}
#endif
		}

		/// <summary>
		/// if RegisterToConsole called, the UnregisterFromConsole must call at the end of this object lifecircle
		/// </summary>
		/// <param name="o">object</param>
		public static void UnregisterFromConsole(this IConsoleObject o)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
#if LOKI_CONSOLE
			if (o is ConsoleManager)
			{
				((ConsoleManager)o).UnregisterConsole(o);
				return;
			}

			ConsoleManager consoleMgr = ConsoleManager.Get();
			if (consoleMgr != null)
			{
				consoleMgr.UnregisterConsole(o);
			}
#endif
		}
	}
}
