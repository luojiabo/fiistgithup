using DG.Tweening;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
	public class RobotVehicle : RobotPredefined
	{
		[SerializeField] private RotationPartComponent[] mLeftRotationParts;
		[SerializeField] private RotationPartComponent[] mRightRotationParts;
		[SerializeField, Range(0, 1.0f)] private float mMoveFactor = 0.02f;
		[SerializeField, Range(0.0f, 1000.0f)] private float mRotationSpeed = 10.0f;
		[SerializeField, Range(0.1f, 360.0f)] private float mAngularVelocity = 30.0f;
		/// <summary>
		/// 自身转速：为正数效果朝前，负数效果向后
		/// </summary>
		private float mVehRotatedSpeed;
		/// <summary>
		/// 移动速度：为正数效果朝前，负数效果向后
		/// </summary>
		private float mMoveVelocity;
		/// <summary>
		/// 左边转动速度：为正数效果朝前，负数效果向后
		/// </summary>
		private float mLeftRotatedSpeed;
		/// <summary>
		/// 右边转动速度：为正数效果朝前，负数效果向后
		/// </summary>
		private float mRightRotatedSpeed;
		/// <summary>
		/// 两组转动零件之间的间隔
		/// </summary>
		private float m_LR_Width;

		#region 做圆周运动时的变量
		private float mRadius;
		private Vector3 mCenter;
		private float mOmga;
		#endregion

		private Transform mPivot;
		private Rigidbody mRigidbody;
		private TweenProperty mRotationTween;

		public  RotationPartComponent[] leftRotationParts { get => mLeftRotationParts; set => mLeftRotationParts = value; }
		public  RotationPartComponent[] rightRotationParts { get => mRightRotationParts; set => mRightRotationParts = value; }
		public float moveFactor { get => mMoveFactor; set => mMoveFactor = value; }
		public float rotationSpeed { get => mRotationSpeed; set => mRotationSpeed = value; }
		public float angularVelocity { get => mAngularVelocity; set => mAngularVelocity = value; }

		protected override void OnInitialize()
		{
			base.OnInitialize();
			mPivot = transform;
			mRigidbody = mPivot.GetComponent<Rigidbody>();
			mRigidbody.useGravity = false;
			m_LR_Width = CalculateLRWidth();
		}

		protected override void OnStop()
		{
			base.OnStop();
			mRadius = 0;
			mCenter = mPivot.position;
			mOmga = 0;
			mRigidbody.velocity = Vector3.zero;
			mVehRotatedSpeed = 0;
			mMoveVelocity = 0;
		}

		private void OnCollisionExit(Collision collision)
		{
			mLeftRotatedSpeed = 0;
			mRightRotatedSpeed = 0;
		}

		private void FixedUpdate()
		{
			if (Mathf.Approximately(mRadius, 0.0f))
			{
				if(!Mathf.Approximately(mVehRotatedSpeed, 0.0f))
				{
					var euler = mPivot.localEulerAngles;
					euler.y -= mVehRotatedSpeed;
					var tween = mPivot.DORotate(euler, Mathf.Abs(mVehRotatedSpeed) / mAngularVelocity);
					mRotationTween.SetTween(tween);
					// mPivot.localEulerAngles = euler;
				}
				else
				{
					mRotationTween.Stop();
				}
			}
			else
			{
				var entad = (mCenter - mPivot.position).normalized;
				mRigidbody.AddForce(entad * mOmga, ForceMode.Force);
				UpdateLookAt(mRigidbody.velocity);
			}
		}

		//[SerializeField, Range(-50.0f, 50.0f)]
		//private float left = 2;
		//[SerializeField, Range(-50.0f, 50.0f)]
		//private float right = 4;
		//private GameObject go;
		private void Update()
		{

			////Test
			//m_LR_Width = 2;
			//var leftRotatedSpeed = left;
			//var rightRotatedSpeed = right;

			if (m_LR_Width <= 0)
			{
				mRadius = 0;
				mCenter = mPivot.position;
				mOmga = 0;
				mRigidbody.velocity = Vector3.zero;
				mVehRotatedSpeed = 0;
				mMoveVelocity = 0;
				return;
			}

			var leftRotatedSpeed = -UpdateRotationPartsSpeed(mLeftRotationParts);
			var rightRotatedSpeed = UpdateRotationPartsSpeed(mRightRotationParts);
			if (Mathf.Approximately(leftRotatedSpeed, mLeftRotatedSpeed) && Mathf.Approximately(rightRotatedSpeed, mRightRotatedSpeed))
				return;
			mLeftRotatedSpeed = leftRotatedSpeed;
			mRightRotatedSpeed = rightRotatedSpeed;
			DebugUtility.Log(LoggerTags.Module, "mLeftRotatedSpeed:{0} mRightRotatedSpeed:{1}", mLeftRotatedSpeed, mRightRotatedSpeed);

			if (leftRotatedSpeed * rightRotatedSpeed <= 0)
			{
				//自身旋转
				mRadius = 0;
				mCenter = mPivot.position;
				mOmga = 0;
				mRigidbody.velocity = Vector3.zero;
				mVehRotatedSpeed = mRotationSpeed * (rightRotatedSpeed - leftRotatedSpeed) / 2.0f % 180.0f;
				mMoveVelocity = 0;
				DebugUtility.Log(LoggerTags.Module, "mRotatedSpeed:{0}", mVehRotatedSpeed);
			}
			else
			{
				//圆周运动或者直线运动
				mVehRotatedSpeed = 0;
				mMoveVelocity = mMoveFactor * (leftRotatedSpeed + rightRotatedSpeed) / 2;
				var absLeftVelocity = Mathf.Abs(leftRotatedSpeed);
				var absRightVelocity = Mathf.Abs(rightRotatedSpeed);
				if (absRightVelocity < absLeftVelocity)
				{
					mRadius = CalculateRadius(absRightVelocity, absLeftVelocity);
					var entad = mPivot.TransformDirection(Vector3.right) * mRadius;
					mCenter = entad + mPivot.position;
					mOmga = mMoveVelocity * mMoveVelocity / mRadius;
				}
				else if (absRightVelocity > absLeftVelocity)
				{
					mRadius = CalculateRadius(absLeftVelocity, absRightVelocity);
					var entad = mPivot.TransformDirection(Vector3.left) * mRadius;
					mCenter = entad + mPivot.position;
					mOmga = mMoveVelocity * mMoveVelocity / mRadius;
				}
				else
				{
					mRadius = 0;
					mCenter = Vector3.zero;
					mOmga = 0;
				}

				mRigidbody.velocity = mPivot.TransformDirection(Vector3.forward) * mMoveVelocity;
				UpdateLookAt(mRigidbody.velocity);
				DebugUtility.Log(LoggerTags.Module, "mCenter:{0} mRadius:{1} mMoveVelocity:{2}", mCenter, mRadius, mMoveVelocity);
			}
		}

		private float CalculateRadius(float minVelocity, float maxVelocity)
		{
			return m_LR_Width / (1 - minVelocity / maxVelocity);
		}

		private float CalculateLRWidth()
		{
			if (mLeftRotationParts == null || mRightRotationParts == null) return 0;
			if (mLeftRotationParts.Length <= 0 || mRightRotationParts.Length <= 0) return 0;

			var l = mLeftRotationParts[0];
			var r = mRightRotationParts[0];
			return (l.transform.position - r.transform.position).magnitude;
		}

		private void UpdateLookAt(Vector3 velocity)
		{
			if (mMoveVelocity > 0)
			{
				mRotationTween.Stop();
				mPivot.LookAt(velocity + mPivot.position, Vector3.up);
				//var tween = mPivot.DOLookAt(velocity + mPivot.position, Vector3.Angle(mPivot.transform.forward, velocity.normalized) / mAngularVelocity);
				//mRotationTween.SetTween(tween);
			}
			else
			{
				mRotationTween.Stop();
				mPivot.LookAt(-velocity + mPivot.position, Vector3.up);
				//var tween = mPivot.DOLookAt(-velocity + mPivot.position, Vector3.Angle(mPivot.transform.forward, velocity.normalized) / mAngularVelocity);
				//mRotationTween.SetTween(tween);
			}
		}

		private float UpdateRotationPartsSpeed(RotationPartComponent[] rotationParts)
		{
			if (rotationParts == null) return 0;
			var count = rotationParts.Length;

			var speed = 0f;
			for (int i = 0; i < count; i++)
			{
				var rotationPart = rotationParts[i];
				speed += rotationPart.rotatedSpeed;
			}

			return speed;
		}
	}
}
