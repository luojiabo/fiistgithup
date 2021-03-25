using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	/// <summary>
	/// The actor is not the asset
	/// It always exists in-memory.
	/// </summary>
	[DisallowMultipleComponent]
	[NameToType]
	public partial class Actor : UObjectCache
	{
		private bool mInitializing = false;
		private bool mInitialized = false;

		protected virtual bool initializeOnAwake => true;

		public bool pendingKill { get; private set; } = false;

		protected override void Awake()
		{
			if (mInitializing)
				return;

			base.Awake();
			if (initializeOnAwake)
			{
				Initialize();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public Actor AsImmortal()
		{
			return (Actor)AsImmortal("LokiEngineModule.Actor.Immortal");
		}

		public T AsImmortal<T>() where T : Actor
		{
			return (T)AsImmortal();
		}

		public void Initialize()
		{
			if (mInitializing || mInitialized)
				return;
			mInitializing = true;
			OnInitialize();
			mInitializing = false;
			mInitialized = true;
		}

		protected virtual void OnInitialize()
		{

		}
	}
}
