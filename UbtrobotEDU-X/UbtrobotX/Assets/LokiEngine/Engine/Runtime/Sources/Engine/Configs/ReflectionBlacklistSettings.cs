using UnityEngine;
using System;
using System.Collections.Generic;

namespace Loki
{
	[CreateAssetMenu(menuName = "Loki/Configs/Engine/Reflection Blacklist Settings", fileName = "ReflectionBlacklistSettings")]
	public class ReflectionBlacklistSettings : UAssetObject
	{
		public List<string> blacklist = new List<string>();
	}
}
