using Loki;

namespace Ubtrobot.MiscCommands
{
	public abstract class MiscCommand : Command
	{
		public override ECommand commandID { get { return ECommand.MiscCommand; } }
	}

	public class SetIDCommand : MiscCommand
	{
		/// <summary>
		/// 1：舵机
		/// 2：电机
		/// 3：红外
		/// 4：超声波
		/// 5：眼灯
		/// 6：触碰
		/// 7：亮度
		/// 8：声音
		/// 9：温湿度
		/// 10：颜色
		/// </summary>
		public int targetDevice { get; set; }

		public int oldID { get; set; }

		public int newID { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<SetIDCommand>.defaultInstance.Push(this);
		}

		public static SetIDCommand New(int targetDevice, int oldID, int newID)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<SetIDCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new SetIDCommand();

				cmd.targetDevice = targetDevice;
				cmd.oldID = oldID;
				cmd.newID = newID;
				return cmd;
			});
		}
	}

	public class QueryDeviceCommand : MiscCommand
	{
		/// <summary>
		/// 1：舵机
		/// 2：电机
		/// 3：红外
		/// 4：超声波
		/// 5：眼灯
		/// 6：触碰
		/// 7：亮度
		/// 8：声音
		/// 9：温湿度
		/// 10：颜色
		/// </summary>
		public int targetDevice { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<QueryDeviceCommand>.defaultInstance.Push(this);
		}

		public static QueryDeviceCommand New(int targetDevice)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<QueryDeviceCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new QueryDeviceCommand();

				cmd.targetDevice = targetDevice;
				return cmd;
			});
		}
	}

	public class QueryDeviceUpgradableCommand : MiscCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<QueryDeviceUpgradableCommand>.defaultInstance.Push(this);
		}

		public static QueryDeviceUpgradableCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<QueryDeviceUpgradableCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new QueryDeviceUpgradableCommand();

				return cmd;
			});
		}
	}

	public class StopAllDevicesCommand : MiscCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<StopAllDevicesCommand>.defaultInstance.Push(this);
		}

		public static StopAllDevicesCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<StopAllDevicesCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new StopAllDevicesCommand();

				return cmd;
			});
		}
	}

	public class GetVersionCommand : MiscCommand
	{
		/// <summary>
		/// 1：舵机
		/// 2：电机
		/// 3：红外
		/// 4：超声波
		/// 5：眼灯
		/// 6：触碰
		/// 7：亮度
		/// 8：声音
		/// 9：温湿度
		/// 10：颜色
		/// </summary>
		public int targetDevice { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<GetVersionCommand>.defaultInstance.Push(this);
		}

		public static GetVersionCommand New(int targetDevice)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<GetVersionCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new GetVersionCommand();

				cmd.targetDevice = targetDevice;
				return cmd;
			});
		}
	}

	public class StartUpgradeCommand : MiscCommand
	{
		/// <summary>
		/// 1：舵机
		/// 2：电机
		/// 3：红外
		/// 4：超声波
		/// 5：眼灯
		/// 6：触碰
		/// 7：亮度
		/// 8：声音
		/// 9：温湿度
		/// 10：颜色
		/// </summary>
		public int targetDevice { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<StartUpgradeCommand>.defaultInstance.Push(this);
		}

		public static StartUpgradeCommand New(int targetDevice)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<StartUpgradeCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new StartUpgradeCommand();

				cmd.targetDevice = targetDevice;
				return cmd;
			});
		}
	}

	public class TickUpgradeCommand : MiscCommand
	{
		/// <summary>
		/// 1：舵机
		/// 2：电机
		/// 3：红外
		/// 4：超声波
		/// 5：眼灯
		/// 6：触碰
		/// 7：亮度
		/// 8：声音
		/// 9：温湿度
		/// 10：颜色
		/// </summary>
		public int targetDevice { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<TickUpgradeCommand>.defaultInstance.Push(this);
		}

		public static TickUpgradeCommand New(int targetDevice)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<TickUpgradeCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new TickUpgradeCommand();

				cmd.targetDevice = targetDevice;
				return cmd;
			});
		}
	}

	public class EndUpgradeCommand : MiscCommand
	{
		/// <summary>
		/// 1：舵机
		/// 2：电机
		/// 3：红外
		/// 4：超声波
		/// 5：眼灯
		/// 6：触碰
		/// 7：亮度
		/// 8：声音
		/// 9：温湿度
		/// 10：颜色
		/// </summary>
		public int targetDevice { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<EndUpgradeCommand>.defaultInstance.Push(this);
		}

		public static EndUpgradeCommand New(int targetDevice)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<EndUpgradeCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new EndUpgradeCommand();

				cmd.targetDevice = targetDevice;
				return cmd;
			});
		}
	}
}
