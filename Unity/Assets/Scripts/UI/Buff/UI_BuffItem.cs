using UnityEngine;
using UnityEngine.UI;

public class UI_BuffItem : MonoBehaviour
{
    [SerializeField] private UIReward reward;
    [SerializeField] private Image icon;

    public void SetItem(CardMeta card)
    {
        if (card == null)
        {
            Hide();
            return;
        }

        if (icon != null)
        {
            icon.gameObject.SetActive(true);
            icon.LoadCardImage(card.Image);
        }

        reward.SetItems(card.Reward != null ? card.Reward[0] : new RewardMeta[0], new RewardMeta[0], false);
    }

    public void Hide()
    {
        if (icon != null)
            icon.gameObject.SetActive(false);

        reward.Hide();

    }
}
