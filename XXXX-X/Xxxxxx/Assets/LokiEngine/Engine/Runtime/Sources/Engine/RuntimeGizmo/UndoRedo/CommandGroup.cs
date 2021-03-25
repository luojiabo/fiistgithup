using System;
using System.Collections.Generic;

namespace CommandUndoRedo
{
	public class CommandGroup : ICommand
	{
		private List<ICommand> mCommands = new List<ICommand>();

		public CommandGroup() { }
		public CommandGroup(List<ICommand> commands)
		{
			this.mCommands.AddRange(commands);
		}

		public void Set(List<ICommand> commands)
		{
			this.mCommands = commands;
		}

		public void Add(ICommand command)
		{
			mCommands.Add(command);
		}

		public void Remove(ICommand command)
		{
			mCommands.Remove(command);
		}

		public void Clear()
		{
			mCommands.Clear();
		}

		public void Execute()
		{
			for (int i = 0; i < mCommands.Count; i++)
			{
				mCommands[i].Execute();
			}
		}

		public void UnExecute()
		{
			for (int i = mCommands.Count - 1; i >= 0; i--)
			{
				mCommands[i].UnExecute();
			}
		}
	}
}
