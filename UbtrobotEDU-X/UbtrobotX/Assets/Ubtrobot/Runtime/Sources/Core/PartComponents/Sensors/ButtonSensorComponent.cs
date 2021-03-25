using System;
using System.Collections.Generic;
using System.Linq;
using Loki;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ubtrobot
{
	/// <summary>
	/// 传感器按钮状态
	/// </summary>
	public enum EButtonSensorState
	{
		/// <summary>
		/// 无变化
		/// </summary>
		None,
		/// <summary>
		/// 点击
		/// </summary>
		Click,
		/// <summary>
		/// 双击
		/// </summary>
		DoubleClick,
		/// <summary>
		/// 长按
		/// </summary>
		LongPress,
	}

	/// <summary>
	/// 按压传感器
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "按压传感器", tooltip = "点击、双击、长按")]
	public class ButtonSensorComponent : UKitComponent, IPartComponentInput
	{
		/// <summary>
		/// 正在等待返回的协议
		/// </summary>
		private TriggerCommandResponseAsync mWaitingInputAsync;

		/// <summary>
		/// 开始等待输入时间
		/// </summary>
		private float mWatingStartTime = 0.0f;

		/// <summary>
		/// 按键事件时间
		/// </summary>
		private float mStateChangedTime = 0.0f;

		/// <summary>
		/// 按键等待时间
		/// </summary>
		[SerializeField]
		private float mWaitingTimeout = 15.0f;

		/// <summary>
		/// 是否正在等待输入
		/// </summary>
		[PreviewMember]
		public bool waitForInput { get { return waitingInputAsync != null; } }

		/// <summary>
		/// 在接收到这个按钮的事件之后，多少秒后认为状态失效
		/// </summary>
		[SerializeField]
		private float mEventTimeout = 0.003f;

		[PreviewMember(rangeMin = 0.0f, rangeMax = 1.0f)]
		public float eventTimeout { get { return mEventTimeout; } set { mEventTimeout = value; } }

		[PreviewMember]
		public float timeout { get { return mWaitingTimeout; } set { mWaitingTimeout = value; } }

		public TriggerCommandResponseAsync waitingInputAsync
		{
			get
			{
				return mWaitingInputAsync;
			}
			set
			{
				if (mWaitingInputAsync != value)
				{
					if (mWaitingInputAsync != null)
					{
						mWaitingInputAsync.done = true;
					}
					mWaitingInputAsync = value;
					if (mWaitingInputAsync != null)
					{

						mWatingStartTime = Time.realtimeSinceStartup;
					}
					else
					{
						mWatingStartTime = 0.0f;
					}
				}
			}
		}

		public EButtonSensorState buttonState { get; set; } = EButtonSensorState.None;

		public override DriversType driversType => DriversType.UKitButtonSensor;
		//DriversType

		public bool hasEventTimeout
		{
			get
			{
				if (eventTimeout > 0.0f)
				{
					if (mStateChangedTime > 0.0f && Time.realtimeSinceStartup - mStateChangedTime > eventTimeout)
					{
						return true;
					}
				}

				return false;
			}
		}

		public bool hasWaitingTimeout
		{
			get
			{
				if (timeout > 0.0f && mWatingStartTime > 0.0f)
				{
					return Time.realtimeSinceStartup - mWatingStartTime >= timeout;
				}
				return false;
			}
		}

		public void ResetChangedTime()
		{
			buttonState = EButtonSensorState.None;
			mStateChangedTime = 0.0f;
		}

		public void OnRecvInputEvent(InputEventData e)
		{
			PartInputEventData ed = e as PartInputEventData;
			switch (ed.state)
			{
				case EInputState.Click:
					{
						buttonState = EButtonSensorState.Click;
						break;
					}
				case EInputState.DoubleClick:
					{
						buttonState = EButtonSensorState.DoubleClick;
						break;
					}
				case EInputState.LongPress:
					{
						buttonState = EButtonSensorState.LongPress;
						break;
					}
			}
			mStateChangedTime = Time.realtimeSinceStartup;
			e.Use();

			if (waitForInput)
			{
				if (Populate((ExploreProtocol)waitingInputAsync.protocol))
				{
					waitingInputAsync = null;
				}
			}
			ResetChangedTime();
		}

		private bool Populate(ExploreProtocol result)
		{
			if (buttonState == EButtonSensorState.None)
			{
				result.code = 1;
				return false;
			}

			result.code = 0;
			switch (buttonState)
			{
				case EButtonSensorState.Click:
					// 点击
					result.SetDatas(1);
					break;
				case EButtonSensorState.DoubleClick:
					// 双击
					result.SetDatas(2);
					break;
				case EButtonSensorState.LongPress:
					// 长按
					result.SetDatas(3);
					break;
				default:
					result.SetDatas(1);
					break;
			}
			return true;
		}

		protected override void Tick(float deltaTime)
		{
			bool resetChanged = false;
			base.Tick(deltaTime);
			if (waitForInput)
			{
				//if (hasWaitingTimeout)
				//{
				//	resetChanged = true;
				//	waitingInputAsync = null;
				//}
			}

			if (resetChanged)
			{
				ResetChangedTime();
			}
		}

		protected override bool OnVerify(ICommand command)
		{
			if (command is UKitCommands.ButtonCommand)
			{
				return true;
			}

			return false;
		}

		public override ICommandResponseAsync Execute(ICommand command)
		{
			bool resImme = false;
			IProtocol result = null;

			switch (command.commandID)
			{
				case ECommand.UKitCommand:
					{
						resImme = ExecuteUKitCommands(command, out result);
						break;
					}
			}

			if (result == null)
			{
				DebugUtility.LogError(LoggerTags.Project, "Failure to execute command : {0}", command);
			}

			ICommandResponseAsync cra = null;
			if (resImme)
			{
				DebugUtility.Log(LoggerTags.Project, "Success to execute command : {0} - Immediately", command);
				var cra2 = new CommandResponseAsync(result);
				cra2.host = command.host;
				cra2.context = command.context;
				cra = cra2;
			}
			else if (result != null)
			{
				DebugUtility.Log(LoggerTags.Project, "Success to execute command : {0} - Delay", command);
				waitingInputAsync = new TriggerCommandResponseAsync(result);
				waitingInputAsync.host = command.host;
				waitingInputAsync.context = command.context;
				cra = waitingInputAsync;
			}
			return cra;
		}

		protected bool ExecuteUKitCommands(ICommand command, out IProtocol result)
		{
			bool resImme = true;
			var result2 = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			do
			{
				if (command is UKitCommands.ButtonCommand)
				{
					debug = command.debug;
					if (!Populate(result2))
					{
						resImme = false;
					}
					break;
				}

			} while (false);

			result = result2;
			return resImme;
		}
	}

	public static class ButtonSensorStateHelper
	{
		public static EButtonSensorState FromState(int state)
		{
			switch (state)
			{
				case 1:
					return EButtonSensorState.Click;
				case -1:
					return EButtonSensorState.LongPress;
				case 2:
					return EButtonSensorState.DoubleClick;
				default:
					return EButtonSensorState.None;
			}
		}
	}

}
