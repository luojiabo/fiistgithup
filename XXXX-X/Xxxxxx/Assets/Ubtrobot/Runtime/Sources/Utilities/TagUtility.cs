using System;
using System.Collections.Generic;
using Loki;

namespace Ubtrobot
{
	public class TagUtility
	{
		public static readonly string MainCamera = "MainCamera";
		public static readonly string Untagged = "Untagged";
		public static readonly string EditorOnly = "EditorOnly";
	}

	[LoggerTags]
	public static class UBTTags
	{
	}
}
