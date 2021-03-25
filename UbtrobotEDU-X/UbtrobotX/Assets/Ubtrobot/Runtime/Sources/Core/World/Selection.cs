using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class Selection : USingletonObject<Selection>, ISystem
	{
		private readonly List<Actor> mSelectedActors = new List<Actor>();
		//private GameObject mAxisTranslation;
		//private GameObject mAxisRotation;
		//private GameObject mAxisScale;
		//private ETRSMode mTRSMode = ETRSMode.None;

		private static readonly Type msType = typeof(Selection);

		public string systemName { get { return msType.Name; } }

		public IModuleInterface module { get; set; }

		//[PreviewMember]
		//public ETRSMode trsMode
		//{
		//	get
		//	{
		//		return mTRSMode;
		//	}
		//	set
		//	{
		//		if (mTRSMode != value)
		//		{
		//			mTRSMode = value;
		//			TranslationSelected(mTRSMode == ETRSMode.Translation);
		//			RotationSelected(mTRSMode == ETRSMode.Rotation);
		//			ScaleSelected(mTRSMode == ETRSMode.Scale);
		//		}
		//	}
		//}

		//[PreviewMember]
		//public GameObject axisTranslation
		//{
		//	get
		//	{
		//		if (mAxisTranslation == null)
		//		{
		//			mAxisTranslation = UbtrobotSettings.GetOrLoad().selectionSettings.Translation();
		//			if (mAxisTranslation != null)
		//			{
		//				mAxisTranslation.transform.Reset();
		//			}
		//		}
		//		return mAxisTranslation;
		//	}
		//}

		//[PreviewMember]
		//public GameObject axisRotation
		//{
		//	get
		//	{
		//		if (mAxisRotation == null)
		//		{
		//			mAxisRotation = UbtrobotSettings.GetOrLoad().selectionSettings.Rotation();
		//			if (mAxisRotation != null)
		//			{
		//				mAxisRotation.transform.Reset();
		//			}
		//		}
		//		return mAxisRotation;
		//	}
		//}

		//[PreviewMember]
		//public GameObject axisScale
		//{
		//	get
		//	{
		//		if (mAxisScale == null)
		//		{
		//			mAxisScale = UbtrobotSettings.GetOrLoad().selectionSettings.Scale();
		//			if (mAxisScale != null)
		//			{
		//				mAxisScale.transform.Reset();
		//			}
		//		}
		//		return mAxisScale;
		//	}
		//}

		//[PreviewMember]
		//public Robot robot
		//{
		//	get
		//	{
		//		return actor as Robot;
		//	}
		//	set
		//	{
		//		if (robot != value)
		//		{
		//			trsMode = ETRSMode.None;
		//			if (robot != null)
		//			{
		//				robot.OnUnselected();
		//			}
		//			actor = value;
		//			if (value != null)
		//			{
		//				value.OnSelected();
		//			}
		//			UpdateSelection();
		//		}
		//	}
		//}

		[PreviewMember]
		public Actor actor
		{
			get
			{
				if (mSelectedActors.Count > 0)
					return mSelectedActors[0];
				return null;
			}
			set
			{
				if (actor != null)
				{
					OnUnselected(actor);
				}

				mSelectedActors.Clear();
				if (value != null)
				{
					mSelectedActors.Add(value);
					OnSelected(value);
				}

				//UpdateSelection();
			}
		}

		public List<Actor> actors
		{
			get
			{
				return mSelectedActors;
			}
			set
			{
				//trsMode = ETRSMode.None;
				mSelectedActors.Clear();
				if (value != null)
				{
					mSelectedActors.AddRange(value);
				}
				//UpdateSelection();
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
		}

		public void OnLateUpdate()
		{
		}

		public void OnUpdate(float deltaTime)
		{
			InputSystem inputSystem = ModuleManager.Get().GetSystemChecked<InputSystem>();
			if (inputSystem == null)
				return;

			if (inputSystem.GetTouchPosition(out var position))
			{
				CameraSystem cameraSystem = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				if (cameraSystem != null && cameraSystem.activeController != null)
				{
					var cc = cameraSystem.activeController;
					Ray ray = cc.ScreenPointToRay(position);

					if (Physics.Raycast(ray, out var hitInfo, 1000, LayerUtility.LayerToMask(LayerUtility.RobotLayer)))
					{
						actor = hitInfo.collider.GetComponent<Robot>();
					}
				}
			}

			//if (inputSystem.GetKey(KeyCode.W))
			//{
			//	trsMode = ETRSMode.Translation;
			//}
			//if (inputSystem.GetKey(KeyCode.E))
			//{
			//	trsMode = ETRSMode.Rotation;
			//}
			//if (inputSystem.GetKey(KeyCode.R))
			//{
			//	trsMode = ETRSMode.Scale;
			//}
			//if (inputSystem.GetKey(KeyCode.Q))
			//{
			//	trsMode = ETRSMode.None;
			//}
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

		//public void TranslationSelected(bool active)
		//{
		//	if (active)
		//	{
		//		var go = actor;
		//		if (go != null)
		//		{
		//			SetCoordinateAxis(go.transform);
		//			active = true;
		//		}
		//		else // fix
		//		{
		//			active = false;
		//		}
		//	}

		//	if (axisTranslation)
		//	{
		//		axisTranslation.SetActive(active);
		//	}
		//}

		//public void RotationSelected(bool active)
		//{
		//	if (active)
		//	{
		//		var go = actor;
		//		if (go != null)
		//		{
		//			SetCoordinateAxis(go.transform);
		//			active = true;
		//		}
		//		else // fix
		//		{
		//			active = false;
		//		}
		//	}

		//	if (axisRotation)
		//	{
		//		axisRotation.SetActive(active);
		//	}
		//}

		//public void ScaleSelected(bool active)
		//{
		//	if (active)
		//	{
		//		var go = actor;
		//		if (go != null)
		//		{
		//			SetCoordinateAxis(go.transform);
		//			active = true;
		//		}
		//		else // fix
		//		{
		//			active = false;
		//		}
		//	}

		//	if (axisScale)
		//	{
		//		axisScale.SetActive(active);
		//	}
		//}

		//public void SetCoordinateAxis(Transform parent)
		//{
		//	if (parent == null)
		//		return;

		//	if (axisTranslation != null)
		//	{
		//		axisTranslation.transform.CopyFrom(parent, true, Space.World);
		//	}
		//}

		//private void UpdateSelection()
		//{
		//	TranslationSelected(mTRSMode == ETRSMode.Translation);
		//	RotationSelected(mTRSMode == ETRSMode.Rotation);
		//	ScaleSelected(mTRSMode == ETRSMode.Scale);
		//}

		private void OnUnselected(Actor actor)
		{
			if (actor != null)
			{
				actor.SendMessage("OnUnselected", SendMessageOptions.DontRequireReceiver);
			}
		}

		private void OnSelected(Actor actor)
		{
			if (actor != null)
			{
				actor.SendMessage("OnSelected", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
