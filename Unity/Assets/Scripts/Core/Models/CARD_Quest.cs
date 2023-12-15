using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.ActionPanel;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class CARD_Quest : MonoBehaviour, ICard
    {
        [SerializeField] private UIReward uIReward;
        [SerializeField] private UITarget uITarget;
        [SerializeField] private Text title;
        [SerializeField] private Text descr;
        [SerializeField] private Text targetText;

        private SwipeData data = null;

        public void SetActive(bool enable)
        {
            gameObject.SetActive(enable);
        }


        public void UpdateData(SwipeData data)
        {
            this.data = data;

            Services.Player.Profile.Cards.TryGetValue(data.Card.Id, out CardData cardData);
            if (cardData.Value == CardMeta.QUEST_SUCCESS)
            {
                targetText.gameObject.SetActive(false);
                descr.gameObject.SetActive(false);
            }
            else if (cardData.Value == CardMeta.QUEST_ACTIVE)
            {
                uITarget.SetItems(data.Card.SC, data.Card.ST);
                descr.Localize(data.Card.Desc, LocalizePartEnum.CardDescription);
                descr.gameObject.SetActive(true);
                targetText.gameObject.SetActive(true);
            }

            uIReward.SetItems(data.Card.SR);

            title.Localize(data.Card.Name, LocalizePartEnum.CardName);

        }

        public void ChangeDirection(int i)
        {

        }

        public void DropCard()
        {

        }

        public void TakeCard()
        {

        }

        public void OnChangeDeviation(float obj)
        {

        }
    }
}