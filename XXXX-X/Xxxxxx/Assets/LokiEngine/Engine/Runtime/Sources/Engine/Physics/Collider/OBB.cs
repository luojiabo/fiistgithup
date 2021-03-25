using System;
using UnityEngine;

namespace Loki
{
	public struct OBB
	{
		private Matrix4x4 m_LocalToWorldMatrix;
		private VectorPoints m_Points;
		private Vector3 m_Size;

		public Vector3 this[int index]
		{
			get
			{
				return m_Points[index];
			}
		}

		public Matrix4x4 localToWorldMatrix
		{
			get
			{
				return m_LocalToWorldMatrix;
			}
			set
			{
				if (m_LocalToWorldMatrix != value)
				{
					m_LocalToWorldMatrix = value;
					ForceUpdate();
				}
			}
		}

		public Vector3 size
		{
			get
			{
				return m_Size;
			}
			set
			{
				if (m_Size != value)
				{
					m_Size = value;
					ForceUpdate();
				}
			}
		}

		public void ForceUpdate()
		{
			m_Points[0] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f));
			m_Points[1] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f));
			m_Points[2] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(size.x * 0.5f, size.y * 0.5f, -size.z * 0.5f));
			m_Points[3] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-size.x * 0.5f, size.y * 0.5f, -size.z * 0.5f));
			m_Points[4] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-size.x * 0.5f, -size.y * 0.5f, size.z * 0.5f));
			m_Points[5] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(size.x * 0.5f, -size.y * 0.5f, size.z * 0.5f));
			m_Points[6] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f));
			m_Points[7] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-size.x * 0.5f, size.y * 0.5f, size.z * 0.5f));
		}
	}
}
