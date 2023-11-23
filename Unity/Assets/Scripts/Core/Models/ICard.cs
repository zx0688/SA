namespace Core
{
    public interface ICard
    {
        void UpdateData(SwipeData data);
        void SetActive(bool enable);
    }
}