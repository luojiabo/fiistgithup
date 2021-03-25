using System;

namespace Loki
{
	public enum EEngineAttributeType
	{
		UCLASS,
		USTRUCT,
		UENUM,
		UPROPERTY,
		UFUNCTION,
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class UCLASSAttribute : LokiMainAttribute
	{
		public string config { get; set; }

		public UCLASSAttribute()
		{
			config = string.Empty;
		}
	}

	[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class USTRUCTAttribute : LokiMainAttribute
	{
		public string config { get; set; }

		public USTRUCTAttribute()
		{
			config = string.Empty;
		}
	}

	[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
	public class UENUMAttribute : LokiMainAttribute
	{
		public UENUMAttribute()
		{
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class UPROPERTYAttribute : LokiMainAttribute
	{
		public bool consoleVar { get; set; }

		public UPROPERTYAttribute()
		{
			consoleVar = false;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class UFUNCTIONAttribute : LokiMainAttribute
	{
		public bool exe { get; set; }

		public UFUNCTIONAttribute()
		{
			exe = false;
		}
	}


}
