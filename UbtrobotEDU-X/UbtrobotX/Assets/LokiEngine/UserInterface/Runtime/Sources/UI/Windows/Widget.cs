namespace Loki.UI
{
    /// <inheritdoc />
    /// <summary>
    /// Widget基类（Window > Panel > Widget）
    /// </summary>
    public abstract class Widget : UComponent, IWidget
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

        public void SetActive(bool value)
        {
            if (gameObject.activeSelf == value) return;
            gameObject.SetActive(value);
        }

        public abstract void OnInit();

        public virtual void OnUninit() { }
	}
}
