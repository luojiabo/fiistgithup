namespace Loki.UI
{
    public interface IWidget
    {
        void OnInit();
        void OnUninit();
        void SetActive(bool value);
    }
}
