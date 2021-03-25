using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class Rotation : MonoBehaviour
	{
		public float rotateSpeed = 0.0f;

		private void Start()
		{
			gameObject.SetActive(true);
		}

		private void Update()
		{
			if (rotateSpeed > 0.0f)
			{
				transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
			}
		}

	}
}
