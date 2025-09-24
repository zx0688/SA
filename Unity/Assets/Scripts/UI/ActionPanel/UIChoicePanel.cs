using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System.Data;
using Cysharp.Text;
using UI.Components;
using System.Linq;
using System.Drawing;
using haxe.root;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace UI.ActionPanel
{
    public class UIChoicePanel : MonoBehaviour
    {
        [SerializeField] private Text actionText;
        [SerializeField] private Image nextCard;


        public async void ShowChoice(CardMeta cardMeta)
        {
            this.gameObject.SetActive(true);
            string text =
            actionText.Localize(cardMeta.Name, LocalizePartEnum.CardName);
            nextCard.gameObject.SetActive(true);
            nextCard.LoadCardImage(cardMeta.Image);

            //              if (cardMeta.TradeLimit > 0)
            //             {
            //                 //                 RewardMeta rr = new RewardMeta();
            //                 //                 rr.Id = cardMeta.Reward[0][info.RewardIndex].Id;
            //                 //                 rr.Count = SL.GetRewardMinCount(rr.Id, cardMeta, Services.Meta.Game);
            //                 //                 //reward.SetItems(new RewardMeta[1] { rr }, null, false);
            //                 //                 allRewards.Add(rr.Id);
            //                 // 
            //                 //                 rr = new RewardMeta();
            //                 //                 rr.Id = cardMeta.Cost[0][info.CostIndex].Id;
            //                 //                 rr.Count = SL.GetRewardMinCount(rr.Id, cardMeta, Services.Meta.Game);
            //                 //                 // cost.SetItems(null, new RewardMeta[1] { rr }, false);
            //                 //                 allRewards.Add(rr.Id);
            // 
            // 
            //                 rewardPanel.SetActive(true);
            //             }
            //             else if (hasRewardOrCost)
            //             {
            //                 rewardPanel.SetActive(true);
            // 
            //                 List<RewardMeta> total = new List<RewardMeta>();
            //                 if (info.RI != -1)
            //                     total.AddRange(cardMeta.Reward[info.RI].ToList());
            //                 if (info.CI != -1 && cardMeta.Cost.HasReward())
            //                     total.AddRange(cardMeta.Cost[info.CI].ToList());
            // 
            //                 reward.SetItems(
            //                     info.RI != -1 && cardMeta.Reward.HasReward() ? cardMeta.Reward[info.RI].ToList() : null,
            //                     info.CI != -1 && cardMeta.Cost.HasReward() ? cardMeta.Cost[info.CI].ToList() : null);
            // 
            //                 allRewards.AddRange(total.Select(r => r.Id));
            // 
            //                 //backpack.gameObject.SetActive(true);
            //                 //backpack.SetItems(total.Select(r => new ItemData(r.Id, 0)).ToList(), profile, Services.Meta.Game, false);
            //             }
            //             else
            //             {
            //                 reward.Hide();
            //             }

        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            choiceTitlePanel.SetActive(false);
            nextCard.gameObject.SetActive(false);
        }


    }
}
