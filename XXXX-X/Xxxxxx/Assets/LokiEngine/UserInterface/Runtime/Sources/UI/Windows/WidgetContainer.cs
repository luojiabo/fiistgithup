namespace Loki.UI
{
    /// <inheritdoc />
    /// <summary>
    /// WidgetContainer
    /// </summary>
    public abstract class WidgetContainer : UComponent, IWidget
    {
        public bool IsActive => !HasDestroyed() && gameObject.activeSelf;

        public void SetActive(bool value)
        {
            if (gameObject.activeSelf == value) return;
            gameObject.SetActive(value);
        }

        public abstract void OnInit();

        public virtual void OnUninit() { }
    }
}
