using System;
using System.Diagnostics;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Loki
{
	public sealed class ProfilingUtility
	{
		//
		// 摘要:
		//     Returns the number of bytes that Unity has allocated. This does not include bytes
		//     allocated by external libraries or drivers.
		//
		// 返回结果:
		//     Size of the memory allocated by Unity (or 0 if the profiler is disabled).
		public static long usedHeapSizeLong { get { return Profiler.usedHeapSizeLong; } }
		//
		// 摘要:
		//     Enables the logging of profiling data to a file.
		public static bool enableBinaryLog { get { return Profiler.enableBinaryLog; } set{ Profiler.enableBinaryLog = value; } }
		//
		// 摘要:
		//     Sets the maximum amount of memory that Profiler uses for buffering data. This
		//     property is expressed in bytes.
		public static int maxUsedMemory { get { return Profiler.maxUsedMemory; } set { Profiler.maxUsedMemory = value; } }
		//
		// 摘要:
		//     Enables the Profiler.
		public static bool enabled { get { return Profiler.enabled; } set { Profiler.enabled = value; } }
		//
		// 摘要:
		//     The number of ProfilerArea|Profiler Areas that you can profile.
		public static int areaCount { get { return Profiler.areaCount; } }
		
		public static bool supported { get { return Profiler.supported; } }

		public static string logFile { get { return Profiler.logFile; } set { Profiler.logFile = value; } }

		//
		// 摘要:
		//     Displays the recorded profile data in the profiler.
		//
		// 参数:
		//   file:
		//     The name of the file containing the frame data, including extension.
		[Conditional("UNITY_EDITOR")]
		public static void AddFramesFromFile(string file)
		{
			Profiler.AddFramesFromFile(file);
		}
		//
		// 摘要:
		//     Begin profiling a piece of code with a custom label.
		//
		// 参数:
		//   name:
		//     A string to identify the sample in the Profiler window.
		//
		//   targetObject:
		//     An object that provides context to the sample,.
		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string name, Object targetObject)
		{
			Profiler.BeginSample(name, targetObject);
		}
		//
		// 摘要:
		//     Begin profiling a piece of code with a custom label.
		//
		// 参数:
		//   name:
		//     A string to identify the sample in the Profiler window.
		//
		//   targetObject:
		//     An object that provides context to the sample,.
		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string name)
		{
			Profiler.BeginSample(name);
		}

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string arg0, string arg1)
		{
			Profiler.BeginSample(string.Concat(arg0, arg1));
		}

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string arg0, string arg1, string arg2)
		{
			Profiler.BeginSample(string.Concat(arg0, arg1, arg2));
		}

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string arg0, string arg1, string arg2, string arg3)
		{
			Profiler.BeginSample(string.Concat(arg0, arg1, arg2, arg3));
		}

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string arg0, string arg1, string arg2, string arg3, string arg4)
		{
			Profiler.BeginSample(string.Concat(arg0, arg1, arg2, arg3, arg4));
		}


		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5)
		{
			Profiler.BeginSample(string.Concat(arg0, arg1, arg2, arg3, arg4, arg5));
		}


		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(params string[] args)
		{
			Profiler.BeginSample(string.Concat(args));
		}

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string format, params object[] args)
		{
			Profiler.BeginSample(string.Format(format, args));
		}
		//
		// 摘要:
		//     Enables profiling on the thread from which you call this method.
		//
		// 参数:
		//   threadGroupName:
		//     The name of the thread group to which the thread belongs.
		//
		//   threadName:
		//     The name of the thread.
		[Conditional("ENABLE_PROFILER")]
		public static void BeginThreadProfiling(string threadGroupName, string threadName)
		{
			Profiler.BeginThreadProfiling(threadGroupName, threadName);
		}

		[Conditional("ENABLE_PROFILER")]
		public static void EmitFrameMetaData<T>(Guid id, int tag, NativeArray<T> data) where T : struct
		{
#if UNITY_2019
			Profiler.EmitFrameMetaData<T>(id, tag, data);
#endif
		}
		[Conditional("ENABLE_PROFILER")]
		public static void EmitFrameMetaData<T>(Guid id, int tag, List<T> data) where T : struct
		{
#if UNITY_2019
			Profiler.EmitFrameMetaData<T>(id, tag, data);
#endif
		}
		//
		// 摘要:
		//     Write metadata associated with the current frame to the Profiler stream.
		//
		// 参数:
		//   id:
		//     Module identifier. Used to distinguish metadata streams between different plugins,
		//     packages or modules.
		//
		//   tag:
		//     Data stream index.
		//
		//   data:
		//     Binary data.
		[Conditional("ENABLE_PROFILER")]
		public static void EmitFrameMetaData(Guid id, int tag, Array data)
		{
#if UNITY_2019
			Profiler.EmitFrameMetaData(id, tag, data);
#endif
		}
		//
		// 摘要:
		//     Ends the current profiling sample.
		[Conditional("ENABLE_PROFILER")]
		public static void EndSample()
		{
			Profiler.EndSample();
		}
		//
		// 摘要:
		//     Frees the internal resources used by the Profiler for the thread.
		public static void EndThreadProfiling()
		{
			Profiler.EndThreadProfiling();
		}
		//
		// 摘要:
		//     Returns the amount of allocated memory for the graphics driver, in bytes. Only
		//     available in development players and editor.
		public static long GetAllocatedMemoryForGraphicsDriver()
		{
			return Profiler.GetAllocatedMemoryForGraphicsDriver();
		}
		//
		// 摘要:
		//     Returns whether or not a given ProfilerArea is currently enabled.
		//
		// 参数:
		//   area:
		//     Which area you want to check the state of.
		//
		// 返回结果:
		//     Returns whether or not a given ProfilerArea is currently enabled.
		public static bool GetAreaEnabled(ProfilerArea area)
		{
			return Profiler.GetAreaEnabled(area);
		}
		//
		// 摘要:
		//     Returns the size of the reserved space for managed-memory.
		//
		// 返回结果:
		//     The size of the managed heap. This returns 0 if the Profiler is not available.
		public static long GetMonoHeapSizeLong()
		{
			return Profiler.GetMonoHeapSizeLong();
		}

		// 摘要:
		//     The allocated managed-memory for live objects and non-collected objects.
		//
		// 返回结果:
		//     A long integer value of the memory in use. This returns 0 if the Profiler is
		//     not available.
		public static long GetMonoUsedSizeLong()
		{
			return Profiler.GetMonoUsedSizeLong();
		}
		//
		// 摘要:
		//     Gathers the native-memory used by a Unity object.
		//
		// 参数:
		//   o:
		//     The target Unity object.
		//
		// 返回结果:
		//     The amount of native-memory used by a Unity object. This returns 0 if the Profiler
		//     is not available.
		public static long GetRuntimeMemorySizeLong(Object o)
		{
			return Profiler.GetRuntimeMemorySizeLong(o);
		}
		//
		// 摘要:
		//     Returns the size of the temp allocator.
		//
		// 返回结果:
		//     Size in bytes.
		public static uint GetTempAllocatorSize()
		{
			return Profiler.GetTempAllocatorSize();
		}
		//
		// 摘要:
		//     The total memory allocated by the internal allocators in Unity. Unity reserves
		//     large pools of memory from the system. This function returns the amount of used
		//     memory in those pools.
		//
		// 返回结果:
		//     The amount of memory allocated by Unity. This returns 0 if the Profiler is not
		//     available.
		public static long GetTotalAllocatedMemoryLong()
		{
			return Profiler.GetTotalAllocatedMemoryLong();
		}
		// 摘要:
		//     The total memory Unity has reserved.
		//
		// 返回结果:
		//     Memory reserved by Unity in bytes. This returns 0 if the Profiler is not available.
		public static long GetTotalReservedMemoryLong()
		{
			return Profiler.GetTotalReservedMemoryLong();
		}
		//
		// 摘要:
		//     Unity allocates memory in pools for usage when unity needs to allocate memory.
		//     This function returns the amount of unused memory in these pools.
		//
		// 返回结果:
		//     The amount of unused memory in the reserved pools. This returns 0 if the Profiler
		//     is not available.
		public static long GetTotalUnusedReservedMemoryLong()
		{
			return Profiler.GetTotalUnusedReservedMemoryLong();
		}
		//
		// 摘要:
		//     Enable or disable a given ProfilerArea.
		//
		// 参数:
		//   area:
		//     The area you want to enable or disable.
		//
		//   enabled:
		//     Enable or disable the collection of data for this area.
		[Conditional("ENABLE_PROFILER")]
		public static void SetAreaEnabled(ProfilerArea area, bool enabled)
		{
			Profiler.SetAreaEnabled(area, enabled);
		}
		//
		// 摘要:
		//     Sets the size of the temp allocator.
		//
		// 参数:
		//   size:
		//     Size in bytes.
		//
		// 返回结果:
		//     Returns true if requested size was successfully set. Will return false if value
		//     is disallowed (too small).
		public static bool SetTempAllocatorRequestedSize(uint size)
		{
			return Profiler.SetTempAllocatorRequestedSize(size);
		}
	}
}
