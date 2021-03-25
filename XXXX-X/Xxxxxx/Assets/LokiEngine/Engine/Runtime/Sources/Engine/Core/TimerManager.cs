using System;
using System.Collections.Generic;

namespace Loki
{
	public delegate void TimerCallback(object target);

	public struct TimerHandler : IEqualityComparer<TimerHandler>
	{
		/// <summary>
		/// to optimize memory
		/// </summary>
		private UniqueID mHandle;

		public bool IsValid
		{
			get { return mHandle != UniqueID.Empty && mHandle != default(UniqueID); }
		}

		public static bool operator ==(TimerHandler x, TimerHandler y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(TimerHandler x, TimerHandler y)
		{
			return !x.Equals(y);
		}

		public static TimerHandler CreateEmpty()
		{
			return new TimerHandler(UniqueID.Empty);
		}

		public TimerHandler(UniqueID id)
		{
			mHandle = id;
		}

		public void SetExpired()
		{
			mHandle = UniqueID.Empty;
		}

		public bool Equals(TimerHandler x, TimerHandler y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(TimerHandler obj)
		{
			return obj.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is TimerHandler)
			{
				return Equals(((TimerHandler)obj));
			}
			return false;
		}

		public bool Equals(TimerHandler handle)
		{
			return this.mHandle == handle.mHandle;
		}

		public override int GetHashCode()
		{
			return mHandle.GetHashCode();
		}

		public override string ToString()
		{
			return string.Concat("{", mHandle.ToString(), "}");
		}

	}

	public interface ITimerCallbackTarget
	{
		bool isValid { get; }
	}

	public class TimerManager : IUpdatable
	{
		struct Timer : IEquatable<Timer>
		{
			public TimerHandler handler;
			public TimerCallback callback;
			public float rate;
			public int loopCount;
			public float duration;
			public bool pause;

			public bool requiringTarget;
			public object target;
			public Func<object, bool> validateFunction;

			public bool isValid { get { return loopCount != 0 && handler.IsValid && (!requiringTarget || ((validateFunction != null) && validateFunction(target))); } }

			public void Callback()
			{
				try
				{
					if (callback != null)
					{
						callback(target);
					}
				}
				catch(Exception ex)
				{
					DebugUtility.LogException(ex);
				}
			}

			public bool Equals(Timer other)
			{
				return handler.Equals(other.handler);
			}
		}

		/// <summary>
		/// The internal target validate for improve performance
		/// </summary>
		internal class TimerTargetValidateFactory
		{
			public static bool ValidateUObject(object o)
			{
				UnityEngine.Object oPtr = o as UnityEngine.Object;
				return oPtr != null;
			}

			public static bool ValidateTimerCallbackTarget(object o)
			{
				ITimerCallbackTarget oPtr = o as ITimerCallbackTarget;
				return (oPtr != null) && (oPtr.isValid);
			}
		}

		private static readonly Type msType = typeof(TimerManager);
		private readonly List<Timer> mRunningTimers = new List<Timer>();
		private bool mIsTransiting = false;

		public string systemName { get { return msType.Name; } }

		private void ClearInvalidTimers()
		{
			mRunningTimers.RemoveAll(timer => !timer.isValid);
		}

		private Func<object, bool> GetValidateFunction(object target)
		{
			Func<object, bool> validateFunction = null;
			if ((target is UnityEngine.Object))
			{
				validateFunction = TimerTargetValidateFactory.ValidateUObject;
			}
			else if (target is ITimerCallbackTarget)
			{
				validateFunction = TimerTargetValidateFactory.ValidateTimerCallbackTarget;
			}
			return validateFunction;
		}

		public void SetTimer(ref TimerHandler handler, TimerCallback callback, float rate, object target, Func<object, bool> validateFunction = null, int loopCount = 1, float inFirstDelay = -1.0f, bool pause = false)
		{
			StopTimer(ref handler);

			DebugUtility.AssertFormat(callback != null, "Please ensure that the callback is not null.");
			DebugUtility.AssertFormat(loopCount != 0, "Please ensure that the loopCount is not Zero.");

			if ((target != null) && (validateFunction == null))
			{
				// Set validate function at SetTimer is better that DoValidate to check some types.
				validateFunction = GetValidateFunction(target);
			}

			DebugUtility.AssertFormat((target == null && validateFunction == null) || (target != null && validateFunction != null), "Please ");

			float duration;
			if (inFirstDelay > 0.0f)
			{
				duration = inFirstDelay;
			}
			else
			{
				duration = rate;
			}
			handler = new TimerHandler(UniqueID.New());
			Timer timer;
			timer.rate = rate;
			timer.loopCount = loopCount;
			timer.callback = callback;
			timer.handler = handler;
			timer.duration = duration;
			timer.pause = pause;

			bool requiringTarget = timer.requiringTarget = (target != null) && ((validateFunction != null) && validateFunction(target));
			timer.target = (requiringTarget ? target : null); // for avoid the invalid reference (The Unity design reason)
			timer.validateFunction = validateFunction;

			mRunningTimers.Add(timer);
		}

		public void SetTimer(ref TimerHandler handler, TimerCallback callback, float rate, int loopCount = 1, float inFirstDelay = -1.0f, bool pause = false)
		{
			object target = null;
			Func<object, bool> validateFunction = null;
			SetTimer(ref handler, callback, rate, target, validateFunction, loopCount, inFirstDelay, pause);
		}

		public void StopTimer(ref TimerHandler handler)
		{
			if (!handler.IsValid)
				return;

			ProfilingUtility.BeginSample("TimerManager.StopTimer");
			int timerCount = mRunningTimers.Count;
			for (int idx = 0; idx < timerCount; ++idx)
			{
				Timer timer = mRunningTimers[idx];
				if (timer.handler == handler)
				{
					handler.SetExpired();
					timer.pause = true;
					timer.loopCount = 0;
					mRunningTimers[idx] = timer;
					break;
				}
			}
			ProfilingUtility.EndSample();
		}

		public void PauseTimer(TimerHandler handler)
		{
			if (!handler.IsValid)
				return;
			ProfilingUtility.BeginSample("TimerManager.PauseTimer");
			int timerCount = mRunningTimers.Count;
			for (int idx = 0; idx < timerCount; ++idx)
			{
				Timer timer = mRunningTimers[idx];
				if (timer.handler == handler)
				{
					timer.pause = true;
					mRunningTimers[idx] = timer;
					break;
				}
			}
			ProfilingUtility.EndSample();
		}

		public void ResumeTimer(TimerHandler handler)
		{
			if (!handler.IsValid)
				return;

			ProfilingUtility.BeginSample("TimerManager.ResumeTimer");
			int timerCount = mRunningTimers.Count;
			for (int idx = 0; idx < timerCount; ++idx)
			{
				Timer timer = mRunningTimers[idx];
				if (timer.handler == handler)
				{
					timer.pause = false;
					mRunningTimers[idx] = timer;
					break;
				}
			}
			ProfilingUtility.EndSample();
		}

		public void OnUpdate(float deltaTime)
		{
			if (mIsTransiting)
				return;

			ProfilingUtility.BeginSample("TimerManager.OnUpdate");

			ProfilingUtility.BeginSample("TimerManager.DoUpdate");
			DoUpdate(deltaTime);
			ProfilingUtility.EndSample();

			ProfilingUtility.BeginSample("TimerManager.ClearInvalidTimers");
			ClearInvalidTimers();
			ProfilingUtility.EndSample();

			ProfilingUtility.EndSample();
		}

		private void DoUpdate(float deltaTime)
		{
			mIsTransiting = true;

			int timerCount = mRunningTimers.Count;
			for (int idx = 0; idx < timerCount; ++idx)
			{
				Timer timer = mRunningTimers[idx];
				if (timer.isValid && !timer.pause)
				{
					bool exec = false;
					float duration = timer.duration - deltaTime;
					if (duration <= 0.0f)
					{
						exec = true;
						duration = timer.rate;
						if (timer.loopCount > 0)
						{
							timer.loopCount -= 1;
						}
					}
					timer.duration = duration;
					mRunningTimers[idx] = timer;

					if (exec)
					{
						ProfilingUtility.BeginSample("TimerManager.OnUpdate.ExecTimerCallback");
						try
						{
							timer.Callback();
						}
						catch (Exception ex)
						{
							DebugUtility.LogException(ex);
						}
						ProfilingUtility.EndSample();
					}
				}
			}

			mIsTransiting = false;
		}
	}
}
