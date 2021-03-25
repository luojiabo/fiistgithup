using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public class HierarchyUtility
	{
		public static void CopyHierarchy(Transform transform)
		{
			LitJson.JsonData hierarchy = new LitJson.JsonData("Hierarchy");


			GUIUtility.systemCopyBuffer = hierarchy.ToJson();
		}

		public static void PasteHierarchy(Transform transform)
		{
			if (string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
			{
				return;
			}
		}
	}
}
