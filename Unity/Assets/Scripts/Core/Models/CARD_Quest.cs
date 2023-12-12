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
        [SerializeField] private UIChoice uIChoice;

        private SwipeData data = null;

        public void SetActive(bool enable)
        {
            gameObject.SetActive(enable);
        }


        public void UpdateData(SwipeData data)
        {
            this.data = data;

            uIReward.SetItems(data.Quest.SR);
            if (data.Quest.ST != null && data.Quest.ST.Length > 0)
            {
                uIChoice.Show(Services.Meta.Game.Cards[data.Quest.ST[0].Id]);
            }
            else
            {
                uIChoice.Hide();
            }

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