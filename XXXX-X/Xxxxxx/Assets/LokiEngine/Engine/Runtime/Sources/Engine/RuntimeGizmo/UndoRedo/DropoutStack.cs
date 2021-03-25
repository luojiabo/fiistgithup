using System;
using System.Collections.Generic;

namespace CommandUndoRedo
{
	public class DropoutStack<T> : LinkedList<T>
	{
		private int mMaxLength = int.MaxValue;

		public int maxLength { get { return mMaxLength; } set { SetMaxLength(value); } }

		public DropoutStack() { }
		public DropoutStack(int maxLength)
		{
			this.maxLength = maxLength;
		}

		public void Push(T item)
		{
			if (this.Count > 0 && this.Count + 1 > maxLength)
			{
				this.RemoveLast();
			}

			if (this.Count + 1 <= maxLength)
			{
				this.AddFirst(item);
			}
		}

		public T Pop()
		{
			T item = this.First.Value;
			this.RemoveFirst();
			return item;
		}

		void SetMaxLength(int max)
		{
			mMaxLength = max;

			if (this.Count > mMaxLength)
			{
				int leftover = this.Count - mMaxLength;
				for (int i = 0; i < leftover; i++)
				{
					this.RemoveLast();
				}
			}
		}
	}
}
