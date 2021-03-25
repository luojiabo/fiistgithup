using Loki;

namespace Ubtrobot.PatrolSensorCommands
{
	public abstract class PatrolSensorCommand : Command
	{
		public override ECommand commandID { get { return ECommand.PatrolSensorCommand; } }

	}

	public class PatrolSensorBasicCommand : PatrolSensorCommand
	{
		/// <summary>
		/// 0: 深
		/// 1: 浅
		/// </summary>
		public bool isGray { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<PatrolSensorBasicCommand>.defaultInstance.Push(this);
		}

		public static PatrolSensorBasicCommand New(int gray)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<PatrolSensorBasicCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new PatrolSensorBasicCommand();
				cmd.isGray = gray == 0;
				return cmd;
			});
		}
	}

}
