using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	[CreateAssetMenu(menuName = "Loki/Configs/Project/Selection Settings", fileName = "SelectionSettings", order = -999)]
	public class SelectionSettings : UAssetObject
	{
		public GameObject translation;
		public GameObject rotation;
		public GameObject scale;

		public GameObject Translation()
		{
			if (translation == null)
				return null;
			var go = GameObject.Instantiate(translation);
			go.name = translation.name;
			return go;
		}

		public GameObject Rotation()
		{
			if (rotation == null)
				return null;
			var go = GameObject.Instantiate(rotation);
			go.name = rotation.name;
			return go;
		}

		public GameObject Scale()
		{
			if (scale == null)
				return null;
			var go = GameObject.Instantiate(scale);
			go.name = scale.name;
			return go;
		}
	}
}
