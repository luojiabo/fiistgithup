using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public class OBBCollider : Collider
	{
		[SerializeField]
		private Vector3 m_Center;

		[SerializeField]
		private Vector3 m_Extend;

		public Vector3 center
		{
			get => m_Center;
			set
			{
				if (m_Center != value)
				{
					m_Center = value;

				}
			}
		}

		public Vector3 extend
		{
			get => m_Extend;
			set
			{
				if (m_Extend != value)
				{
					m_Extend = value;

				}
			}
		}
	}
}
