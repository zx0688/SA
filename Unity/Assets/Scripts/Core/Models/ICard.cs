namespace Core
{
    public interface ICard
    {
        void UpdateData(SwipeData data);
        void SetActive(bool enable);
        void ChangeDirection(int i);
        void DropCard();
        void TakeCard();
        void OnChangeDeviation(float obj);
    }
}