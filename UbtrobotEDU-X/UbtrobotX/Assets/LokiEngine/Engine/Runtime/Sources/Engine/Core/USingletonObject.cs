namespace Loki
{
	public enum ELifetime
	{
		UserDecide,
		App,
	}

	public abstract class USingletonObject : UObject, ISingleton
	{
		public virtual string immortalName { get { return "DefaultImmortal"; } }
		public virtual ELifetime lifetime { get { return ELifetime.UserDecide; } }
		public abstract void Release();
	}

	public abstract class USingletonObject<TMostDerived> : USingletonObject where TMostDerived : USingletonObject<TMostDerived>
	{
		private static TMostDerived msInstance;

		public static bool Exist()
		{
			return msInstance != null;
		}

		public static TMostDerived Get()
		{
			return msInstance;
		}

		public static TMostDerived GetOrAlloc()
		{
			if (msInstance == null)
			{
				msInstance = FindObjectOfType<TMostDerived>();
				if (msInstance != null)
				{
					//var type = typeof(TMostDerived);
					//if (type != null)
					//{
					//	var awake = type.GetMethod("Awake");
					//	if (awake != null && !awake.IsStatic)
					//	{
					//		awake.Invoke(msInstance, null);
					//	}
					//}
				}
				else
				{
#if UNITY_EDITOR
					if (!UnityEngine.Application.isPlaying)
						return null;
#endif
					msInstance = NewObject<TMostDerived>(typeof(TMostDerived).Name);
				}
			}
			return msInstance;
		}

		public static void SafeRelease()
		{
			if (msInstance != null)
			{
				msInstance.Release();
			}
		}

		protected override void Awake()
		{
			DebugUtility.AssertFormat(this.GetType() == typeof(TMostDerived), "The instance type [{0}] must be the Most Derived Type : [{1}].", GetType().Name, typeof(TMostDerived).Name);

			if (OnAwake())
			{
				msInstance = (TMostDerived)this;
				OnInitialize();
			}
			else
			{
				DebugUtility.LogErrorTrace(LoggerTags.Engine, "This type [{0}] is derived from the singleton-type, but there is some objects with the same type at the same time.", typeof(TMostDerived).Name);
				enabled = false;
			}
			base.Awake();
		}

		protected virtual void OnInitialize()
		{
			switch(lifetime)
			{
				case ELifetime.App:
					{
						AsImmortal(immortalName);
						break;
					}
			}
		}

		/// <summary>
		/// Must call base.OnDestroy() after your overrided method
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (msInstance == this)
				msInstance = null;
		}

		protected virtual bool OnAwake(bool doReplace = false)
		{
			if (!doReplace)
			{
				if (msInstance == null || msInstance == this)
					return true;
			}
			else
			{
				DestroyInstance();
				return true;
			}
			return false;
		}

		public override void Release()
		{
			if (this != null)
			{
				Destroy(this.gameObject);
			}
		}

		private void DestroyInstance()
		{
			if (msInstance != null && msInstance != this)
			{
				var ins = msInstance;
				msInstance = null;
				Destroy(ins.gameObject);
			}
		}
	}
}
