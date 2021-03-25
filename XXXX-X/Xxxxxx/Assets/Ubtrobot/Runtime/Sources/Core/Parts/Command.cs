using Loki;
using System;
using UnityEngine;

namespace Ubtrobot
{
	/// <summary>
	/// The base class of all commands
	/// </summary>
	public abstract class Command : ICommand
	{
		private bool mReleased = false;
		public bool debug { get; set; }
		public string uuid { get; set; }
		public abstract ECommand commandID { get; }
		public bool taskCommand { get; set; }
		public int id { get; set; }
		public int device { get; set; }
		public int cmdMode { get; set; }
		public object context { get; set; }
		public string host { get; set; }


		public static implicit operator bool(Command command)
		{
			if (command == null)
				return false;

			return true;
		}

		protected Command()
		{
		}

		public void Release()
		{
			if (!mReleased)
			{
				mReleased = true;
				OnRelease();
			}
		}

		protected virtual void OnRelease()
		{
		}
	}
}
