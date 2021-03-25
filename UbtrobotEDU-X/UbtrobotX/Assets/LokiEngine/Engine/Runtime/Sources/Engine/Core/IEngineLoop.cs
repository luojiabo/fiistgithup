using System.Collections;

namespace Loki
{
	internal interface IEngineLoop : System.IDisposable
	{
		bool initialized { get; }
		IEnumerator Initialize(UEngine engine);
		void FixedUpdate(float fixedDeltaTime);
		void Update(float deltaTime);
		void LateUpdate();
		void OnApplicationQuit();
		void ClearPendingCleanupObjects();
	}
}
