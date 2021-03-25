using System;
using Loki;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Text;

namespace Ubtrobot
{
	[RequireComponent(typeof(ScratchWebSocketClientSimulator))]
	public class RadarWallChecker : UComponent
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

		public ProtocolValue[] checkDistanceToWall;
		public ProtocolValue[] runServos;
		public ProtocolValue[] stopServos;
		public float stopDistance = 300.0f;

		private bool mToCheckDistance = false;

		private ProtocolValue[] mCurrent = null;

		[InspectorMethod]
		[ConsoleMethod(aliasName = "radar.checkWall")]
		private void CheckWall()
		{
			// 方案1. 如果墙距离传感器发射点超过5cm，角模式旋转1度, 否则等待100ms;一直做这个步骤
			// 方案2. 如果墙距离传感器发射点不超过5cm，停止轮模式，否则，设置成轮模式;一直做这个步骤
			// 挂钩子
			RobotManager.Get().protocolHook = this;
			simulator.StopDebug();
			mToCheckDistance = true;
			mCurrent = null;
			simulator.StartCoroutine(CheckWallE());
		}

		private void OnProtocolHook(IProtocol protocol)
		{
			var exp = (ExploreProtocol)protocol;
			var json = exp.ToJsonData();
			DebugUtility.Log(LoggerTags.Online, "OnProtocolHook : {0}", json.ToJson());

			if (exp.uuid == "asonic")
			{
				if (exp.GetParamf(0, out var distance) && distance <= stopDistance)
				{
					mCurrent = stopServos;
					mToCheckDistance = true;
				}
				else
				{
					mCurrent = runServos;
					mToCheckDistance = true;
				}
			}
		}

		private IEnumerator CheckWallE()
		{
			yield return null;
			do
			{
				if (mToCheckDistance && mCurrent == null)
				{
					mCurrent = checkDistanceToWall;
				}
				simulator.recvContents = mCurrent;
				mCurrent = null;
				yield return simulator.StartCoroutine(simulator.SimulateRecvAllMessagesCo());
				while (mCurrent == null && !mToCheckDistance)
				{
					if (waitForSecondsLoop > 0.0f)
						yield return new WaitForSeconds(waitForSecondsLoop);
					else
						yield return null;
				}
			} while (true);
		}
	}
}
