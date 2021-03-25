using Loki;
using UnityEngine;

namespace Ubtrobot.UKitCommands
{
	/// <summary>
	/// UKit
	/// </summary>
	public abstract class UKitCommand : Command
	{
		public override ECommand commandID { get { return ECommand.UKitCommand; } }

	}

	/// <summary>
	/// 超声波
	/// </summary>
	public class UltrasonicCommand : UKitCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<UltrasonicCommand>.defaultInstance.Push(this);
		}

		public static UltrasonicCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<UltrasonicCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new UltrasonicCommand();

				return cmd;
			});
		}
	}

	/// <summary>
	/// 红外
	/// </summary>
	public class InfraredCommand : UKitCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<InfraredCommand>.defaultInstance.Push(this);
		}

		public static InfraredCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<InfraredCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new InfraredCommand();

				return cmd;
			});
		}
	}

	/// <summary>
	/// 按压
	/// </summary>
	public class ButtonCommand : UKitCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<ButtonCommand>.defaultInstance.Push(this);
		}

		public static ButtonCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<ButtonCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new ButtonCommand();

				return cmd;
			});
		}
	}

	/// <summary>
	/// 亮度
	/// </summary>
	public class LuminanceCommand : UKitCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<LuminanceCommand>.defaultInstance.Push(this);
		}

		public static LuminanceCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<LuminanceCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new LuminanceCommand();

				return cmd;
			});
		}
	}

	/// <summary>
	/// 声音
	/// </summary>
	public class SoundCommand : UKitCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<SoundCommand>.defaultInstance.Push(this);
		}

		public static SoundCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<SoundCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new SoundCommand();

				return cmd;
			});
		}
	}

	/// <summary>
	/// 温湿度
	/// </summary>
	public class TemperatureCommand : UKitCommand
	{
		/// <summary>
		/// 0: 摄氏度
		/// 1: 华氏度
		/// 2: 湿度
		/// </summary>
		public int mode { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<TemperatureCommand>.defaultInstance.Push(this);
		}

		public static TemperatureCommand New(int mode)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<TemperatureCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new TemperatureCommand();

				cmd.mode = mode;
				return cmd;
			});
		}
	}

	public class ColorRecognitionCommand : UKitCommand
	{
		/// <summary>
		/// 0：红色
		/// 1：绿色
		/// 2：蓝色
		/// 3：黄色
		/// 4：青色
		/// 5：紫色
		/// 6：橙色
		/// 7：黑色
		/// 8：白色
		/// 9：灰色
		/// <see cref="https://www.sojson.com/rgb.html"/>
		/// </summary>
		public int mode { get; set; }

		public Color color
		{
			get
			{
				switch (mode)
				{
					case 0:
						return Color.red;
					case 1:
						return Color.green;
					case 2:
						return Color.blue;
					case 3:
						return Color.yellow;
					case 4:
						return Color.cyan;
					case 5:
						return new Color(160 / 255.0f, 32 / 255.0f, 240 / 255.0f);
					case 6:
						return new Color(255 / 255.0f, 165 / 255.0f, 240 / 255.0f);
					case 7:
						return Color.black;
					case 8:
						return Color.white;
					case 9:
						return Color.gray;
				}
				return Color.white;
			}
		}

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<ColorRecognitionCommand>.defaultInstance.Push(this);
		}

		public static ColorRecognitionCommand New(int mode)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<ColorRecognitionCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new ColorRecognitionCommand();


				cmd.mode = mode;
				return cmd;
			});
		}
	}

	public class ColorRecognitionRGBCommand : UKitCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<ColorRecognitionRGBCommand>.defaultInstance.Push(this);
		}

		public static ColorRecognitionRGBCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<ColorRecognitionRGBCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new ColorRecognitionRGBCommand();

				return cmd;
			});
		}
	}

	/// <summary>
	/// 超声波灯光
	/// </summary>
	public class EnableUltrasonicLightCommand : UKitCommand
	{
		public Color color { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<EnableUltrasonicLightCommand>.defaultInstance.Push(this);
		}

		public static EnableUltrasonicLightCommand New(Color color)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<EnableUltrasonicLightCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new EnableUltrasonicLightCommand();

				cmd.color = color;
				return cmd;
			});
		}
	}

	/// <summary>
	/// 超声波灯光
	/// </summary>
	public class DisableUltrasonicLightCommand : UKitCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<DisableUltrasonicLightCommand>.defaultInstance.Push(this);
		}

		public static DisableUltrasonicLightCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<DisableUltrasonicLightCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new DisableUltrasonicLightCommand();

				return cmd;
			});
		}
	}

}
