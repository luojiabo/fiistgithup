using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Loki.UI
{
	[AddComponentMenu("UI/Trigger/Enter and Exit Trigger")]
	public class UIEventeEnterExitListener : EventTrigger, IPointerEnterHandler, IPointerExitHandler
	{
		[Serializable]
		public class EnterEvent : UnityEvent<UIEventeEnterExitListener, PointerEventData>
		{
		}

		[Serializable]
		public class ExitEvent : UnityEvent<UIEventeEnterExitListener, PointerEventData>
		{
		}

		[SerializeField]
		private EnterEvent m_EnterEvent = new EnterEvent();

		[SerializeField]
		private ExitEvent m_ExitEvent = new ExitEvent();

		public EnterEvent enter => m_EnterEvent;

		public ExitEvent exit => m_ExitEvent;

		public void OnPointerEnter(PointerEventData eventData)
		{
#if UNITY_EDITOR
			DebugUtility.Log(LoggerTags.UI, "OnPointerEnter : {0}", name);
#endif
			m_EnterEvent.Invoke(this, eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
#if UNITY_EDITOR
			DebugUtility.Log(LoggerTags.UI, "OnPointerExit : {0}", name);
#endif
			m_ExitEvent.Invoke(this, eventData);
		}
	}
}
