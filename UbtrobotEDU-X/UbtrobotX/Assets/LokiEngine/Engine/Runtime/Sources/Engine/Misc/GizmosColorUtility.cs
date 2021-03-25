using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Loki
{
	public sealed class GizmosColorScope : IDisposable
	{
		public GizmosColorScope(Color color)
		{
			GizmosColorUtility.Push();
			Gizmos.color = color;
		}

		public void Dispose()
		{
			GizmosColorUtility.Pop();
		}
	}

	public class GizmosColorUtility
	{
		private static readonly Stack<Color> msColorStack = new Stack<Color>();

		public GizmosColorUtility()
		{

		}

		public static void Push()
		{
			msColorStack.Push(Gizmos.color);
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
			msColorStack.Push(Gizmos.color);
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
				Gizmos.color = msColorStack.Pop();
			}
		}

		public static void Peek()
		{
			if (msColorStack.Count > 0)
			{
				Gizmos.color = msColorStack.Peek();
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
