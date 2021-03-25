using System;
using System.Collections.Generic;

namespace Loki
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ConsoleVariableAttribute : LokiAttribute
	{
		public string command { get; set; }
		public string helper { get; set; }

	}
}
