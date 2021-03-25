using Loki;
using UnityEngine;

namespace Ubtrobot.KeyFeedbackCommands
{
	public abstract class KeyFeedbackCommand : Command
	{
		public override ECommand commandID { get { return ECommand.KeyFeedbackCommand; } }

	}

	/// <summary>
	/// 要求获得按键反馈
	/// </summary>
	public class KeyPressedFeedbackCommand : KeyFeedbackCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<KeyPressedFeedbackCommand>.defaultInstance.Push(this);
		}

		public static KeyPressedFeedbackCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<KeyPressedFeedbackCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new KeyPressedFeedbackCommand();
				


				return cmd;
			});
		}
	}
}
