using Loki;
using UnityEngine;

namespace Ubtrobot.SoundCommands
{
	public abstract class SoundCommand : Command
	{
		public override ECommand commandID { get { return ECommand.SoundCommand; } }

	}

	/// <summary>
	/// 音调
	/// </summary>
	public class PlayPitchCommand : SoundCommand
	{
		public int pitch { get; set; }

		public float duration { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<PlayPitchCommand>.defaultInstance.Push(this);
		}

		public static PlayPitchCommand New(int pitch, float duration)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<PlayPitchCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new PlayPitchCommand();

				cmd.pitch = pitch;
				cmd.duration = duration;
				return cmd;
			});
		}
	}

	/// <summary>
	/// 频率
	/// </summary>
	public class PlayFrequencyCommand : SoundCommand
	{
		public int frequency { get; set; }

		public float duration { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<PlayFrequencyCommand>.defaultInstance.Push(this);
		}

		public static PlayFrequencyCommand New(int frequency, float duration)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<PlayFrequencyCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new PlayFrequencyCommand();

				cmd.frequency = frequency;
				cmd.duration = duration;
				return cmd;
			});
		}
	}

	/// <summary>
	/// 停止声音
	/// </summary>
	public class StopCommand : SoundCommand
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
}
