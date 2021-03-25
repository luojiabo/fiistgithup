using UnityEngine;

namespace Loki
{
	public interface ICameraController
	{
		int postProcessingLayerMask { get; set; }
		string name { get; set; }
		Transform transform { get; }
		bool active { get; set; }
		Camera controlled { get; }
		Ray ScreenPointToRay(Vector3 screenPosition);
		bool CompareTag(string tag);
	}
}
