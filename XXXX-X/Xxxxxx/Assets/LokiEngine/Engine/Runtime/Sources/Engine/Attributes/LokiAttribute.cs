using System;

namespace Loki
{
	public abstract class LokiAttribute : Attribute
	{
		public string tooltip { get; set; }
		public string category { get; set; }

		public LokiAttribute()
		{
			tooltip = string.Empty;
			category = string.Empty;
		}

		public static implicit operator bool(LokiAttribute attr)
		{
			return attr != null;
		}
	}

	public abstract class LokiMainAttribute : LokiAttribute
	{
	}
}
