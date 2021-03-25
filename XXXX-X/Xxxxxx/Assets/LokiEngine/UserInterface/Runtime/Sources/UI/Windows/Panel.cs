namespace Loki.UI
{
    /// <inheritdoc />
    /// <summary>
    /// 面板基类（Window > Panel > Widget）
    /// </summary>
    public abstract class Panel : WidgetContainer
    {
	    protected override void Awake()
	    {
		    base.Awake();
		    OnInit();
	    }

	    protected override void OnDestroy()
	    {
		    base.OnDestroy();
		    OnUninit();
	    }

		protected virtual void OnDisable()
		{
			OnClose();
		}

        protected virtual void OnOpen(object param) { }

        protected virtual void OnClose() { }

		public void Open(object param)
		{
			SetActive(true);
			OnOpen(param);
		}

		public void Close()
		{
			SetActive(false);
		}
	}
}

