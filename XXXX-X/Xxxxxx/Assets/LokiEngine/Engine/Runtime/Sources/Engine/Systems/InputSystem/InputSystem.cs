using System;
using System.Collections;
using UnityEngine;

namespace Loki
{
	public static class DefaultInputSystemConfig
	{
		public static readonly string LeftHorizontal = "LeftHorizontal";
		public static readonly string LeftVertical = "LeftVertical";
		public static readonly string RightHorizontal = "RightHorizontal";
		public static readonly string RightVertical = "RightVertical";
		public static readonly string Up = "Up";
		public static readonly string Down = "Down";
		public static readonly string Left = "Left";
		public static readonly string Right = "Right";
		public static readonly string ButtonA = "A";
		public static readonly string ButtonB = "B";
		public static readonly string ButtonX = "X";
		public static readonly string ButtonY = "Y";
		public static readonly string RB = "RB";
		public static readonly string RT = "RT";
		public static readonly string LB = "LB";
		public static readonly string LT = "LT";
		public static readonly string Back = "Back";
		public static readonly string Start = "Start";
		public static readonly string Lock = "Lock";
	}

	public class InputSystem : ISystem
	{
		private static readonly Type msType = typeof(InputSystem);

		public string systemName { get { return msType.Name; } }

		public IModuleInterface module { get; set; }

		public InputEventData currentEventData { get; set; }

		public float GetAsix(string asixName)
		{
			return Input.GetAxis(asixName);
		}

		public float GetMouseScrollY()
		{
			return Input.mouseScrollDelta.y;
		}

		public bool GetButton(string buttonName)
		{
			return Input.GetButton(buttonName);
		}

		public bool GetKey(KeyCode key)
		{
			return Input.GetKey(key);
		}

		public float GetButtonAsValue(string buttonName, float pressedValue, float releasedValue = 0.0f)
		{
			return GetButton(buttonName) ? pressedValue : releasedValue;
		}

		public float GetButtonAsValue(bool condition, string buttonName, float pressedValue, float releasedValue = 0.0f)
		{
			if (!condition)
			{
				return releasedValue;
			}
			return GetButton(buttonName) ? pressedValue : releasedValue;
		}

		public bool GetMouseButton(int button)
		{
			return Input.GetMouseButton(button);
		}

		public bool GetMouseButtonDown(int button)
		{
			return Input.GetMouseButtonDown(button);
		}

		public bool GetMouseButtonUp(int button)
		{
			return Input.GetMouseButtonUp(button);
		}

		/// <summary>
		/// The position of the touch/mouse(If the left mouse button pressed) in pixel coordinates.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool GetTouchPosition(out Vector3 position)
		{
			return GetTouchPosition(0, out position);
		}

		/// <summary>
		/// The position of the touch/mouse(If the left mouse button pressed) in pixel coordinates.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool GetCursorPosition(out Vector3 position)
		{
			return GetCursorPosition(0, out position);
		}

		/// <summary>
		/// The position of the touch/mouse(If the left mouse button pressed) in pixel coordinates.
		/// </summary>
		/// <param name="touchID"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool GetCursorPosition(int touchID, out Vector3 position)
		{
			if (Input.touchCount > touchID)
			{
				position = Input.GetTouch(touchID).position;
				return true;
			}

			position = Input.mousePosition;
			return true;
		}

		/// <summary>
		/// The position of the touch/mouse(If the left mouse button pressed) in pixel coordinates.
		/// </summary>
		/// <param name="touchID"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool GetTouchPosition(int touchID, out Vector3 position)
		{
			if (Input.touchCount > touchID)
			{
				position = Input.GetTouch(touchID).position;
				return true;
			}

			if (Input.GetMouseButton(0))
			{
				position = Input.mousePosition;
				return true;
			}

			position = Vector3.zero;
			return false;
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
