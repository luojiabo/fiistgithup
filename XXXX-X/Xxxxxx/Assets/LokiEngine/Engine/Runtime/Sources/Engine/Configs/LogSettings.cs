using UnityEngine;
using System;
using System.Collections.Generic;

namespace Loki
{
	[CreateAssetMenu(menuName = "Loki/Configs/Engine/Log Settings", fileName = "LogSettings")]
	public class LogSettings : UAssetObject
	{
		public List<string> filterTags = new List<string>();
		public List<string> filterIgnoreTags = new List<string>();

		public LogType filterLogType = LogType.Log;
	}
}
