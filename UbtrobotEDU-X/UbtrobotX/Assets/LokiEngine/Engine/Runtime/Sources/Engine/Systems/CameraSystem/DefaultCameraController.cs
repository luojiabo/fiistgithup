using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	/// <summary>
	/// Use DefaultCameraController by attach component to GameObject
	/// </summary>
	public class DefaultCameraController : CameraController, IUpdatable
	{
		public const float MaxRotationSpeed = 360.0f;
		public const float MaxMovementSpeed = 1000.0f;

		[SerializeField, HideInInspector]
		private float mRotationSpeed = 30.0f;
		[SerializeField, HideInInspector]
		private float mMoveSpeed = 3.0f;
		[SerializeField, HideInInspector]
		private float mShiftMoveSpeedAddition = 30.0f;
		[SerializeField, HideInInspector]
		private float mScrollValueFactory = 30.0f;
		[SerializeField, HideInInspector]
		private Transform mMovementRoot = null;
		[SerializeField, HideInInspector]
		private Transform mRotationRoot = null;
		[SerializeField, HideInInspector]
		private bool mEnableTRS = true;

		public CameraZone cameraZone;

		private Vector3 mStartPosition;
		private Quaternion mStartRotation;

		[PreviewMember]
		public bool enableTRS
		{
			get { return mEnableTRS; }
			set { mEnableTRS = value; }
		}

#if UNITY_EDITOR
		[PreviewMember]
		public Color gizmosColor { get; set; } = Color.yellow;
#endif

		[PreviewMember(0.0f, MaxRotationSpeed)]
		public float rotationSpeed
		{
			get
			{
				return mRotationSpeed;
			}
			set
			{
				mRotationSpeed = value;
			}
		}

		[PreviewMember(0.0f, MaxMovementSpeed)]
		public float moveSpeed
		{
			get
			{
				return mMoveSpeed;
			}
			set
			{
				mMoveSpeed = value;
			}
		}

		[PreviewMember(0.0f, MaxMovementSpeed)]
		public float shiftMoveSpeedAddition
		{
			get
			{
				return mShiftMoveSpeedAddition;
			}
			set
			{
				mShiftMoveSpeedAddition = value;
			}
		}

		[PreviewMember(0.0f, 10.0f)]
		public float scrollValueFactory
		{
			get
			{
				return mScrollValueFactory;
			}
			set
			{
				mScrollValueFactory = value;
			}
		}

		[PreviewMember]
		public Transform movementRoot
		{
			get
			{
				return mMovementRoot != null ? mMovementRoot : this.transform;
			}
			set
			{
				mMovementRoot = value;
			}
		}

		[PreviewMember]
		public Transform rotationRoot
		{
			get
			{
				return mRotationRoot != null ? mRotationRoot : this.transform;
			}
			set
			{
				mRotationRoot = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			mStartPosition = transform.position;
			mStartRotation = transform.rotation;
		}

		private InputSystem GetInputSystem()
		{
			var inputSys = ModuleManager.Get().GetSystemChecked<InputSystem>();
			return inputSys;
		}

		public void Move(float h, float v, float y, float speed, float deltaTime)
		{
			h *= speed * deltaTime;
			v *= speed * deltaTime;
			y *= speed * deltaTime;
			var translation = h * transform.right + v * transform.forward + y * transform.up;
			if (cameraZone != null)
			{
				cameraZone.Move(movementRoot, translation);
			}
			else
			{
				movementRoot.position += translation;
			}
		}

		private float AddCameraAngle(float angle, float addition)
		{
			if (Misc.Nearly(angle, 0.0f))
			{
				if (addition < 0.0f)
				{
					angle = 360.0f;
				}
				
			}
			else if (Misc.Nearly(angle, 360.0f))
			{
				if (addition > 0.0f)
				{
					angle = 0.0f;
				}
			}

			if (angle >= 0.0f && angle <= 90)
			{
				angle = Mathf.Clamp(angle + addition, 0.0f, 89.0f);
			}
			else if (angle >= 270.0f && angle <= 360.0f)
			{
				angle = Mathf.Clamp(angle + addition, 271.0f, 360.0f);
			}
			else
			{
				
			}
			return angle;
		}

		public void Rotate(float h, float v, float deltaTime)
		{
			h *= rotationSpeed * deltaTime;
			v *= -rotationSpeed * deltaTime;
			var eulerAngles = rotationRoot.eulerAngles;
			eulerAngles += new Vector3(0, h, 0.0f);
			eulerAngles.x = AddCameraAngle(eulerAngles.x, v);
			eulerAngles.z = 0.0f;
			eulerAngles = Misc.FixAngle(eulerAngles);
			rotationRoot.eulerAngles = eulerAngles;
		}

		public void OnUpdate(float deltaTime)
		{
			var inputSystem = GetInputSystem();
			if (inputSystem == null)
				return;

			if (inputSystem.GetKey(KeyCode.Home))
			{
				transform.position = mStartPosition;
				transform.rotation = mStartRotation;
				return;
			}

			if (enableTRS)
			{
				float horizontalValue = inputSystem.GetAsix(DefaultInputSystemConfig.LeftHorizontal);
				float verticalValue = inputSystem.GetAsix(DefaultInputSystemConfig.LeftVertical);

				bool mb1 = inputSystem.GetMouseButton(1);

				float lbValue = inputSystem.GetButtonAsValue(mb1, DefaultInputSystemConfig.LB, -1.0f);
				float rbValue = inputSystem.GetButtonAsValue(mb1, DefaultInputSystemConfig.RB, 1.0f);

				float addition = 0.0f;
				if (inputSystem.GetKey(KeyCode.LeftShift))
				{
					addition = shiftMoveSpeedAddition;
				}

				float scroll = inputSystem.GetMouseScrollY();
				if (Mathf.Abs(scroll) >= 0.1f)
				{
					scroll *= scrollValueFactory;
					verticalValue = scroll;
				}

				//DebugUtility.Log(LoggerTags.cameraSystem, "Move ({0},{1})", horizontalValue, verticalValue);
				Move(horizontalValue, verticalValue, lbValue + rbValue, moveSpeed + addition, deltaTime);

				if (mb1)
				{
					float rotationXValue = inputSystem.GetAsix(DefaultInputSystemConfig.RightHorizontal);
					float rotationYValue = inputSystem.GetAsix(DefaultInputSystemConfig.RightVertical);
					//DebugUtility.Log(LoggerTags.cameraSystem, "Rotation ({0},{1})", rotationXValue, rotationYValue);
					Rotate(rotationXValue, rotationYValue, deltaTime);
				}
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Color tempColor = Gizmos.color;
			Matrix4x4 tempMat = Gizmos.matrix;
			Camera c = controlled;
			Gizmos.color = gizmosColor;

			if (c.orthographic)
			{
				var size = c.orthographicSize;
				Gizmos.DrawWireCube(Vector3.forward * (c.nearClipPlane + (c.farClipPlane - c.nearClipPlane) / 2)
					, new Vector3(size * 2.0f, size * 2.0f * c.aspect, c.farClipPlane - c.nearClipPlane));
			}
			else
			{
				Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
				Gizmos.DrawFrustum(Vector3.zero, c.fieldOfView, c.farClipPlane, c.nearClipPlane, c.aspect);
			}

			Gizmos.color = tempColor;
			Gizmos.matrix = tempMat;
		}
#endif
	}
}
