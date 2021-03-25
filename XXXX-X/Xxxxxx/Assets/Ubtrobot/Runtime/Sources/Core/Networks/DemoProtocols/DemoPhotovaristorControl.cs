using System;
using Loki;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Text;

namespace Ubtrobot
{
	[RequireComponent(typeof(ScratchWebSocketClientSimulator))]
	public partial class DemoPhotovaristorControl : UComponent
	{
		private ScratchWebSocketClientSimulator mSimulator;
		public ScratchWebSocketClientSimulator simulator
		{
			get
			{
				if (mSimulator == null)
				{
					mSimulator = GetComponent<ScratchWebSocketClientSimulator>();
				}
				return mSimulator;
			}
		}


		public float waitForSecondsLoop = 0.3f;
		/// <summary>
		/// 是否正在等待协议
		/// </summary>
		private bool mWaitForProtocol = false;

		private readonly ProtocolValue[] mServoInstructs = new ProtocolValue[2];

		[Header("===温湿度功能===")]
		/// <summary>
		/// 获取传感器信息
		/// </summary>
		[Tooltip("获取传感器信息")]
		public ProtocolValue[] checkSensors;

		[Tooltip("Servo1三档控制, 0 停止， 1 慢转， 2快转")]
		public ProtocolValue[] servo1_motions;

		[Tooltip("Servo2三档控制, 0 停止， 1 慢转， 2快转")]
		public ProtocolValue[] servo2_motions;

		// 亮度
		[Tooltip("亮度阈值")]
		public float luminanceTargetValue = 800.0f;
		// 温度
		[Tooltip("温度阈值")]
		public float temperatureTargetValue = 30.0f;
		// 湿度
		[Tooltip("相对湿度阈值")]
		[Range(0.0f, 100.0f)]
		public float humidityTargetValue = 60.0f;

		/// <summary>
		/// 亮度
		/// </summary>
		private float luminanceValue = 0.0f;
		/// <summary>
		/// 温度
		/// </summary>
		private float temperatureValue = 0.0f;
		/// <summary>
		/// 湿度
		/// </summary>
		private float humidityValue = 0.0f;

		[InspectorMethod(aliasName = "停止功能")]
		[ConsoleMethod(aliasName = "ph.StopDemo")]
		private void StopDemo()
		{
			if (RobotManager.Get().protocolHook == this)
				RobotManager.Get().protocolHook = null;
			simulator.StopAllCoroutines();
			simulator.StopDebug();
		}

		/// <summary>
		/// 这个Demo 接受温湿度以及亮度传感器控制舵机
		/// </summary>
		[InspectorMethod(aliasName = "亮度温湿度传感器")]
		[ConsoleMethod(aliasName = "ph.StartDemo0")]
		private void StartDemo0()
		{
			StopDemo();
			// 挂钩子
			RobotManager.Get().protocolHook = this;
			simulator.StartCoroutine(Demo0Loop());
		}

		/// <summary>
		/// 决定舵机运动状态
		/// </summary>
		private void DeterminateServoState()
		{
			if (luminanceValue >= luminanceTargetValue)
			{
				if (temperatureValue >= temperatureTargetValue)
				{
					if (humidityValue >= humidityTargetValue)
					{
						// 舵机ID-1	舵机ID-2
						// 快转		快转
						mServoInstructs[0] = servo1_motions[2];
						mServoInstructs[1] = servo2_motions[2];
					}
					else
					{
						// 舵机ID-1	舵机ID-2
						// 快转		慢转
						mServoInstructs[0] = servo1_motions[2];
						mServoInstructs[1] = servo2_motions[1];
					}
				}
				else
				{
					if (humidityValue >= humidityTargetValue)
					{
						// 舵机ID-1	舵机ID-2
						// 慢转		快转
						mServoInstructs[0] = servo1_motions[1];
						mServoInstructs[1] = servo2_motions[2];
					}
					else
					{
						// 舵机ID-1	舵机ID-2
						// 快转		停转
						mServoInstructs[0] = servo1_motions[2];
						mServoInstructs[1] = servo2_motions[0];
					}
				}
			}
			else
			{
				if (temperatureValue >= temperatureTargetValue)
				{
					if (humidityValue >= humidityTargetValue)
					{
						// 舵机ID-1	舵机ID-2
						// 慢转		慢转
						mServoInstructs[0] = servo1_motions[1];
						mServoInstructs[1] = servo2_motions[1];
					}
					else
					{
						// 舵机ID-1	舵机ID-2
						// 慢转		停转
						mServoInstructs[0] = servo1_motions[1];
						mServoInstructs[1] = servo2_motions[0];
					}
				}
				else
				{
					if (humidityValue >= humidityTargetValue)
					{
						// 舵机ID-1	舵机ID-2
						// 停转		慢转
						mServoInstructs[0] = servo1_motions[0];
						mServoInstructs[1] = servo2_motions[1];
					}
					else
					{
						// 舵机ID-1	舵机ID-2
						// 停转		停转
						mServoInstructs[0] = servo1_motions[0];
						mServoInstructs[1] = servo2_motions[0];
					}
				}
			}
		}

		/// <summary>
		/// Demo0 循环, 这个Demo 接受温湿度以及亮度传感器控制舵机
		/// 指令修改后可以测试
		/// </summary>
		/// <returns></returns>
		private IEnumerator Demo0Loop()
		{
			while (true)
			{
				yield return null;

				simulator.recvContents = checkSensors;
				yield return simulator.StartCoroutine(simulator.SimulateRecvAllMessagesCo((idx) => !mWaitForProtocol, (idx) => { mWaitForProtocol = true; }));

				DeterminateServoState();

				simulator.recvContents = mServoInstructs;
				yield return simulator.StartCoroutine(simulator.SimulateRecvAllMessagesCo());
			}
		}

		/// <summary>
		/// 光传感器
		/// </summary>
		/// <param name="protocol"></param>
		private void OnRecvLuminanceSensor(ExploreProtocol protocol)
		{
		}

		/// <summary>
		/// 板载按钮
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookButton(ExploreProtocol protocol)
		{

		}

		/// <summary>
		/// 超声波传感器
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookAsonic(ExploreProtocol protocol)
		{

		}

		/// <summary>
		/// 红外传感器
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookFrared(ExploreProtocol protocol)
		{

		}

		/// <summary>
		/// 亮度传感器
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookLuminance(ExploreProtocol protocol)
		{
			protocol.GetParamf(0, out luminanceValue);
			mWaitForProtocol = false;
		}

		/// <summary>
		/// 温湿度传感器，湿度回传
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookMidity(ExploreProtocol protocol)
		{
			protocol.GetParamf(0, out humidityValue);
			mWaitForProtocol = false;
		}

		/// <summary>
		/// 温湿度传感器，温度回传
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookRature(ExploreProtocol protocol)
		{
			protocol.GetParamf(0, out temperatureValue);
			mWaitForProtocol = false;
		}

		/// <summary>
		/// RGB 传感器 - RGB回传
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookRGBSensor(ExploreProtocol protocol)
		{

		}

		/// <summary>
		/// RGB 传感器- 判定指定色
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookRGBRecogSensor(ExploreProtocol protocol)
		{

		}

		/// <summary>
		/// 巡线传感器 - 判断深浅色
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookPatrolSensor(ExploreProtocol protocol)
		{

		}

		/// <summary>
		/// 传感器数据回读
		/// </summary>
		/// <param name="protocol">协议</param>
		private void OnProtocolHook(IProtocol protocol)
		{
			var exp = (ExploreProtocol)protocol;
			var json = exp.ToJsonData();
			DebugUtility.Log(LoggerTags.Online, "OnProtocolHook : {0}", json.ToJson());

			// 板载按钮
			if (exp.uuid == "button")
			{
				OnHookButton(exp);
			}
			// 按压传感器
			else if (exp.uuid == "sensor")
			{
				OnHookButtonSensor(exp);
			}
			// 红外传感器
			else if (exp.uuid == "frared")
			{
				OnHookFrared(exp);
			}
			// 超声波传感器
			else if (exp.uuid == "asonic")
			{
				OnHookAsonic(exp);
			}
			// 亮度传感器
			else if (exp.uuid == "htness")
			{
				OnHookLuminance(exp);
			}
			// 温湿度传感器，湿度回传
			else if (exp.uuid == "midity")
			{
				OnHookMidity(exp);
			}
			// 温湿度传感器，温度回传
			else if (exp.uuid == "rature")
			{
				OnHookRature(exp);
			}
			// 颜色传感器,RGB 回传
			else if (exp.uuid == "eadrgb")
			{
				OnHookRGBSensor(exp);
			}
			// 颜色传感器, 判断是否指定颜色
			else if (exp.uuid == "dcolor")
			{
				OnHookRGBRecogSensor(exp);
			}
			// 巡线传感器， 判断目标深浅色
			else if (exp.uuid == "ndline")
			{
				OnHookPatrolSensor(exp);
			}
		}


	}


	public partial class DemoPhotovaristorControl
	{
		[Header("===按压功能===")]
		[Tooltip("Motor1三档控制, 0 停止， 1 慢转， 2快转")]
		public ProtocolValue[] motor1_motions;

		[Tooltip("获取按压传感器状态")]
		public ProtocolValue[] checkButtonSensors;

		private EButtonSensorState mSensorState = EButtonSensorState.None;
		private bool mWaitForButtonSensorState = false;

		/// <summary>
		/// 这个Demo 接受按压传感器控制电机
		/// </summary>
		[InspectorMethod(aliasName = "按压传感器")]
		[ConsoleMethod(aliasName = "ph.StartDemo1")]
		private void StartDemo1()
		{
			StopDemo();
			// 挂钩子
			RobotManager.Get().protocolHook = this;
			simulator.StartCoroutine(Demo1Loop());
		}

		/// <summary>
		/// Demo1 循环, 这个Demo 接受按压传感器处理电机
		/// </summary>
		/// <returns></returns>
		private IEnumerator Demo1Loop()
		{
			while (true)
			{
				yield return null;

				mWaitForButtonSensorState = true;
				simulator.recvContents = checkButtonSensors;
				yield return simulator.StartCoroutine(simulator.SimulateRecvAllMessagesCo());

				while (mWaitForButtonSensorState)
				{
					yield return null;
				}

				if (mSensorState != EButtonSensorState.None)
				{
					simulator.recvContents = new[] { motor1_motions[(int)mSensorState - 1] };
					yield return simulator.StartCoroutine(simulator.SimulateRecvAllMessagesCo());
				}
			}
		}

		/// <summary>
		/// 按压传感器
		/// </summary>
		/// <param name="protocol"></param>
		private void OnHookButtonSensor(ExploreProtocol protocol)
		{
			if (protocol.GetParam(0, out int state))
			{
			}
			mSensorState = ButtonSensorStateHelper.FromState(state);
			mWaitForButtonSensorState = false;
		}
	}
}