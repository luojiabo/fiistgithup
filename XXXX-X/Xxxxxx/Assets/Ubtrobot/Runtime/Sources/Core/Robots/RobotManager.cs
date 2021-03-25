using System;
using System.Collections;
using System.Collections.Generic;
using Loki;
using Loki.UI;
using UnityEngine;

namespace Ubtrobot
{
	public partial class RobotManager : USingletonObject<RobotManager>, ISystem
	{
		class RobotConnection
		{
			public readonly List<IProtocol> protocols = new List<IProtocol>();
			public readonly List<ICommandResponseAsync> waitings = new List<ICommandResponseAsync>();
			public IConnection conn;
			public string host;

			public List<IProtocol> ExtractProtocols(bool byOrder)
			{
				if (waitings.Count > 0)
				{
					if (byOrder)
					{
						var w = waitings[0];
						if (w.done)
						{
							w.GetProtocols(protocols);
							waitings.RemoveAt(0);
						}
					}
					else
					{
						foreach (var w in waitings)
						{
							if (w.done)
							{
								w.GetProtocols(protocols);
							}
						}
						waitings.RemoveAll(w => w.done);
					}
				}

				return protocols;
			}

			public void Release()
			{
				var netMgr = NetworkManager.Get();
				if (netMgr != null)
				{
					netMgr.DisconnectToScratch(host);
				}
				conn = null;
				protocols.Clear();
				waitings.Clear();
			}
		}

		private static readonly Type msType = typeof(RobotManager);

		private readonly List<IProtocol> mHandleProtocols = new List<IProtocol>();
		private readonly List<IProtocol> mRecvProtocols = new List<IProtocol>();
		private readonly List<PartComponent> mColliderComponentCache = new List<PartComponent>();

		private readonly object mProtocolLock = new object();
		private RobotConnection mAcitivedConn = null;

		private readonly Dictionary<IRobot, RobotConnection> mConnInfos = new Dictionary<IRobot, RobotConnection>();
		private readonly List<IRobot> mRobots = new List<IRobot>();
		private bool mInitConnect = false;

		private bool mPostInited = false;
		public Action onRobotChanged = null;

		public string systemName { get { return msType.Name; } }

		public IModuleInterface module { get; set; }

		public Component protocolHook { get; set; }

		[PreviewMember]
		public int maxProtocolsPF { get; set; } = -1;

		[PreviewMember]
		[ConsoleProperty(aliasName = "robot.autoHost")]
		public bool autoHost { get; set; } = false;

		public IRobot firstRobot
		{
			get
			{
				return mRobots.Find(r => r.gameObject.activeSelf);
			}
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
		}

		/// <summary>
		/// 可在Factory 中直接注册Robot到此，完成兼容PIE模式以及PIR模式的功能
		/// </summary>
		/// <param name="robot"></param>
		public void AddRobot(IRobot robot)
		{
			if (!mRobots.Contains(robot))
			{
				mRobots.Add(robot);
				robot.Initialize();
			}
			if (mRobots.Count == 1 && mPostInited)
			{
				ActiveRobot(robot.gameObject.name);
			}
			else
			{
				robot.gameObject.SetActive(false);
			}
			Misc.SafeInvoke(onRobotChanged);
		}

		public void RemoveRobot(IRobot robot)
		{
			if (mRobots.Remove(robot))
			{
				Misc.SafeInvoke(onRobotChanged);
			}
		}

		private void HandleMessage(ICommand cmd, IRobot robot)
		{
			DebugUtility.Log(LoggerTags.Online, "{0} Handling message : {1}", robot, cmd);
			ICommandResponseAsync waiting = robot.Execute(cmd);
			if (waiting == null)
			{
				DebugUtility.LogError(LoggerTags.Online, "Can't handle message : {0}", cmd);
				return;
			}

			if (mConnInfos.TryGetValue(robot, out var conn))
			{
				DebugUtility.Log(LoggerTags.Online, "Waiting : {0}", waiting);
				conn.waitings.Add(waiting);
			}
			else if (mAcitivedConn != null)
			{
				mAcitivedConn.waitings.Add(waiting);
			}
		}

		public void PushMessage(IProtocol protocol)
		{
			lock (mProtocolLock)
			{
				mRecvProtocols.Add(protocol);
			}
		}

		public void HandleMessage(IProtocol protocol)
		{
			var cmd = CommandFactory.CreateCommand(protocol);
			if (cmd != null)
			{
				// 找到第一个不禁用命令控制，并且可以接收指定的Robot
				IRobot robot = mRobots.Find(r => r.commandType != ECommandType.Disable && r.gameObject.activeInHierarchy && r.isKinematicReady && r.Verify(cmd));
				if (robot != null)
				{
					HandleMessage(cmd, robot);
				}
				else
				{
					DebugUtility.Log(LoggerTags.Online, "Can't Handle message : {0}", cmd);
				}
			}
		}

		public IEnumerator Initialize()
		{
			return null;
		}

		public IEnumerator PostInitialize()
		{
			mPostInited = true;
			try
			{
				if (mRobots.Count > 0)
				{
					ActiveRobot(mRobots[0].gameObject.name);
					WindowManager.CloseAll();
					WindowManager.Open<SimulationWindow>();
				}

				//Reconnect();
				//var hostRef = NetworkManager.GetOrAlloc().hostSettingRef;
				//if (hostRef.connType == EHostConnType.Deploy)
				//{
				//	var conn = netMgr.ConnectToScratch();
				//	mAcitivedConn = new RobotConnection() { host = netMgr.hostSettingRef.GetHost(), conn = conn };
				//	return;
				//}

				////var wconn = new RobotConnection() { conn = windmill, host = hostRef.scratchWindmillSIMHost };
				////var rconn = new RobotConnection() { conn = roboticArm, host = hostRef.scratchRoboticArmSIMHost };

				//var windmill = NetworkManager.GetOrAlloc().ConnectToScratch(hostRef.scratchWindmillSIMHost, true);
				//var roboticArm = NetworkManager.GetOrAlloc().ConnectToScratch(hostRef.scratchRoboticArmSIMHost, true);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}

			yield break;
		}


		public void OnFixedUpdate(float fixedDeltaTime)
		{
		}

		public void OnLateUpdate()
		{
		}

		private void OnUpdate(RobotConnection info, float deltaTime)
		{
			if (info == null)
				return;

			var conn = info.conn;
			if (conn == null)
				return;

			var protocols = info.ExtractProtocols(false);

			int count = 0;
			while ((protocols.Count > 0) && (conn != null) && (count < protocols.Count) && (maxProtocolsPF < 0 || count <= maxProtocolsPF))
			{
				var protocol = protocols[count];
				conn.SendMessage(protocol);
				try
				{
					if (protocolHook != null)
					{
						protocolHook.SendMessage("OnProtocolHook", protocol, SendMessageOptions.DontRequireReceiver);
					}
				}
				catch (Exception ex)
				{
					DebugUtility.LogException(ex);
				}

				count++;
			}

			if (count > 0)
			{
				if (protocols.Count <= count)
				{
					protocols.Clear();
				}
				else
				{
					protocols.RemoveRange(0, count);
				}
			}
		}

		private void HandleRobots(float deltaTime)
		{
			foreach (var robot in mRobots)
			{
				if (mConnInfos.TryGetValue(robot, out var info))
				{
					OnUpdate(info, deltaTime);
				}
			}
			OnUpdate(mAcitivedConn, deltaTime);
		}

		private void HandleRecvProtocols()
		{
			lock (mProtocolLock)
			{
				mHandleProtocols.AddRange(mRecvProtocols);
				mRecvProtocols.Clear();
			}

			if (mHandleProtocols.Count > 0)
			{
				for (int i = 0; i < mHandleProtocols.Count; i++)
				{
					IProtocol protocol = mHandleProtocols[i];
					mHandleProtocols[i] = null;
					if (protocol != null)
					{
						HandleMessage(protocol);
					}
				}
				mHandleProtocols.Clear();
			}
		}

		public void OnUpdate(float deltaTime)
		{
			HandleRecvProtocols();
			HandleInputSystem(deltaTime);
			HandleRobots(deltaTime);
		}

		public void RewindRobots()
		{
			foreach (var robot in mRobots)
			{
				robot.Rewind();
			}
		}

		private void Reconnect(string activeTarget)
		{
			var netMgr = NetworkManager.GetOrAlloc();
			string targetHost = netMgr.hostSettingRef.GetHost();
			if (targetHost.StartsWith(WebBridgeClient.ProtocolHeader, StringComparison.OrdinalIgnoreCase))
			{
				targetHost = string.Concat(WebBridgeClient.ProtocolHeader, activeTarget);
			}
			var conn = netMgr.GetScratchConn(targetHost);
			if (conn == null)
			{
				conn = netMgr.ConnectToScratch(targetHost, true);
			}
			mAcitivedConn = new RobotConnection() { host = targetHost, conn = conn };
		}

		public void Shutdown()
		{
		}

		public void Startup()
		{
		}

		public void Uninitialize()
		{
			mAcitivedConn.Release();
			foreach (var conn in mConnInfos)
			{
				conn.Value.Release();
			}
			mConnInfos.Clear();
		}

		[ConsoleMethod(aliasName = "robot.active")]
		public void ActiveRobot(string target)
		{
			bool actived = false;
			foreach (IRobot robot in mRobots)
			{
				bool currentStatus = robot.gameObject.activeSelf;
				actived = !actived && robot.gameObject.name == target;
				robot.gameObject.SetActive(actived);

				if (actived)
				{
					if (autoHost)
					{
						var setting = NetworkManager.GetOrAlloc().hostSettingRef;
						if (setting.debug && setting.FindHostEndsWith(target, StringComparison.OrdinalIgnoreCase, out var connType))
						{
							setting.connType = connType;
						}
					}
					Reconnect(target);
				}
			}
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "robot.stopTest")]
		private void StopTest()
		{
			Test(false);
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "robot.startTest")]
		private void StartTest()
		{
			Test(true);
		}

		[ConsoleMethod(aliasName = "robot.test")]
		public void Test(bool test)
		{
			if (test)
			{
				autoHost = true;
				var setting = NetworkManager.GetOrAlloc().hostSettingRef;
				setting.debug = true;
				var first = firstRobot;
				if (first != null)
					ActiveRobot(firstRobot.gameObject.name);
			}
			else
			{
				autoHost = false;
				var setting = NetworkManager.GetOrAlloc().hostSettingRef;
				setting.Default();
				var first = firstRobot;
				if (first != null)
					ActiveRobot(firstRobot.gameObject.name);
			}
		}
	}

	public partial class RobotManager
	{
		private IPartComponentInput mCurrentSelectedPart = null;

		private int mClickCount = 0;
		private float mFirstClickTime = -1.0f;
		private float mLastClickTime = -1.0f;
		private bool mPressed = false;
		private float mPressedEventTime = -1.0f;
		private float mPressedThreshold = 0.3f;
		private float mDoubleClickThreshold = 0.3f;

		public int clickCount
		{
			get
			{
				return mClickCount;
			}
			set
			{
				if (mClickCount != value)
				{
					mClickCount = value;
					if (mClickCount == 0)
					{
						mLastClickTime = mFirstClickTime = -1.0f;

					}
					else if (mClickCount == 1)
					{
						mLastClickTime = mFirstClickTime = Time.realtimeSinceStartup;
					}
					else
					{
						mLastClickTime = Time.realtimeSinceStartup;
					}
				}
			}
		}

		public bool pressed
		{
			get
			{
				return mPressed;
			}
			set
			{
				if (mPressed != value)
				{
					mPressed = value;
					if (mPressed)
					{
						mPressedEventTime = Time.realtimeSinceStartup;
					}
					else
					{
						mPressedEventTime = 0.0f;
					}
				}
			}
		}

		public bool triggerPressed
		{
			get
			{
				if (pressed)
				{
					return (Time.realtimeSinceStartup - mPressedEventTime) >= mPressedThreshold;
				}
				return false;
			}
		}

		private void ResetClickInfo()
		{
			// DebugUtility.LogTrace(LoggerTags.Project, "ResetClickInfo");
			mClickCount = 0;
			mFirstClickTime = -1.0f;
			mLastClickTime = -1.0f;
			mCurrentSelectedPart = null;
		}

		private InputSystem GetInputSystem(bool resetEventData = false)
		{
			InputSystem inputSystem = ModuleManager.Get().GetSystemChecked<InputSystem>();
			var eventData = ToInputEventData(inputSystem);
			if (resetEventData)
			{
				eventData.Reset();
			}
			return inputSystem;
		}

		private PartInputEventData ToInputEventData(InputSystem inputSystem)
		{
			if (inputSystem.currentEventData == null || !(inputSystem.currentEventData is PartInputEventData))
			{
				inputSystem.currentEventData = new PartInputEventData();
			}
			PartInputEventData data = inputSystem.currentEventData as PartInputEventData;
			return data;
		}

		private void FlushEventDatas(InputSystem inputSystem)
		{
			var data = ToInputEventData(inputSystem);
			if (data != null)
			{

			}
		}

		private void HandleInputSystem(float deltaTime)
		{
			InputSystem inputSystem = GetInputSystem(true);
			// 鼠标按键0状态从0到1变化时触发OnPointerDown
			if (inputSystem.GetMouseButtonDown(0))
			{
				OnPointerDown(inputSystem);
			}

			//FlushEventDatas(inputSystem);

			if (pressed)
			{
				// 如果之前处于按压状态，则判断当前是否允许弹起
				if (inputSystem.GetMouseButtonUp(0))
				{
					pressed = false;
					FlushEventDatas(inputSystem);
					OnPointerUp(inputSystem);
				}
				else if (triggerPressed)
				{
					pressed = false;
					FlushEventDatas(inputSystem);
					OnPress(inputSystem);
					ResetClickInfo();
				}
			}

			if (!pressed)
			{
				if (clickCount > 0)
				{
					float now = Time.realtimeSinceStartup;
					if (now - mFirstClickTime > mDoubleClickThreshold)
					{
						if (clickCount > 1)
						{
							FlushEventDatas(inputSystem);
							OnDoubleClick(inputSystem);
							ResetClickInfo();
						}
						else
						{
							FlushEventDatas(inputSystem);
							OnClick(inputSystem);
							ResetClickInfo();
						}
					}
					else
					{
						if (clickCount > 1)
						{
							FlushEventDatas(inputSystem);
							OnDoubleClick(inputSystem);
							ResetClickInfo();
						}
					}
				}
			}

			if (inputSystem.GetKey(KeyCode.Escape))
			{
				ULokiEngine.Get().QuitApplication();
			}
		}

		private void OnPointerUp(InputSystem inputSystem)
		{
			var eventData = ToInputEventData(inputSystem);
			eventData.state = EInputState.Released;
			// DebugUtility.Log(LoggerTags.InputSystem, "OnPointerUp");
			if (mCurrentSelectedPart != null)
			{
				mCurrentSelectedPart.OnRecvInputEvent(eventData);
			}
		}

		private void OnPress(InputSystem inputSystem)
		{
			var eventData = ToInputEventData(inputSystem);
			eventData.state = EInputState.LongPress;
			DebugUtility.LogTrace(LoggerTags.InputSystem, "OnPress");
			if (mCurrentSelectedPart != null)
			{
				mCurrentSelectedPart.OnRecvInputEvent(eventData);
			}
		}

		private void OnClick(InputSystem inputSystem)
		{
			var eventData = ToInputEventData(inputSystem);
			eventData.state = EInputState.Click;
			DebugUtility.LogTrace(LoggerTags.InputSystem, "OnClick");
			if (mCurrentSelectedPart != null)
			{
				mCurrentSelectedPart.OnRecvInputEvent(eventData);
			}
		}

		private void OnDoubleClick(InputSystem inputSystem)
		{
			var eventData = ToInputEventData(inputSystem);
			eventData.state = EInputState.DoubleClick;
			DebugUtility.LogTrace(LoggerTags.InputSystem, "OnDoubleClick");
			if (mCurrentSelectedPart != null)
			{
				mCurrentSelectedPart.OnRecvInputEvent(eventData);
			}
		}

		private void OnPointerDown(InputSystem inputSystem)
		{
			bool hitInputPart = false;
			IPartComponentInput currentInput = null;

			if (inputSystem.GetTouchPosition(out var currentPosition))
			{
				CameraSystem cameraSystem = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				if (cameraSystem != null && cameraSystem.activeController != null)
				{
					var cc = cameraSystem.activeController;
					Ray ray = cc.ScreenPointToRay(currentPosition);

					if (Physics.Raycast(ray, out var hitInfo, 1000, LayerUtility.LayerToMask(LayerUtility.PartLayer)))
					{
						DebugUtility.Log("Single ray", "hitInfo : {0}", hitInfo.transform.name);
						var part = hitInfo.collider.GetComponentInParent<Part>();
						if (part != null)
						{
							foreach (var hitPart in part.partsComponents)
							{
								if (hitPart is IPartComponentInput)
								{
									currentInput = (IPartComponentInput)hitPart;
									hitInputPart = true;
									break;
								}
							}
						}
					}
				}
			}

			// 过期
			if (mDoubleClickThreshold > 0.0f && mLastClickTime > 0.0f && Time.realtimeSinceStartup - mLastClickTime > mDoubleClickThreshold)
			{
				ResetClickInfo();
			}

			// 点击次数加1
			if (currentInput != null && currentInput == this.mCurrentSelectedPart)
			{
				clickCount++;
			}
			else if (currentInput == null || currentInput != this.mCurrentSelectedPart)
			{
				ResetClickInfo();
				clickCount = 1;
				this.mCurrentSelectedPart = currentInput;
			}

			// 使用当前是否初次点击中输入模块来判断
			pressed = hitInputPart;
		}
	}
}
