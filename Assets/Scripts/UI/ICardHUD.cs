using UnityEngine;

public interface ICardHUD
{
    void UpdateData(SwipeData data);
    void SetActive(bool enable);

    void OnTakeCard();
    void OnStartSwipe();
    void OnChangeDirection(int obj);
    void OnChangeDeviation(float obj);
    void OnDrop();


}
