using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Loki
{
	public struct ReflectionTypeInfo
	{
		private readonly List<FieldInfo> mFieldInfos;
		private readonly List<PropertyInfo> mPropertyInfos;
		private readonly List<MethodInfo> mMethodInfos;
		public List<FieldInfo> fieldInfos
		{
			get
			{
				if (mFieldInfos.Count == 0)
				{
					mFieldInfos.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
				}
				return mFieldInfos;
			}
		}

		public List<PropertyInfo> propertyInfos
		{
			get
			{
				if (mPropertyInfos.Count == 0)
				{
					mPropertyInfos.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
				}
				return mPropertyInfos;
			}
		}

		public List<MethodInfo> methodInfos
		{
			get
			{
				if (mMethodInfos.Count == 0)
				{
					mMethodInfos.AddRange(type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
				}
				return mMethodInfos;
			}
		}

		public LokiMainAttribute engineAttribute { get; private set; }
		public Type type { get; private set; }

		public TAttribute GetLokiAttribute<TAttribute>() where TAttribute : LokiMainAttribute
		{
			return engineAttribute as TAttribute;
		}

		public TAttribute GetCustomAttribute<TAttribute>() where TAttribute : Attribute
		{
			return type.GetCustomAttribute<TAttribute>();
		}

		public IEnumerable<TAttribute> GetCustomAttributes<TAttribute>() where TAttribute : Attribute
		{
			return type.GetCustomAttributes<TAttribute>();
		}

		public List<MethodInfo> FindMethods<TAttribute>(List<MethodInfo> caches = null) where TAttribute : Attribute
		{
			return FindMethods((m) => m.GetCustomAttribute<TAttribute>() != null, caches);
		}

		public List<MethodInfo> FindMethods<TAttribute>(bool inherit, List<MethodInfo> caches = null) where TAttribute : Attribute
		{
			return FindMethods((m) => m.GetCustomAttribute<TAttribute>(inherit) != null, caches);
		}

		public List<MethodInfo> FindMethods(Predicate<MethodInfo> predicate, List<MethodInfo> caches = null)
		{
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindMethods");
			if (caches == null)
			{
				caches = new List<MethodInfo>();
			}
			foreach (var m in methodInfos)
			{
				if (predicate(m))
				{
					caches.Add(m);
				}
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public List<FieldInfo> FindFields<TAttribute>(List<FieldInfo> caches = null) where TAttribute : Attribute
		{
			return FindFields((m) => m.GetCustomAttribute<TAttribute>() != null, caches);
		}

		public List<MemberInfo> FindMembers<TAttribute>(bool inherit = true, List<MemberInfo> caches = null) where TAttribute : Attribute
		{
			if (caches == null)
				caches = new List<MemberInfo>();
			caches.AddRange(FindFields((m) => m.GetCustomAttribute<TAttribute>(inherit) != null));
			caches.AddRange(FindProperties((m) => m.GetCustomAttribute<TAttribute>(inherit) != null));
			return caches;
		}

		public List<MemberInfo> FindMembers(List<MemberInfo> caches = null)
		{
			if (caches == null)
				caches = new List<MemberInfo>();
			caches.AddRange(fieldInfos);
			caches.AddRange(propertyInfos);
			return caches;
		}

		public bool IsFieldValueOfObject(object target, object value, out FieldInfo result)
		{
			result = null;
			foreach (var info in fieldInfos)
			{
				if (info.GetValue(target) == value)
				{
					result = info;
					return true;
				}
			}
			return false;
		}

		public bool IsPropertyValueOfObject(object target, object value, out PropertyInfo result)
		{
			result = null;
			foreach (var info in propertyInfos)
			{
				if (info.GetValue(target) == value)
				{
					result = info;
					return true;
				}
			}
			return false;
		}

		public bool IsMemberValueOfObject(object target, object value)
		{
			if (IsFieldValueOfObject(target, value, out var field))
			{
				return true;
			}
			if (IsPropertyValueOfObject(target, value, out var p))
			{
				return true;
			}
			return false;
		}

		public List<FieldInfo> FindSerializeFields(List<FieldInfo> caches = null)
		{
			return FindFields((fieldInfo) => (!fieldInfo.IsStatic) && (fieldInfo.IsPublic || fieldInfo.GetCustomAttribute<UnityEngine.SerializeField>() != null), caches);
		}

		public List<FieldInfo> FindFields<TAttribute>(bool inherit, List<FieldInfo> caches = null) where TAttribute : Attribute
		{
			return FindFields((field) => field.GetCustomAttribute<TAttribute>(inherit) != null, caches);
		}

		public List<PropertyInfo> FindProperties<TAttribute>(bool inherit, List<PropertyInfo> caches = null) where TAttribute : Attribute
		{
			return FindProperties((field) => field.GetCustomAttribute<TAttribute>(inherit) != null, caches);
		}

		public List<FieldInfo> FindFields(Predicate<FieldInfo> predicate, List<FieldInfo> caches = null)
		{
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindFields");
			if (caches == null)
			{
				caches = new List<FieldInfo>();
			}
			foreach (var field in fieldInfos)
			{
				if (predicate(field))
				{
					caches.Add(field);
				}
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public List<PropertyInfo> FindProperties(Predicate<PropertyInfo> predicate, List<PropertyInfo> caches = null)
		{
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindProperties");
			if (caches == null)
			{
				caches = new List<PropertyInfo>();
			}
			foreach (var property in propertyInfos)
			{
				if (predicate(property))
				{
					caches.Add(property);
				}
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public object Invoke(string method, object target, object[] paramsters, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic)
		{
			try
			{
				if (type == null)
				{
					return null;
				}

				if (target == null)
				{
					flags |= BindingFlags.Static;
				}
				else
				{
					flags |= BindingFlags.Instance;
				}

				var methodInfo = type.GetMethod(method, flags);
				if (methodInfo != null)
				{
					return methodInfo.Invoke(target, paramsters);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return null;
		}

		public ReflectionTypeInfo(Type type)
		{
			this.type = type;
			engineAttribute = type.GetCustomAttribute<LokiMainAttribute>();
			mMethodInfos = new List<MethodInfo>();
			mFieldInfos = new List<FieldInfo>();
			mPropertyInfos = new List<PropertyInfo>();
		}
	}

	public class ReflectionAssemblyInfo
	{
		private readonly Dictionary<Type, ReflectionTypeInfo> mTypesInfo = new Dictionary<Type, ReflectionTypeInfo>(EqualityComparer<Type>.Default);

		public Assembly Assembly { get; private set; }

		public ReflectionAssemblyInfo(Assembly assembly)
		{
			Assembly = assembly;
		}

		public ReflectionTypeInfo FindOrAdd(Type type)
		{
			ReflectionTypeInfo info;
			if (!mTypesInfo.TryGetValue(type, out info))
			{
				info = new ReflectionTypeInfo(type);
				mTypesInfo.Add(type, info);
			}
			return info;
		}

		public ReflectionTypeInfo FindType(string typeName, bool isFullName)
		{
			ReflectionTypeInfo result = default;
			ProfilingUtility.BeginSample("ReflectionAssemblyInfo.FindType");
			foreach (var typeInfo in mTypesInfo.Values)
			{
				if ((!isFullName && typeInfo.type.Name == typeName) || 
					(isFullName && typeInfo.type.FullName == typeName))
				{
					result = typeInfo;
					break;
				}
			}
			ProfilingUtility.EndSample();
			return result;
		}

		public List<Type> FindTypes<TAttribute>(bool allowAbstract, List<Type> caches = null) where TAttribute : Attribute
		{
			ProfilingUtility.BeginSample("ReflectionAssemblyInfo.FindTypes");
			foreach (var typeInfo in mTypesInfo.Values)
			{
				if (typeInfo.type.IsAbstract && !allowAbstract)
					continue;

				if (typeInfo.GetCustomAttribute<TAttribute>() != null)
				{
					if (caches == null)
					{
						caches = new List<Type>();
					}
					caches.Add(typeInfo.type);
				}
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public List<Type> FindTypes<TAttribute>(Type typeConstraint, bool allowAbstract, List<Type> caches = null) where TAttribute : Attribute
		{
			ProfilingUtility.BeginSample("ReflectionAssemblyInfo.FindTypes");
			foreach (var typeInfo in mTypesInfo.Values)
			{
				if (typeInfo.type.IsAbstract && !allowAbstract)
					continue;

				if ((typeInfo.GetCustomAttribute<TAttribute>() != null) && typeInfo.type.IsCompatibleWith(typeConstraint))
				{
					if (caches == null)
						caches = new List<Type>();

					caches.Add(typeInfo.type);
				}
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public List<Type> FindTypes(Type typeConstraint, bool allowAbstract, List<Type> caches = null)
		{
			ProfilingUtility.BeginSample("ReflectionAssemblyInfo.FindTypes");
			foreach (var typeInfo in mTypesInfo.Values)
			{
				if (typeInfo.type.IsAbstract && !allowAbstract)
					continue;

				if (typeInfo.type.IsCompatibleWith(typeConstraint))
				{
					if (caches == null)
						caches = new List<Type>();

					caches.Add(typeInfo.type);
				}
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public Type FindType<TAttribute>(bool allowAbstract) where TAttribute : Attribute
		{
			Type result = null;
			ProfilingUtility.BeginSample("ReflectionAssemblyInfo.FindType");
			foreach (var typeInfo in mTypesInfo.Values)
			{
				if (typeInfo.type.IsAbstract && !allowAbstract)
					continue;

				if (typeInfo.GetCustomAttribute<TAttribute>() != null)
				{
					result = typeInfo.type;
					break;
				}
			}
			ProfilingUtility.EndSample();
			return result;
		}

		public Type FindType<TAttribute>(Type typeConstraint, bool allowAbstract) where TAttribute : Attribute
		{
			Type result = null;
			ProfilingUtility.BeginSample("ReflectionAssemblyInfo.FindType");
			foreach (var typeInfo in mTypesInfo.Values)
			{
				if (typeInfo.type.IsAbstract && !allowAbstract)
					continue;

				if ((typeInfo.GetCustomAttribute<TAttribute>() != null) && typeInfo.type.IsCompatibleWith(typeConstraint))
				{
					result = typeInfo.type;
					break;
				}
			}
			ProfilingUtility.EndSample();
			return result;
		}

		public Type FindType(Type typeConstraint, bool allowAbstract)
		{
			Type result = null;
			ProfilingUtility.BeginSample("ReflectionAssemblyInfo.FindType");
			foreach (var typeInfo in mTypesInfo.Values)
			{
				if (typeInfo.type.IsAbstract && !allowAbstract)
					continue;

				if (typeInfo.type.IsCompatibleWith(typeConstraint))
				{
					result = typeInfo.type;
					break;
				}
			}
			ProfilingUtility.EndSample();
			return result;
		}

		public void Load()
		{
			try
			{
				Type[] types = Assembly.GetTypes();
				foreach (var type in types)
				{
					FindOrAdd(type);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Engine, "Catch exception when load types from assembly: {0}", Assembly.FullName);
				DebugUtility.LogException(ex);
			}
		}
	}

	public class GlobalReflectionCache
	{
		private static readonly List<string> msIgnoreAssemblyNames = new List<string>();

		private static readonly Dictionary<Assembly, ReflectionAssemblyInfo> msAssemblyTypeCaches;

		static GlobalReflectionCache()
		{
			msAssemblyTypeCaches = new Dictionary<Assembly, ReflectionAssemblyInfo>(EqualityComparer<Assembly>.Default);
		}

		private static void LoadBlacklistIfNeeds()
		{
			if (msIgnoreAssemblyNames.Count == 0)
			{
				msIgnoreAssemblyNames.AddRange(EngineSettings.GetOrLoad().reflectionBlacklist.blacklist);
			}
		}

		public static void CollectTypes(Assembly assembly)
		{
			ProfilingUtility.BeginSample("GlobalReflectionCache.CollectTypes_", assembly.FullName);

			ReflectionAssemblyInfo typesCache;
			if (!msAssemblyTypeCaches.TryGetValue(assembly, out typesCache))
			{
				typesCache = new ReflectionAssemblyInfo(assembly);
				msAssemblyTypeCaches.Add(assembly, typesCache);
				typesCache.Load();
			}
			ProfilingUtility.EndSample();
		}

		public static void LoadAssemblies(bool force = false)
		{
			if (msAssemblyTypeCaches.Count > 0 && !force)
				return;
			msAssemblyTypeCaches.Clear();

			Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			if (allAssemblies == null)
				return;

			ProfilingUtility.BeginSample("GlobalReflectionCache.LoadAssemblies");
			foreach (Assembly assembly in allAssemblies)
			{
				if (assembly.FullName.Contains("Microsoft."))
					continue;

				//DebugUtility.LogTrace(LoggerTags.Engine, "GlobalReflectionCache.CollectTypes {0}", assembly.FullName);
				ProfilingUtility.BeginSample("GlobalReflectionCache.CollectTypes_", assembly.FullName);
				GlobalReflectionCache.CollectTypes(assembly);
				ProfilingUtility.EndSample();
			}
			ProfilingUtility.EndSample();
		}

		public static Type FindType<TAttribute>(bool allowAbstract) where TAttribute : Attribute
		{
			Type result = null;
			LoadAssemblies();
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindType");
			foreach (var assemblyInfo in msAssemblyTypeCaches.Values)
			{
				result = assemblyInfo.FindType<TAttribute>(allowAbstract);
				if (result != null)
				{
					break;
				}
			}
			ProfilingUtility.EndSample();
			return result;
		}

		public static Type FindType<TAttribute>(Type typeConstraint, bool allowAbstract) where TAttribute : Attribute
		{
			Type result = null;
			LoadAssemblies();
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindType");
			foreach (var assemblyInfo in msAssemblyTypeCaches.Values)
			{
				result = assemblyInfo.FindType<TAttribute>(typeConstraint, allowAbstract);
				if (result != null)
				{
					break;
				}
			}
			ProfilingUtility.EndSample();
			return result;
		}

		public static Type FindType(Type typeConstraint, bool allowAbstract)
		{
			Type result = null;
			LoadAssemblies();
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindType");
			foreach (var assemblyInfo in msAssemblyTypeCaches.Values)
			{
				result = assemblyInfo.FindType(typeConstraint, allowAbstract);
				if (result != null)
				{
					break;
				}
			}
			ProfilingUtility.EndSample();
			return result;
		}

		public static List<Type> FindTypes<TAttribute>(bool allowAbstract, List<Type> caches = null) where TAttribute : Attribute
		{
			LoadAssemblies();
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindAllTypesContainsAttribute");
			foreach (var assemblyInfo in msAssemblyTypeCaches.Values)
			{
				caches = assemblyInfo.FindTypes<TAttribute>(allowAbstract, caches);
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public static List<Type> FindTypes<TAttribute>(Type typeConstraint, bool allowAbstract, List<Type> caches = null) where TAttribute : Attribute
		{
			LoadAssemblies();
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindAllTypesContainsAttribute");
			foreach (var assemblyInfo in msAssemblyTypeCaches.Values)
			{
				caches = assemblyInfo.FindTypes<TAttribute>(typeConstraint, allowAbstract, caches);
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public static List<Type> FindTypes(Type typeConstraint, bool allowAbstract, List<Type> caches = null)
		{
			LoadAssemblies();
			ProfilingUtility.BeginSample("GlobalReflectionCache.FindAllTypesContainsAttribute");
			foreach (var assemblyInfo in msAssemblyTypeCaches.Values)
			{
				caches = assemblyInfo.FindTypes(typeConstraint, allowAbstract, caches);
			}
			ProfilingUtility.EndSample();
			return caches;
		}

		public static ReflectionTypeInfo FindOrAdd(Type type)
		{
			CollectTypes(type.Assembly);
			if (msAssemblyTypeCaches.TryGetValue(type.Assembly, out var info))
			{
				return info.FindOrAdd(type);
			}
			return default;
		}

		public static ReflectionTypeInfo Find(string typeName, bool isFullName, string assemblyName = "")
		{
			ReflectionTypeInfo result = default;
			if (string.IsNullOrEmpty(typeName)) return result;
			ProfilingUtility.BeginSample("GlobalReflectionCache.Find");
			foreach (var item in msAssemblyTypeCaches)
			{
				if (string.IsNullOrEmpty(assemblyName) || item.Key.FullName == assemblyName)
				{
					result = item.Value.FindType(typeName, isFullName);
					if(result.type != null)
					{
						break;
					}
				}
			}
			ProfilingUtility.EndSample();
			return result;
		}
	}

	public class ReflectionUtility<T>
	{
		private static readonly Assembly msAssemblyOfT;

		static ReflectionUtility()
		{
			msAssemblyOfT = typeof(T).Assembly;
		}

		//private readonly Dictionary<Assembly, >

		public static Assembly GetAssembly()
		{
			return msAssemblyOfT;
		}

		public static void CollectTypes()
		{
			ProfilingUtility.BeginSample("Reflection<T>.CollectTypes_", msAssemblyOfT.FullName);
			GlobalReflectionCache.CollectTypes(GetAssembly());
			ProfilingUtility.EndSample();
		}
	}

}
