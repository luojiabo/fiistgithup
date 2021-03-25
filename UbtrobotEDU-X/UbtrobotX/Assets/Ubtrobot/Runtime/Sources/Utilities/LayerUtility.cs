using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ubtrobot
{
	public class LayerUtility
	{
		#region Layer Index
		public static readonly int DefaultLayer;
		public static readonly int TransparentFxLayer;
		public static readonly int IgnoreRaycastLayer;
		public static readonly int WaterLayer;
		public static readonly int UILayer;
		public static readonly int PartLayer;
		public static readonly int RobotLayer;
		public static readonly int InputLayer;
		public static readonly int WallLayer;
		public static readonly int TerrainLayer;
		public static readonly int TouchLayer;
		#endregion

		public static readonly int LineTraceColliderMask;

		public static int LayerToMask(int layerIndex)
		{
			return 1 << layerIndex;
		}

		public static int LayerToMask(params int[] layerIndices)
		{
			int result = 0;
			foreach (var idx in layerIndices)
			{
				result |= (1 << idx);
			}
			return result;
		}

		public static bool CompareLayer(Component component, int layer)
		{
			if (component && component.gameObject && component.gameObject.layer == layer)
			{
				return true;
			}
			return false;
		}

		public static bool CompareLayer(RaycastHit hit, int layer)
		{
			return CompareLayer(hit.transform, layer);
		}

		public static bool CompareLayer(GameObject gameObject, int layer)
		{
			if (gameObject && gameObject.layer == layer)
			{
				return true;
			}
			return false;
		}

		static LayerUtility()
		{
			DefaultLayer = LayerMask.NameToLayer("Default");
			TransparentFxLayer = LayerMask.NameToLayer("TransparentFx");
			IgnoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
			WaterLayer = LayerMask.NameToLayer("Water");
			UILayer = LayerMask.NameToLayer("UI");
			PartLayer = LayerMask.NameToLayer("Part");
			TouchLayer = LayerMask.NameToLayer("Touch");
			InputLayer = LayerMask.NameToLayer("Input");
			RobotLayer = LayerMask.NameToLayer("Robot");
			WallLayer = LayerMask.NameToLayer("Wall");
			TerrainLayer = LayerMask.NameToLayer("Terrain");

			LineTraceColliderMask = LayerToMask(WallLayer);
		}
	}
}
