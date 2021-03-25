using System;
using System.Linq;
using System.Reflection;

namespace Loki
{
	public static class AttributeUtility
	{
		public static Attribute GetCustomAttribute(this ICustomAttributeProvider type, Type attributeType, bool inherit)
		{
			object[] attributes = type.GetCustomAttributes(attributeType, inherit);
			if (attributes != null && attributes.Length > 0)
			{
				return attributes[0] as Attribute;
			}
			return null;
		}

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider type, bool inherit) where T : Attribute
		{
			object[] attributes = type.GetCustomAttributes(typeof(T), inherit);
			if (attributes != null && attributes.Length > 0)
			{
				return attributes[0] as T;
			}
			return null;
		}

		public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider type, bool inheri) where T : Attribute
		{
			object[] attributes = type.GetCustomAttributes(typeof(T), inheri);
			if (attributes != null)
			{
				var result = new T[attributes.Length];
				for (var idx = 0; idx < result.Length; ++idx)
				{
					result[idx] = attributes[idx] as T;
				}
			}
			return null;
		}

		public static Attribute[] GetCustomAttributes(this ICustomAttributeProvider type, Type attributeType, bool inherit)
		{
			object[] attributes = type.GetCustomAttributes(attributeType, inherit);
			if (attributes != null)
			{
				var result = new Attribute[attributes.Length];
				for (var idx = 0; idx < result.Length; ++idx)
				{
					result[idx] = attributes[idx] as Attribute;
				}
			}
			return null;
		}

		public static bool IsStatic(this PropertyInfo source, bool nonPublic = false)
		{
			return source.GetAccessors(nonPublic).Any(x => x.IsStatic);
		}
	}
}
