using Loki;

namespace Ubtrobot.ServoCommands
{
	public abstract class ServoCommand : Command
	{
		public override ECommand commandID { get { return ECommand.ServoCommand; } }
	}

	/// <summary>
	/// 舵机设置 轮模式
	/// </summary>
	public sealed class RotationAlwaysCommand : ServoCommand
	{
		public float velocity { get; set; }

		/// <summary>
		/// true : forward
		/// otherwise : backward
		/// </summary>
		public bool forward { get; set; }

		public override string ToString()
		{
			return string.Format("ServoCommand-RotationAlwaysCommand. [id: {0}, device : {1}, velocity : {2}, forward : {3}]", id, device, velocity, forward);
		}

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<RotationAlwaysCommand>.defaultInstance.Push(this);
		}

		public static RotationAlwaysCommand New(int forward, float velocity)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<RotationAlwaysCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new RotationAlwaysCommand();

				cmd.forward = forward == 0;
				cmd.velocity = velocity;
				return cmd;
			});
		}
	}

	/// <summary>
	/// 舵机角模式-旋转到指定角度
	/// </summary>
	public sealed class RotateCommand : ServoCommand
	{
		/// <summary>
		/// -118.0f ~ 118.0f
		/// </summary>
		public float angel { get; set; }

		/// <summary>
		/// The duration (s)
		/// 0.1f ~ 5.0f
		/// </summary>
		public float duration { get; set; }

		public override string ToString()
		{
			return string.Format("ServoCommand-RotateCommand. [id: {0}, device : {1}, angel : {2}, duration : {3}]", id, device, angel, duration);
		}

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<RotateCommand>.defaultInstance.Push(this);
		}

		public static RotateCommand New(float angel, float duration)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<RotateCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new RotateCommand();

				cmd.angel = angel;
				cmd.duration = duration;
				return cmd;
			});
		}
	}

	/// <summary>
	/// 停止舵机
	/// </summary>
	public sealed class StopCommand : ServoCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<StopCommand>.defaultInstance.Push(this);
		}

		public static StopCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<StopCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new StopCommand();

				return cmd;
			});
		}
	}

	/// <summary>
	/// 读取旋转角
	/// </summary>
	public sealed class ReadRotationCommand : ServoCommand
	{
		/// <summary>
		/// allow rotation
		/// </summary>
		public bool enable { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<ReadRotationCommand>.defaultInstance.Push(this);
		}

		public static ReadRotationCommand New(int enable)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<ReadRotationCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new ReadRotationCommand();

				cmd.enable = enable == 0;
				return cmd;
			});
		}
	}
}
