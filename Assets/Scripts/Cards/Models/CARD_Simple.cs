using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using DG.Tweening;
using Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class CARD_Simple : CARD_Base
    {

        protected UI_Reward reward;
        protected bool hasReward;
        protected CanvasGroup canvasGroupReward;
        protected GameObject chanceTf;
        protected Text chanceValue;

        private bool isOneReward;
        private GameObject costPanel;
        private UI_RewardItem costItem;

        protected override void Awake()
        {
            base.Awake();
            reward = GetComponentInChildren<UI_Reward>();
            //canvasGroupReward = reward.GetComponentInChildren<CanvasGroup>();
            chanceTf = transform.Find("Chance").gameObject;
            chanceValue = chanceTf.transform.Find("ChanceValue").GetComponent<Text>();
            costPanel = transform.Find("CostPanel").gameObject;
            costItem = costPanel.GetComponentInChildren<UI_RewardItem>();
        }

        protected override async void UpdateHUD()
        {
            return;
            base.UpdateHUD();

            hasReward = (Data.Right.Action.Reward.Count > 0) || (Data.Left.Action != null && Data.Left.Action.Reward.Count > 0);

            costPanel.SetActive(false);
            chanceTf.SetActive(false);
            reward.gameObject.SetActive(false);
            hero.gameObject.SetActive(false);
            isOneReward = false;
            chanceValue.color = Color.yellow;

            if (hasReward)
            {

                if (Data.Left.Action == null || Data.Left.Action.Reward.Count == 0)
                {
                    if (Data.Right.Chance > 0)
                    {
                        chanceTf.SetActive(true);
                        if (Data.Right.Chance > Data.Right.Action.Chance)
                            chanceValue.color = Color.green;
                        else if (Data.Right.Chance < Data.Right.Action.Chance)
                            chanceValue.color = Color.red;

                        chanceValue.text = Data.Right.Chance + "%";
                    }

                    RewardData cost = Data.Right.Action.Reward.Find(r => r.Count < 0);
                    RewardData buy = Data.Right.Action.Reward.Find(r => r.Count > 0);

                    if (cost != null && buy != null)
                    {
                        costPanel.SetActive(true);
                        cost = cost.Clone();
                        cost.Count = Math.Abs(cost.Count);
                        costItem.SetItem(cost);
                    }

                    if (Data.Right.Action.Reward.Count == 1 && Data.Right.Action.Reward[0].Count == 1)
                    {
                        // hero.gameObject.SetActive (true);
                        int id = Data.Right.Action.Reward[0].Id;
                        isOneReward = true;
                        switch (Data.Right.Action.Reward[0].Tp)
                        {
                            case DataService.SKILL_ID:
                                Services.Assets.SetSpriteIntoImage(hero, "Skills/" + id + "/icon", true).Forget();
                                break;
                            case DataService.ITEM_ID:
                                //Services.Assets.SetSpriteIntoImage (back, "Actions/back", true).Forget ();
                                Services.Assets.SetSpriteIntoImage(hero, "Items/" + id + "/icon", true).Forget();
                                break;
                            case DataService.BUILDING_ID:
                                Services.Assets.SetSpriteIntoImage(hero, "Buildings/" + id + "/icon", true).Forget();
                                break;
                        }

                    }
                    else
                    {
                        reward.gameObject.SetActive(true);
                        reward.SetItems(Data.Right.Action.Reward);
                    }

                }
                else if (Data.Right.Action.Reward.Count == 0)
                {
                    if (Data.Left.Chance > 0)
                    {
                        chanceTf.SetActive(true);
                        if (Data.Left.Chance > Data.Left.Action.Chance)
                            chanceValue.color = Color.green;
                        else if (Data.Left.Chance < Data.Left.Action.Chance)
                            chanceValue.color = Color.red;
                        chanceValue.text = Data.Left.Chance + "%";
                    }
                    reward.gameObject.SetActive(true);
                    reward.SetItems(Data.Left.Action.Reward);
                }
            }

            if (Data.Left.Action == null)
            {
                choice.text = LocalizationManager.Localize(Data.Right.Action.Text);
                choice.gameObject.SetActive(true);
            }
            else
                choice.gameObject.SetActive(false);

            if (Data.Data.Hero == -1)
            {
                reward.gameObject.SetActive(false);
                title.gameObject.SetActive(false);
                hero.gameObject.SetActive(true);
                //Utils.SetAlpha(art, 0.6f);
            }
            else if (Data.Data.Hero > 0)
            {
                reward.gameObject.SetActive(false);
                //Utils.SetAlpha(art, 0.6f);
                //ItemData heroData = Services.Data.SkillInfo(Data.Data.Hero);
                //title.text = heroData.Name;
                hero.gameObject.SetActive(true);
                Services.Assets.SetSpriteIntoImage(hero, "Heroes/" + Data.Data.Hero + "/icon", true).Forget();
            }
            else if (hero != null)
            {

                //Utils.SetAlpha(art, 1f);
            }

            // description.text = data.cardData.text != null ? LocalizationManager.Localize (data.cardData.text) : LocalizationManager.Localize (data.cardData.id + "cardd");
            //hero.gameObject.SetActive (true);
            //canvasGroupReward.DOKill ();
            //canvasGroupReward.alpha = 1f;
        }

        public override void OnTakeCard()
        {
            return;
            //canvasGroupReward.alpha = 0f;
            base.OnTakeCard();

            choice.gameObject.SetActive(true);
            reward.gameObject.SetActive(hasReward && Data.Data.Hero == 0 && isOneReward == false);

            if (Data.Left.Action != null && Data.Left.Chance > 0)
            {
                chanceTf.SetActive(true);
                chanceValue.text = Data.Left.Chance + "%";
            }
            else if (Data.Right.Chance > 0)
            {
                chanceTf.SetActive(true);
                chanceValue.text = Data.Right.Chance + "%";
            }

            if (hasReward && hero.gameObject.activeSelf)
            {
                // Utils.SetAlpha(hero, 0.2f);
                //hero.gameObject.SetActive (false);
            }
            if (art.gameObject.activeSelf)
            {
                // Utils.SetAlpha (art, 0.8f);
            }
        }

        public override void OnChangeDirection(int obj)
        {
            return;
            base.OnChangeDirection(obj);

            /* if (!isRewarded) {
                 reward.gameObject.SetActive (false);
                 return;
             }*/

            //canvasGroupReward.DOKill ();
            //canvasGroupReward.DOFade (0f, 0.1f).OnComplete (() => {

            if (Data.Left.Action == null)
            {

                if (Data.Right.Chance > 0)
                {
                    chanceValue.text = Data.Right.Chance + "%";
                }
                if (hasReward && Data.Right.Action.Reward.Count > 0 && isOneReward == false)
                    reward.SetItems(Data.Right.Action.Reward);

            }
            else if (obj == Swipe.LEFT_CHOICE)
            {

                if (Data.Left.Chance > 0)
                {
                    chanceValue.text = Data.Left.Chance + "%";
                }

                chanceTf.SetActive(Data.Left.Chance > 0);
                if (hasReward && Data.Left.Action.Reward.Count > 0)
                    reward.SetItems(Data.Left.Action.Reward);

                choice.text = LocalizationManager.Localize(Data.Left.Action.Text);

            }
            else if (obj == Swipe.RIGHT_CHOICE)
            {

                if (Data.Right.Chance > 0)
                {
                    chanceValue.text = Data.Right.Chance + "%";
                }
                chanceTf.SetActive(Data.Right.Chance > 0);
                if (hasReward && Data.Right.Action.Reward.Count > 0)
                    reward.SetItems(Data.Right.Action.Reward);

                choice.text = LocalizationManager.Localize(Data.Right.Action.Text);

            }

            // reward.PlaceAround ();
            //canvasGroupReward.DOFade (1f, 0.1f);

            // });
        }

        public override void OnDrop()
        {
            return;
            base.OnDrop();

            if (Data.Left.Action != null && Data.Left.Action.Text != null)
                choice.gameObject.SetActive(false);

            if (hasReward)
            {
                chanceTf.SetActive(Data.Left.Action == null && Data.Right.Chance > 0);
                reward.gameObject.SetActive(isOneReward == false && (Data.Left.Action == null || Data.Right.Action.Reward.Count == 0));
            }
            //reward.gameObject.SetActive (false);

            if (hasReward && hero.gameObject.activeSelf)
            {
                //Utils.SetAlpha(hero, 1f);

            }
            if (art != null)
            {
                //art.gameObject.SetActive (true);
                // Utils.SetAlpha (art, 1f);
            }

            // reward.gameObject.SetActive (false);

            /*if (data == null || gameObject.activeSelf == false || !isRewarded) {
                reward.gameObject.SetActive (false);
                return;
            }*/

            /*canvasGroupReward.DOFade (0f, 0.1f).OnComplete (() => {
                reward.gameObject.SetActive (false);
                canvasGroupReward.DOKill ();
            });*/

        }

    }
}