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

namespace UI.ActionPanel
{
    public class UIChoicePanel : MonoBehaviour
    {

        [SerializeField] private UIReward reward;
        [SerializeField] private Image image;
        [SerializeField] private Image hero;
        [SerializeField] private Text action;
        [SerializeField] private GameObject followPrompt;
        [SerializeField] private RectTransform icon;
        [SerializeField] private List<Color32> colors;
        [SerializeField] private Image bordrer;
        [SerializeField] private UILevelGroup levels;

        private RectTransform rect => GetComponent<RectTransform>();
        private RectTransform rewardRect => reward.GetComponent<RectTransform>();

        public void FadeIn()
        {
            rewardRect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.15f);
            rect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f);
            bordrer.DOColor(new Color32(180, 180, 180, 255), 0.1f);
        }

        public void FadeOut()
        {
            rewardRect.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            rect.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            bordrer.DOColor(new Color32(142, 129, 129, 255), 0.1f);
        }

        public void ShowChoice(CardMeta cardMeta, CardNextInfo info, bool showFollowPrompt)
        {
            gameObject.SetActive(true);

            rewardRect.DOKill();
            rewardRect.localScale = new Vector3(1f, 1f, 1f);

            rect.DOKill();
            rect.localScale = new Vector3(1f, 1f, 1f);

            bordrer.DOKill();
            bordrer.color = new Color32(142, 129, 129, 255);

            // if (cardMeta.Id == "28393500")
            // {
            //     icon.gameObject.SetActive(false);
            //     reward.gameObject.SetActive(false);
            //     levels.gameObject.SetActive(false);
            //     reward.Hide();
            //     return;
            // }
            // else
            // {
            levels.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            reward.gameObject.SetActive(true);
            //}

            int index = info != null ? info.RewardIndex : 0;
            if (index != -1)
                reward.SetItems(cardMeta.Reward != null &&
                index < cardMeta.Reward.Length ? cardMeta.Reward[index].Where(r => r.Chance == 0).ToArray() : null,
                cardMeta.Cost != null && index < cardMeta.Cost.Length ? cardMeta.Cost[index].Where(r => r.Chance == 0).ToArray() : null);
            else
                reward.Hide();

            //skipText.text = "Action.Continue".Localize(LocalizePartEnum.GUI);

            levels.SetLevel(cardMeta.Level);

            followPrompt.gameObject.SetActive(showFollowPrompt);

            image.LoadCardImage(cardMeta.Image);
            image.gameObject.SetActive(true);

            if (cardMeta.Hero != null)
            {
                hero.LoadHeroImage(cardMeta.Hero);
                hero.gameObject.SetActive(true);
            }
            else
                hero.gameObject.SetActive(false);

            if (cardMeta.Act != null)
                action.Localize(cardMeta.Act, LocalizePartEnum.CardName);
            else
                action.Localize(cardMeta.Name, LocalizePartEnum.CardName);

            action.gameObject.SetActive(true);
            //action.color = colors[0];
        }

        public void HideAll()
        {
            action.gameObject.SetActive(false);
            reward.Hide();
            image.gameObject.SetActive(false);
            hero.gameObject.SetActive(false);
            followPrompt.gameObject.SetActive(false);

            gameObject.SetActive(false);
        }


    }
}
