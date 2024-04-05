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

        private RectTransform rect => GetComponent<RectTransform>();
        private RectTransform rewardRect => reward.GetComponent<RectTransform>();

        public void FadeIn()
        {
            rewardRect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.15f);
            rect.DOScale(new Vector3(1.03f, 1.03f, 1.03f), 0.15f);
            bordrer.DOColor(new Color32(180, 180, 180, 255), 0.1f);
        }

        public void FadeOut()
        {
            rewardRect.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            rect.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            bordrer.DOColor(new Color32(142, 129, 129, 255), 0.1f);
        }

        public void ShowChoice(CardMeta ch, bool showFollowPrompt)
        {
            rewardRect.DOKill();
            rewardRect.localScale = new Vector3(1f, 1f, 1f);

            rect.DOKill();
            rect.localScale = new Vector3(1f, 1f, 1f);

            bordrer.DOKill();
            bordrer.color = new Color32(142, 129, 129, 255);

            if (ch.Reward != null && ch.Reward.Length > 0)
                reward.SetItems(SL.GetRewardByCondition(ch.Reward, ch.Con, Services.Meta.Game, Services.Player.Profile, null, null));
            else
                reward.Hide();

            followPrompt.gameObject.SetActive(showFollowPrompt);
            gameObject.SetActive(true);


            image.LoadCardImage(ch.Image);
            image.gameObject.SetActive(true);

            if (ch.Hero != null)
            {
                hero.LoadHeroImage(ch.Hero);
                hero.gameObject.SetActive(true);
            }
            else
                hero.gameObject.SetActive(false);

            if (ch.ActionT.HasText())
                action.Localize(ch.ActionT, LocalizePartEnum.CardName);
            else
                action.Localize(ch.Name, LocalizePartEnum.CardName);


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
