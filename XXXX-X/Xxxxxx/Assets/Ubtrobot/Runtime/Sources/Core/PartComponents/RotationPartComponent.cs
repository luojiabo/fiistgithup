using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	public enum ERotationWrapMode
	{
		Clamp,
		Loop,
	}

	[DisallowMultipleComponent]
	public abstract class RotationPartComponent : AdvancedPartComponent
	{
		[SerializeField, AutoSerializeField("DP_Axis")]
		protected AxleConnectivity mAxisDP = null;

		[SerializeField, HideInInspector]
		protected float mMinEularAngleY = -180.0f;

		[SerializeField, HideInInspector]
		protected float mMaxEularAngleY = 180.0f;

		[SerializeField, HideInInspector]
		protected ERotationWrapMode mWrapMode = ERotationWrapMode.Loop;

		[SerializeField, HideInInspector]
		protected EEulerAngleConvertMode mEulerConvertMode = EEulerAngleConvertMode.N0P360;

		protected float mInitLocalEulerAnglesY = 0.0f;

		public AxleConnectivity axisDP
		{
			get
			{
				return mAxisDP;
			}
			set
			{
				mAxisDP = value;
			}
		}

		[PreviewMember]
		public EEulerAngleConvertMode convertMode
		{
			get
			{
				return mEulerConvertMode;
			}
			set
			{
				if (mEulerConvertMode != value)
				{
					mEulerConvertMode = value;
				}
			}
		}

		[ConsoleProperty(aliasName = "eularAngleInterval")]
		public float eularAngleInterval { get; set; } = 90.0f;

#if UNITY_EDITOR
		[PreviewMember]
		public bool enableInspectorUpdate { get; set; } = false;
#endif

		[PreviewMember]
		public ERotationWrapMode wrapMode
		{
			get { return mWrapMode; }
			set { mWrapMode = value; }
		}

		[PreviewMember(-360.0f, 360.0f)]
		public virtual float minEularAngleY
		{
			get
			{
				return mMinEularAngleY;
			}
			set
			{
				if (Misc.Nearly(mMinEularAngleY, value))
				{
					return;
				}
				mMinEularAngleY = Mathf.Min(value, mMaxEularAngleY);
				OnEualrRangeChanged();
			}
		}

		[PreviewMember(-360.0f, 360.0f)]
		public virtual float maxEularAngleY
		{
			get
			{
				return mMaxEularAngleY;
			}
			set
			{
				if (Misc.Nearly(mMaxEularAngleY, value))
				{
					return;
				}
				mMaxEularAngleY = Mathf.Max(value, mMinEularAngleY);
				OnEualrRangeChanged();
			}
		}

		//[PreviewMember]
		[PreviewMemberDynamicProperty("minEularAngleY", "maxEularAngleY")]
		public virtual float localEulerAnglesY
		{
			get
			{
				if (mAxisDP != null)
				{
					var angle = mAxisDP.localEulerAngles.y;
					angle = Misc.Convert(convertMode, angle);
					return angle;
				}
				return 0.0f;
			}
			set
			{
				float angle = value;// Misc.FixAngle(value);
				if (!Misc.Nearly(localEulerAnglesY, angle))
				{
					SetEularAngleY(angle);
				}
			}
		}

		public virtual float rotatedSpeed => 0;

		[PreviewMember]
		public bool enableKeyboardControl { get; set; } = false;

		public override void Rewind()
		{
			localEulerAnglesY = mInitLocalEulerAnglesY;
		}

		public void ForceUpdateID()
		{
			SetID(mPartID);
		}

		public void ForceUpdateRotation()
		{
			// SetEularAngleY(eularAngleY);
		}

		private string GetExternalDPName(int id)
		{
			return string.Concat("DP_Axis_", id.ToString());
		}

		private void UpdateExternalDP(int id)
		{
			if (mAxisDP != null && mAxisDP.joint != null)
			{
				string temp = GetExternalDPName(id);
				if (mAxisDP.joint.name != temp)
				{
					mAxisDP.joint.name = temp;
				}
			}
		}

		public override void SetID(int id)
		{
			base.SetID(id);
			UpdateExternalDP(id);
		}

		private void SetEularAngleY(float y)
		{
			Rotate(y - localEulerAnglesY);
		}

		public void Rotate(float angle)
		{
			CheckExternalDP();
			if (mAxisDP == null)
			{
				return;
			}

			switch (wrapMode)
			{
				case ERotationWrapMode.Clamp:
					{
						float current = localEulerAnglesY;
						float target = Mathf.Clamp(current + angle, minEularAngleY, maxEularAngleY);
						mAxisDP.Rotate(target - current);
						break;
					}
				case ERotationWrapMode.Loop:
					{
						mAxisDP.Rotate(angle);
						break;
					}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			CheckExternalDP();
			mInitLocalEulerAnglesY = localEulerAnglesY;
			//ForceUpdateID();
			//ForceUpdateRotation();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

		}

		private void OnEualrRangeChanged()
		{
			localEulerAnglesY = Mathf.Clamp(localEulerAnglesY, minEularAngleY, maxEularAngleY);
		}

		public void CheckExternalDP()
		{
			if (mAxisDP != null)
			{
				if (mAxisDP.joint != null)
				{
					if (mAxisDP.joint.controller != mAxisDP)
					{
						mAxisDP.joint = null;
					}
				}

				if (mAxisDP.joint == null)
				{
					var part = GetPart();
					if (part != null && part.robot != null)
					{
						var robotTr = part.robot.transform;
						string externalDPName = GetExternalDPName(id);

						Transform joint = robotTr.FindOrAdd(externalDPName);
						mAxisDP.joint = joint.GetOrAllocComponent<AxleJointComponent>();
						if (mAxisDP.joint.controller != null && mAxisDP.joint.controller != mAxisDP)
						{
							List<AxleJointComponent> jointComponents = new List<AxleJointComponent>();
							robotTr.GetComponentsInChildren(jointComponents);
							AxleJointComponent bestCom = null;
							foreach (var com in jointComponents)
							{
								if (com.controller == null && bestCom == null)
								{
									bestCom = com;
								}
								if ((com.name == externalDPName) && (com.controller == null || com.controller == mAxisDP))
								{
									bestCom = com;
									break;
								}
							}
							if (bestCom == null)
							{
								joint = robotTr.AddChild(externalDPName);
								bestCom = joint.GetOrAllocComponent<AxleJointComponent>();
							}
							mAxisDP.joint = bestCom;
						}
						mAxisDP.joint.controller = mAxisDP;
						mAxisDP.joint.transform.SetParent(robotTr);
						mAxisDP.joint.transform.CopyFrom(mAxisDP.transform, Space.World);
					}
				}
				// mAxisDP.Sync();
			}
		}

		protected override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);

			CheckExternalDP();
			if (enableKeyboardControl)
			{
				var inputSys = ModuleManager.Get().GetSystemChecked<InputSystem>();
				if (inputSys != null)
				{
					float offset = 0.0f;
					if (inputSys.GetKey(KeyCode.LeftArrow))
					{
						offset -= 1.0f;
					}
					if (inputSys.GetKey(KeyCode.RightArrow))
					{
						offset += 1.0f;
					}
					Rotate(offset * eularAngleInterval * Time.deltaTime);
				}
			}
		}


#if UNITY_EDITOR
		protected override void DoDrawGizmos()
		{
			var settings = UbtrobotSettings.GetOrLoad().gizmosSettings;
			GizmosUtility.DrawMeshs(gameObject, settings.drawServo, settings.servoColor);
		}

		protected override void OnEditorUpdate()
		{
			base.OnEditorUpdate();

			//Tick(0.033f);
		}

		public override void OnInspectorUpdate()
		{
			if (enableInspectorUpdate || !UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
			{
				base.OnInspectorUpdate();

				ForceUpdateID();
				ForceUpdateRotation();
			}
		}

		[InspectorMethod(aliasName = "CheckConnectivity")]
		public void CheckConnectivity()
		{
			CheckExternalDP();
		}
#endif
	}
}
