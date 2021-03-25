using System;
using System.Collections.Generic;
using System.Linq;

namespace Loki
{
	public abstract class ConsoleAttribute : LokiAttribute
	{
		public string validate { get; set; }
		public string aliasName { get; set; }

		protected ConsoleAttribute()
		{
			validate = "";
			aliasName = "";
		}
	}

	public abstract class ConsoleMemberAttribute : ConsoleAttribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class ConsoleMethodAttribute : ConsoleAttribute
	{
		public ConsoleMethodAttribute()
		{
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ConsoleFieldAttribute : ConsoleMemberAttribute
	{
		public ConsoleFieldAttribute()
		{
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ConsolePropertyAttribute : ConsoleMemberAttribute
	{
		public ConsolePropertyAttribute()
		{
		}
	}
}
