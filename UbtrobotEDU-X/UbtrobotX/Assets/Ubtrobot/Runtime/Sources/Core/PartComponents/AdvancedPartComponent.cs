using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	[DisallowMultipleComponent]
	[DebuggerDisplay("{GetType()} ID={id}")]
	public abstract class AdvancedPartComponent : PartComponent, IPartIDComponent, ICommandHandler
	{
		protected MeshRenderer mNum1 = null;
		protected MeshRenderer mNum2 = null;

		[SerializeField, HideInInspector]
		protected int mPartID;

		[PreviewMember]
		public MeshRenderer num1
		{
			get
			{
				CheckID();
				return mNum1;
			}
		}

		[PreviewMember]
		public MeshRenderer num2
		{
			get
			{
				CheckID();
				return mNum2;
			}
		}

		[PreviewMember(0, 99)]
		public int id
		{
			get
			{
				return mPartID;
			}
			set
			{
				if (mPartID != value)
				{
					SetID(value);
				}
			}
		}

		[PreviewMember]
		public virtual DeviceType deviceID { get; } = DeviceType.None;

		[PreviewMember]
		public virtual DriversType driversType { get; } = DriversType.None;

		[PreviewMember]
		public ECommandType commandType { get; set; } = ECommandType.Unicast;

		/// <summary>
		/// 前向
		/// </summary>
		public Vector3 forward
		{
			get
			{
				return transform.forward;
			}
		}

		protected void Update()
		{
			Tick(Time.deltaTime);
		}

		private void CheckID()
		{
			if (mNum1 == null || mNum2 == null)
			{
				Transform trID = GetPart().transform.FindRecursion("ID");
				if (trID != null)
				{
					mNum1 = trID.GetComponent<MeshRenderer>("Num1");
					mNum2 = trID.GetComponent<MeshRenderer>("Num2");
				}
			}
		}

#if UNITY_EDITOR
		protected string DynamicDrawerName(string title)
		{
			return string.Concat(title, "(ID = ", id.ToString(), ")");
		}
#endif
		public virtual void SetID(int id)
		{
			mPartID = id;
			CheckID();

			if (mNum1 != null && mNum2 != null)
			{
				UbtrobotSettings.GetOrLoad().numMaterials.GetIDMaterial(id, out var _10, out var _1);
				mNum1.sharedMaterial = _10;
				mNum2.sharedMaterial = _1;

				var localPos1 = mNum1.transform.localPosition;
				var localPos2 = mNum2.transform.localPosition;

				if (localPos2.y - localPos1.y < 0.001f || localPos2.y - localPos1.y > 0.0015f)
				{
					localPos2.y = localPos1.y + 0.0011f;
					mNum2.transform.localPosition = localPos2;
				}
			}
		}

		protected virtual void Tick(float deltaTime)
		{

		}

		public abstract ICommandResponseAsync Execute(ICommand command);

		public abstract bool Verify(ICommand command);

		public virtual void Stop()
		{

		}
	}
}
