using System;
using System.Collections.Generic;
using System.Text;
using Loki;
using Loki.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ubtrobot
{
	public partial class SimulationWindow : Window
	{
		#region Field
		private readonly List<KeyValuePair<PartComponent, ToolTipInfo>> partComponentMap2UI = new List<KeyValuePair<PartComponent, ToolTipInfo>>();
		private readonly List<RectTransform> disableSliderList = new List<RectTransform>();
		private readonly List<PartComponent> inputCommandGroup = new List<PartComponent>();
		private static readonly string[] cameraArray = { "SceneViewer", "FollowViewer" };

		private Vector3 robotOldPos = Vector3.zero;
		private GameObject item;
		private RectTransform tips;
		private int isFullScreen = 1;

		private IRobot robot;
		//private Image handImage;
		public Texture2D cursorTexture1;

		private int cameraIdx = 0;
		private SceneInfo sceneInfo;

		private ICameraController cameraController;
		private Vector3 oldViewCameraPos = Vector3.zero;
		private Vector3 oldFollowCameraPos = Vector3.zero;
		private float sceneViewerOldFieldOfView = 0;
		private float followViewerOldFieldOfView = 0;

		private RectTransform env;
		private RectTransform detailsPanel;
		private RectTransform itemSlider;
		private RectTransform dropDownImage;

		private bool isFoldEnviromentPanel = false;
		private bool isStart = false;

		private float detailsPanelHeight = 0f;
		private float itemHeight = 50f;

		private Button playButton;
		private Button resetButton;
		private Button backButton;
		private Button changeCameraIndexButton;
		private Button magnifyButton;
		private Button shrinkButton;
		private Button fullScreenButton;
		private Button foldButton;
		private Toggle setHightlightToggle;

		private Text currModelDetailsText;
		private Text detailsPanelRobotTitle;
		private Text modelX;
		private Text modelY;
		private Text modelZ;
		private Text buttonSensorTips;
		private Text foldButtonText;
		private Text backButtonText;
		private Text changeCameraIndexTitle;
		private Text currModelTitle;
		private Text envNameText;
		private Text playText;
		private Text resetText;
		private Text showIdText;
		private Text magnifyText;
		private Text shrinkText;
		private Text fullScreenText;

		private Part currentTouch;

		private EnvRange envRange
		{
			get
			{
				if (sceneInfo == null)
					return EnvRange.Default;

				return sceneInfo.config;
			}
		}

		public Part currentTouchPart
		{
			get
			{
				return currentTouch;
			}
			set
			{
				if (currentTouch != value)
				{
					if (currentTouch != null)
					{
						OnTouchEnd(currentTouch);
					}
					currentTouch = value;
					if (currentTouch != null)
					{
						OnTouchBegin(currentTouch);
					}
				}
			}
		}
		#endregion

		public override void OnInit()
		{
			InitItem();
		}

		protected override void OnOpen(object param)
		{
			setHightlightToggle.isOn = false;
			sceneInfo = FindObjectOfType<SceneInfo>();
			if (sceneInfo != null)
			{
				env.gameObject.SetActive(sceneInfo.config.showEnvParameter);
				foldButton.gameObject.SetActive(sceneInfo.config.showEnvParameter);

				if (sceneInfo.config.showEnvParameter)
				{
					SetEvnSliders(itemSlider, detailsPanel);
				}
			}

			RegisterUnityAction();
			ActiveCameraController(cameraIdx);
			UpdateRobot();
			SetUILanguageText();

		}

		private void Update()
		{
			if (robot != null)
			{
				SetRobotPositionText(robot.transform.position);
				UpdateSensorTipsPos();

			}

			CheckTouch(robot != null && setHightlightToggle.isOn);
		}

		protected override void OnClose()
		{
			UnRegisterUnityAction();
			CleanupCommandGroup();
			sceneInfo = null;
			robot = null;
			cameraController = null;
			cameraIdx = 0;
			DetailsPanelSliderListOnClose();
			oldViewCameraPos = Vector3.zero;
			oldFollowCameraPos = Vector3.zero;
			sceneViewerOldFieldOfView = 0;
			followViewerOldFieldOfView = 0;
			setHightlightToggle.isOn = false;
			Part.SetGlobalLightIntensity(false);
			base.OnClose();
		}
	}

	public partial class SimulationWindow
	{
		private void InitItem()
		{
			env = contentNode.GetComponent<RectTransform>("Env");
			env.gameObject.SetActive(true);

			foldButton = contentNode.GetComponent<Button>("FoldButton");
			foldButtonText = foldButton.GetComponent<Text>("Title");
			dropDownImage = foldButton.GetComponent<RectTransform>("DropDownImage");

			detailsPanel = env.GetComponent<RectTransform>("DetailsPanel");
			detailsPanelRobotTitle = env.GetComponent<Text>("DetailsPanel/Title/Title");
			itemSlider = env.GetComponent<RectTransform>("ItemSlider");

			currModelDetailsText = contentNode.GetComponent<Text>("CurrModle/Details");
			backButton = contentNode.GetComponent<Button>("Back");
			backButtonText = backButton.GetComponent<Text>("Title");

			modelX = contentNode.GetComponent<Text>("Position/X");
			modelY = contentNode.GetComponent<Text>("Position/Y");
			modelZ = contentNode.GetComponent<Text>("Position/Z");

			tips = contentNode.GetComponent<RectTransform>("Tips");
			tips.gameObject.SetActive(false);

			playButton = contentNode.GetComponent<Button>("Down/Left/Play");
			resetButton = contentNode.GetComponent<Button>("Down/Left/Reset");

			setHightlightToggle = contentNode.GetComponent<Toggle>("Scale/ShowComponentName");
			changeCameraIndexButton = contentNode.GetComponent<Button>("Scale/ChangeCameraState");
			changeCameraIndexTitle = contentNode.GetComponent<Text>("Scale/ChangeCameraState/Title");
			magnifyButton = contentNode.GetComponent<Button>("Scale/Magnify");
			shrinkButton = contentNode.GetComponent<Button>("Scale/Shrink");
			fullScreenButton = contentNode.GetComponent<Button>("Scale/FullScreen");

			//handImage = contentNode.GetComponent<Image>("Hand");
			//handImage.enabled = false;
			//currModleTitle = contentNode.GetComponent<Text>("CurrModle/Title");
			//envNameText = contentNode.GetComponent<Text>("Env/Top/NameText");
			//playText = playButton.GetComponent<Text>("Title");
			//resetText = resetButton.GetComponent<Text>("Title");
			//showIdText = setHightlightToggle.GetComponent<Text>("Title");
			//magnifyText = magnifyButton.GetComponent<Text>("Title");
			//shrinkText = shrinkButton.GetComponent<Text>("Title");
			//fullScreenText = fullScreenButton.GetComponent<Text>("Title");
		}

		private void OnTouchEnd(Part part)
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

			foreach (var dicPair in partComponentMap2UI)
			{
				if (dicPair.Value.part == part)
				{
					//dicPair.Value.label.enabled = false;
					dicPair.Value.root.gameObject.SetActive(false);
					dicPair.Value.part.UseOutline(true, 0);
				}
			}
		}

		private void OnTouchBegin(Part part)
		{
			foreach (var dicPair in partComponentMap2UI)
			{
				if (dicPair.Value.part == part)
				{
					var result = dicPair.Value.root.SetAsAnchoredPosition(cameraController.controlled, dicPair.Key.transform.position);
					//dicPair.Value.label.enabled = result;
					dicPair.Value.root.gameObject.SetActive(result);
					dicPair.Value.part.UseOutline(true, 1);

					if (dicPair.Key is ButtonSensorComponent && cursorTexture1 != null)
					{
						Cursor.SetCursor(cursorTexture1, Vector2.zero, CursorMode.Auto);
					}
				}
				else
				{
					dicPair.Value.part.UseOutline(true, 0);
				}
			}
		}

		private void CheckTouch(bool enableCheck)
		{
			if (!enableCheck)
			{
				currentTouchPart = null;
				return;
			}

			InputSystem inputSystem = ModuleManager.Get().GetSystemChecked<InputSystem>();
			if (inputSystem == null)
			{
				currentTouchPart = null;
				return;
			}

			bool touching = false;

			if (inputSystem.GetCursorPosition(out var position))
			{
				CameraSystem cameraSystem = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				if (cameraSystem != null && cameraSystem.activeController != null)
				{
					var cc = cameraSystem.activeController;
					Ray ray = cc.ScreenPointToRay(position);

					if (Physics.Raycast(ray, out var hitInfo, 1000, LayerUtility.LayerToMask(LayerUtility.PartLayer)))
					{
						currentTouchPart = hitInfo.collider.GetComponentInParent<Part>(false, true);
						touching = currentTouchPart != null;
					}
				}
			}

			if (!touching)
			{
				currentTouchPart = null;
			}
		}

		private void SetUILanguageText()
		{
			//backButtonText.text = UILanguageText.backTitle;
			//currModleTitle.text = UILanguageText.currModleTitle;
			//envNameText.text = UILanguageText.envNameText;
			//playText.text = UILanguageText.playStartText;
			//resetText.text = UILanguageText.resetText;
			//showIdText.text = UILanguageText.showIDTitle;
			//magnifyText.text = UILanguageText.magnifyText;
			//shrinkText.text = UILanguageText.shrinkText;
			//fullScreenText.text = UILanguageText.fullScreenText;

			foldButtonText.text = UILanguageText.foldTitleText;
		}

		private void UpdateRobot(IRobot currentRobot, bool force = false)
		{
			if (robot == currentRobot && !force)
				return;

			CleanupCommandGroup();
			robot = currentRobot;

			if (robot != null)
			{
				// temp code
				robot.nameOnUI = sceneInfo.config.robotNameOnUI;

				InitCommandGroup();
				SetDetailsPanelRobotTitleText(robot.nameOnUI);
				SetSimulationInfoText(robot.nameOnUI);
				SetRobotPositionText(robot.transform.position);
			}
			else
			{
				SetDetailsPanelRobotTitleText(string.Empty);
				SetSimulationInfoText(string.Empty);
				SetRobotPositionText(Vector3.zero);
			}
		}

		private void CleanupCommandGroup()
		{
			foreach (var item in partComponentMap2UI)
			{
				item.Value.Release();
			}
			partComponentMap2UI.Clear();
			inputCommandGroup.Clear();
		}

		private void InitCommandGroup()
		{
			robot.GetCommandComponents(inputCommandGroup);

			for (int i = 0; i < inputCommandGroup.Count; i++)
			{
				var component = inputCommandGroup[i];
				var sceneDrawer = component.GetType().GetCustomAttribute<SceneDrawerAttribute>(true);
				if (sceneDrawer == null)
					continue;

				int id = -1;
				if (component is IPartIDComponent idp)
				{
					id = idp.id;
				}

				GameObject tempGameObject = Instantiate(tips.gameObject, contentNode, false);
				tempGameObject.SetActive(true);
				RectTransform rectTr = tempGameObject.transform as RectTransform;

				var label = rectTr.GetComponent<Text>("Label");
				label.text = id >= 0 ? string.Concat(sceneDrawer.sceneTitle, "(ID=<color=red>", id.ToString(), "</color>)") : sceneDrawer.sceneTitle;

				var toolTip = rectTr.GetComponent<Text>("Tooltip");
				toolTip.text = string.Concat("(", sceneDrawer.tooltip, ")");

				//RectTransform tooltipRectTransform = rectTr.GetComponent<RectTransform>("Tooltip");
				//Text toolText = tooltipRectTransform.GetComponent<Text>();		    
				//toolText.text = sceneDrawer.tooltip;
				//toolText.enabled = false;

				//var uiEvent = label.GetComponent<UIEventeEnterExitListener>();
				//uiEvent.enter.RemoveAllListeners();
				//uiEvent.exit.RemoveAllListeners();

				//uiEvent.enter.AddListener((listener, date) =>
				//{
				//	toolText.enabled = true;
				//});
				//uiEvent.exit.AddListener((listener, date) =>
				//{
				//	toolText.enabled = false;
				//});
				//label.enabled = false;
				rectTr.SetAsAnchoredPosition(cameraController.controlled, component.transform.position);
				Part part = component.GetPart();

				partComponentMap2UI.Add(new KeyValuePair<PartComponent, ToolTipInfo>(component, new ToolTipInfo()
				{
					part = part,
					root = rectTr,
					label = label,
					//tooltip = toolText,
				}));
				rectTr.gameObject.SetActive(false);
			}
		}

		private void UpdateSensorTipsPos()
		{
			//if (showComponentName.isOn)
			//{
			//	foreach (var item in partComponentMap2UI)
			//	{
			//		var result = item.Value.root.SetAsAnchoredPosition(cameraController.controlled, item.Key.transform.position);
			//		item.Value.label.enabled = result;
			//	}
			//}
		}

		#region GetSystem
		private void ActiveCameraController(int cameraType)
		{
			if (cameraType == 0 || robot == null)
			{
				var cameraSystem = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				if (cameraSystem.ActiveController("SceneViewer"))
				{
					cameraController = cameraSystem.activeController;

					if (oldViewCameraPos == Vector3.zero && sceneViewerOldFieldOfView == 0)
					{
						oldViewCameraPos = cameraController.transform.position;
						sceneViewerOldFieldOfView = cameraController.controlled.fieldOfView;
					}
					else
					{
						cameraController.transform.position = oldViewCameraPos;
						cameraController.controlled.fieldOfView = sceneViewerOldFieldOfView;
					}
					changeCameraIndexTitle.text = UILanguageText.overallModel;

					//if (robot != null)
					//{
					//	cameraController.transform.LookAt(robot.transform);
					//}

					DefaultCameraController defaultCameraController = cameraController as DefaultCameraController;
					if (defaultCameraController != null)
					{
						defaultCameraController.cameraZone = sceneInfo.sphereCameraZone;
					}
				}
				else
				{
					changeCameraIndexTitle.text = "";
					DebugUtility.LogError(LoggerTags.UI, "Get SceneViewer failed");
				}
			}
			else if (cameraType == 1)
			{
				var cameraSystem = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				if (cameraSystem.ActiveController("FollowViewer"))
				{
					cameraController = cameraSystem.activeController;

					if (oldFollowCameraPos == Vector3.zero && followViewerOldFieldOfView == 0)
					{
						oldFollowCameraPos = cameraController.transform.position;
						followViewerOldFieldOfView = cameraController.controlled.fieldOfView;
					}
					else
					{
						cameraController.transform.position = oldFollowCameraPos;
						cameraController.controlled.fieldOfView = followViewerOldFieldOfView;
					}

					changeCameraIndexTitle.text = UILanguageText.followModel;
				}
				else
				{
					changeCameraIndexTitle.text = "";
					DebugUtility.LogError(LoggerTags.UI, "Get FollowViewer failed");
				}
			}
		}

		private void UpdateRobot()
		{
			RobotManager robotManager = ModuleManager.Get().GetSystemChecked<RobotManager>();
			if (robotManager != null)
			{
				robotManager.onRobotChanged = OnRobotChanged;
				UpdateRobot(robotManager.firstRobot, true);
			}
			else
			{
				UpdateRobot(null, true);
				DebugUtility.LogError(LoggerTags.UI, "RobotManager is null");
			}
		}

		private void OnRobotChanged()
		{
			RobotManager robotManager = ModuleManager.Get().GetSystemChecked<RobotManager>();
			if (robotManager != null)
			{
				UpdateRobot(robotManager.firstRobot, false);
			}
		}
		#endregion

		#region UpdateText
		public void SetSimulationInfoText(string robotName)
		{
			if (currModelDetailsText != null)
			{
				currModelDetailsText.text = robotName;
			}
		}

		private void SetDetailsPanelRobotTitleText(string robotName)
		{
			if (detailsPanelRobotTitle != null)
			{
				detailsPanelRobotTitle.text = string.Format("{0}  <size=13>{1}</size>", robotName, UILanguageText.detailsPanelRobotTitle);
			}
		}

		public void SetRobotPositionText(Vector3 robotPos)
		{
			if (Loki.Misc.Nearly(robotOldPos, robotPos))
			{
				return;
			}

			robotOldPos = robotPos;
			if (modelX != null && modelY != null && modelZ != null)
			{
				modelX.text = "X:" + robotPos.x.ToString("f1");
				modelY.text = "Y:" + robotPos.y.ToString("f1");
				modelZ.text = "Z:" + robotPos.z.ToString("f1");
			}
		}

		private void UpdateTitle(Text titleText, string titleContent)
		{
			if (titleText == null)
				return;

			titleText.text = titleContent;
		}
		#endregion

		#region Fold
		private void FoldButtonOnClick()
		{
			if (detailsPanel != null)
			{
				//foreach (Transform childtTransform in detailsPanel)
				//{
				//	childtTransform.gameObject.SetActive(isFoldEnviromentPanel);
				//}

				detailsPanel.gameObject.SetActive(isFoldEnviromentPanel);
				dropDownImage.Rotate(Vector3.forward, 180);
				//if (foldButtonText != null)
				//{
				//	foldButtonText.text = isFoldEnviromentPanel ? UILanguageText.foldText : UILanguageText.unFoldText;
				//}

				isFoldEnviromentPanel = !isFoldEnviromentPanel;
			}
		}
		#endregion

		#region RegisterUnityAction
		private void RegisterUnityAction()
		{
			RegisterButton(foldButton, FoldButtonOnClick);
			RegisterButton(playButton, PlayButtonOnClick);
			RegisterButton(resetButton, ResetButtonOnClick);
			RegisterButton(changeCameraIndexButton, ChangeCameraOnClick);
			RegisterButton(magnifyButton, MagnifyOnClick);
			RegisterButton(shrinkButton, ShrinkOnClick);
			RegisterButton(fullScreenButton, FullScreenOnClick);
			RegisterButton(backButton, BackOnClick);

			setHightlightToggle.onValueChanged.AddListener(SetPartComponentHightlight);
		}

		private void SetPartComponentHightlight(bool unlit)
		{
			if (partComponentMap2UI.Count > 0)
			{
				Part.SetGlobalLightIntensity(unlit);
				foreach (var item in partComponentMap2UI)
				{
					item.Value.part.UseOutline(unlit, 0);
					item.Value.part.SetHighlight(unlit);
				}
			}
		}

		private void UnRegisterUnityAction()
		{
			UnRegisterButton(foldButton, FoldButtonOnClick);
			UnRegisterButton(playButton, PlayButtonOnClick);
			UnRegisterButton(resetButton, ResetButtonOnClick);
			UnRegisterButton(changeCameraIndexButton, ChangeCameraOnClick);
			UnRegisterButton(magnifyButton, MagnifyOnClick);
			UnRegisterButton(shrinkButton, ShrinkOnClick);
			UnRegisterButton(fullScreenButton, FullScreenOnClick);
			UnRegisterButton(backButton, BackOnClick);

			RobotManager robotManager = ModuleManager.Get().GetSystemChecked<RobotManager>();
			if (robotManager != null)
			{
				robotManager.onRobotChanged = null;
			}

			setHightlightToggle.onValueChanged.RemoveListener(SetPartComponentHightlight);
		}

		private void RegisterButton(Button button, UnityAction onValueChanged)
		{
			if (button != null)
			{
				button.onClick.AddListener(onValueChanged);
			}
		}

		private void UnRegisterButton(Button slider, UnityAction onValueChanged)
		{
			if (slider != null)
			{
				slider.onClick.RemoveListener(onValueChanged);
			}
		}
		#endregion

		#region ButtonOnClick
		private void FullScreenOnClick()
		{
			isFullScreen = isFullScreen == 1 ? 0 : 1;
			BridgeCenter.Caller.SetFullScreen(isFullScreen);
		}

		private void ResetButtonOnClick()
		{
			isStart = false;
			playText.text = UILanguageText.playStartText;
#if UNITY_EDITOR
			GameObject.FindObjectOfType<ScratchWebSocketClientSimulator>().StopDebug();
#endif
		}

		private void PlayButtonOnClick()
		{
			isStart = !isStart;
			playText.text = isStart ? UILanguageText.playStartText : UILanguageText.playStopText;

#if !UNITY_EDITOR
			if (isStart)
			{
				BridgeCenter.GetOrAlloc().StartLaunching();
			}
			else
			{
				BridgeCenter.GetOrAlloc().StopLaunching();
			}
#endif
			GameObject.FindObjectOfType<ScratchWebSocketClientSimulator>().SimulateRecvAllMessages();
		}

		private void ChangeCameraOnClick()
		{
			int cameraType = (++cameraIdx) % cameraArray.Length;
			ActiveCameraController(cameraType);
		}

		private void ShrinkOnClick()
		{
			if (cameraController != null)
			{
				Camera activeCamera = cameraController.controlled;
				float temporary = activeCamera.fieldOfView;
				temporary += envRange.incAmountOfFOV;
				activeCamera.fieldOfView = Mathf.Clamp(temporary, envRange.minOfFOV, envRange.maxOfFOV);
				if (robot!=null)
				{
					activeCamera.transform.LookAt(robot.transform);
				}

			}
		}

		private void MagnifyOnClick()
		{
			if (cameraController != null)
			{
				Camera activeCamera = cameraController.controlled;
				float temporary = activeCamera.fieldOfView;
				temporary -= envRange.incAmountOfFOV;
				activeCamera.fieldOfView = Mathf.Clamp(temporary, envRange.minOfFOV, envRange.maxOfFOV);
		
				if (robot != null)
				{
					activeCamera.transform.LookAt(robot.transform);
				}
			}
		}

		private void BackOnClick()
		{
			DebugUtility.Log(LoggerTags.UI, "返回按钮");
		}

		#endregion

		#region Slider
		private void DetailsPanelSliderListOnClose()
		{
			foreach (RectTransform childtTransform in detailsPanel)
			{
				if (childtTransform.GetComponentInChildren<Slider>() != null)
				{
					childtTransform.gameObject.SetActive(false);
					disableSliderList.Add(childtTransform);
				}
			}

			for (int i = 0; i < disableSliderList.Count; i++)
			{
				disableSliderList[i].SetParent(detailsPanel.parent);
				disableSliderList[i].GetComponent<Slider>("Slider").onValueChanged.RemoveAllListeners();
			}
		}

		private void SetEvnSliders(RectTransform original, RectTransform parent)
		{
			if (envRange == null || envRange.rangArgs == null)
				return;

			int index = 0;
			RectTransform temp;

			foreach (var arg in envRange.rangArgs)
			{
				if (index < disableSliderList.Count)
				{
					temp = disableSliderList[index];
					index++;
				}
				else
				{
					temp = Instantiate(original);
				}

				temp.SetParent(parent);
				temp.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				temp.localScale = Vector3.one;
				temp.gameObject.SetActive(true);
				Slider temSlider = temp.GetComponent<Slider>("Slider");

				temSlider.minValue = arg.min;
				temSlider.maxValue = arg.max;
				temSlider.wholeNumbers = arg.wholeNumbers;
				temSlider.value = arg.GetEnvironment(sceneInfo.activeEnv);
				Text title = temp.GetComponent<Text>("Title");
				title.text = arg.GetDesc(sceneInfo.activeEnv);

				var inst = arg;

				temSlider.onValueChanged.AddListener(currentValue =>
				{
					inst.UpdateEnvironment(sceneInfo.activeEnv, currentValue);
					title.text = arg.GetDesc(sceneInfo.activeEnv);
				});
			}
		}
		#endregion
	}
}
