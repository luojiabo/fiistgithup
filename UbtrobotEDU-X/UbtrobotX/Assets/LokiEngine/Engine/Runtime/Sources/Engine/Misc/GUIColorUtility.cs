using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Loki
{
	public sealed class GUIColorScope : IDisposable
	{
		public GUIColorScope(Color color)
		{
			GUIColorUtility.Push();
			GUI.color = color;
		}

		public void Dispose()
		{
			GUIColorUtility.Pop();
		}
	}

	public class GUIColorUtility
	{
		private static readonly Stack<Color> msColorStack = new Stack<Color>();

		public GUIColorUtility()
		{

		}

		public static void Push()
		{
			msColorStack.Push(GUI.color);
		}

		public static void Push(Color color)
		{
			msColorStack.Push(color);
		}

		public static void Abandon()
		{
			if (msColorStack.Count > 0)
			{
				msColorStack.Pop();
			}
		}

		public static void NewStack()
		{
			msColorStack.Clear();
			msColorStack.Push(GUI.color);
		}

		public static void NewStack(Color top)
		{
			msColorStack.Clear();
			msColorStack.Push(top);
		}

		public static void Clear()
		{
			msColorStack.Clear();
		}

		public static void Pop()
		{
			if (msColorStack.Count > 0)
			{
				GUI.color = msColorStack.Pop();
			}
		}

		public static void Peek()
		{
			if (msColorStack.Count > 0)
			{
				GUI.color = msColorStack.Peek();
			}
		}

		public static void Revert()
		{
			while (msColorStack.Count > 1)
			{
				msColorStack.Pop();
			}
			Pop();
		}
	}
}
