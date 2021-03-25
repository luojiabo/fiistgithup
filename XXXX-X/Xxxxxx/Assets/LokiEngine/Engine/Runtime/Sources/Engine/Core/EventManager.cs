using System;
using System.Collections.Generic;

namespace Loki
{
	public static class EventManager
	{
		public static void RegisterEvent(string eventName, Action handler)
		{
			EventManager.RegisterEvent(eventName, (Delegate)handler);
		}

		public static void RegisterEvent(object obj, string eventName, Action handler)
		{
			EventManager.RegisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void RegisterEvent<T>(string eventName, Action<T> handler)
		{
			EventManager.RegisterEvent(eventName, (Delegate)handler);
		}

		public static void RegisterEvent<T>(object obj, string eventName, Action<T> handler)
		{
			EventManager.RegisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void RegisterEvent<T, U>(string eventName, Action<T, U> handler)
		{
			EventManager.RegisterEvent(eventName, (Delegate)handler);
		}

		public static void RegisterEvent<T, U>(object obj, string eventName, Action<T, U> handler)
		{
			EventManager.RegisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void RegisterEvent<T, U, V>(string eventName, Action<T, U, V> handler)
		{
			EventManager.RegisterEvent(eventName, (Delegate)handler);
		}

		public static void RegisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler)
		{
			EventManager.RegisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void RegisterEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler)
		{
			EventManager.RegisterEvent(eventName, (Delegate)handler);
		}

		public static void RegisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler)
		{
			EventManager.RegisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void ExecuteEvent(string eventName)
		{
			Action action = EventManager.GetDelegate(eventName) as Action;
			if (action != null)
			{
				action();
			}
		}

		public static void ExecuteEvent(object obj, string eventName)
		{
			Action action = EventManager.GetDelegate(obj, eventName) as Action;
			if (action != null)
			{
				action();
			}
		}

		public static void ExecuteEvent<T>(string eventName, T arg1)
		{
			Action<T> action = EventManager.GetDelegate(eventName) as Action<T>;
			if (action != null)
			{
				action(arg1);
			}
		}

		public static void ExecuteEvent<T>(object obj, string eventName, T arg1)
		{
			Action<T> action = EventManager.GetDelegate(obj, eventName) as Action<T>;
			if (action != null)
			{
				action(arg1);
			}
		}

		public static void ExecuteEvent<T, U>(string eventName, T arg1, U arg2)
		{
			Action<T, U> action = EventManager.GetDelegate(eventName) as Action<T, U>;
			if (action != null)
			{
				action(arg1, arg2);
			}
		}

		public static void ExecuteEvent<T, U>(object obj, string eventName, T arg1, U arg2)
		{
			Action<T, U> action = EventManager.GetDelegate(obj, eventName) as Action<T, U>;
			if (action != null)
			{
				action(arg1, arg2);
			}
		}

		public static void ExecuteEvent<T, U, V>(string eventName, T arg1, U arg2, V arg3)
		{
			Action<T, U, V> action = EventManager.GetDelegate(eventName) as Action<T, U, V>;
			if (action != null)
			{
				action(arg1, arg2, arg3);
			}
		}

		public static void ExecuteEvent<T, U, V>(object obj, string eventName, T arg1, U arg2, V arg3)
		{
			Action<T, U, V> action = EventManager.GetDelegate(obj, eventName) as Action<T, U, V>;
			if (action != null)
			{
				action(arg1, arg2, arg3);
			}
		}

		public static void ExecuteEvent<T, U, V, W>(string eventName, T arg1, U arg2, V arg3, W arg4)
		{
			Action<T, U, V, W> action = EventManager.GetDelegate(eventName) as Action<T, U, V, W>;
			if (action != null)
			{
				action(arg1, arg2, arg3, arg4);
			}
		}

		public static void ExecuteEvent<T, U, V, W>(object obj, string eventName, T arg1, U arg2, V arg3, W arg4)
		{
			Action<T, U, V, W> action = EventManager.GetDelegate(obj, eventName) as Action<T, U, V, W>;
			if (action != null)
			{
				action(arg1, arg2, arg3, arg4);
			}
		}

		public static void UnregisterEvent(string eventName, Action handler)
		{
			EventManager.UnregisterEvent(eventName, (Delegate)handler);
		}

		public static void UnregisterEvent(object obj, string eventName, Action handler)
		{
			EventManager.UnregisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void UnregisterEvent<T>(string eventName, Action<T> handler)
		{
			EventManager.UnregisterEvent(eventName, (Delegate)handler);
		}

		public static void UnregisterEvent<T>(object obj, string eventName, Action<T> handler)
		{
			EventManager.UnregisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void UnregisterEvent<T, U>(string eventName, Action<T, U> handler)
		{
			EventManager.UnregisterEvent(eventName, (Delegate)handler);
		}

		public static void UnregisterEvent<T, U>(object obj, string eventName, Action<T, U> handler)
		{
			EventManager.UnregisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void UnregisterEvent<T, U, V>(string eventName, Action<T, U, V> handler)
		{
			EventManager.UnregisterEvent(eventName, (Delegate)handler);
		}

		public static void UnregisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler)
		{
			EventManager.UnregisterEvent(obj, eventName, (Delegate)handler);
		}

		public static void UnregisterEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler)
		{
			EventManager.UnregisterEvent(eventName, (Delegate)handler);
		}

		public static void UnregisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler)
		{
			EventManager.UnregisterEvent(obj, eventName, (Delegate)handler);
		}

		private static void RegisterEvent(string eventName, Delegate handler)
		{
			Delegate @delegate;
			if (EventManager.msGlobalEventTable.TryGetValue(eventName, out @delegate))
			{
				if (@delegate == null || !Array.Exists<Delegate>(@delegate.GetInvocationList(), (Delegate element) => element == handler))
				{
					EventManager.msGlobalEventTable[eventName] = Delegate.Combine(@delegate, handler);
				}
			}
			else
			{
				EventManager.msGlobalEventTable.Add(eventName, handler);
			}
		}

		private static void RegisterEvent(object obj, string eventName, Delegate handler)
		{
			Dictionary<string, Delegate> dictionary;
			if (!EventManager.msEventTable.TryGetValue(obj, out dictionary))
			{
				dictionary = new Dictionary<string, Delegate>();
				EventManager.msEventTable.Add(obj, dictionary);
			}
			Delegate a;
			if (dictionary.TryGetValue(eventName, out a))
			{
				dictionary[eventName] = Delegate.Combine(a, handler);
			}
			else
			{
				dictionary.Add(eventName, handler);
			}
		}

		private static Delegate GetDelegate(string eventName)
		{
			Delegate result;
			if (EventManager.msGlobalEventTable.TryGetValue(eventName, out result))
			{
				return result;
			}
			return null;
		}

		private static Delegate GetDelegate(object obj, string eventName)
		{
			Dictionary<string, Delegate> dictionary;
			Delegate result;
			if (EventManager.msEventTable.TryGetValue(obj, out dictionary) && dictionary.TryGetValue(eventName, out result))
			{
				return result;
			}
			return null;
		}

		private static void UnregisterEvent(string eventName, Delegate handler)
		{
			Delegate source;
			if (EventManager.msGlobalEventTable.TryGetValue(eventName, out source))
			{
				EventManager.msGlobalEventTable[eventName] = Delegate.Remove(source, handler);
			}
		}

		private static void UnregisterEvent(object obj, string eventName, Delegate handler)
		{
			Dictionary<string, Delegate> dictionary;
			Delegate source;
			if (EventManager.msEventTable.TryGetValue(obj, out dictionary) && dictionary.TryGetValue(eventName, out source))
			{
				dictionary[eventName] = Delegate.Remove(source, handler);
			}
		}

		private static Dictionary<object, Dictionary<string, Delegate>> msEventTable = new Dictionary<object, Dictionary<string, Delegate>>();

		private static Dictionary<string, Delegate> msGlobalEventTable = new Dictionary<string, Delegate>();
	}
}
