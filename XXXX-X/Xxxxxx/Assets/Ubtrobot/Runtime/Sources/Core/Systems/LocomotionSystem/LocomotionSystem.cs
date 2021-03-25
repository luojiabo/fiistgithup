//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Loki;
//using UnityEngine;
//using UnityEngine.AI;

//namespace Ubtrobot
//{
//	public class LocomotionSystem : ISystem
//	{
//		class LocomotionInfo
//		{
//			public float lastNOSUsageTime;
//			public Robot lastRobot = null;

//			public bool isValid
//			{
//				get
//				{
//					return lastRobot != null;
//				}
//			}

//			public Rigidbody rigidbody
//			{
//				get
//				{
//					return lastRobot.GetComponentCache<Rigidbody>();
//				}
//			}

//			public float nosValue
//			{
//				get
//				{
//					if (lastNOSUsageTime <= 0.0f || (Time.timeSinceLevelLoad - lastNOSUsageTime) >= lastRobot.nosUsageInterval)
//					{
//						return lastRobot.nosValue;
//					}
//					return 0.0f;
//				}
//			}

//			public void UseNOS()
//			{
//				lastNOSUsageTime = Time.timeSinceLevelLoad;
//			}

//			public void Reset(Robot robot)
//			{
//				lastNOSUsageTime = 0.0f;
//				lastRobot = robot;
//			}
//		}

//		private static readonly Type msType = typeof(LocomotionSystem);

//		private readonly LocomotionInfo mLocomotionInfo = new LocomotionInfo();

//		public string systemName { get { return msType.Name; } }

//		public IModuleInterface module { get; set; }

//		private InputSystem GetInputSystem()
//		{
//			var inputSys = ModuleManager.Get().GetSystemChecked<InputSystem>();
//			return inputSys;
//		}

//		private LocomotionInfo UpdateSelectedRobot()
//		{
//			var current = Selection.GetOrAlloc().actor as Robot;
//			if (current != mLocomotionInfo.lastRobot)
//			{
//				mLocomotionInfo.Reset(current);
//			}
//			return mLocomotionInfo;
//		}

//		public IEnumerator Initialize()
//		{
//			return null;
//		}

//		public IEnumerator PostInitialize()
//		{
//			return null;
//		}

//		public void OnFixedUpdate(float fixedDeltaTime)
//		{
//		}

//		public void OnLateUpdate()
//		{
//		}

//		public void OnUpdate(float deltaTime)
//		{
//			var inputSystem = GetInputSystem();
//			var info = UpdateSelectedRobot();
//			if (inputSystem != null && info.isValid)
//			{
//				float forward = inputSystem.GetAsix(DefaultInputSystemConfig.LeftVertical);
//				float right = inputSystem.GetAsix(DefaultInputSystemConfig.LeftHorizontal);
//				//float lbValue = inputSystem.GetButtonAsValue(DefaultInputSystemConfig.LB, -1.0f);
//				//float rbValue = inputSystem.GetButtonAsValue(DefaultInputSystemConfig.RB, 1.0f);

//				if (Misc.Nearly(forward, 0.0f))
//				{
//					return;
//				}

//				float nosValue = 0.0f;
//				if (inputSystem.GetKey(KeyCode.LeftShift))
//				{
//					nosValue = info.nosValue;
//					info.UseNOS();
//				}

//				if (forward < 0.0f)
//				{
//					right *= -1.0f;
//				}

//				var lastRobot = info.lastRobot;
//				lastRobot.Move(forward * deltaTime, right * deltaTime, nosValue);
//			}
//		}

//		public void Shutdown()
//		{
//		}

//		public void Startup()
//		{
//		}

//		public void Uninitialize()
//		{
//		}
//	}
//}
