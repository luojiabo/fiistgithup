using Loki;

namespace Ubtrobot.GyroCommands
{
	public abstract class GyroCommand : Command
	{
		public override ECommand commandID { get { return ECommand.PatrolSensorCommand; } }
	}

	public class GyroFusionAngleCommand : GyroCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<GyroFusionAngleCommand>.defaultInstance.Push(this);
		}

		public static GyroFusionAngleCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<GyroFusionAngleCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new GyroFusionAngleCommand();
				

				return cmd;
			});
		}
	}
}
