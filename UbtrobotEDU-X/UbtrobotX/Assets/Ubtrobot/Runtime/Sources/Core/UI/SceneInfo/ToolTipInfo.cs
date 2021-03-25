using UnityEngine;
using UnityEngine.UI;

namespace Ubtrobot
{
	public struct ToolTipInfo
	{
		public Part part;
		public RectTransform root;
		public Text label;

		public void Release()
		{
			if (root != null)
			{
				UnityEngine.Object.Destroy(root.gameObject);
			}
			root = null;
		}
	}
}
