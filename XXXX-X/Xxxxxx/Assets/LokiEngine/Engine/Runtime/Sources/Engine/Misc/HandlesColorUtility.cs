using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Loki
{
	public sealed class HandlesColorScope : IDisposable
	{
		public HandlesColorScope(Color color)
		{
			HandlesColorUtility.Push();
#if UNITY_EDITOR
			Handles.color = color;
#endif
		}

		public void Dispose()
		{
			HandlesColorUtility.Pop();
		}
	}

	public class HandlesColorUtility
	{
		private static readonly Stack<Color> msColorStack = new Stack<Color>();

		public HandlesColorUtility()
		{

		}

		public static void Push()
		{
#if UNITY_EDITOR
			msColorStack.Push(Handles.color);
#endif
		}

		public static void Push(Color color)
		{
#if UNITY_EDITOR
			msColorStack.Push(color);
#endif
		}

		public static void Abandon()
		{
#if UNITY_EDITOR
			if (msColorStack.Count > 0)
			{
				msColorStack.Pop();
			}
#endif
		}

		public static void NewStack()
		{
#if UNITY_EDITOR
			msColorStack.Clear();
			msColorStack.Push(Handles.color);
#endif
		}

		public static void NewStack(Color top)
		{
#if UNITY_EDITOR
			msColorStack.Clear();
			msColorStack.Push(top);
#endif
		}

		public static void Clear()
		{
			msColorStack.Clear();
		}

		public static void Pop()
		{
#if UNITY_EDITOR
			if (msColorStack.Count > 0)
			{
				Handles.color = msColorStack.Pop();
			}
#endif
		}

		public static void Peek()
		{
#if UNITY_EDITOR
			if (msColorStack.Count > 0)
			{
				Handles.color = msColorStack.Peek();
			}
#endif
		}

		public static void Revert()
		{
#if UNITY_EDITOR
			while (msColorStack.Count > 1)
			{
				msColorStack.Pop();
			}
			Pop();
#endif
		}
	}
}
