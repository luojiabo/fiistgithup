using System;
using System.Collections.Generic;

namespace Loki
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class LoggerAttribute : LokiAttribute
	{
		public List<string> loggerBlacklist { get; private set; }

		public LoggerAttribute()
		{
			loggerBlacklist = new List<string>();
		}

		public LoggerAttribute(List<string> blacklist)
		{
			loggerBlacklist = blacklist;
		}

		public LoggerAttribute(params string[] blacklist)
		{
			loggerBlacklist = new List<string>();
			if (blacklist != null && blacklist.Length > 0)
				loggerBlacklist.AddRange(blacklist);
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public class LoggerTagsAttribute : LokiAttribute
	{
	}
}
