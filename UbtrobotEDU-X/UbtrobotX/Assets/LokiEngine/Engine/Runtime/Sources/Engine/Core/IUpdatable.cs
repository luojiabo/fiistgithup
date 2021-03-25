namespace Loki
{
	public interface IFixedUpdatable
	{
		void OnFixedUpdate(float fixedDeltaTime);
	}

	public interface IUpdatable
	{
		void OnUpdate(float deltaTime);
	}

	public interface ILateUpdatable
	{
		void OnLateUpdate();
	}
}
