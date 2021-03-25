using Loki;
using UnityEngine;

namespace Ubtrobot.BatteryCommands
{
	public abstract class BatteryCommand : Command
	{
		public override ECommand commandID { get { return ECommand.BatteryCommand; } }
	}

	public class BatteryVoltageCommand : BatteryCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<BatteryVoltageCommand>.defaultInstance.Push(this);
		}

		public static BatteryVoltageCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<BatteryVoltageCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new BatteryVoltageCommand();

				
				return cmd;
			});
		}
	}
}
