//using System;
//using System.Collections.Generic;
//using Loki;
//using UnityEngine;
//using UnityEngine.AI;

//namespace Ubtrobot
//{
//	public class RobotCar : RobotPredefined
//	{
//		[SerializeField]
//		private ServoPartComponent mLeftFront;
//		[SerializeField]
//		private ServoPartComponent mRightFront;
//		[SerializeField]
//		private ServoPartComponent mLeftBack;
//		[SerializeField]
//		private ServoPartComponent mRightBack;

//		[SerializeField]
//		private float mServoSpeed = 10.0f;

//		[SerializeField]
//		private GameObject mControlTips;

//		private Vector3 mStartPosition;
//		private Quaternion mStartRotation;

//		private NavMeshAgent mAgent;

//		private bool mTraceFinger = false;

//		[ConsoleProperty(aliasName = "car.trace")]
//		[PreviewMember]
//		public bool traceFinger
//		{
//			get { return mTraceFinger; }
//			set
//			{
//				if (mTraceFinger != value)
//				{
//					mTraceFinger = value;
//					if (agent != null)
//					{
//						agent.enabled = value;
//					}
//				}
//			}
//		}

//		public NavMeshAgent agent
//		{
//			get
//			{
//				if (mAgent == null)
//				{
//					mAgent = GetComponent<NavMeshAgent>();
//				}
//				return mAgent;
//			}
//		}

//		protected override void OnInitialize()
//		{
//			base.OnInitialize();

//			mStartPosition = transform.position;
//			mStartRotation = transform.rotation;
//		}

//		private void Update()
//		{
//			InputSystem inputSys = ModuleManager.Get().GetSystemChecked<InputSystem>();
//			if (inputSys != null)
//			{
//				if (traceFinger)
//				{
//					if (agent != null)
//					{
//						if (inputSys.GetMouseButtonDown(0) && inputSys.GetTouchPosition(out var position))
//						{
//							CameraSystem cameraSystem = ModuleManager.Get().GetSystemChecked<CameraSystem>();
//							if (cameraSystem != null && cameraSystem.activeController != null)
//							{
//								var cc = cameraSystem.activeController;
//								Ray ray = cc.ScreenPointToRay(position);
//								if (Physics.Raycast(ray, out var hitInfo, 1000, LayerUtility.LayerToMask(LayerUtility.TerrainLayer)))
//								{
//									agent.destination = hitInfo.point;
//									GameObject pointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//									pointObj.transform.position = hitInfo.point;
//									pointObj.transform.localScale = Vector3.one * 0.3f;
//									Destroy(pointObj, 0.3f);
//								}
//							}
//						}
//					}
//				}

//				if (inputSys.GetKey(KeyCode.Home))
//				{
//					transform.position = mStartPosition;
//					transform.rotation = mStartRotation;
//				}
//			}
//		}

//		private void RotateServoParts(float angle)
//		{
//			if (mLeftFront)
//				mLeftFront.localEulerAnglesY -= angle;
//			if (mRightFront)
//				mRightFront.localEulerAnglesY += angle;
//			if (mLeftBack)
//				mLeftBack.localEulerAnglesY -= angle;
//			if (mRightBack)
//				mRightBack.localEulerAnglesY += angle;
//		}

//		public override void MoveForward(float movement)
//		{
//			base.MoveForward(movement);
//			RotateServoParts(movement * mServoSpeed);

//			//transform.position += transform.forward * movement;
//		}


//		public override void OnSelected()
//		{
//			base.OnSelected();
//			if (mControlTips)
//			{
//				mControlTips.SetActive(false);
//			}
//		}

//		public override void OnUnselected()
//		{
//			base.OnUnselected();
//			if (mControlTips)
//			{
//				mControlTips.SetActive(true);
//			}
//		}
//	}
//}
