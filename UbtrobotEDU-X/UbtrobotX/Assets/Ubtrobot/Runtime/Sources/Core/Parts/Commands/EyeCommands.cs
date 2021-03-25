using Loki;
using UnityEngine;

namespace Ubtrobot.EyeCommands
{
	public abstract class EyeCommand : Command
	{
		public override ECommand commandID { get { return ECommand.EyeCommand; } }

	}

	public class EnableLightCommand : EyeCommand
	{
		public Color color { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<EnableLightCommand>.defaultInstance.Push(this);
		}

		public static EnableLightCommand New(Color color)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<EnableLightCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new EnableLightCommand();
				

				cmd.color = color;
				return cmd;
			});
		}
	}

	public class DisableLightCommand : EyeCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<DisableLightCommand>.defaultInstance.Push(this);
		}

		public static DisableLightCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<DisableLightCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new DisableLightCommand();
				
				return cmd;
			});
		}
	}

	public class CustomLightCommand : EyeCommand
	{
		/// <summary>
		/// The color Indices [1, 2, 3, 4, 5, 6, 7, 8]
		/// </summary>
		public int[] colors { get; private set; }
		/// <summary>
		/// Seconds
		/// </summary>
		public float duration { get; private set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<CustomLightCommand>.defaultInstance.Push(this);
		}

		public static CustomLightCommand New(float duration, int color0, int color1, int color2, int color3, int color4, int color5, int color6, int color7)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<CustomLightCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
				{
					cmd = new CustomLightCommand();
					cmd.colors = new int[8];
				}
				cmd.duration = duration;
				cmd.colors[0] = color0;
				cmd.colors[1] = color1;
				cmd.colors[2] = color2;
				cmd.colors[3] = color3;
				cmd.colors[4] = color4;
				cmd.colors[5] = color5;
				cmd.colors[6] = color6;
				cmd.colors[7] = color7;
				return cmd;
			});
		}
	}

	public class ExpressionCommand : EyeCommand
	{
		/// <summary>
		/// 0：眨眼
		/// 1：害羞
		/// 2：热泪盈眶
		/// 3：泪光闪动
		/// 4：哭泣
		/// 5：晕 
		/// 6：开心
		/// 7：惊讶
		/// 8：呼吸
		/// 9：闪烁
		/// 10：风扇
		/// 11：雨刮
		/// </summary>
		public int mode { get; set; }

		public Color color { get; set; }

		public int count { get; set; }

		public bool blockFlow { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<ExpressionCommand>.defaultInstance.Push(this);
		}

		public static ExpressionCommand New(int exp, Color color, int count, bool blockFlow)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<ExpressionCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new ExpressionCommand();

				

				cmd.mode = exp;
				cmd.color = color;
				cmd.count = count;
				cmd.blockFlow = blockFlow;
				return cmd;
			});
		}
	}

	public class PredefineLightColorCommand : EyeCommand
	{
		/// <summary>
		/// 0：七彩跑马灯
		/// 1：Disco
		/// 2：三原色
		/// 3：颜色堆叠
		/// </summary>
		public int mode { get; set; }

		public int count { get; set; }

		public bool blockFlow { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<PredefineLightColorCommand>.defaultInstance.Push(this);
		}

		public static PredefineLightColorCommand New(int mode, int count, bool blockFlow)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<PredefineLightColorCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new PredefineLightColorCommand();

				


				cmd.mode = mode;
				cmd.count = count;
				cmd.blockFlow = blockFlow;
				return cmd;
			});
		}
	}

}
