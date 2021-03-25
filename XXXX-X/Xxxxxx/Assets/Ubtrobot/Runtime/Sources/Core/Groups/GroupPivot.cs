using System;
using System.Collections.Generic;
using Loki;

using UnityEngine;

namespace Ubtrobot
{
	public class GroupPivot : UComponent
	{
		public Space space = Space.Self;

		public AxisType originUpAxis = AxisType.Y;
		public AxisType originForwardAxis = AxisType.Z;
		public AxisType targetUpAxis = AxisType.Y;
		public AxisType targetForwardAxis = AxisType.Z;

		private void Reset()
		{
			if (transform.parent == null)
			{
				// name = "Pivot";
			}
			else
			{
				name = transform.parent.name + "_Pivot";
			}
		}

		private void OnDrawGizmos()
		{
			GizmosColorUtility.NewStack();
			//Gizmos.DrawSphere(transform.position, 0.02f);
			if (space == Space.Self)
			{
				GizmosUtility.DrawArrow(transform.position, transform.forward, Color.blue);
				GizmosUtility.DrawArrow(transform.position, transform.right, Color.red);
				GizmosUtility.DrawArrow(transform.position, transform.up, Color.green);
			}
			else
			{
				GizmosUtility.DrawArrow(transform.position, Vector3.forward, Color.blue);
				GizmosUtility.DrawArrow(transform.position, Vector3.right, Color.red);
				GizmosUtility.DrawArrow(transform.position, Vector3.up, Color.green);
			}
			GizmosColorUtility.Revert();
		}

#if UNITY_EDITOR
		[InspectorMethod(aliasName = "以此作为轴心")]
		public void SetAsPivot()
		{
			Group group = GetComponent<Group>() ?? GetComponentInParent<Group>();
			group.transform.ExeIgnoreChildren(tr =>
			{
				tr.CopyFrom(transform, Space.World);
			});

			//List<Transform> children = new List<Transform>();
			//for (int i = 0; i < group.transform.childCount; ++i)
			//{
			//	children.Add(group.transform.GetChild(i));
			//}
			//group.transform.DetachChildren();
			//group.transform.CopyFrom(transform, Space.World);
			//foreach (var tr in children)
			//{
			//	tr.transform.SetParent(group.transform, true);
			//}
		}

		[InspectorMethod(aliasName = "提取作为根直接子节点")]
		private void ExtractAsRootsChild()
		{
			if (transform.parent != null)
				transform.SetParent(transform.root);
		}

		[InspectorMethod(aliasName = "提取作为组的子节点")]
		private void ExtractAsGroupChild()
		{
			Group group = GetComponentInParent<Group>();
			if (group != null)
				transform.SetParent(group.transform);
		}

		[InspectorMethod(aliasName = "整体移动到原点")]
		private void MoveToZero()
		{
			if (transform.root != null)
				transform.root.position = Vector3.zero;
		}

		[InspectorMethod(aliasName = "变换到标准空间")]
		private void ConvertSpace()
		{

		}
#endif
	}
}
