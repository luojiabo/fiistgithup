using System;
using UnityEngine;

namespace Loki
{
	public struct VectorPoints
	{
		private Vector3 m_Point0;
		private Vector3 m_Point1;
		private Vector3 m_Point2;
		private Vector3 m_Point3;
		private Vector3 m_Point4;
		private Vector3 m_Point5;
		private Vector3 m_Point6;
		private Vector3 m_Point7;

		public Vector3 this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return m_Point0;
					case 1:
						return m_Point1;
					case 2:
						return m_Point2;
					case 3:
						return m_Point3;
					case 4:
						return m_Point4;
					case 5:
						return m_Point5;
					case 6:
						return m_Point6;
					case 7:
						return m_Point7;
				}

				throw new IndexOutOfRangeException(string.Format("Index was out of bounds of array. The valid range is [0, 7], The Index is : {0}", index.ToString()));
			}
			set
			{
				switch (index)
				{
					case 0:
						m_Point0  = value;
						break;
					case 1:
						m_Point1 = value;
						break;
					case 2:
						m_Point2 = value;
						break;
					case 3:
						m_Point3 = value;
						break;
					case 4:
						m_Point4 = value;
						break;
					case 5:
						m_Point5 = value;
						break;
					case 6:
						m_Point6 = value;
						break;
					case 7:
						m_Point7 = value;
						break;
				}

				throw new IndexOutOfRangeException(string.Format("Index was out of bounds of array. The valid range is [0, 7], The Index is : {0}", index.ToString()));
			}
		}
	}
}
