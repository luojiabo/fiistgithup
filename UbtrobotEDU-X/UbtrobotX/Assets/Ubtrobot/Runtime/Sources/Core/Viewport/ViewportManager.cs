﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class ViewportManager : USingletonObject<Selection>, ISystem
	{
		private static readonly Type msType = typeof(ViewportManager);

		private readonly List<IViewport> mViewports = new List<IViewport>();

		public string systemName => msType.Name;

		public IModuleInterface module { get; set; }


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
		}

		public void OnLateUpdate()
		{
		}

		public void OnUpdate(float deltaTime)
		{
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
