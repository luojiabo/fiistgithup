using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class EnvironmentSystem : USingletonObject<EnvironmentSystem>, ISystem
	{
		private static readonly Type msType = typeof(EnvironmentSystem);

		protected readonly UpdateHandler mUpdator = new UpdateHandler();
		private readonly List<Environment> mEnvs = new List<Environment>();
		private Environment mDefaultEnvironment = null;

		public string systemName { get { return msType.Name; } }

		public IModuleInterface module { get; set; }

		public Environment defaultEnvironment
		{
			get
			{
				// 如果没有设置默认环境，那么第一个添加的环境就是默认环境
				if (mDefaultEnvironment == null)
				{
					if (mEnvs.Count > 0)
					{
						return mEnvs[0];
					}
				}
				return mDefaultEnvironment;
			}
		}

		/// <summary>
		/// 根据位置获取环境
		/// </summary>
		/// <param name="position">需要感知环境的位置</param>
		/// <param name="forward">需要感知环境的目标朝向</param>
		/// <param name="env">环境</param>
		/// <returns>有没有找到环境</returns>
		public bool GetEnvironment(Vector3 position, Vector3 forward, out Environment env)
		{
			env = mEnvs.Find(e => e.IsMatch(position, forward));
			// 如果找不到更合适的环境对象，则返回默认环境对象
			if (env == null)
			{
				env = defaultEnvironment;
			}
			return env != null;
		}

		public void Register(Environment env)
		{
			mEnvs.Add(env);
			mUpdator.Add(env);
			if (env.isDefault)
			{
				mDefaultEnvironment = env;
			}
		}

		public void Unregister(Environment env)
		{
			mEnvs.Remove(env);
			mUpdator.Remove(env);
			if (env == defaultEnvironment)
			{
				mDefaultEnvironment = mEnvs.Find(e => e.isDefault);
			}
		}

		public IEnumerator Initialize()
		{
			return null;
		}

		public IEnumerator PostInitialize()
		{
			return null;
		}

		public void OnFixedUpdate(float fixedDeltaTime)
		{
			mUpdator.OnFixedUpdate(fixedDeltaTime);
		}

		public void OnLateUpdate()
		{
			mUpdator.OnLateUpdate();
		}

		public void OnUpdate(float deltaTime)
		{
			mUpdator.OnUpdate(deltaTime);
		}

		public void Shutdown()
		{
		}

		public void Startup()
		{
		}

		public void Uninitialize()
		{
		}
	}
}
