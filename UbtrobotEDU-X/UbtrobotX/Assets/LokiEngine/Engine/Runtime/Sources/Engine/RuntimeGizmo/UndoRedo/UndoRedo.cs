using System;
using System.Collections.Generic;

namespace CommandUndoRedo
{
	public class UndoRedo
	{
		private DropoutStack<ICommand> mUndoCommands = new DropoutStack<ICommand>();
		private DropoutStack<ICommand> mRedoCommands = new DropoutStack<ICommand>();

		public int maxUndoStored { get { return mUndoCommands.maxLength; } set { SetMaxLength(value); } }

		public UndoRedo() { }
		public UndoRedo(int maxUndoStored)
		{
			this.maxUndoStored = maxUndoStored;
		}

		public void Clear()
		{
			mUndoCommands.Clear();
			mRedoCommands.Clear();
		}

		public void Undo()
		{
			if (mUndoCommands.Count > 0)
			{
				ICommand command = mUndoCommands.Pop();
				command.UnExecute();
				mRedoCommands.Push(command);
			}
		}

		public void Redo()
		{
			if (mRedoCommands.Count > 0)
			{
				ICommand command = mRedoCommands.Pop();
				command.Execute();
				mUndoCommands.Push(command);
			}
		}

		public void Insert(ICommand command)
		{
			if (maxUndoStored <= 0) return;

			mUndoCommands.Push(command);
			mRedoCommands.Clear();
		}

		public void Execute(ICommand command)
		{
			command.Execute();
			Insert(command);
		}

		void SetMaxLength(int max)
		{
			mUndoCommands.maxLength = max;
			mRedoCommands.maxLength = max;
		}
	}
}
