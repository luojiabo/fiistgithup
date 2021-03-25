using System;
using System.Collections.Generic;

namespace Loki
{
	
	public class UpdateHandler : IUpdatable, IFixedUpdatable, ILateUpdatable
	{
		private readonly TSafeForeachList<IUpdatable> mUpdatables = new TSafeForeachList<IUpdatable>();
		private readonly TSafeForeachList<IFixedUpdatable> mFixedUpdatables = new TSafeForeachList<IFixedUpdatable>();
		private readonly TSafeForeachList<ILateUpdatable> mLateUpdatables = new TSafeForeachList<ILateUpdatable>();

		public void Add(object updatabe)
		{
			if (updatabe is IFixedUpdatable)
			{
				AddFixedUpdatable((IFixedUpdatable)updatabe);
			}
			if (updatabe is IUpdatable)
			{
				AddUpdatable((IUpdatable)updatabe);
			}
			if (updatabe is ILateUpdatable)
			{
				AddLateUpdatable((ILateUpdatable)updatabe);
			}
		}

		public void Remove(object updatabe)
		{
			if (updatabe is IFixedUpdatable)
			{
				RemoveFixedUpdatable((IFixedUpdatable)updatabe);
			}
			if (updatabe is IUpdatable)
			{
				RemoveUpdatable((IUpdatable)updatabe);
			}
			if (updatabe is ILateUpdatable)
			{
				RemoveLateUpdatable((ILateUpdatable)updatabe);
			}
		}

		public void AddFixedUpdatable(IFixedUpdatable updatable)
		{
			if (updatable == null)
				return;
			mFixedUpdatables.Union(updatable);
		}

		public void AddUpdatable(IUpdatable updatable)
		{
			if (updatable == null)
				return;
			mUpdatables.Union(updatable);
		}

		public void AddLateUpdatable(ILateUpdatable updatable)
		{
			if (updatable == null)
				return;
			mLateUpdatables.Union(updatable);
		}

		public void RemoveFixedUpdatable(IFixedUpdatable updatable)
		{
			if (updatable == null)
				return;
			mFixedUpdatables.Remove(updatable);
		}

		public void RemoveUpdatable(IUpdatable updatable)
		{
			if (updatable == null)
				return;
			mUpdatables.Remove(updatable);
		}

		public void RemoveLateUpdatable(ILateUpdatable updatable)
		{
			if (updatable == null)
				return;
			mLateUpdatables.Remove(updatable);
		}

		public void OnFixedUpdate(float fixedDeltaTime)
		{
			if (mFixedUpdatables.BeginForeach())
			{
				int count = mFixedUpdatables.loopingCount;
				for (int i = 0; i < count; ++i)
				{
					mFixedUpdatables[i].OnFixedUpdate(fixedDeltaTime);
				}

				mFixedUpdatables.EndForeach();
			}
		}

		public void OnUpdate(float deltaTime)
		{
			if (mUpdatables.BeginForeach())
			{
				int count = mUpdatables.loopingCount;
				for (int i = 0; i < count; ++i)
				{
					mUpdatables[i].OnUpdate(deltaTime);
				}

				mUpdatables.EndForeach();
			}
		}

		public void OnLateUpdate()
		{
			if (mLateUpdatables.BeginForeach())
			{
				int count = mLateUpdatables.loopingCount;
				for (int i = 0; i < count; ++i)
				{
					mLateUpdatables[i].OnLateUpdate();
				}

				mLateUpdatables.EndForeach();
			}
		}
	}
}
