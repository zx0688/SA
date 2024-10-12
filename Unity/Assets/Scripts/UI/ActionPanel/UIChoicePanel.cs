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

        private void FadeIn()
        {
            rewardRect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.15f);
            rect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f);
            bordrer.DOColor(new Color32(180, 180, 180, 255), 0.1f);
        }

        private void FadeOut()
        {
            rewardRect.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            rect.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            bordrer.DOColor(new Color32(142, 129, 129, 255), 0.1f);
        }

        public async void ShowChoice(CardMeta cardMeta, ChoiceInfo info, bool showFollowPrompt, List<string> allRewards)
        {
            rewardRect.DOKill();
            rewardRect.localScale = new Vector3(1f, 1f, 1f);

            rect.DOKill();
            rect.localScale = new Vector3(1f, 1f, 1f);

            bordrer.DOKill();
            bordrer.color = new Color32(142, 129, 129, 255);

            levels.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            reward.gameObject.SetActive(true);

            int index = info != null ? info.RewardIndex : 0;
            if (index != -1)
            {
                RewardMeta[] r = cardMeta.Reward != null ? cardMeta.Reward[index].Where(rr => rr.Count > 0).ToArray() : null;
                reward.SetItems(r, null, false);
                rewardText.gameObject.SetActive(r.GetCountIfNull() > 0);
                if (cardMeta.Reward != null)
                    allRewards.AddRange(cardMeta.Reward[index].Select(rr => rr.Id));


                r = cardMeta.Reward != null ? cardMeta.Reward[index].Where(rr => rr.Count < 0).ToArray() : new RewardMeta[] { };
                var c = cardMeta.Cost != null ? cardMeta.Cost[index].Where(rr => rr.Count > 0).ToArray() : new RewardMeta[] { };
                cost.SetItems(null, r.Concat(c).ToArray(), false);
                costText.gameObject.SetActive(c.Length > 0);

                if (cardMeta.Cost != null)
                    allRewards.AddRange(cardMeta.Cost[index].Select(rr => rr.Id));
            }
            else
            {
                rewardText.gameObject.SetActive(false);
                costText.gameObject.SetActive(false);
                reward.Hide();
                cost.Hide();
            }


            //skipText.text = "Action.Continue".Localize(LocalizePartEnum.GUI);

            levels.SetLevel(cardMeta.Level);

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
            if (info.Next == info.Id)
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

            gameObject.SetActive(false);
            tap.Disable();
        }


    }
}
