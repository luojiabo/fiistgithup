using Loki;
using UnityEngine;

namespace Ubtrobot.RGBLEDCommands
{
	public abstract class RGBLedCommand : Command
	{
		public override ECommand commandID { get { return ECommand.RGBLedCommand; } }
	}

	public class EnableRGBLedCommand : RGBLedCommand
	{
		public Color color { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<EnableRGBLedCommand>.defaultInstance.Push(this);
		}

		public static EnableRGBLedCommand New(Color color)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<EnableRGBLedCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new EnableRGBLedCommand();

				cmd.color = color;
				return cmd;
			});
		}
	}

	public class DisableRGBLedCommand : RGBLedCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<DisableRGBLedCommand>.defaultInstance.Push(this);
		}

		public static DisableRGBLedCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<DisableRGBLedCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new DisableRGBLedCommand();

				return cmd;
			});
		}
	}
}
