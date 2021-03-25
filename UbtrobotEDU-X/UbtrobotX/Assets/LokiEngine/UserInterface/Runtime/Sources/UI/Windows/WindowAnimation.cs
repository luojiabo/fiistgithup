using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Loki.UI
{
	public abstract class WindowAnimation : MonoBehaviour
	{
		private Window mOwner = null;

		public virtual void Initialize(Window window)
		{
			mOwner = window;
		}

		public virtual void OnWindowOpen()
		{
			 
		}

		public virtual void OnWindowClose()
		{

		}

		public virtual void Trigger(string action)
		{

		}
	}
}
