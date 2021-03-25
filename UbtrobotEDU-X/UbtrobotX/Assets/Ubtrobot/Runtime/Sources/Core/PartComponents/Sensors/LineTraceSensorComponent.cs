using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Loki;
using UnityEngine;
using UnityEngine.UI;
using LineRenderer = UnityEngine.LineRenderer;

namespace Ubtrobot
{
	public abstract class LineTraceSensorComponent : UKitComponent
	{
		/// <summary>
		/// 最大识别范围
		/// </summary>
		[SerializeField, HideInInspector]
		private float mExpectedRange = 100.0f;

		/// <summary>
		/// 线条
		/// </summary>
		private LineRenderer mLineTracer = null;
		/// <summary>
		/// 距离文字
		/// </summary>
		private TextMesh mDistanceText = null;

		/// <summary>
		/// 最大识别范围
		/// </summary>
		[PreviewMember]
		public float expectedRange
		{
			get { return mExpectedRange; }
			set { mExpectedRange = value; }
		}

		protected override void Awake()
		{
			base.Awake();
		}

		/// <summary>
		/// 使用射线获取距离
		/// </summary>
		/// <param name="distance">单位米</param>
		/// <returns></returns>
		public bool LineTrace(out float distance)
		{
			distance = 0.0f;
			if (Physics.Raycast(transform.position, forward, out var hitInfo, expectedRange, 
				LayerUtility.LineTraceColliderMask, QueryTriggerInteraction.UseGlobal))
			{
				distance = hitInfo.distance;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 使用射线获取距离
		/// </summary>
		/// <param name="distance">单位米</param>
		/// <param name="point">触碰点</param>
		/// <returns></returns>
		public bool LineTrace(out float distance, out Vector3 point)
		{
			distance = 0.0f;
			point = Vector3.zero;

			if (Physics.Raycast(transform.position, forward, out var hitInfo, expectedRange,
				LayerUtility.LineTraceColliderMask, QueryTriggerInteraction.UseGlobal))
			{
				distance = hitInfo.distance;
				point = hitInfo.point;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 使用射线获取距离以及颜色
		/// </summary>
		/// <param name="distance">单位米</param>
		/// <param name="rgb">触碰点RGB</param>
		/// <returns></returns>
		public bool LineTrace(out float distance, out Color rgb)
		{
			distance = 0.0f;
			rgb = Color.white;

			if (Physics.Raycast(transform.position, forward, out var hitInfo, expectedRange,
				LayerUtility.LineTraceColliderMask, QueryTriggerInteraction.UseGlobal))
			{
				distance = hitInfo.distance;
				return Misc.PickedColor(hitInfo, out rgb);
			}
			return false;
		}

		protected override void SetDebug(bool flag)
		{
			base.SetDebug(flag);
			var traceResult = LineTrace(out var distance);
			UpdateLineTracer(traceResult, distance);
			UpdateDistanceInfo(traceResult, distance);
		}

		private void UpdateDistanceInfo(bool traceResult, float distance)
		{
			if (mDistanceText == null)
			{
				var debugNode = GetDebugTransform();
				if (debugNode != null)
				{
					mDistanceText = debugNode.GetComponent<TextMesh>("DistanceText");
				}
			}

			if (mDistanceText != null)
			{
				if (!debug)
				{
					mDistanceText.gameObject.SetActive(false);
					return;
				}

				mDistanceText.gameObject.SetActive(true);
				if (traceResult)
				{
					mDistanceText.text = distance.ToString("#.##m");
					mDistanceText.color = Color.green;
				}
				else
				{
					mDistanceText.text = "(NAN)";
					mDistanceText.color = Color.red;
				}
				if (Camera.main != null)
				{
					Vector3 offset = transform.position - Camera.main.transform.position;
					mDistanceText.transform.LookAt(transform.position + offset.normalized);
				}
			}
		}

		private void UpdateLineTracer(bool traceResult, float distance)
		{
			if (mLineTracer == null)
			{
				var debugNode = GetDebugTransform();
				if (debugNode != null)
				{
					mLineTracer = debugNode.GetComponent<LineRenderer>("LineTracer");
				}
			}

			if (mLineTracer != null)
			{
				if (!debug)
				{
					mLineTracer.gameObject.SetActive(false);
					return;
				}

				mLineTracer.gameObject.SetActive(true);
				mLineTracer.useWorldSpace = true;

				if (mLineTracer.positionCount != 2 || !Application.isPlaying)
				{
					Vector3 pos = transform.position;
					mLineTracer.SetPositions(new Vector3[] { pos, pos + forward * 10.0f });
				}

				if (traceResult)
				{
					mLineTracer.SetPosition(0, transform.position);
					mLineTracer.SetPosition(1, transform.position + forward * distance);
					mLineTracer.startColor = Color.green;
					mLineTracer.endColor = Color.green;
				}
				else
				{
					mLineTracer.SetPosition(0, transform.position);
					mLineTracer.SetPosition(1, transform.position + forward * expectedRange);
					mLineTracer.startColor = Color.red;
					mLineTracer.endColor = Color.red;
				}
			}

		}

		protected override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (debug)
			{
				var traceResult = LineTrace(out var distance);
				UpdateLineTracer(traceResult, distance);
				UpdateDistanceInfo(traceResult, distance);
			}
		}

#if UNITY_EDITOR
		protected override void DoDrawGizmos()
		{
			base.DoDrawGizmos();
			if (LineTrace(out var distance))
			{
				Gizmos.color = Color.green;
				Gizmos.DrawRay(transform.position, forward * distance);
			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawRay(transform.position, forward * expectedRange);
			}
		}

		protected override void OnEditorUpdate()
		{
			base.OnEditorUpdate();

		}

		public override void OnInspectorUpdate()
		{
			base.OnInspectorUpdate();

		}
#endif
	}
}
