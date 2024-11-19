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

        [SerializeField] private UIReward reward;
        [SerializeField] private UIReward cost;
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private Image image;
        [SerializeField] private Image hero;
        [SerializeField] private Text action;
        [SerializeField] private GameObject followPrompt;
        [SerializeField] private RectTransform icon;
        [SerializeField] private List<Color32> colors;
        [SerializeField] private Image bordrer;
        [SerializeField] private UILevelGroup levels;
        [SerializeField] private TapMechanic tap;
        [SerializeField] private Text rewardText;
        [SerializeField] private Text costText;
        [SerializeField] private Text dialog;

        public event Action<UIChoicePanel> OnEndTap;

        private RectTransform rect => GetComponent<RectTransform>();
        private RectTransform rewardRect => reward.GetComponent<RectTransform>();
        private RectTransform rectIcon;

        public bool IsTaped => tap.IsTaped;

        void Awake()
        {
            tap.OnRealTaped += OnTaped;
            rectIcon = icon.GetComponent<RectTransform>();
        }

        private void OnTaped()
        {
            //action.gameObject.SetActive(false);
            //reward.Hide();
            // image.gameObject.SetActive(false);
            // hero.gameObject.SetActive(false);
            // followPrompt.gameObject.SetActive(false);

            OnEndTap?.Invoke(this);
        }

        public void FadeIn()
        {
            rect.DOKill();
            rect.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            //rewardRect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.15f);
            rect.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
            //bordrer.DOColor(new Color32(180, 180, 180, 255), 0.1f);
        }

        public void FadeOut()
        {
            //rewardRect.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            rect.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            //bordrer.DOColor(new Color32(142, 129, 129, 255), 0.1f);
        }

        public async void ShowChoice(CardMeta cardMeta, ChoiceInfo info, bool showFollowPrompt, List<string> allRewards)
        {
            //levels.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            reward.gameObject.SetActive(true);
            dialog.gameObject.SetActive(false);
            rewardPanel.SetActive(false);

            bool hasRewardOrCost = info.RewardIndex != -1 || info.CostIndex != -1;
            if (cardMeta.TradeLimit > 0)
            {
                RewardMeta rr = new RewardMeta();
                rr.Id = cardMeta.Reward[0][info.RewardIndex].Id;
                rr.Count = SL.GetRewardMinCount(rr.Id, cardMeta, Services.Meta.Game);
                reward.SetItems(new RewardMeta[1] { rr }, null, false);
                allRewards.Add(rr.Id);

                rr = new RewardMeta();
                rr.Id = cardMeta.Cost[0][info.CostIndex].Id;
                rr.Count = SL.GetRewardMinCount(rr.Id, cardMeta, Services.Meta.Game);
                cost.SetItems(null, new RewardMeta[1] { rr }, false);
                allRewards.Add(rr.Id);

                costText.gameObject.SetActive(true);
                rewardText.gameObject.SetActive(true);
                rewardPanel.SetActive(true);
            }
            else if (hasRewardOrCost)
            {
                RewardMeta[] _r = info.RewardIndex != -1 ? cardMeta.Reward[info.RewardIndex].Where(rr => rr.Count > 0).ToArray() : null;
                reward.SetItems(_r, null, false);
                rewardText.gameObject.SetActive(_r.GetCountIfNull() > 0);
                if (info.RewardIndex != -1)
                    allRewards.AddRange(cardMeta.Reward[info.RewardIndex].Select(rr => rr.Id));


                var r = info.RewardIndex != -1 ? cardMeta.Reward[info.RewardIndex].Where(rr => rr.Count < 0).ToArray() : new RewardMeta[] { };
                var c = info.CostIndex != -1 && cardMeta.Cost.HasReward() ? cardMeta.Cost[info.CostIndex].Where(rr => rr.Count > 0).ToArray() : new RewardMeta[] { };
                cost.SetItems(null, r.Concat(c).ToArray(), false);
                costText.gameObject.SetActive(c.Length > 0);

                rewardPanel.SetActive(true);
                if (info.CostIndex != -1 && cardMeta.Cost.HasReward())
                    allRewards.AddRange(cardMeta.Cost[info.CostIndex].Select(rr => rr.Id));
            }
            else if (cardMeta.RewardText != null)
            {
                dialog.gameObject.SetActive(true);
                dialog.text = cardMeta.RewardText.Localize(LocalizePartEnum.CardDescription);

                rewardText.gameObject.SetActive(false);
                costText.gameObject.SetActive(false);
                reward.Hide();
                cost.Hide();
            }
            else
            {
                rewardText.gameObject.SetActive(false);
                costText.gameObject.SetActive(false);
                reward.Hide();
                cost.Hide();
            }


            //skipText.text = "Action.Continue".Localize(LocalizePartEnum.GUI);

            //levels.SetLevel(cardMeta.Level);

            followPrompt.gameObject.SetActive(showFollowPrompt);

            image.LoadCardImage(cardMeta.Image);
            image.gameObject.SetActive(true);

            // if (cardMeta.Hero != null)
            // {
            //     hero.LoadHeroImage(cadMeta.Hero);
            //     hero.gameObject.SetActive(true);
            // }
            // else
            //     hero.gameObject.SetActive(false);
            if (info.Next == info.Id && cardMeta.TradeLimit == 0)
                action.Localize(cardMeta.Shure, LocalizePartEnum.CardAction);
            else if (cardMeta.Act.HasText())
                action.Localize(cardMeta.Act, LocalizePartEnum.CardAction);
            else
                action.Localize(cardMeta.Name, LocalizePartEnum.CardName);

            action.gameObject.SetActive(true);

            tap.ConstructNewTap();
            tap.WaitTap();

            gameObject.SetActive(true);

            //action.color = colors[0];
        }

        public void HideAll()
        {
            action.gameObject.SetActive(false);
            reward.Hide();
            image.gameObject.SetActive(false);
            //ero.gameObject.SetActive(false);
            followPrompt.gameObject.SetActive(false);
            rewardPanel.SetActive(false);

            rewardRect.DOKill();
            rewardRect.localScale = new Vector3(1f, 1f, 1f);

            rect.DOKill();
            rect.localScale = new Vector3(1f, 1f, 1f);

            gameObject.SetActive(false);
            tap.Disable();
        }


    }
}
