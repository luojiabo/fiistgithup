using System;
using System.Collections.Generic;

namespace Loki
{
	public abstract class AutomateAttribute : LokiAttribute
	{

	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ModuleAttribute : AutomateAttribute
	{
		public int order { get; set; }

		public bool engine { get; set; }

		public string entryPoint { get; set; }

		public ModuleAttribute()
		{
			order = 0;
			engine = false;
			entryPoint = "";
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class InspectorMethodAttribute : LokiAttribute
	{
		public bool allowMultipleTargets { get; set; }
		public string aliasName { get; set; }
		public float width { get; set; }

		public InspectorMethodAttribute()
		{
			allowMultipleTargets = true;
			aliasName = "";
			width = 200;
		}
	}


	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class CollectionAttribute : LokiAttribute
	{
		public Type keyType { get; set; }
		public Type valueType { get; set; }
	}


	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutoSerializeFieldAttribute : AutomateAttribute
	{
		public string aliasName { get; set; }

		public string autoRemovePrefix { get; set; } = "m";

		public AutoSerializeFieldAttribute()
		{
			this.aliasName = string.Empty;
		}

		public AutoSerializeFieldAttribute(string aliasName)
		{
			this.aliasName = aliasName;
		}
	}

	public abstract class PreviewMemberBaseAttribute : AutomateAttribute
	{
		public bool useRange { get; set; }
		public PreviewMemberBaseAttribute()
		{
			useRange = false;
		}

	}


	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PreviewMemberAttribute : PreviewMemberBaseAttribute
	{
		public PreviewMemberAttribute()
		{
			useRange = false;
		}

		public float rangeMin { get; set; }
		public float rangeMax { get; set; }

		public PreviewMemberAttribute(float min, float max)
		{
			useRange = true;
			rangeMin = min;
			rangeMax = max;
		}
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PreviewMemberDynamicPropertyAttribute : PreviewMemberBaseAttribute
	{
		public string rangeMin { get; set; }
		public string rangeMax { get; set; }

		public PreviewMemberDynamicPropertyAttribute()
		{
			useRange = false;
		}

		public PreviewMemberDynamicPropertyAttribute(string min, string max)
		{
			useRange = true;
			rangeMin = min;
			rangeMax = max;
		}
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PreviewMemberDynamicFieldAttribute : PreviewMemberBaseAttribute
	{
		public string rangeMin { get; set; }
		public string rangeMax { get; set; }

		public PreviewMemberDynamicFieldAttribute()
		{
			useRange = false;
		}

		public PreviewMemberDynamicFieldAttribute(string min, string max)
		{
			useRange = true;
			rangeMin = min;
			rangeMax = max;
		}
	}

	public class DrawArrowCapAttribute : LokiAttribute
	{

	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SceneDrawerAttribute : LokiAttribute
	{
		public string sceneTitle { get; set; }

		public bool breakCombineTooltips { get; set; } = false;

		public SceneDrawerAttribute()
		{
			sceneTitle = string.Empty;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DynamicSceneDrawerAttribute : SceneDrawerAttribute
	{
		/// <summary>
		/// use the "DynamicDrawerName(sceneTitle) to get the title"
		/// <para/>
		/// use the "DynamicDrawerTooltip(sceneTitle, tooltips) to get the tooltip"
		/// </summary>
		public DynamicSceneDrawerAttribute()
		{
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class NameToTypeAttribute  : LokiAttribute
	{

	}
}
